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

    public int ExecutionOrder
    {
        get => _executionOrder;
        set => _executionOrder = value;
    }

    public async UniTask<Chunk> Execute(Chunk chunk, WorldMap worldMap, CancellationToken token)
    {
        // �����𐶐�����
        if (_noise == null)
        {
            _noise = new int[worldMap.CaveCombine.Length];
            for (int i = 0; i < worldMap.CaveCombine.Length; i++)
            {
                _noise[i] = chunk.GetNoise(_executionOrder, Int16.MaxValue);
            }
        }

        // ����������-1�ɂ���
        int[,] grid = new int[chunk.GetChunkLength(0), chunk.GetChunkLength(1)];
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                grid[x, y] = -1;
            }
        }

        for (int i = 0; i < worldMap.CaveCombine.Length; i++)
        {
            CaveCombineData caveCombine = worldMap.CaveCombine[i];

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    Vector2Int worldPosition = chunk.GetWorldPosition(x, y, worldMap.OneChunkSize);
                    // ���݂̍��W�Ƀ^�C�������ɂ������玟�̍��W�ֈړ�����
                    if (grid[x, y] != 0) { continue; }

                    // ���݂̍��W�̃m�C�Y�l���擾
                    float noisePower = Mathf.PerlinNoise
                    (
                        (worldPosition.x + _noise[i]) * caveCombine.Scale.x,
                        (worldPosition.y + _noise[i]) * caveCombine.Scale.y
                    );

                    // ���̑��������߂�
                    if (noisePower > caveCombine.HollowSize)
                    {
                        noisePower = 1f - noisePower;
                    }

                    int fillBlockId = 0;
                    if (caveCombine.IsBackfill && 0 <= caveCombine.BackfillTileID)
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
                        // �Ώۂ̏ꏊ�𖄂߂�ꍇ��
                        grid[x, y] = (noisePower > caveCombine.ClodSize)
                            ? fillBlockId
                            : VOID_ID;
                    }
                    else
                    {
                        grid[x, y] = (noisePower > caveCombine.ClodSize)
                            ? VOID_ID
                            : fillBlockId;
                    }
                }
            }
        }

        // ���ʂ��`�����N�ɔ��f
        for (int y = 0; y < chunk.GetChunkLength(1); y++)
        {
            for (int x = 0; x < chunk.GetChunkLength(0); x++)
            {
                if (grid[x, y] == VOID_ID) { continue; }

                chunk.SetBlock(x, y, grid[x, y]);
            }
        }

        Debug.Log("<color=#00ff00ff>���A�̏����I��</color>");
        return await UniTask.RunOnThreadPool(() => chunk);
    }
}