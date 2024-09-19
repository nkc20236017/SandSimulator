using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEditor.U2D.Aseprite;

namespace WorldCreation
{
    public class LayerGenerate : IWorldGeneratable
    {
        private int _seed;
        private int _executionOrder;

        private int[] _layerBorderRangeHeights; // 構造：[境界底辺0, 境界上限0, 境界底辺1, 境界上限1, ...]
        private int[] _layerHeights;
        private int[] _layerNoise;

        public int ExecutionOrder
        {
            get => _executionOrder;
            set => _executionOrder = value;
        }

        public async UniTask<Chunk> Execute(Chunk chunk, WorldMap worldMap, CancellationToken token)
        {
            // 地層の境界範囲が分からない場合は新規作成する
            if (_layerBorderRangeHeights == null)
            {
                FindingLayerBorder(worldMap);
            }

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
                        chunk.SetBlock
                        (
                            x,
                            y,
                            worldMap.WorldLayers[layerIndex].MaterialTileID
                        );
                    }
                }

                return await UniTask.RunOnThreadPool(() => chunk);
            }

            // チャンクを跨いでいた場合地層の歪みを生成する
            for (int y = 0; y < chunk.GetChunkLength(1); y++)
            {
                for (int x = 0; x < chunk.GetChunkLength(0); x++)
                {
                    int worldPosition = chunk.Position.x * worldMap.OneChunkSize.x + x;
                    int borderHeight = GetBorder(chunk, worldMap, worldPosition, layerIndex - 1);

                    if (borderHeight > y)
                    {
                        // 地層の境界より下であれば普通のタイル
                        chunk.SetBlock
                        (
                            x,
                            y,
                            worldMap.WorldLayers[layerIndex].MaterialTileID
                        );
                    }
                    else
                    {
                        // 地層の境界より下であれば次のタイル
                        chunk.SetBlock
                        (
                            x,
                            y,
                            worldMap.WorldLayers[layerIndex - 1].MaterialTileID
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

        private int GetBorder(Chunk chunk, WorldMap worldMap, int x, int layerNumber)
        {
            if (_layerNoise == null)
            {
                _layerNoise = new int[worldMap.LayerRatios.Length];
                for (int i = 0; i < _layerNoise.Length; i++)
                {
                    _layerNoise[i] = chunk.GetNoise(_executionOrder + i) / 1000;
                }
            }
            
            return (int)
            (
                Mathf.PerlinNoise1D((x + _layerNoise[layerNumber]) * worldMap.BorderAmplitude)
                * worldMap.BorderDistortionPower
            );
        }

        private void FindingLayerBorder(WorldMap worldMap)
        {
            _layerHeights = new int[worldMap.LayerRatios.Length];
            _layerBorderRangeHeights = new int[worldMap.LayerRatios.Length * 2];
            int layerMaxRatio = worldMap.WorldSize.y;

            for (int i = 0; i < worldMap.LayerRatios.Length; i++)    // 一度に2つのデータを入れるため2上昇させる
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
