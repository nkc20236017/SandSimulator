using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveGenerater : IWorldGeneratable
{
    private Tilemap _worldTilemap;

    public CaveGenerater(Tilemap worldTilemap)
    {
        _worldTilemap = worldTilemap;
    }

    public void Execute(WorldLayer[] worldLayers)
    {
        foreach (WorldLayer layer in worldLayers)
        {
            DeleteLayerBlock(layer);
        }
    }

    private void DeleteLayerBlock(WorldLayer layer)
    {
        // �J�n���W�ƏI�����W�𖾎��I�ɐ錾
        Vector2Int StartPosition = layer.MinImpactAreaPosition;
        Vector2Int EndPosition = layer.MinImpactAreaPosition + layer.MaxImpactAreaPosition;

        Vector2 randomSeed = new(Random.Range(0, layer.Seed.x), Random.Range(0, layer.Seed.y));

        for (int x = StartPosition.x; x < EndPosition.x; x++)
        {
            for (int y = StartPosition.y; y < EndPosition.y; y++)
            {
                Vector3Int targetTile = new(x, y);
                // �Ώۃ^�C���������ł���Ύ��̃^�C��������
                if (_worldTilemap.GetTile(targetTile) == layer.FillingTile) { continue; }

                float noise = Mathf.PerlinNoise
                (
                    x * layer.Frequency + randomSeed.x,
                    y * layer.Frequency + randomSeed.y
                );

                if (layer.Extent < noise)
                {
                    _worldTilemap.SetTile(targetTile, layer.FillingTile);
                }
            }
        }
    }
}
