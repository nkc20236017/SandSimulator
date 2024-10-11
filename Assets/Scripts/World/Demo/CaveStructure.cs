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
        private int currentLumpIndex;
        private int maxLumpIndex;
        private int noise;
        private Vector3[][] lumpPoints;

        public void CaveView()
        {
            if (lumpFrames == null)
            {
                lumpFrames = new();
                // 初回実行であればデータを作成する
                CreateLumpFrame();
            }

            lumpPoints = new Vector3[lumpFrames.Count][];
            for (int i = 0; i < lumpFrames.Count; i++)
            {
                lumpPoints[i] = lumpFrames[i]
                    .Select(lump => (Vector3)(Vector2)lump)
                    .ToArray();
            }

            for (int i = 0; i < lumpPoints.Length; i++)
            {
                Gizmos.color = Color.yellow;
                for (int j = 0; j < lumpPoints[i].Length; j++)
                {
                    Gizmos.DrawSphere(lumpPoints[i][j], 1);
                }

                // Gizmos.DrawLineStrip(lumpPoints[i], true);
            }
        }

        private void CreateLumpFrame()
        {
            int[,] lumpIndexes = new int[worldMap.WorldSplidCount.x, worldMap.WorldSplidCount.y];
            noise = _randomization.OrderInt(10, 0, Int16.MaxValue);
            bool isInsideLumpOld = false;

            for (int y = 0; y < worldMap.WorldSplidCount.y; y++)
            {
                for (int x = 0; x < worldMap.WorldSplidCount.x; x++)
                {
                    if (IsLump(x, y) && !isInsideLumpOld)
                    {
                        /*塊に入った瞬間であればリストに追加して下を調べる*/
                        currentLumpIndex = maxLumpIndex;
                        maxLumpIndex++;

                        // 新しいグループを作成してリストに追加する
                        lumpFrames.Add(new());
                        int halfIndex = lumpFrames[currentLumpIndex].Count / 2;
                        lumpFrames[currentLumpIndex].Insert(halfIndex, new Vector2Int(x, y));

                        CheckSameLump(x, y - 1, lumpIndexes);

                        lumpIndexes[x, y] = currentLumpIndex;
                        isInsideLumpOld = true;
                    }
                    else if (IsLump(x, y))
                    {
                        // 既に塊の中であれば下を調べる
                        CheckSameLump(x, y - 1, lumpIndexes);
                        lumpIndexes[x, y] = currentLumpIndex;
                    }
                    else if (isInsideLumpOld)
                    {
                        // 塊を抜けた瞬間であれば一個前のブロックをリストに追加
                        int halfIndex = lumpFrames[currentLumpIndex].Count / 2;
                        lumpFrames[currentLumpIndex].Insert(halfIndex, new Vector2Int(x - 1, y));

                        lumpIndexes[x, y] = -1;
                        isInsideLumpOld = false;
                    }
                    else
                    {
                        // 空間の中であればその情報を入れるだけ
                        lumpIndexes[x, y] = -1;
                    }
                }
            }

            // データの軽量化
            for (int i = 0; i < lumpFrames.Count; i++)
            {
                for (int j = 0; j < lumpFrames[i].Count; j++)
                {
                    if (j - 1 < 0) { continue; }
                    // X座標が前回と同じ場所にあれば現在の座標を削除
                    if (lumpFrames[i][j].x == lumpFrames[i][j - 1].x)
                    {
                        lumpFrames[i].RemoveAt(j);
                    }
                    // Y座標が前回と同じ場所にあれば現在の座標を削除
                    if (lumpFrames[i][j].y == lumpFrames[i][j - 1].y)
                    {
                        lumpFrames[i].RemoveAt(j);
                    }
                }
            }
        }

        private bool IsLump(int x, int y)
        {
            if (worldMap.WorldSplidCount.x < x || worldMap.WorldSplidCount.y < y) { return false; }
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

        private void CheckSameLump(int x, int y, int[,] lumpIndexes)
        {
            // 範囲外なら処理をしない
            bool isInsideRangeX = 0 <= x && x < worldMap.WorldSplidCount.x;
            bool isInsideRangeY = 0 <= y && y < worldMap.WorldSplidCount.y;
            if (!(isInsideRangeX && isInsideRangeY)) { return; }
            // 対象が塊でなければ処理をしない
            if (lumpIndexes[x, y] == -1) { return; }
            // 塊であれば同じ塊か確かめる
            if (lumpIndexes[x, y] == currentLumpIndex) { return; }

            // 別の塊のため情報を現在のものと統合する
            lumpFrames[lumpIndexes[x, y]].AddRange(lumpFrames[currentLumpIndex]);
            lumpFrames.RemoveAt(currentLumpIndex);
            maxLumpIndex--;
            for (int lumpY = 0; lumpY < lumpIndexes.GetLength(1); lumpY++)
            {
                for (int lumpX = 0; lumpX < lumpIndexes.GetLength(0); lumpX++)
                {
                    if (lumpIndexes[lumpX, lumpY] > currentLumpIndex)
                    {
                        lumpIndexes[lumpX, lumpY]--;
                    }
                    if (lumpIndexes[lumpX, lumpY] == currentLumpIndex)
                    {
                        lumpIndexes[lumpX, lumpY] = lumpIndexes[x, y];
                    }
                }
            }
            currentLumpIndex = lumpIndexes[x, y];
        }
    }
}

#endif