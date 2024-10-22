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
    [Tooltip("全体の長さ")]
    [SerializeField] private float _length = 1f;
    [Tooltip("線の太さ")][MinValue(0f), MaxValue(1f)]
    [SerializeField] private float _lineWidth = 0.1f;
    [Tooltip("重力")]
    [SerializeField] private Vector3 _gravity = new(0, -9.81f, 0);

    private float _axesLength;
    private List<float> _lengths = new();
    private List<Vector3> _positions = new();
    private List<Transform> _axises = new();
    private List<LineRenderer> _lineRenderers = new();

    private void Start()
    {
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
            if (distance < 1e-6 || change < 1e-8)
            {
                break;
            }

            _positions[^1] = targetPos;
            for (var i = _positions.Count - 1; i >= 1; i--)
            {
                var direction = (_positions[i] - _positions[i - 1]).normalized;
                _positions[i - 1] = _positions[i] - direction * _lengths[i - 1];
            }

            _positions[0] = basePos;
            for (var i = 0; i <= _positions.Count - 2; i++)
            {
                var direction = (_positions[i + 1] - _positions[i]).normalized;
                _positions[i + 1] = _positions[i] + direction * _lengths[i];
            }

            // 重力
            for (var i = 1; i < _positions.Count; i++)
            {
                _positions[i] += _gravity * (Time.deltaTime * Time.deltaTime);
            }
        }

        for (var i = 0; i < _positions.Count - 1; i++)
        {
            var origin = _axises[i].position;
            var current = _axises[i + 1].position;
            var target = _positions[i + 1];
            var delta = GetDeltaRotation(origin, current, target);
            _axises[i].rotation = delta * _axises[i].rotation;
        }

        for (var i = 0; i < _lineRenderers.Count; i++)
        {
            _lineRenderers[i].SetPosition(0, _axises[i].position + new Vector3(0, 0, -0.25f));
            _lineRenderers[i].SetPosition(1, _axises[i + 1].position + new Vector3(0, 0, -0.25f));
        }
    }

    private void VacuumMovement()
    {
        // バキュームの移動制限
        var dis = Vector3.Distance(_firstPoint.position, _targetPoint.position);
        if (dis >= _length)
        {
            var direction = (_targetPoint.position - _firstPoint.position).normalized;
            _targetPoint.position = _firstPoint.position + direction * _length;
        }
    }

    private void VacuumRotate()
    {
        // バキュームの向きを設定
        var dir = (_targetPoint.position - _player.position).normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _targetPoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private static Quaternion GetDeltaRotation(Vector3 origin, Vector3 current, Vector3 target)
    {
        var beforeDirection = (current - origin).normalized;
        var afterDirection = (target - origin).normalized;
        return Quaternion.FromToRotation(beforeDirection, afterDirection);
    }
}