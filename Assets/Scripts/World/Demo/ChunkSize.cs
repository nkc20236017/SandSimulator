#if UNITY_EDITOR

using UnityEngine;

namespace WorldCreation.Preview
{
    public partial class WorldMapPreview
    {
        public void ChunkSizeView()
        {
            Gizmos.color = Color.blue;
            Vector3 worldScale = (Vector2)worldMap.WorldSize;
            Vector3 size = (Vector2)worldMap.OneChunkSize;
            Vector3 anchor = size * 0.5f;
            Vector3 center = Vector3.zero;
            int split
                = worldMap.WorldSize.x * (worldMap.OneChunkSize.x / 2)
                + worldMap.WorldSize.y * (worldMap.OneChunkSize.y / 2);
            Vector3[] points = new Vector3[split * 2];

            for (int i = 0; i < points.Length / 4; i += 4)
            {
                center += size;
                if (center.x < worldScale.x)
                {
                    points[i] = new(center.x, 0);
                    points[i + 1] = new(center.x, worldScale.y);
                }

                if (center.y < worldScale.y)
                {
                    points[i + 2] = new(0, center.y);
                    points[i + 3] = new(worldScale.x, center.y);
                }
            }

            Gizmos.DrawLineList(points);
        }
    }
}
#endif