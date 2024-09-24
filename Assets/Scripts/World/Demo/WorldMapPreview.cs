#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Events;
using WorldCreation;
using System.Linq;
using System.Collections.Generic;

namespace WorldCreation.Preview
{
    [RequireComponent(typeof(WorldMapCreator))]
    [ExecuteInEditMode]
    public partial class WorldMapPreview : MonoBehaviour
    {
        [SerializeField]    // プレビューを表示する対象データ
        private WorldMap worldMap;
        [SerializeField]
        private UnityEvent previewAlways;
        [SerializeField]
        private UnityEvent previewSelected;

        private ManagedRandom _randomization;
        private LayerGenerator _layerGenerator;
        private const float GUIDELINE_ALPHA = 0.25f;
        private Vector3 _tilemapOrigin;
        private List<List<Vector2Int>> clodFrames;

        [ContextMenu("Refresh")]
        public void Refresh()
        {
            if (!Application.isPlaying)
            {
                _randomization = new(GetComponent<WorldMapCreator>().Seed);
                _layerGenerator = new();

                if (clodFrames != null)
                {
                    currentClodIndex = 0;
                    maxClodIndex = 0;
                    clodPoints = null;
                    clodFrames.Clear();
                    clodFrames = null;
                }
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
                Refresh();
            }
            previewAlways?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) { return; }
            previewSelected?.Invoke();
        }
    }
}
#endif