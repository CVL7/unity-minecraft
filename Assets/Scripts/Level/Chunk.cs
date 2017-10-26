using UnityEngine;
using Utils;

namespace Level
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class Chunk : MonoBehaviour
    {
        public const int size = 16;
        public readonly Block[,,] blocks = new Block[size, size, size];

        public bool isRendered;
        public World world { private get; set; }
        public Vector3i chunkPosition;

        MeshFilter meshFilter;
        MeshCollider meshCollider;
        new Transform transform;

        static bool InBounds(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < size && y < size && z < size;
        }

        void Awake()
        {
            transform = GetComponent<Transform>();
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
        }

        public void Init()
        {
            chunkPosition = World.GetChunkPosition(transform.localPosition);
            TerrainGenerator.GenerateChunk(this);
        }

        public void UpdateMesh()
        {
            MeshBuilder.BuildChunk(this, meshFilter.mesh);
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = meshFilter.mesh;
            isRendered = true;
        }

        public Block GetBlock(int x, int y, int z)
        {
            if (InBounds(x, y, z)) return blocks[x, y, z];

            var worldPosition = chunkPosition + new Vector3i(x, y, z);
            var targetChunkPosition = World.GetChunkPosition(worldPosition);
            var chunk = world.GetChunk(targetChunkPosition);
            if (chunk)
            {
                var blockInsideTargetChunk = worldPosition - targetChunkPosition;
                return chunk.GetBlock(blockInsideTargetChunk);
            }
            // Was Nullable, but this is faster
            return new Block(BlockType.Air);
        }

        public Block GetBlock(Vector3i position)
        {
            return GetBlock(position.x, position.y, position.z);
        }

        public void CreateBlock(BlockType blockType, Vector3i position)
        {
            if (InBounds(position.x, position.y, position.z))
            {
                blocks[position.x, position.y, position.z] = new Block(blockType);
                UpdateMesh();
                return;
            }
            var worldPosition = chunkPosition + position;
            var targetChunkPosition = World.GetChunkPosition(worldPosition);
            var chunk = world.GetChunk(targetChunkPosition);
            if (chunk)
            {
                var blockInsideTargetChunk = worldPosition - targetChunkPosition;
                chunk.CreateBlock(blockType, blockInsideTargetChunk);
            }
        }

        public void DestroyBlock(Vector3i position)
        {
            if (GetBlock(position).type != BlockType.Air)
            {
                blocks[position.x, position.y, position.z] = new Block(BlockType.Air);
                UpdateMesh();
                // Update neighbours meshes
                if (position.x == 0) world.GetChunk(chunkPosition + new Vector3i(-size, 0, 0))?.UpdateMesh();
                if (position.y == 0) world.GetChunk(chunkPosition + new Vector3i(0, -size, 0))?.UpdateMesh();
                if (position.z == 0) world.GetChunk(chunkPosition + new Vector3i(0, 0, -size))?.UpdateMesh();
                if (position.x == size - 1) world.GetChunk(chunkPosition + new Vector3i(size, 0, 0))?.UpdateMesh();
                if (position.y == size - 1) world.GetChunk(chunkPosition + new Vector3i(0, size, 0))?.UpdateMesh();
                if (position.z == size - 1) world.GetChunk(chunkPosition + new Vector3i(0, 0, size))?.UpdateMesh();
            }
        }
    }
}