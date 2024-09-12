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

        public int[,] Fill(Vector2Int maxWorldSize)
        {
            for (int x = 0; x < maxWorldSize.x; x++)
            {

            }

            return default;
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
