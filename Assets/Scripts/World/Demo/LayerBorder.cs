#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;

namespace WorldCreation.Preview
{
    public partial class WorldMapPreview
    {
        public void LayerBorderView()
        {
            int layerHeight = worldMap.WorldSplidCount.y;
            for (int i = 0; i < worldMap.LayerRatios.Length; i++)
            {
                layerHeight -= (int)(layerHeight * worldMap.LayerRatios[i]);

                Vector3[] border = GetBorder(layerHeight, i)
                .Select(point => (Vector3)(Vector2)point)
                .ToArray();

                Gizmos.color = worldMap.WorldLayers[i + 1].LayerColor;
                Gizmos.DrawLineStrip(border, false);

                Color layerColor = worldMap.WorldLayers[i + 1].LayerColor;
                layerColor.a = GUIDELINE_ALPHA;

                Gizmos.color = layerColor;
                Gizmos.DrawLine
                (
                    new(0, layerHeight + worldMap.BorderDistortionPower),
                    new(worldMap.WorldSplidCount.x, layerHeight + worldMap.BorderDistortionPower)
                );
                Gizmos.DrawLine
                (
                    new(0, layerHeight),
                    new(worldMap.WorldSplidCount.x, layerHeight)
                );
            }
        }

        public Vector2Int[] GetBorder(int altitude, int layerCount)
        {
            Vector2Int[] borders = new Vector2Int[worldMap.WorldSplidCount.x];
            int noise = _randomization.OrderInt(layerCount + 2, 0, Int16.MaxValue);
            for (int x = 0; x < worldMap.WorldSplidCount.x; x++)
            {
                int border = (int)(Mathf.PerlinNoise1D
                (
                    x * worldMap.BorderAmplitude + noise
                ) * worldMap.BorderDistortionPower);
                borders[x] = new Vector2Int
                (
                    x - (int)_tilemapOrigin.x,
                    border + altitude - (int)_tilemapOrigin.y
                );
            }

            return borders;
        }
    }
}
#endif