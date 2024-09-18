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

        private int[] _layerBorderRangeHeights; // �\���F[���E���0, ���E���0, ���E���1, ���E���1, ...]

        public int ExecutionOrder
        {
            get => _executionOrder;
            set => _executionOrder = value;
        }

        public async UniTask<Chunk> Execute(Chunk chunk, WorldMap worldMap, CancellationToken token)
        {
            // �n�w�̋��E�͈͂�������Ȃ��ꍇ�͐V�K�쐬����
            if (_layerBorderRangeHeights == null)
            {
                FindingLayerBorder(worldMap);
            }

            int worldHeight = worldMap.OneChunkSize.y * chunk.Position.y;
            // �`�����N���n�w�̋��E���ɂ܂������Ă��邩���肷��
            int layerPositionIndex = Array.IndexOf
            (
                _layerBorderRangeHeights
                .Concat(new int[] { worldHeight })
                .OrderByDescending(y => y)
                .ToArray(),
                worldHeight
            );



            // �`�����N���܂����ł��Ȃ���Βn�w��ID�œh��Ԃ�
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
            // ��Ɉ�ԏ�̑w�̐F�œh��Ԃ�
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
            for (int i = 0; i < worldMap.LayerRatios.Length; i += 2)    // ��x��2�̃f�[�^�����邽��2�㏸������
            {
                _layerBorderRangeHeights[i] = (int)(worldMap.WorldSize.y * worldMap.LayerRatios[i]);
                _layerBorderRangeHeights[i + 1] = _layerBorderRangeHeights[i] + (int)worldMap.BorderDistortionPower;
            }
        }
    }
}
