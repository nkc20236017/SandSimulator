#if UNITY_EDITOR

using UnityEngine;

namespace WorldCreation.Preview
{
    public partial class WorldMapPreview
    {
        public void WorldScaleView()
        {
            _tilemapOrigin = new
            (
                _randomization.OrderInt(0, worldMap.MinOriginGapRange.x, worldMap.MaxOriginGapRange.x),
                _randomization.OrderInt(1, worldMap.MinOriginGapRange.y, worldMap.MaxOriginGapRange.y),
                0
            );

            Gizmos.color = new Color(0.5f, 0.5f, 0, 1f);
            Vector3 size = (Vector2)worldMap.WorldSplidCount;
            Vector3 center = Vector3.Scale(size, new(0.5f, 0.5f, 0)) + _tilemapOrigin;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
#endif