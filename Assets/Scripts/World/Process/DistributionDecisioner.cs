using RandomExtensions;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public abstract class DistributionDecisioner : WorldDecisionerBase
{
    private struct Point
    {
        public Vector2Int position;
        public bool active;
    }

    private List<Point> points = new List<Point>();

    protected Vector2Int[] BlueNoise(Vector2Int size, OreDecisionData oreDecision)
    {
        float cellSize = oreDecision.Space / Mathf.Sqrt(2);
        int[,] grid
            = new int[Mathf.CeilToInt(size.x / cellSize), Mathf.CeilToInt(size.y / cellSize)];

        points.Clear();
        List<Vector2> spawnPoints = new List<Vector2>();

        // 最初のポイントを配置
        Vector2 firstPoint = new Vector2
        (
            _random.NextFloat() * size.x,
            _random.NextFloat() * size.y
        );
        points.Add(new Point
        {
            position = Vector2Int.RoundToInt(firstPoint),
            active = true
        });
        spawnPoints.Add(firstPoint);
        grid[Mathf.FloorToInt(firstPoint.x / cellSize), Mathf.FloorToInt(firstPoint.y / cellSize)]
            = 1;

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = _random.NextInt(spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < oreDecision.Bias; i++)
            {
                float angle = _random.NextFloat() * Mathf.PI * 2;
                float distance = oreDecision.Space + _random.NextFloat() * oreDecision.Space;
                Vector2 candidate = spawnCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

                if (IsValid(candidate, size.x, size.y, cellSize, oreDecision.Space, grid))
                {
                    Vector2Int roundedCandidate = Vector2Int.RoundToInt(candidate);
                    points.Add(new Point { position = roundedCandidate, active = true });
                    spawnPoints.Add(candidate);
                    grid[Mathf.FloorToInt(candidate.x / cellSize), Mathf.FloorToInt(candidate.y / cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }

            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points.ConvertAll(p => p.position).ToArray();
    }

    private bool IsValid(Vector2 candidate, int width, int height, float cellSize, float radius, int[,] grid)
    {
        bool isInsideX = candidate.x >= 0 && candidate.x < width;
        bool isInsideY = candidate.y >= 0 && candidate.y < height;

        if (isInsideX && isInsideY)
        {
            int cellX = Mathf.FloorToInt(candidate.x / cellSize);
            int cellY = Mathf.FloorToInt(candidate.y / cellSize);
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDst
                            = (candidate - points[pointIndex].position).sqrMagnitude;
                        if (sqrDst < radius * radius) { return false; }
                    }
                }
            }
            return true;
        }
        return false;
    }
}