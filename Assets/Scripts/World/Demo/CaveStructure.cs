#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Unity.Collections.AllocatorManager;

namespace WorldCreation.Preview
{
    public partial class WorldMapPreview
    {
        private int currentClodIndex;
        private int maxClodIndex;
        private int noise;
        private Vector3[][] clodPoints;

        public void CaveView()
        {
            if (clodFrames == null)
            {
                clodFrames = new();
                // ������s�ł���΃f�[�^���쐬����
                CreateClodFrame();
            }

            clodPoints = new Vector3[clodFrames.Count][];
            for (int i = 0; i < clodFrames.Count; i++)
            {
                clodPoints[i] = clodFrames[i]
                    .Select(clod => (Vector3)(Vector2)clod)
                    .ToArray();
            }

            for (int i = 0; i < clodPoints.Length; i++)
            {
                Gizmos.color = Color.yellow;
                for (int j = 0; j < clodPoints[i].Length; j++)
                {
                    Gizmos.DrawSphere(clodPoints[i][j], 1);
                }

                // Gizmos.DrawLineStrip(clodPoints[i], true);
            }
        }

        private void CreateClodFrame()
        {
            int[,] clodIndexes = new int[worldMap.WorldSize.x, worldMap.WorldSize.y];
            noise = _randomization.Order(10, 0, Int16.MaxValue);
            bool isInsideClodOld = false;

            for (int y = 0; y < worldMap.WorldSize.y; y++)
            {
                for (int x = 0; x < worldMap.WorldSize.x; x++)
                {
                    if (IsClod(x, y) && !isInsideClodOld)
                    {
                        /*��ɓ������u�Ԃł���΃��X�g�ɒǉ����ĉ��𒲂ׂ�*/
                        currentClodIndex = maxClodIndex;
                        maxClodIndex++;

                        // �V�����O���[�v���쐬���ă��X�g�ɒǉ�����
                        clodFrames.Add(new());
                        int halfIndex = clodFrames[currentClodIndex].Count / 2;
                        clodFrames[currentClodIndex].Insert(halfIndex, new Vector2Int(x, y));

                        CheckSameClod(x, y - 1, clodIndexes);

                        clodIndexes[x, y] = currentClodIndex;
                        isInsideClodOld = true;
                    }
                    else if (IsClod(x, y))
                    {
                        // ���ɉ�̒��ł���Ή��𒲂ׂ�
                        CheckSameClod(x, y - 1, clodIndexes);
                        clodIndexes[x, y] = currentClodIndex;
                    }
                    else if (isInsideClodOld)
                    {
                        // ��𔲂����u�Ԃł���Έ�O�̃u���b�N�����X�g�ɒǉ�
                        int halfIndex = clodFrames[currentClodIndex].Count / 2;
                        clodFrames[currentClodIndex].Insert(halfIndex, new Vector2Int(x - 1, y));

                        clodIndexes[x, y] = -1;
                        isInsideClodOld = false;
                    }
                    else
                    {
                        // ��Ԃ̒��ł���΂��̏������邾��
                        clodIndexes[x, y] = -1;
                    }
                }
            }

            // �f�[�^�̌y�ʉ�
            for (int i = 0; i < clodFrames.Count; i++)
            {
                for (int j = 0; j < clodFrames[i].Count; j++)
                {
                    if (j - 1 < 0) { continue; }
                    // X���W���O��Ɠ����ꏊ�ɂ���Ό��݂̍��W���폜
                    if (clodFrames[i][j].x == clodFrames[i][j - 1].x)
                    {
                        clodFrames[i].RemoveAt(j);
                    }
                    // Y���W���O��Ɠ����ꏊ�ɂ���Ό��݂̍��W���폜
                    if (clodFrames[i][j].y == clodFrames[i][j - 1].y)
                    {
                        clodFrames[i].RemoveAt(j);
                    }
                }
            }
        }

        private bool IsClod(int x, int y)
        {
            if (worldMap.WorldSize.x < x || worldMap.WorldSize.y < y) { return false; }
            for (int i = 0; i < worldMap.CaveCombines.Length; i++)
            {
                float noisePower = Mathf.PerlinNoise
                (
                    x * worldMap.CaveCombines[i].Scale.x + noise,
                    y * worldMap.CaveCombines[i].Scale.y + noise
                );

                if (worldMap.CaveCombines[i].HollowSize <= noisePower)
                {
                    return true;
                }
            }

            return false;
        }

        private void CheckSameClod(int x, int y, int[,] clodIndexes)
        {
            // �͈͊O�Ȃ珈�������Ȃ�
            bool isInsideRangeX = 0 <= x && x < worldMap.WorldSize.x;
            bool isInsideRangeY = 0 <= y && y < worldMap.WorldSize.y;
            if (!(isInsideRangeX && isInsideRangeY)) { return; }
            // �Ώۂ���łȂ���Ώ��������Ȃ�
            if (clodIndexes[x, y] == -1) { return; }
            // ��ł���Γ����򂩊m���߂�
            if (clodIndexes[x, y] == currentClodIndex) { return; }

            // �ʂ̉�̂��ߏ������݂̂��̂Ɠ�������
            clodFrames[clodIndexes[x, y]].AddRange(clodFrames[currentClodIndex]);
            clodFrames.RemoveAt(currentClodIndex);
            maxClodIndex--;
            for (int clodY = 0; clodY < clodIndexes.GetLength(1); clodY++)
            {
                for (int clodX = 0; clodX < clodIndexes.GetLength(0); clodX++)
                {
                    if (clodIndexes[clodX, clodY] > currentClodIndex)
                    {
                        clodIndexes[clodX, clodY]--;
                    }
                    if (clodIndexes[clodX, clodY] == currentClodIndex)
                    {
                        clodIndexes[clodX, clodY] = clodIndexes[x, y];
                    }
                }
            }
            currentClodIndex = clodIndexes[x, y];
        }
    }
}

#endif