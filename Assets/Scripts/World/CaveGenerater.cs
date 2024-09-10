using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorldCreation;

public class CaveGenerater// : IWorldGeneratable
{
    private Tilemap _worldTilemap;

    public CaveGenerater(Tilemap worldTilemap)
    {
        _worldTilemap = worldTilemap;
    }

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

    private float[,] GetDeletedBlock(CaveLayer layer)
    {
        // �J�n���W�ƏI�����W�𖾎��I�ɐ錾
        Vector2Int StartPosition = layer.MinImpactAreaPosition;
        Vector2Int EndPosition = layer.MaxImpactAreaPosition - layer.MinImpactAreaPosition;

        Vector2 randomSeed = new(Random.Range(0, layer.Seed.x), Random.Range(0, layer.Seed.y));

        float[,] blocks = new float[layer.MaxImpactAreaPosition.x, layer.MaxImpactAreaPosition.x];

        for (int x = 0; x < EndPosition.x; x++)
        {
            for (int y = 0; y < EndPosition.y; y++)
            {
                Vector3Int targetTile = new(x, y);
                // �Ώۃ^�C���������ł���Ύ��̃^�C��������
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