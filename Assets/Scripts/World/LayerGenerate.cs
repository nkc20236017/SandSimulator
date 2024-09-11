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

        public Vector2Int[] GetBorder(int maxWorldWidth, int altitude, float noisePower, float randomLimit)
        {
            Vector2Int[] border = new Vector2Int[maxWorldWidth];
            for (int x = 0; x < maxWorldWidth; x++)
            {
                int noise = (int)(Mathf.PerlinNoise1D(x * 0.01f) * noisePower);
                border[x] = new Vector2Int(x, noise + altitude);
            }

            return border;
        }
    }
}
