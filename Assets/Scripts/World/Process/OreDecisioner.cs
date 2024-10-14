/*using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class OreDecisioner : WorldDecisionerBase
{
    private struct Point
    {
        public Vector2Int position;
        public bool active;
    }

    private int _runCount;
    private List<Point> points = new List<Point>();

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

    public Vector2Int[] BlueNoise(int width, int height, int space, int seed)
    {
        System.Random random = new System.Random(seed);
        bool[,] grid = new bool[width, height];
        int activeCount = 0;
        int squareSpace = space * space;

        points.Clear();

        // 最初のポイントをランダムに配置
        Vector2Int firstPoint = new Vector2Int(random.Next(width), random.Next(height));
        points.Add(new Point { position = firstPoint, active = true });
        grid[firstPoint.x, firstPoint.y] = true;
        activeCount++;

        while (activeCount > 0)
        {
            int index = random.Next(activeCount);
            Point point = points[index];

            bool foundValidPoint = false;

            for (int i = 0; i < 30; i++) // 30回試行
            {
                float angle = (float)random.NextDouble() * Mathf.PI * 2;
                float distance = space + (float)random.NextDouble() * space;
                Vector2Int newPoint = new Vector2Int(
                    Mathf.RoundToInt(point.position.x + Mathf.Cos(angle) * distance),
                    Mathf.RoundToInt(point.position.y + Mathf.Sin(angle) * distance)
                );

                if (IsValidPoint(newPoint, width, height, grid, squareSpace))
                {
                    grid[newPoint.x, newPoint.y] = true;
                    points.Add(new Point { position = newPoint, active = true });
                    activeCount++;
                    foundValidPoint = true;
                    break;
                }
            }

            if (!foundValidPoint)
            {
                points[index] = points[activeCount - 1];
                points[activeCount - 1] = new Point { position = point.position, active = false };
                activeCount--;
            }
        }

        return points.ConvertAll(p => p.position).ToArray();
    }

    private bool IsValidPoint(Vector2Int point, int width, int height, bool[,] grid, int squareSpace)
    {
        if (point.x < 0 || point.x >= width || point.y < 0 || point.y >= height || grid[point.x, point.y])
            return false;

        int minX = Mathf.Max(0, point.x - (int)Mathf.Sqrt(squareSpace));
        int maxX = Mathf.Min(width - 1, point.x + (int)Mathf.Sqrt(squareSpace));
        int minY = Mathf.Max(0, point.y - (int)Mathf.Sqrt(squareSpace));
        int maxY = Mathf.Min(height - 1, point.y + (int)Mathf.Sqrt(squareSpace));

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (grid[x, y])
                {
                    int dx = x - point.x;
                    int dy = y - point.y;
                    if (dx * dx + dy * dy < squareSpace)
                        return false;
                }
            }
        }

        return true;
    }
}
*/