using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using RandomExtensions;

namespace WorldCreation
{
    public class LayerDecisioner : WorldDecisionerBase
    {
        private int[] _layerBorderRangeHeights; // 構造：[境界底辺0, 境界上限0, 境界底辺1, 境界上限1, ...]
        private int[] _layerHeights;
        private int[] _layerNoise;

        public override void Initalize(GameChunk chunk, WorldCreatePrinciple createPrinciple, IRandom managedRandom)
        {
            // 初期化処理を実行
            base.Initalize(chunk, createPrinciple, managedRandom);

            FindingLayerBorder(_createPrinciple.LayerDecision);
        }

        public override async UniTask<GameChunk> Execute(CancellationToken token)
        {
            // 地層用のデータ取得
            LayerDecisionData layerDecision = _createPrinciple.LayerDecision;
            // 現在のチャンクの下と上の座標を取得
            int worldLowerHeight = _gameChunk.Size.y * _gameChunk.GameChunkPosition.y;
            int worldUpperHeight = worldLowerHeight + (_gameChunk.Size.y - 1);

            // 地層をまたいでいる場所の取得
            int[] straddles = _layerBorderRangeHeights
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
                for (int y = 0; y < _gameChunk.Size.y; y++)
                {
                    for (int x = 0; x < _gameChunk.Size.x; x++)
                    {
                        TileBase material = layerDecision.WorldLayers[layerIndex].MaterialTile;
                        _gameChunk.SetBlock
                        (
                            x,
                            y,
                            _createPrinciple.Blocks.GetBlockID(material)
                        );

                        // チャンクの地層情報を書き込む
                        _gameChunk.SetLayerIndex(x, y, layerIndex);
                    }
                }

                Debug.Log("<color=#00ff00ff>地層の生成処理終了</color>");
                return await UniTask.RunOnThreadPool(() => _gameChunk);
            }

            // チャンクを跨いでいた場合地層の歪みを生成する
            for (int y = 0; y < _gameChunk.Size.y; y++)
            {
                for (int x = 0; x < _gameChunk.Size.x; x++)
                {
                    // 地層の境界線の位置を取得する
                    Vector2Int worldPosition = _gameChunk.RawGameChunkPositionToWorldPosition(x, y);
                    int borderHeight = GetBorder(_gameChunk, layerDecision, worldPosition.x, layerIndex - 1);

                    borderHeight
                        = _layerHeights[layerIndex - 1]
                        - (int)layerDecision.BorderDistortionPower
                        + borderHeight;

                    TileBase material;
                    if (borderHeight > worldPosition.y)
                    {
                        // 地層の境界より下であれば普通のタイル
                        material = layerDecision.WorldLayers[layerIndex].MaterialTile;
                        _gameChunk.SetBlock
                        (
                            x,
                            y,
                            _createPrinciple.Blocks.GetBlockID(material)
                        );

                        // チャンクの地層情報を書き込む
                        _gameChunk.SetLayerIndex(x, y, layerIndex);
                    }
                    else
                    {
                        material = layerDecision.WorldLayers[layerIndex - 1].MaterialTile;
                        // 地層の境界より下であれば次のタイル
                        _gameChunk.SetBlock
                        (
                            x,
                            y,
                            _createPrinciple.Blocks.GetBlockID(material)
                        );

                        // チャンクの地層情報を書き込む
                        _gameChunk.SetLayerIndex(x, y, layerIndex - 1);
                    }
                }
            }

            Debug.Log("<color=#00ff00ff>地層の生成処理終了</color>");
            return await UniTask.RunOnThreadPool(() => _gameChunk);
        }

        // TODO: 地層の判定を別メソッドで作る

        private int GetBorder(GameChunk chunk, LayerDecisionData layerDecision, int x, int layerNumber)
        {
            int[] layerNoise = new int[layerDecision.LayerRatios.Length];
            for (int i = 0; i < layerNoise.Length; i++)
            {
                layerNoise[i] = _random.NextInt(Int16.MinValue, Int16.MaxValue);
            }

            return (int)
            (
                Mathf.PerlinNoise1D(x * layerDecision.BorderAmplitude + layerNoise[layerNumber])
                    * layerDecision.BorderDistortionPower
            );
        }

        private void FindingLayerBorder(LayerDecisionData layerDecision)
        {
            int layerMaxRatio = GetWorldScale().y;
            // 地層の数分の配列を作成
            _layerHeights = new int[layerDecision.LayerRatios.Length];
            _layerBorderRangeHeights = new int[layerDecision.LayerRatios.Length * 2];

            for (int i = 0; i < layerDecision.LayerRatios.Length; i++)
            {
                // それぞれの地層の割り合いを求める
                _layerBorderRangeHeights[i]
                    = (int)(layerMaxRatio
                    * (1 - layerDecision.LayerRatios[i]));

                _layerBorderRangeHeights[_layerBorderRangeHeights.Length - (i + 1)]
                    = _layerBorderRangeHeights[i]
                    + (int)layerDecision.BorderDistortionPower;

                _layerHeights[i]
                    = _layerBorderRangeHeights[i]
                    + (int)layerDecision.BorderDistortionPower;

                layerMaxRatio = _layerHeights[i];
            }
            // 地層の境界線を高さ順に並び変える
            _layerBorderRangeHeights = _layerBorderRangeHeights.OrderBy(i => i).ToArray();
        }
    }
}
