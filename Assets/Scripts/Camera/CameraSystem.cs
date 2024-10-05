using UnityEngine;
using Cinemachine;
using System.Threading;
using Cysharp.Threading.Tasks;


public class CameraSystem : MonoBehaviour, ICameraEffect
{
    [Header("Camera Components")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private Vector2 _overSize;
    [SerializeField] private Vector2 _offset;

    private bool _hasMapCollider = false;
    private PolygonCollider2D _polygonCollider2D;
    private CancellationTokenSource _tokenSource;
    private CinemachineConfiner2D _confiner2D;
    private CinemachineBasicMultiChannelPerlin _noise;

    private void Start()
    {
        Transform parentObject = transform.parent;

        for (int i = 0; i < parentObject.childCount; i++)
        {
            if (parentObject.GetChild(i).TryGetComponent(out _polygonCollider2D))
            {
                _hasMapCollider = true;
                break;
            }
        }

        _confiner2D = GetComponent<CinemachineConfiner2D>();

        if (!_hasMapCollider) { return; }

        _noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _noise.enabled = false;
    }

    private void OnApplicationQuit()
    {
        StopShake();
    }

    /// <summary>
    /// カメラの設定
    /// </summary>
    /// <param name="followTarget">追跡するターゲット</param>
    /// <param name="mapSize">マップのサイズ</param>
    public void CameraConfig(Transform followTarget, Vector2 mapSize = default)
    {
        _virtualCamera.Follow = followTarget;

        if (_hasMapCollider)
        {
            Vector2 size = new
            (
                _overSize.x / 2,
                _overSize.y / 2
            );

            _polygonCollider2D.points = new Vector2[]
            {
            Vector2.zero - size + _offset,
            new Vector2(mapSize.x + size.x, 0 - size.y) + _offset,
            mapSize + size + _offset,
            new Vector2(0 - size.x, mapSize.y + size.y) + _offset
            };
        }

        // 状態を更新
        _confiner2D.InvalidateCache();
    }

    /// <summary>
    /// カメラを指定した時間揺らす
    /// </summary>
    /// <param name="shakeTime">揺らす時間</param>
    /// <param name="targetCameraRole">揺らすカメラの対象</param>
    public void CameraShake(float shakeTime)
    {
        if (!_hasMapCollider) { return; }

        _tokenSource = new();

        // カメラ揺れを開始
        Shaking(shakeTime, _tokenSource.Token);
    }

    /// <summary>
    /// 揺れているカメラを止める
    /// </summary>
    /// <param name="targetCameraRole">止めるカメラの対象</param>
    public void StopShake()
    {
        if (_tokenSource != null)
        {
            // カメラ揺れをキャンセル
            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }
    }

    private async void Shaking(float shakeTime, CancellationToken token)
    {
        // 振動を有効化
        _noise.enabled = true;

        // 継続時間分待機
        await UniTask.WaitForSeconds(shakeTime).SuppressCancellationThrow();

        // 振動を無効化
        _noise.enabled = false;
    }
}
