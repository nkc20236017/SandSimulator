using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace WorldCreation
{
    public class LayerGenerator : IWorldGeneratable
    {
        private int _seed;
        private int _executionOrder;

        private int[] _layerBorderRangeHeights; // 構造：[境界底辺0, 境界上限0, 境界底辺1, 境界上限1, ...]
        private int[] _layerHeights;
        private int[] _layerNoise;

        public int ExecutionOrder => _executionOrder;

        public void Initalize(Chunk chunk, WorldMap worldMap, int executionOrder)
        {
            _executionOrder = executionOrder;

            FindingLayerBorder(worldMap);
        }

        public async UniTask<Chunk> Execute(Chunk chunk, WorldMap worldMap, CancellationToken token)
        {
            // 現在のチャンクの下と上の座標を取得
            int worldLowerHeight = worldMap.OneChunkSize.y * chunk.Position.y;
            int worldUpperHeight = worldLowerHeight + (worldMap.OneChunkSize.y - 1);

            // 地層をまたいでいる場所の取得
            int[] straddles
                = _layerBorderRangeHeights
                .Where(y => worldLowerHeight <= y && y <= worldUpperHeight)
                .ToArray();

            // チャンクの下側がどの地層に存在しているか取得する
            int layerIndex = Array.IndexOf
            (
                _layerHeights
                .Concat(new int[] { worldLowerHeight })
                .OrderByDescending(y => y)
                .ToArray(),
                worldLowerHeight
            );

            // チャンクをまたいでいなければ地層のIDで塗りつぶす
            if (straddles.Length == 0)
            {
                for (int y = 0; y < chunk.GetChunkLength(1); y++)
                {
                    for (int x = 0; x < chunk.GetChunkLength(0); x++)
                    {
                        TileBase material = worldMap.WorldLayers[layerIndex].MaterialTile;
                        chunk.SetBlock
                        (
                            x,
                            y,
                            worldMap.Blocks.GetBlockID(material)
                        );

                        // チャンクの地層情報を書き込む
                        chunk.SetLayerIndex(x, y, layerIndex);
                    }
                }

                Debug.Log("<color=#00ff00ff>地層の生成処理終了</color>");
                return await UniTask.RunOnThreadPool(() => chunk);
            }

            // チャンクを跨いでいた場合地層の歪みを生成する
            for (int y = 0; y < chunk.GetChunkLength(1); y++)
            {
                for (int x = 0; x < chunk.GetChunkLength(0); x++)
                {
                    Vector2Int worldPosition = chunk.GetWorldPosition(x, y, worldMap.OneChunkSize);
                    int borderHeight = GetBorder(chunk, worldMap, worldPosition.x, layerIndex - 1);

                    borderHeight
                        = _layerHeights[layerIndex - 1]
                        - (int)worldMap.BorderDistortionPower
                        + borderHeight;

                    TileBase material;
                    if (borderHeight > worldPosition.y)
                    {
                        // 地層の境界より下であれば普通のタイル
                        material = worldMap.WorldLayers[layerIndex].MaterialTile;
                        chunk.SetBlock
                        (
                            x,
                            y,
                            worldMap.Blocks.GetBlockID(material)
                        );

                        // チャンクの地層情報を書き込む
                        chunk.SetLayerIndex(x, y, layerIndex);
                    }
                    else
                    {
                        material = worldMap.WorldLayers[layerIndex - 1].MaterialTile;
                        // 地層の境界より下であれば次のタイル
                        chunk.SetBlock
                        (
                            x,
                            y,
                            worldMap.Blocks.GetBlockID(material)
                        );

                        // チャンクの地層情報を書き込む
                        chunk.SetLayerIndex(x, y, layerIndex - 1);
                    }
                }
            }

            Debug.Log("<color=#00ff00ff>地層の生成処理終了</color>");
            return await UniTask.RunOnThreadPool(() => chunk);
        }

        private int GetBorder(Chunk chunk, WorldMap worldMap, int x, int layerNumber)
        {
            if (_layerNoise == null)
            {
                _layerNoise = new int[worldMap.LayerRatios.Length];
                for (int i = 0; i < _layerNoise.Length; i++)
                {
                    _layerNoise[i] = chunk.GetNoise(_executionOrder + i, Int16.MaxValue);
                }
            }

            return (int)
            (
                Mathf.PerlinNoise1D(x * worldMap.BorderAmplitude + _layerNoise[layerNumber])
                * worldMap.BorderDistortionPower
            );
        }

        private void FindingLayerBorder(WorldMap worldMap)
        {
            _layerHeights = new int[worldMap.LayerRatios.Length];
            _layerBorderRangeHeights = new int[worldMap.LayerRatios.Length * 2];
            int layerMaxRatio = worldMap.WorldSize.y;

            for (int i = 0; i < worldMap.LayerRatios.Length; i++)
            {
                _layerBorderRangeHeights[i] = (int)(layerMaxRatio * (1 - worldMap.LayerRatios[i]));
                _layerBorderRangeHeights[_layerBorderRangeHeights.Length - (i + 1)] = _layerBorderRangeHeights[i] + (int)worldMap.BorderDistortionPower;
                _layerHeights[i] = _layerBorderRangeHeights[i] + (int)worldMap.BorderDistortionPower;

                layerMaxRatio = _layerHeights[i];
            }
            _layerBorderRangeHeights = _layerBorderRangeHeights.OrderBy(i => i).ToArray();
        }
    }
}
