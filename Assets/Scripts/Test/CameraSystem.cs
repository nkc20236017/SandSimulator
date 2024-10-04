using UnityEngine;
using Cinemachine;
[RequireComponent(typeof(CinemachineConfiner2D))]
public class CameraSystem : MonoBehaviour
{
    [Header("Camera Components")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private PolygonCollider2D _polygonCollider2D;
    [SerializeField] private Vector2 _overSize;
    [SerializeField] private Vector2 _offset;

    private CinemachineConfiner2D _confiner2D;

    private void Start()
    {
        _confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    /// <summary>
    /// カメラの設定
    /// </summary>
    /// <param name="followTarget">追跡するターゲット</param>
    /// <param name="mapSize">マップのサイズ</param>
    public void CameraConfig(Transform followTarget, Vector2 mapSize = default)
    {
        _virtualCamera.Follow = followTarget;

        if (mapSize != default)
        {
            Vector2 offset = new
            (
                _overSize.x / 2 + _offset.x,
                _overSize.y / 2 + _offset.y
            );

            _polygonCollider2D.points = new Vector2[]
            {
                Vector2.zero - offset,
                new Vector2(mapSize.x + offset.x, 0 - offset.y),
                mapSize + offset,
                new Vector2(0 - offset.x, mapSize.y + offset.y)
            };

        }
        // 状態を更新
        _confiner2D.InvalidateCache();
    }
}
