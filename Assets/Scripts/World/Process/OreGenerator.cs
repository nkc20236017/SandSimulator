using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace WorldCreation
{
    public class OreGenerator : IWorldDecidable
    {
        private int _executionOrder;
        private int _runCount;
        public int ExecutionOrder => _executionOrder;

        public void Initalize(GameChunk chunk, WorldCreatePrinciple worldMap, int executionOrder)
        {
            _executionOrder = executionOrder;
        }

        public async UniTask<GameChunk> Execute(GameChunk chunk, WorldCreatePrinciple worldMap, CancellationToken token)
        {
            int[,] noise = BlueNoise(chunk.GetChunkLength(0), chunk.GetChunkLength(1), worldMap, chunk, 0);

            for (int y = 0; y < chunk.GetChunkLength(1); y++)
            {
                for (int x = 0; x < chunk.GetChunkLength(0); x++)
                {
                    chunk.SetBlock(x, y, noise[x, y] + 1);
                }
            }

            return await UniTask.RunOnThreadPool(() => chunk);
        }

        public int[,] BlueNoise(int width, int height, WorldCreatePrinciple worldMap, GameChunk chunk, int targetOre)
        {
            int[,] grid = new int[width, height];
            List<Vector2Int> activeList = new List<Vector2Int>();
            List<Vector2Int> processedList = new List<Vector2Int>();

            // 最初のポイントをランダムに配置
            Vector2Int firstPoint = new Vector2Int(chunk.GetNoise(_executionOrder, width), chunk.GetNoise(_executionOrder + 1, height));
            activeList.Add(firstPoint);
            grid[firstPoint.x, firstPoint.y] = 1;

            while (activeList.Count > 0)
            {
                int index = chunk.GetNoise(_executionOrder + 2, activeList.Count);
                Vector2Int point = activeList[index];

                bool foundValidPoint = false;

                PrimevalOre ore = worldMap.WorldLayers[chunk.GetLayerIndex(firstPoint.x, firstPoint.y)].PrimevalOres[targetOre];

                for (int i = 0; i < ore.LumpDispersion; i++)
                {
                    float angle = chunk.GetNoise(_executionOrder + 3, Int16.MaxValue) * Mathf.PI * 2;
                    float distance = ore.Space + chunk.GetNoise(_executionOrder + 4, Int16.MaxValue) * ore.Space;
                    Vector2Int newPoint = new Vector2Int(
                        Mathf.RoundToInt(point.x + Mathf.Cos(angle) * distance),
                        Mathf.RoundToInt(point.y + Mathf.Sin(angle) * distance)
                    );

                    if (IsValidPoint(newPoint, width, height, grid, ore.Space))
                    {
                        grid[newPoint.x, newPoint.y] = 1;
                        activeList.Add(newPoint);
                        foundValidPoint = true;
                        break;
                    }
                }

                if (!foundValidPoint)
                {
                    activeList.RemoveAt(index);
                    processedList.Add(point);
                }

                _executionOrder++;
            }

            return grid;
        }

        private bool IsValidPoint(Vector2Int point, int width, int height, int[,] grid, int space)
        {
            if (point.x < 0 || point.x >= width || point.y < 0 || point.y >= height)
                return false;

            if (grid[point.x, point.y] == 1)
                return false;

            for (int x = Mathf.Max(0, point.x - space); x < Mathf.Min(width, point.x + space + 1); x++)
            {
                for (int y = Mathf.Max(0, point.y - space); y < Mathf.Min(height, point.y + space + 1); y++)
                {
                    if (grid[x, y] == 1)
                    {
                        float distance = Vector2Int.Distance(new Vector2Int(x, y), point);
                        if (distance < space)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}