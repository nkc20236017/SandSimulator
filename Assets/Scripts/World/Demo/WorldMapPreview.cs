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
        [SerializeField]    // �v���r���[��\������Ώۃf�[�^
        private WorldMap worldMap;
        [SerializeField]
        private UnityEvent previewAlways;
        [SerializeField]
        private UnityEvent previewSelected;

        private ManagedRandom randomization;
        private LayerGenerate _layerGenerator;
        private int _seed;
        private const float GUIDELINE_ALPHA = 0.25f;

        [ContextMenu("Regenerate")]
        public void Regenerate()
        {
            if (!Application.isPlaying)
            {
                randomization = new(GetComponent<WorldMapCreator>().Seed);
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
        /// ���[���h�S�̂̑傫����\��
        /// </summary>
        public void WorldScaleView()
        {
            Vector3 tilemapOrigin = new
            (
                randomization.Order(0, worldMap.MinOriginGapRange.x, worldMap.MaxOriginGapRange.x),
                randomization.Order(1, worldMap.MinOriginGapRange.y, worldMap.MaxOriginGapRange.y),
                0
            );

            Gizmos.color = new Color(0.5f, 0.5f, 0, 1f);
            Vector3 size = (Vector2)worldMap.WorldSize;
            Vector3 center = Vector3.Scale(size, new(0.5f, 0.5f, 0)) + tilemapOrigin;
            Gizmos.DrawWireCube(center, size);
        }

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
            // �͈͊O�e�X�g
            if (worldMap.MinOriginGapRange.x > 0 || worldMap.MinOriginGapRange.y > 0)
            {
                Gizmos.color = Color.red;
            }
            if (worldMap.MinOriginGapRange.x + worldMap.WorldSize.x < 0 || worldMap.MinOriginGapRange.y + worldMap.WorldSize.y < 0)
            {
                Gizmos.color = Color.red;
            }
            if (worldMap.MaxOriginGapRange.x > 0 || worldMap.MaxOriginGapRange.y > 0)
            {
                Gizmos.color = Color.red;
            }
            if (worldMap.MaxOriginGapRange.x + worldMap.WorldSize.x < 0 || worldMap.MaxOriginGapRange.y + worldMap.WorldSize.y < 0)
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawWireSphere(Vector3.zero, 10f);
        }

        /// <summary>
        /// �`�����N�̕\��
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
        /// ���C���[�̋��E��\��
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