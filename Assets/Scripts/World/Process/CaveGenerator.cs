using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorldCreation;

public class CaveGenerator : IWorldGeneratable
{
    private Tilemap _worldTilemap;

    public int ExecutionOrder { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public int[,] Execute(CaveLayer[] worldLayers, TerrainData terrain)
    {
        int[,] blocks = new int[worldLayers[0].MaxImpactAreaPosition.x, worldLayers[0].MaxImpactAreaPosition.y];
        int[,] layerBlocks = blocks;
        foreach (CaveLayer layer in worldLayers)
        {
            terrain.SetHeights(0, 0, GetDeletedBlock(layer));
        }

        return blocks;
    }

    public UniTask<Chunk> Execute(Chunk chunk, WorldMap worldMap, CancellationToken token)
    {
        throw new System.NotImplementedException();
    }

    private float[,] GetDeletedBlock(CaveLayer layer)
    {
        // 開始座標と終了座標を明示的に宣言
        Vector2Int StartPosition = layer.MinImpactAreaPosition;
        Vector2Int EndPosition = layer.MaxImpactAreaPosition - layer.MinImpactAreaPosition;

        Vector2 randomSeed = new(Random.Range(0, layer.Seed.x), Random.Range(0, layer.Seed.y));

        float[,] blocks = new float[layer.MaxImpactAreaPosition.x, layer.MaxImpactAreaPosition.x];

        for (int x = 0; x < EndPosition.x; x++)
        {
            for (int y = 0; y < EndPosition.y; y++)
            {
                Vector3Int targetTile = new(x, y);
                // 対象タイルが同じであれば次のタイル処理へ
                if (_worldTilemap.GetTile(targetTile) == layer.FillingTile) { continue; }

                float noise = Mathf.PerlinNoise
                (
                    x * layer.Frequency + randomSeed.x,
                    y * layer.Frequency + randomSeed.y
                );

                if (noise > 0.5)
                {
                    noise = 1 - noise;
                }

                blocks[x, y] = noise;

                if (noise > layer.Extent)
                {
                    _worldTilemap.SetTile(targetTile, layer.FillingTile);
                }
            }
        }
        return blocks;
    }
}