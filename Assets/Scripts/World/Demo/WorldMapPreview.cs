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
            if (_layerGenerator == null)
            {
                Regenerate();
            }
            previewAlways?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            previewSelected?.Invoke();
        }

        /// <summary>
        /// ワールド全体の大きさを表示
        /// </summary>
        public void WorldScaleView()
        {
            Gizmos.color = new Color(0.5f, 0.5f, 0, 1f);
            Vector3 size = (Vector2)worldMap.WorldScale;
            Vector3 center = Vector3.Scale(size, new(0.5f, 0.5f, 0));
            Gizmos.DrawWireCube(center, size);
        }

        /// <summary>
        /// チャンクの表示
        /// </summary>
        public void ChunkSizeView()
        {
            Gizmos.color = Color.blue;
            Vector3 worldScale = (Vector2)worldMap.WorldScale;
            Vector3 size = (Vector2)worldMap.OneChunkSize;
            Vector3 anchor = size * 0.5f;
            Vector3 center = Vector3.zero;
            int split
                = worldMap.WorldScale.x * (worldMap.OneChunkSize.x / 2)
                + worldMap.WorldScale.y * (worldMap.OneChunkSize.y / 2);
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

        public void LayerBorderView()
        {
            Gizmos.color = Color.red;
            Vector3[] border = _layerGenerator.GetBorder
            (
                worldMap.WorldScale.x,
                50,
                worldMap.BorderNoiseSize,
                worldMap.RandomLimit
            )
            .Select(point => (Vector3)(Vector2)point)
            .ToArray();

            Gizmos.DrawLineStrip(border, false);
        }
    }
}
#endif