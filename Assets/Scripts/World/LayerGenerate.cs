using UnityEngine;

namespace WorldCreation
{
    public class LayerGenerate
    {
        private int _seed;

        public LayerGenerate(int seed)
        {
            _seed = seed;
        }

        public void Execute(ref int[,] worldTile, WorldMap worldMap)
        {
            // æ‚Éˆê”Ôã‚Ì‘w‚ÌF‚Å“h‚è‚Â‚Ô‚·
            // worldMap.WorldLayers[0].MaterialTiles

            for (int x = 0; x < worldMap.WorldSize.x; x++)
            {
                for (int i = 0; i < worldMap.WorldSize.y; i++)
                {

                }
            }
        }

        public Vector2Int[] GetBorder(int maxWorldWidth, int altitude, float noisePower, int randomLimit, float amplitude)
        {
            Vector2Int[] border = new Vector2Int[maxWorldWidth];
            int seed = _seed * Random.Range(1, randomLimit);
            for (int x = 0; x < maxWorldWidth; x++)
            {
                int noise = (int)(Mathf.PerlinNoise1D((x + seed) * amplitude) * noisePower);
                border[x] = new Vector2Int(x, noise + altitude);
            }

            return border;
        }
    }
}
