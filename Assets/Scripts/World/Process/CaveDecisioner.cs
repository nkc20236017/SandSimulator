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
        // ���������ċ�ID�ɂ���
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
                    // ���݂̍��W�Ƀ^�C�������ɂ������玟�̍��W�ֈړ�����
                    if (!caveProcedures.IsOrverride && grid[x, y] != VOID_ID) { continue; }

                    // ���݂̍��W�̃m�C�Y�l���擾
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
                        caveProcedures.LumpThreshold + 0.01f    // MN 0.01f = �����l�ł͐���������Ȃ����߁A�����̗P�\���������邽�߂̔��ʂȒl
                    );
                    // ���]������ꏊ�����߂�
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
                        // �Ώۂ̏ꏊ�𖄂߂�ꍇ��
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

        // ���ʂ��`�����N�ɔ��f
        for (int y = 0; y < _gameChunk.Size.y; y++)
        {
            for (int x = 0; x < _gameChunk.Size.x; x++)
            {
                if (grid[x, y] == VOID_ID) { continue; }

                _gameChunk.SetBlock(x, y, grid[x, y]);
            }
        }

        Debug.Log("<color=#00ff00ff>���A�̏����I��</color>");
        return await UniTask.RunOnThreadPool(() => _gameChunk);
    }
}