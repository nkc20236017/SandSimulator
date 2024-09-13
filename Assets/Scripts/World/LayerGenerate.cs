using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldCreation
{
    public class LayerGenerate : IWorldGeneratable
    {
        private int _seed;

        public LayerGenerate(int seed)
        {
            _seed = seed;
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

        public async UniTask<TileBase[,]> Execute(TileBase[,] worldTile, WorldMap worldMap, CancellationToken token)
        {
            // æ‚Éˆê”Ôã‚Ì‘w‚ÌF‚Å“h‚è‚Â‚Ô‚·
            // worldMap.WorldLayers[0].MaterialTiles
            for (int i = 0; i < worldMap.WorldLayers.Length; i++)
            {
                if (i == 0)
                {

                }
            }

            for (int x = 0; x < worldMap.WorldSize.x; x++)
            {
                for (int i = 0; i < worldMap.WorldSize.y; i++)
                {

                }
            }
            await UniTask.Yield();
            return await UniTask.RunOnThreadPool(() => worldTile);
        }

        private void LayerFill(int maxWorldWidth, int layerBorderHeight, TileBase[] tiles, float[] tileWeights)
        {
            for (int x = 0; x < maxWorldWidth; x++)
            {
                for (int y = 0; y < layerBorderHeight; y++)
                {

                }
            }
        }
    }
}
