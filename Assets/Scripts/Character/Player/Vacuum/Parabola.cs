using System.Collections.Generic;
using SingularityGroup.HotReload;
using UnityEngine;

public class Parabola : MonoBehaviour
{
    [Header("Parabola Config")]
    [SerializeField] private float interval;
    [SerializeField] private float dotSize;
    [SerializeField] private Transform pivot;
    [SerializeField] private float startDistance;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private LayerMask groundLayerMask;

    private Vector3 _startPosition;
    private Camera _camera;
    private GameObject[] _dots;
    private Queue<GameObject> _dotPool = new();

    private void Start()
    {
        _camera = Camera.main;
    }

    public void GenerateParabola()
    {
        DestroyParabola();
        
        var startDirection = (_camera.ScreenToWorldPoint(Input.mousePosition) - pivot.position).normalized;
        _startPosition = pivot.position + startDirection * startDistance;
        
        var groundHitPosition = GetGroundHitPosition();
        var direction = (groundHitPosition - _startPosition).normalized;
        var distance = Vector3.Distance(_startPosition, groundHitPosition);
        var count = Mathf.CeilToInt(distance / interval);
        _dots = new GameObject[count];
        for (var i = 0; i < count; i++)
        {
            var position = _startPosition + direction * (interval * i);
            _dots[i] = GetDot();
            _dots[i].transform.position = position;
            _dots[i].transform.localScale = Vector3.one * dotSize;
            _dots[i].SetActive(true);
        }
    }

    private void DestroyParabola()
    {
        if (_dots == null) { return; }

        foreach (var dot in _dots)
        {
            ReturnDot(dot);
        }
    }

    private Vector3 GetGroundHitPosition()
    {
        var ray = new Ray(_startPosition, (_camera.ScreenToWorldPoint(Input.mousePosition) - _startPosition).normalized);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, groundLayerMask);
        if (hit.collider == null)
        {
            return ray.origin + ray.direction * 100;
        }
        
        return hit.point;
    }

    private GameObject GetDot()
    {
        if (_dotPool.Count == 0)
        {
            AddDots(1);
        }
        return _dotPool.Dequeue();
    }

    private void ReturnDot(GameObject dot)
    {
        dot.SetActive(false);
        _dotPool.Enqueue(dot);
    }

    private void AddDots(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var newDot = Instantiate(dotPrefab, pivot, true);
            newDot.SetActive(false);
            _dotPool.Enqueue(newDot);
        }
    }
}