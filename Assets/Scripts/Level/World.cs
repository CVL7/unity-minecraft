using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Level
{
    public class World : MonoBehaviour
    {
        public event Action firstGenerationComplete = delegate { };
        
        [SerializeField] Chunk chunkPrefab;
        [SerializeField] GameObject player;
        Coroutine generateWorld;
        Vector3i playerChunkPosition;
        Transform worldTransform;

        public int viewDistance { get; private set; }
        
        int maxViewChunks;
        int verticalViewDistance;
        int cleanUpDistance;
        int travelToGenerateDistance;

        Dictionary<Vector3i, Chunk> chunks;
        List<Vector3i> chunksToRemove;
        Queue<Chunk> chunksToRender;

        int generatedDistance;
        public int currentlyRenderingLayer { get; private set; }
        bool fistGeneration = true;
        public int chunksCount => chunks.Count;

        public static Vector3i GetChunkPosition(Vector3 position)
        {
            return Vector3i.FloorToInt(position / Chunk.size) * Chunk.size;
        }

        void Awake()
        {
            var qualityLevel = QualitySettings.GetQualityLevel();
            if (qualityLevel <= 3)
            {
                maxViewChunks = 7;
            }
            else
            {
                maxViewChunks = 15; 
            }

            viewDistance = maxViewChunks * Chunk.size;
            verticalViewDistance = viewDistance / 2;
            cleanUpDistance = (viewDistance*3)/2;
            travelToGenerateDistance = Chunk.size;
            var maxBlocks = viewDistance * viewDistance * viewDistance;
            chunks = new Dictionary<Vector3i, Chunk>(maxBlocks);
            chunksToRemove = new List<Vector3i>(maxBlocks);
            chunksToRender = new Queue<Chunk>(maxBlocks);
            
            worldTransform = transform;
        }

        void Start()
        {
            generateWorld = StartCoroutine(GenerateWorld());
            StartCoroutine(RenderChunks());
        }

        void Update()
        {
            var updatedPlayerChunkPosition = GetChunkPosition(player.transform.localPosition);

            if (updatedPlayerChunkPosition.ManhattanDistance(playerChunkPosition) >= travelToGenerateDistance)
            {
                RemoveFarChunks();
                StopCoroutine(generateWorld);
                generateWorld = StartCoroutine(GenerateWorld());
            }

            if (fistGeneration && chunksToRender.Count == 0)
            {
                firstGenerationComplete();
                fistGeneration = false;
            }
        }

        public Chunk GetChunk(Vector3i position)
        {
            Chunk chunk;
            chunks.TryGetValue(GetChunkPosition(position), out chunk);
            return chunk;
        }

        Chunk CreateChunk(Vector3i position)
        {
            if (chunks.ContainsKey(position)) return null;

            var chunk = Instantiate(chunkPrefab, position, Quaternion.identity);
            chunk.transform.SetParent(worldTransform);
            // chunk.name = "Chunk " + position; 
            chunk.world = this;
            chunks[position] = chunk;
            chunk.Init();
            return chunk;
        }

        void CreateChunkAroundPlayer(Vector3i position, int radius)
        {
            var chunkPosition = position + playerChunkPosition;
            var chunk = GetChunk(chunkPosition);
            if (!chunk) chunk = CreateChunk(chunkPosition);
            var isInVecticalBounds = (playerChunkPosition.y - chunk.chunkPosition.y) < (verticalViewDistance - Chunk.size);
            if (!chunk.isRendered && radius < viewDistance && isInVecticalBounds) chunksToRender.Enqueue(chunk);
        }

        IEnumerator GenerateWorld()
        {
            chunksToRender.Clear();
            playerChunkPosition = GetChunkPosition(player.transform.localPosition);

            for (var distance = 0; distance <= viewDistance; distance += Chunk.size)
            {
                var xStart = -Math.Min(viewDistance, distance);
                var xEnd = Math.Min(viewDistance, distance);
                for (var x = xStart; x <= xEnd; x += Chunk.size)
                {
                    var yStart = -Math.Min(viewDistance, distance - Math.Abs(x));
                    var yEnd = Math.Min(viewDistance, distance - Math.Abs(x));
                    for (var y = yStart; y <= yEnd; y += Chunk.size)
                    {
                        var z = distance - Math.Abs(x) - Math.Abs(y);
                        if (z > viewDistance || y > verticalViewDistance || y < -verticalViewDistance) continue;

                        CreateChunkAroundPlayer(new Vector3i(x, y, z), distance);
                        if (z != 0) CreateChunkAroundPlayer(new Vector3i(x, y, -z), distance);
                        // Chunks created, yield to prevent frame freeze
                        yield return null;
                    }
                }
                generatedDistance = distance;
            }
        }

        IEnumerator RenderChunks()
        {
            while (true)
            {
                if (chunksToRender.Count > 0)
                {
                    var chunk = chunksToRender.Peek();
                    var distance = playerChunkPosition.ManhattanDistance(chunk.chunkPosition);
                    if (distance < generatedDistance)
                    {
                        currentlyRenderingLayer = distance;
                        chunksToRender.Dequeue().UpdateMesh();
                    }
                }
                // One chunk rendered, yield to prevent frame freeze
                yield return null;
            }
        }

        void RemoveFarChunks()
        {
            foreach (var entry in chunks)
            {
                var chunk = entry.Value;
                var distance = playerChunkPosition.ManhattanDistance(chunk.chunkPosition);
                if (distance > cleanUpDistance)
                {
                    Destroy(chunk.gameObject);
                    chunksToRemove.Add(entry.Key);
                }
            }
            foreach (var chunkPosition in chunksToRemove)
            {
                chunks.Remove(chunkPosition);
            }
            chunksToRemove.Clear();
        }
    }
}