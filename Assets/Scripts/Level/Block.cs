using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public struct Block
    {
        public static readonly Dictionary<BlockType, Vector2Int> tiles = new Dictionary<BlockType, Vector2Int>
        {
            {BlockType.Air, new Vector2Int(0, 0)},
            {BlockType.Snow, new Vector2Int(3, 0)},
            {BlockType.Ice, new Vector2Int(2, 0)},
            {BlockType.Ore, new Vector2Int(1, 0)},
            {BlockType.Stone, new Vector2Int(0, 0)}
        };
        
        public static readonly Dictionary<BlockType, int> health = new Dictionary<BlockType, int>
        {
            {BlockType.Snow, 1},
            {BlockType.Ore, 3},
            {BlockType.Ice, 5},
            {BlockType.Stone, 7}
        };

        public readonly BlockType type;

        public Block(BlockType type = BlockType.Air)
        {            
            this.type = type;
        }
        
        public bool IsSolid()
        {
            return type != BlockType.Air;
        }        
    }
}