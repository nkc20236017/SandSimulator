using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace WorldCreation
{
    public class LayerGenerate : IWorldGeneratable
    {
        private int _seed;
        private int _executionOrder;

        private int[] _layerBorderRangeHeights; // \‘¢F[‹«ŠE’ê•Ó0, ‹«ŠEãŒÀ0, ‹«ŠE’ê•Ó1, ‹«ŠEãŒÀ1, ...]

        public int ExecutionOrder
        {
            get => _executionOrder;
            set => _executionOrder = value;
        }

        public async UniTask<Chunk> Execute(Chunk chunk, WorldMap worldMap, CancellationToken token)
        {
            // ’n‘w‚Ì‹«ŠE”ÍˆÍ‚ª•ª‚©‚ç‚È‚¢ê‡‚ÍV‹Kì¬‚·‚é
            if (_layerBorderRangeHeights == null)
            {
                FindingLayerBorder(worldMap);
            }

            int worldHeight = worldMap.OneChunkSize.y * chunk.Position.y;
            // ƒ`ƒƒƒ“ƒN‚ª’n‘w‚Ì‹«ŠEü‚É‚Ü‚½‚ª‚Á‚Ä‚¢‚é‚©”»’è‚·‚é
            int layerPositionIndex = Array.IndexOf
            (
                _layerBorderRangeHeights
                .Concat(new int[] { worldHeight })
                .OrderByDescending(y => y)
                .ToArray(),
                worldHeight
            );



            // ƒ`ƒƒƒ“ƒN‚ğ‚Ü‚½‚¢‚Å‚¢‚È‚¯‚ê‚Î’n‘w‚ÌID‚Å“h‚è‚Â‚Ô‚·
            if (layerPositionIndex % 2 == 0)
            {
                for (int y = 0; y < chunk.GetChunkLength(1); y++)
                {
                    for (int x = 0; x < chunk.GetChunkLength(0); x++)
                    {
                        chunk.SetBlock
                        (
                            x,
                            y,
                            worldMap.WorldLayers[layerPositionIndex / 2].MaterialTileID
                        );

                    }
                }
            }

            return await UniTask.RunOnThreadPool(() => chunk);
        }

        public Vector2Int[] GetBorder(int maxWorldWidth, int altitude, float noisePower, int randomLimit, float amplitude)
        {
            Vector2Int[] border = new Vector2Int[maxWorldWidth];
            int seed = _seed * UnityEngine.Random.Range(1, randomLimit);
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

        private void FindingLayerBorder(WorldMap worldMap)
        {
            _layerBorderRangeHeights = new int[worldMap.LayerRatios.Length * 2];
            for (int i = 0; i < worldMap.LayerRatios.Length; i += 2)    // ˆê“x‚É2‚Â‚Ìƒf[ƒ^‚ğ“ü‚ê‚é‚½‚ß2ã¸‚³‚¹‚é
            {
                _layerBorderRangeHeights[i] = (int)(worldMap.WorldSize.y * worldMap.LayerRatios[i]);
                _layerBorderRangeHeights[i + 1] = _layerBorderRangeHeights[i] + (int)worldMap.BorderDistortionPower;
            }
        }
    }
}
