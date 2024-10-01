using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
    [Header("Camera Components")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private BoxCollider2D _boxCollider2D;

    /// <summary>
    /// カメラの設定
    /// </summary>
    /// <param name="followTarget">追跡するターゲット</param>
    /// <param name="mapSize">マップのサイズ</param>
    public void CameraConfig(Transform followTarget, Vector2 mapSize)
    {
        _virtualCamera.Follow = followTarget;
        _boxCollider2D.size = mapSize;
        _boxCollider2D.transform.position = mapSize / 2;
    }
}
