using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorldCreation;

public class CaveGenerator : IWorldGeneratable
{
    private const int VOID_ID = -1;

    private Tilemap _worldTilemap;
    private int _executionOrder;
    private int[] _noise;

    public int ExecutionOrder => _executionOrder;

    public void Initalize(Chunk chunk, WorldMap worldMap, int executionOrder)
    {
        _executionOrder = executionOrder;

        _noise = new int[worldMap.CaveCombines.Length];
        for (int i = 0; i < worldMap.CaveCombines.Length; i++)
        {
            _noise[i] = chunk.GetNoise(_executionOrder + i, Int16.MaxValue);
        }
    }

    public async UniTask<Chunk> Execute(Chunk chunk, WorldMap worldMap, CancellationToken token)
    {
        // 初期化して空IDにする
        int[,] grid = new int[chunk.GetChunkLength(0), chunk.GetChunkLength(1)];
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                grid[x, y] = VOID_ID;
            }
        }

        for (int i = 0; i < worldMap.CaveCombines.Length; i++)
        {
            CaveCombine caveCombine = worldMap.CaveCombines[i];

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    Vector2Int worldPosition = chunk.GetWorldPosition(x, y, worldMap.OneChunkSize);
                    // 現在の座標にタイルが既にあったら次の座標へ移動する
                    if (!caveCombine.IsOrverride && grid[x, y] != VOID_ID) { continue; }

                    // 現在の座標のノイズ値を取得
                    float noisePower = Mathf.PerlinNoise
                    (
                        (worldPosition.x + _noise[i]) * caveCombine.Scale.x,
                        (worldPosition.y + _noise[i]) * caveCombine.Scale.y
                    );

                    // 道の太さを決める
                    if (noisePower > caveCombine.HollowSize)
                    {
                        noisePower = 1f - noisePower;
                    }

                    int fillBlockId = 0;
                    if (caveCombine.IsBackfill && 0 < caveCombine.BackfillTileID)
                    {
                        fillBlockId = caveCombine.BackfillTileID;
                    }
                    else if (caveCombine.IsBackfill)
                    {
                        TileBase material = worldMap.WorldLayers[chunk.GetLayerIndex(x, y)].MaterialTile;
                        fillBlockId = worldMap.Blocks.GetBlockID(material);
                    }

                    if (caveCombine.IsInvert)
                    {
                        // 対象の場所を埋める場合は
                        grid[x, y] = (noisePower < caveCombine.LumpSize)
                            ? fillBlockId
                            : VOID_ID;
                    }
                    else
                    {
                        grid[x, y] = (noisePower < caveCombine.LumpSize)
                            ? VOID_ID
                            : fillBlockId;
                    }
                }
            }
        }

        // 結果をチャンクに反映
        for (int y = 0; y < chunk.GetChunkLength(1); y++)
        {
            for (int x = 0; x < chunk.GetChunkLength(0); x++)
            {
                if (grid[x, y] == VOID_ID) { continue; }

                chunk.SetBlock(x, y, grid[x, y]);
            }
        }

        Debug.Log("<color=#00ff00ff>洞窟の処理終了</color>");
        return await UniTask.RunOnThreadPool(() => chunk);
    }
}