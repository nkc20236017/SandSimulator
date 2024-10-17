using Cysharp.Threading.Tasks;
using RandomExtensions;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorldCreation;

public class CaveDecisioner : WorldDecisionerBase
{
    private const int VOID_ID = -1;

    public override async UniTask<GameChunk> Execute(CancellationToken token)
    {
        // 初期化して空IDにする
        int[,] grid = new int[_gameChunk.Size.x, _gameChunk.Size.y];
        for (int y = 0; y < _gameChunk.Size.y; y++)
        {
            for (int x = 0; x < _gameChunk.Size.x; x++)
            {
                grid[x, y] = VOID_ID;
            }
        }

        for (int i = 0; i < _createPrinciple.CaveDecision.CaveProceduresGathering.Length; i++)
        {
            float noiseRandomX = _random.NextInt(Int16.MinValue, Int16.MaxValue);
            float noiseRandomY = _random.NextInt(Int16.MinValue, Int16.MaxValue);

            CaveProcedures caveProcedures = _createPrinciple.CaveDecision.CaveProceduresGathering[i];
            for (int y = 0; y < _gameChunk.Size.y; y++)
            {
                for (int x = 0; x < _gameChunk.Size.x; x++)
                {
                    Vector2Int worldPosition = _gameChunk.RawGameChunkPositionToWorldPosition(x, y);
                    // 現在の座標にタイルが既にあったら次の座標へ移動する
                    if (!caveProcedures.IsOrverride && grid[x, y] != VOID_ID) { continue; }

                    // 現在の座標のノイズ値を取得
                    float noisePower = Mathf.PerlinNoise
                    (
                        worldPosition.x * caveProcedures.Scale.x
                            + noiseRandomX,
                        worldPosition.y * caveProcedures.Scale.y
                            + noiseRandomY
                    );

                    float cropValue = Mathf.Max
                    (
                        caveProcedures.HollowThreshold,
                        caveProcedures.LumpThreshold + 0.01f    // MN 0.01f = 同じ値では生成がされないため、多少の猶予を持たせるための微量な値
                    );
                    // 反転させる場所を決める
                    if (noisePower > cropValue)
                    {
                        noisePower = 1f - noisePower;
                    }

                    int fillBlockId = 0;
                    int blockID = _createPrinciple.Blocks.GetBlockID(caveProcedures.BackfillTile);
                    if (caveProcedures.IsBackfill && 0 < blockID)
                    {
                        fillBlockId = blockID;
                    }
                    else if (caveProcedures.IsBackfill)
                    {
                        TileBase material = _createPrinciple.LayerDecision.WorldLayers[_gameChunk.GetLayerIndex(x, y)].MaterialTile;
                        fillBlockId = _createPrinciple.Blocks.GetBlockID(material);
                    }

                    if (caveProcedures.IsInvert)
                    {
                        // 対象の場所を埋める場合は
                        grid[x, y] = (noisePower < caveProcedures.LumpThreshold)
                            ? fillBlockId
                            : VOID_ID;
                    }
                    else
                    {
                        grid[x, y] = (noisePower < caveProcedures.LumpThreshold)
                            ? VOID_ID
                            : fillBlockId;
                    }
                }
            }
        }

        // 結果をチャンクに反映
        for (int y = 0; y < _gameChunk.Size.y; y++)
        {
            for (int x = 0; x < _gameChunk.Size.x; x++)
            {
                if (grid[x, y] == VOID_ID) { continue; }

                _gameChunk.SetBlock(x, y, grid[x, y]);
            }
        }

        Debug.Log("<color=#00ff00ff>洞窟の処理終了</color>");
        return await UniTask.RunOnThreadPool(() => _gameChunk);
    }
}