using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class IKVacuum : MonoBehaviour
{
    [Header("IK Settings")]
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject _bonePrefab;
    [SerializeField] private LineRenderer _linePrefab;
    [SerializeField] private Transform _firstPoint;
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private Material _lineMaterial;
    
    [Header("IK Config")]
    [Tooltip("間接の数")][Min(0)]
    [SerializeField] private int _maxIteration = 5;
    [Tooltip("線の長さ")]
    [SerializeField] private float _length = 1f;
    [Tooltip("線の太さ")][MinValue(0f), MaxValue(1f)]
    [SerializeField] private float _lineWidth = 0.1f;
    [Tooltip("重力")]
    [SerializeField] private Vector3 _gravity = new(0, -9.81f, 0);
    [Tooltip("バキュームの範囲")] [MinValue(0f)]
    [SerializeField] private float _vacuumRange = 1f;

    private float _axesLength;
    private List<float> _lengths = new();
    private List<Vector3> _positions = new();
    private List<Transform> _axises = new();
    private List<LineRenderer> _lineRenderers = new();
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        
        CreateIK();
    }

    private void CreateIK()
    {
        _axises.Add(_firstPoint);
        Transform parent = _firstPoint;
        _axesLength = _length / _maxIteration;
        for (int i = 0; i < _maxIteration; i++)
        {
            GameObject newPoint = Instantiate(_bonePrefab, parent.position + Vector3.up * _axesLength, Quaternion.identity);
            newPoint.transform.SetParent(parent);
            _axises.Add(newPoint.transform);
            parent = newPoint.transform;
        }

        _lengths.Clear();
        for (var i = 0; i < _axises.Count - 1; i++)
        {
            _lengths.Add(Vector3.Distance(_axises[i].position, _axises[i + 1].position));
        }

        _positions.Clear();
        foreach (var b in _axises)
        {
            _positions.Add(b.position);
        }

        for (var i = 0; i < _axises.Count - 1; i++)
        {
            LineRenderer lineRenderer = Instantiate(_linePrefab, _axises[i]);
            lineRenderer.material = _lineMaterial;
            lineRenderer.startWidth = _lineWidth;
            lineRenderer.endWidth = _lineWidth;
            _lineRenderers.Add(lineRenderer);
        }
    }

    private void Update()
    {
        VacuumMovement();
        VacuumRotate();

        for (var i = 0; i < _axises.Count; i++)
        {
            _positions[i] = _axises[i].position;
        }

        var basePos = _positions[0];
        var targetPos = _targetPoint.position;
        var prevDistance = 0.0f;
        for (var iter = 0; iter < _maxIteration; iter++)
        {
            var distance = Vector3.Distance(_positions[_positions.Count - 1], targetPos);
            var change = Mathf.Abs(distance - prevDistance);
            prevDistance = distance;
            if (distance < 1e-6 || change < 1e-8) { break; }

            // ターゲットの位置を更新
            _positions[^1] = targetPos;
            for (var i = _positions.Count - 1; i >= 1; i--)
            {
                var direction = (_positions[i] - _positions[i - 1]).normalized;
                _positions[i - 1] = _positions[i] - direction * _lengths[i - 1];
            }

            // ベースの位置を更新
            _positions[0] = basePos;
            for (var i = 0; i <= _positions.Count - 2; i++)
            {
                var direction = (_positions[i + 1] - _positions[i]).normalized;
                _positions[i + 1] = _positions[i] + direction * _lengths[i];
            }
            
            // 重力を適用
            for (var i = 1; i < _positions.Count; i++)
            {
                _positions[i] += _gravity * (Time.deltaTime * Time.deltaTime);
            }
        }

        // 各ボーンの回転を計算
        for (var i = 0; i < _positions.Count - 1; i++)
        {
            var origin = _axises[i].position;
            var current = _axises[i + 1].position;
            var target = _positions[i + 1];
            var delta = GetDeltaRotation(origin, current, target);
            _axises[i].rotation = delta * _axises[i].rotation;
        }

        // ボーンの位置を更新
        for (var i = 0; i < _lineRenderers.Count; i++)
        {
            _lineRenderers[i].SetPosition(0, _axises[i].position + new Vector3(0, 0, -0.25f));
            _lineRenderers[i].SetPosition(1, _axises[i + 1].position + new Vector3(0, 0, -0.25f));
        }

        // 各ボーン間の長さを更新
        for (var i = 0; i < _axises.Count - 1; i++)
        {
            _lengths[i] = Vector3.Distance(_axises[i].position, _axises[i + 1].position);
        }
    }

    private void VacuumMovement()
    {
        // バキュームの位置をマウスの位置に設定
        Vector3 cursor = _camera.ScreenToWorldPoint(Input.mousePosition);
        cursor.z = 0;
        
        // バキュームの移動制限
        Vector3 direction = (cursor - _player.position).normalized;
        _targetPoint.position = _player.position + direction * _vacuumRange;
    }

    private void VacuumRotate()
    {
        // バキュームの向きを設定
        Vector3 direction = (_targetPoint.position - _player.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _targetPoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private static Quaternion GetDeltaRotation(Vector3 origin, Vector3 current, Vector3 target)
    {
        // 2つのベクトルの回転を計算
        var beforeDirection = (current - origin).normalized;
        var afterDirection = (target - origin).normalized;
        return Quaternion.FromToRotation(beforeDirection, afterDirection);
    }
    
    private void OnDrawGizmos()
    {
        // バキュームの移動範囲を描画
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_player.position, _vacuumRange);
    }
    
    private void OnValidate()
    {
        _vacuumRange = Mathf.Min(_length, _vacuumRange);
    }
}
