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
                // 初回実行であればデータを作成する
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
                        /*塊に入った瞬間であればリストに追加して下を調べる*/
                        currentClodIndex = maxClodIndex;
                        maxClodIndex++;

                        // 新しいグループを作成してリストに追加する
                        clodFrames.Add(new());
                        int halfIndex = clodFrames[currentClodIndex].Count / 2;
                        clodFrames[currentClodIndex].Insert(halfIndex, new Vector2Int(x, y));

                        CheckSameClod(x, y - 1, clodIndexes);

                        clodIndexes[x, y] = currentClodIndex;
                        isInsideClodOld = true;
                    }
                    else if (IsClod(x, y))
                    {
                        // 既に塊の中であれば下を調べる
                        CheckSameClod(x, y - 1, clodIndexes);
                        clodIndexes[x, y] = currentClodIndex;
                    }
                    else if (isInsideClodOld)
                    {
                        // 塊を抜けた瞬間であれば一個前のブロックをリストに追加
                        int halfIndex = clodFrames[currentClodIndex].Count / 2;
                        clodFrames[currentClodIndex].Insert(halfIndex, new Vector2Int(x - 1, y));

                        clodIndexes[x, y] = -1;
                        isInsideClodOld = false;
                    }
                    else
                    {
                        // 空間の中であればその情報を入れるだけ
                        clodIndexes[x, y] = -1;
                    }
                }
            }

            // データの軽量化
            for (int i = 0; i < clodFrames.Count; i++)
            {
                for (int j = 0; j < clodFrames[i].Count; j++)
                {
                    if (j - 1 < 0) { continue; }
                    // X座標が前回と同じ場所にあれば現在の座標を削除
                    if (clodFrames[i][j].x == clodFrames[i][j - 1].x)
                    {
                        clodFrames[i].RemoveAt(j);
                    }
                    // Y座標が前回と同じ場所にあれば現在の座標を削除
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
            // 範囲外なら処理をしない
            bool isInsideRangeX = 0 <= x && x < worldMap.WorldSize.x;
            bool isInsideRangeY = 0 <= y && y < worldMap.WorldSize.y;
            if (!(isInsideRangeX && isInsideRangeY)) { return; }
            // 対象が塊でなければ処理をしない
            if (clodIndexes[x, y] == -1) { return; }
            // 塊であれば同じ塊か確かめる
            if (clodIndexes[x, y] == currentClodIndex) { return; }

            // 別の塊のため情報を現在のものと統合する
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