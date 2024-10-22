using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class IKVacuum : MonoBehaviour
{
    [Header("IK Settings")]
    [SerializeField] private GameObject _bonePrefab;
    [SerializeField] private LineRenderer _linePrefab;
    [SerializeField, MinValue(0f), MaxValue(1f)] private float _lineWidth = 0.1f;
    [SerializeField] private Transform _firstPoint;
    [SerializeField] private Transform _targetPoint;
    [SerializeField, Min(0)] private int maxIteration = 5;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float _length = 1f;

    private List<float> lengths = new();
    private List<Vector3> positions = new();
    private List<Transform> axises = new();
    private List<LineRenderer> lineRenderers = new();

    private void Start()
    {
        CreateIK();
    }

    private void CreateIK()
    {
        axises.Add(_firstPoint);
        Transform parent = _firstPoint;
        for (int i = 0; i < maxIteration; i++)
        {
            GameObject newPoint = Instantiate(_bonePrefab, parent.position + Vector3.up * _length, Quaternion.identity);
            newPoint.transform.SetParent(parent);
            axises.Add(newPoint.transform);
            parent = newPoint.transform;
        }

        lengths.Clear();
        for (var i = 0; i < axises.Count - 1; i++)
        {
            lengths.Add(Vector3.Distance(axises[i].position, axises[i + 1].position));
        }

        positions.Clear();
        foreach (var b in axises)
        {
            positions.Add(b.position);
        }

        for (var i = 0; i < axises.Count - 1; i++)
        {
            LineRenderer lineRenderer = Instantiate(_linePrefab, axises[i]);
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = _lineWidth;
            lineRenderer.endWidth = _lineWidth;
            lineRenderers.Add(lineRenderer);
        }
    }

    private void Update()
    {
        var dir = (_targetPoint.position - _firstPoint.position).normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _targetPoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        
        for (var i = 0; i < axises.Count; i++)
        {
            positions[i] = axises[i].position;
        }

        var basePos = positions[0];
        var targetPos = _targetPoint.position;
        var prevDistance = 0.0f;
        for (var iter = 0; iter < maxIteration; iter++)
        {
            var distance = Vector3.Distance(positions[positions.Count - 1], targetPos);
            var change = Mathf.Abs(distance - prevDistance);
            prevDistance = distance;
            if (distance < 1e-6 || change < 1e-8)
            {
                break;
            }

            positions[^1] = targetPos;
            for (var i = positions.Count - 1; i >= 1; i--)
            {
                var direction = (positions[i] - positions[i - 1]).normalized;
                positions[i - 1] = positions[i] - direction * lengths[i - 1];
            }

            positions[0] = basePos;
            for (var i = 0; i <= positions.Count - 2; i++)
            {
                var direction = (positions[i + 1] - positions[i]).normalized;
                positions[i + 1] = positions[i] + direction * lengths[i];
            }

            // Apply gravity
            for (var i = 1; i < positions.Count; i++)
            {
                positions[i] += Vector3.down * (gravity * Time.deltaTime * Time.deltaTime);
            }
        }

        for (var i = 0; i < positions.Count - 1; i++)
        {
            var origin = axises[i].position;
            var current = axises[i + 1].position;
            var target = positions[i + 1];
            var delta = GetDeltaRotation(origin, current, target);
            axises[i].rotation = delta * axises[i].rotation;
        }

        for (var i = 0; i < lineRenderers.Count; i++)
        {
            lineRenderers[i].SetPosition(0, axises[i].position + new Vector3(0, 0, -0.25f));
            lineRenderers[i].SetPosition(1, axises[i + 1].position + new Vector3(0, 0, -0.25f));
        }
    }

    private static Quaternion GetDeltaRotation(Vector3 origin, Vector3 current, Vector3 target)
    {
        var beforeDirection = (current - origin).normalized;
        var afterDirection = (target - origin).normalized;
        return Quaternion.FromToRotation(beforeDirection, afterDirection);
    }
}