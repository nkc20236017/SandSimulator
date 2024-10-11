#if UNITY_EDITOR

using UnityEngine;

namespace WorldCreation.Preview
{
    public partial class WorldMapPreview
    {
        public void WorldOriginView()
        {
            Vector3[] xRange =
            {
                new Vector3(worldMap.MinOriginGapRange.x, 0, 0),
                new Vector3(worldMap.MaxOriginGapRange.x, 0, 0),
                new Vector3(worldMap.MinOriginGapRange.x, -10, 0),
                new Vector3(worldMap.MinOriginGapRange.x, 10, 0),
                new Vector3(worldMap.MaxOriginGapRange.x, -10, 0),
                new Vector3(worldMap.MaxOriginGapRange.x, 10, 0)

            };
            Vector3[] yRange =
            {
                new Vector3(0, worldMap.MinOriginGapRange.y, 0),
                new Vector3(0, worldMap.MaxOriginGapRange.y, 0),
                new Vector3(-10, worldMap.MinOriginGapRange.y, 0),
                new Vector3(10, worldMap.MinOriginGapRange.y, 0),
                new Vector3(-10, worldMap.MaxOriginGapRange.y, 0),
                new Vector3(10, worldMap.MaxOriginGapRange.y, 0)
            };

            Gizmos.color = new Color(0.25f, 0.25f, 0, 0.8f);
            Gizmos.DrawLineList(xRange);
            Gizmos.DrawLineList(yRange);

            Gizmos.color = new Color(1, 1, 0, 0.8f);
            // 範囲外テスト
            if (worldMap.MinOriginGapRange.x > 0 || worldMap.MinOriginGapRange.y > 0)
            {
                Gizmos.color = Color.red;
            }
            if (worldMap.MinOriginGapRange.x + worldMap.WorldSplidCount.x < 0 || worldMap.MinOriginGapRange.y + worldMap.WorldSplidCount.y < 0)
            {
                Gizmos.color = Color.red;
            }
            if (worldMap.MaxOriginGapRange.x > 0 || worldMap.MaxOriginGapRange.y > 0)
            {
                Gizmos.color = Color.red;
            }
            if (worldMap.MaxOriginGapRange.x + worldMap.WorldSplidCount.x < 0 || worldMap.MaxOriginGapRange.y + worldMap.WorldSplidCount.y < 0)
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawWireSphere(Vector3.zero, 10f);
        }
    }
}
#endif