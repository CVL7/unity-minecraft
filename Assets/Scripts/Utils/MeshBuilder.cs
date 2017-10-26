using System.Collections.Generic;
using Level;
using UnityEngine;

namespace Utils
{
    public static class MeshBuilder
    {
        const float tileSize = 0.25f;

        static readonly Vector3 v0 = new Vector3(0, 0, 1);
        static readonly Vector3 v1 = new Vector3(1, 0, 1);
        static readonly Vector3 v2 = new Vector3(1, 0, 0);
        static readonly Vector3 v3 = new Vector3(0, 0, 0);
        static readonly Vector3 v4 = new Vector3(0, 1, 1);
        static readonly Vector3 v5 = new Vector3(1, 1, 1);
        static readonly Vector3 v6 = new Vector3(1, 1, 0);
        static readonly Vector3 v7 = new Vector3(0, 1, 0);

        static readonly Dictionary<Vector3i, Vector3[]> sideVertices = new Dictionary<Vector3i, Vector3[]>
        {
            {new Vector3i(0, 0, 1), new[] {v4, v5, v1, v0}},
            {new Vector3i(0, 0, -1), new[] {v6, v7, v3, v2}},
            {new Vector3i(-1, 0, 0), new[] {v7, v4, v0, v3}},
            {new Vector3i(1, 0, 0), new[] {v5, v6, v2, v1}},
            {new Vector3i(0, 1, 0), new[] {v7, v6, v5, v4}},
            {new Vector3i(0, -1, 0), new[] {v0, v1, v2, v3}}
        };

        static readonly Vector3i[] sides =
        {
            new Vector3i(0, 0, 1),
            new Vector3i(0, 0, -1),
            new Vector3i(-1, 0, 0),
            new Vector3i(1, 0, 0),
            new Vector3i(0, 1, 0),
            new Vector3i(0, -1, 0)
        };

        static readonly List<Vector3> verts = new List<Vector3>();
        static readonly List<int> tris = new List<int>();
        static readonly List<Vector2> uvs = new List<Vector2>();

        public static Rect GetTileUVsRect(Vector2Int tilePosition)
        {
            var x = tileSize * tilePosition.x;
            var y = tileSize * tilePosition.y;
            return new Rect(new Vector2(x, y), new Vector2(tileSize, tileSize));
        } 
        
        public static void BuildChunk(Chunk chunk, Mesh mesh)
        {
            verts.Clear();
            tris.Clear();
            uvs.Clear();
            for (var x = 0; x < Chunk.size; x++)
            for (var y = 0; y < Chunk.size; y++)
            for (var z = 0; z < Chunk.size; z++)
            {
                var block = chunk.GetBlock(x, y, z);
                if (block.IsSolid())
                {
                    PlaceBlock(chunk, block, new Vector3i(x, y, z));
                }
            }
            AddDataToMesh(mesh);
        }

        public static void BuildBlock(Vector3 origin, Mesh mesh)
        {
            verts.Clear();
            tris.Clear();
            uvs.Clear();
            foreach (var side in sides)
            {
                AddSideVertices(origin, side);
                AddSideUVs(Vector2Int.zero);
                AddSideTriangles(verts.Count - 1);
            }
            AddDataToMesh(mesh);
        }
        
        public static void ChangeBlockUVs(Mesh mesh, Vector2Int tilePosition)
        {
            uvs.Clear();
            for (var i = 0; i < sides.Length; i++)
            {
                AddSideUVs(tilePosition);
            }
            mesh.SetUVs(0, uvs);
        }   
        
        static void AddDataToMesh(Mesh mesh)
        {
            mesh.Clear();
            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
        }

        static void PlaceBlock(Chunk chunk, Block block, Vector3i position)
        {
            foreach (var side in sides)
            {
                if (ShouldSideBeRendered(chunk, position, side))
                {
                    AddSideVertices(position, side);
                    AddSideUVs(Block.tiles[block.type]);
                    AddSideTriangles(verts.Count - 1);
                }
            }
        }
        
        static bool ShouldSideBeRendered(Chunk chunk, Vector3i position, Vector3i side)
        {
            // Vector computation is too heavy
            var x = position.x + side.x;
            var y = position.y + side.y;
            var z = position.z + side.z;
            var block = chunk.GetBlock(x, y, z);
            return !block.IsSolid();
        }

        static void AddSideTriangles(int verticesOffset)
        {
            tris.Add(verticesOffset);
            tris.Add(verticesOffset - 2);
            tris.Add(verticesOffset - 3);
            tris.Add(verticesOffset);
            tris.Add(verticesOffset - 1);
            tris.Add(verticesOffset - 2);
        }

        static void AddSideVertices(Vector3 origin, Vector3i side)
        {
            for (var i = 0; i < 4; i++)
            {
                verts.Add(origin + sideVertices[side][i]);
            }
        }

        static void AddSideUVs(Vector2Int tilePosition)
        {
            var x = tileSize * tilePosition.x;
            var y = tileSize * tilePosition.y;
            uvs.Add(new Vector2(x + tileSize, y));
            uvs.Add(new Vector2(x + tileSize, y + tileSize));
            uvs.Add(new Vector2(x, y + tileSize));
            uvs.Add(new Vector2(x, y));
        }    
    }
}