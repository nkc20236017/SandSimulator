#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Events;
using WorldCreation;
using System.Linq;

namespace WorldCreation.Preview
{
    [RequireComponent(typeof(WorldMapCreator))]
    [ExecuteInEditMode]
    public class WorldMapPreview : MonoBehaviour
    {
        [SerializeField]    // プレビューを表示する対象データ
        private WorldMap worldMap;
        [SerializeField]
        private UnityEvent previewAlways;
        [SerializeField]
        private UnityEvent previewSelected;

        private LayerGenerate _layerGenerator;
        private int _seed;
        private const float GUIDELINE_ALPHA = 0.25f;

        [ContextMenu("Regenerate")]
        public void Regenerate()
        {
            if (!Application.isPlaying)
            {
                _seed = GetComponent<WorldMapCreator>().CreateSeed();
                _layerGenerator = new(_seed);
            }
        }

        private void OnValidate()
        {
            for (int i = 0; i < previewAlways.GetPersistentEventCount(); i++)
            {
                previewAlways.SetPersistentListenerState(i, UnityEventCallState.EditorAndRuntime);
            }
            for (int i = 0; i < previewSelected.GetPersistentEventCount(); i++)
            {
                previewSelected.SetPersistentListenerState(i, UnityEventCallState.EditorAndRuntime);
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying) { return; }
            if (_layerGenerator == null)
            {
                Regenerate();
            }
            previewAlways?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) { return; }
            previewSelected?.Invoke();
        }

        /// <summary>
        /// ワールド全体の大きさを表示
        /// </summary>
        public void WorldScaleView()
        {
            Gizmos.color = new Color(0.5f, 0.5f, 0, 1f);
            Vector3 size = (Vector2)worldMap.WorldSize;
            Vector3 center = Vector3.Scale(size, new(0.5f, 0.5f, 0));
            Gizmos.DrawWireCube(center, size);
        }

        /// <summary>
        /// チャンクの表示
        /// </summary>
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

        /// <summary>
        /// レイヤーの境界を表示
        /// </summary>
        public void LayerBorderView()
        {
            int layerHeight = worldMap.WorldSize.y;
            for (int i = 0; i < worldMap.LayerRatios.Length; i++)
            {
                layerHeight -= (int)(layerHeight * worldMap.LayerRatios[i]);

                Vector3[] border = _layerGenerator.GetBorder
                (
                    worldMap.WorldSize.x,
                    layerHeight,
                    worldMap.BorderNoiseSize,
                    (int)worldMap.RandomLimit,
                    worldMap.Amplitude
                )
                .Select(point => (Vector3)(Vector2)point)
                .ToArray();

                Gizmos.color = worldMap.WorldLayers[i + 1].DebugLayerColor;
                Gizmos.DrawLineStrip(border, false);

                Color layerColor = worldMap.WorldLayers[i + 1].DebugLayerColor;
                layerColor.a = GUIDELINE_ALPHA;

                Gizmos.color = layerColor;
                Gizmos.DrawLine
                (
                    new(0, layerHeight + worldMap.BorderNoiseSize),
                    new(worldMap.WorldSize.x, layerHeight + worldMap.BorderNoiseSize)
                );
                Gizmos.DrawLine
                (
                    new(0, layerHeight),
                    new(worldMap.WorldSize.x, layerHeight)
                );
            }
        }
    }
}
#endif