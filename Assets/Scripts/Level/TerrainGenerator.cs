using Utils;

namespace Level
{
    public static class TerrainGenerator
    {
        const int snowPeakLevel = 80;
        const int iceLevel = 50;
        
        const int mountainsHeight = 100;
        const int rocksHeight = 15;
        const int oreHeight = 70;
        const int snowHeight = 10;
        const int caveSize = 30;
        
        const float mountainsFreq = 0.004f;
        const float rocksFreq = 0.06f;
        const float oreFreq = 0.033f;
        const float snowFreq = 0.09f;
        const float caveFreq = 0.025f;

        // TODO: Too heavy! Should interpolate between noises
        public static void GenerateChunk(Chunk chunk)
        {
            for (var x = 0; x < Chunk.size; x++)
            for (var z = 0; z < Chunk.size; z++)
            {
                var worldX = chunk.chunkPosition.x + x;
                var worldZ = chunk.chunkPosition.z + z;

                var mountain = GetMountainHeight(worldX, worldZ);
                var snow = mountain + GetSnowHeight(worldX, worldZ);
                var rocks = mountain + GetRocksHeight(worldX, worldZ);
                var ore = GetOreHeight(worldX, worldZ);
                var snowPeak = 2 * mountain - snowPeakLevel;                

                for (var y = 0; y < Chunk.size; y++)
                {
                    var worldY = chunk.chunkPosition.y + y;
                    var caveChance = GetNoise(worldX, worldY, worldZ, caveFreq, 100);

                    if (caveSize >= caveChance) chunk.blocks[x, y, z] = new Block(BlockType.Air);                  
                    else if (worldY <= rocks) chunk.blocks[x, y, z] = new Block(BlockType.Stone);
                    else if (worldY <= snowPeak) chunk.blocks[x, y, z] = new Block(BlockType.Snow);
                    else if (worldY <= ore) chunk.blocks[x, y, z] = new Block(BlockType.Ore);
                    else if (worldY <= snow) chunk.blocks[x, y, z] = new Block(BlockType.Snow);
                    else if (worldY <= iceLevel) chunk.blocks[x, y, z] = new Block(BlockType.Ice);
                    else chunk.blocks[x, y, z] = new Block(BlockType.Air);
                }
            }
        }
        
        static int GetNoise(int x, int y, int z, float scale, int max)
        {
            var normalized = (Noise.Generate(x * scale, y * scale, z * scale) + 1f) / 2;
            return (int) (max * normalized);
        }
        
        static int GetNoise(int x, int y, float scale, int max)
        {            
            var normalized = (Noise.Generate(x * scale, 0,   y * scale) + 1f) / 2;
            return (int) (max * normalized);
        }
        
        static float GetMountainHeight(int x, int y)
        {
            return GetNoise(x, y, mountainsFreq, mountainsHeight);
        }

        static float GetSnowHeight(int x, int y)
        {
            return GetNoise(x, y, snowFreq, snowHeight);
        }
        
        static float GetRocksHeight(int x, int y)
        {
            return GetNoise(x, y, rocksFreq, rocksHeight);
        }

        static float GetOreHeight(int x, int y)
        {
            return GetNoise(x, y, oreFreq, oreHeight);
        }

        static float GetCaveHeight(int x, int y, int z)
        {
            return GetNoise(x, y, z, caveFreq, 100);
        }
    }
}