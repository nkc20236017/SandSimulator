using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorldCreation;

public class CaveDecisioner : WorldDecisionerBase
{
    private const int VOID_ID = -1;

    public override void Initalize(GameChunk gameChunk, WorldCreatePrinciple createPrinciple, ManagedRandom managedRandom)
    {
        // 初期化処理をする
        base.Initalize(gameChunk, createPrinciple, managedRandom);
    }

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

        foreach (CaveProcedures caveProcedures in _createPrinciple.CaveDecision.CaveProceduresGathering)
        {
            for (int y = 0; y < _gameChunk.Size.y; y++)
            {
                for (int x = 0; x < _gameChunk.Size.x; x++)
                {
                    Vector2Int worldPosition = _gameChunk.GetRawWorldPosition(x, y);
                    // 現在の座標にタイルが既にあったら次の座標へ移動する
                    if (!caveProcedures.IsOrverride && grid[x, y] != VOID_ID) { continue; }

                    // 現在の座標のノイズ値を取得
                    float noisePower = Mathf.PerlinNoise
                    (
                        worldPosition.x * caveProcedures.Scale.x + _managedRandom.NextFloat(float.MinValue, float.MaxValue),
                        worldPosition.y * caveProcedures.Scale.y + _managedRandom.NextFloat(float.MinValue, float.MaxValue)
                    );

                    // 反転させる場所を決める
                    if (noisePower > caveProcedures.HollowThreshold)
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