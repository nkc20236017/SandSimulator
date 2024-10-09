using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerActions;

public class Parabola : MonoBehaviour
{
    [Header("Parabola Config")]
    [SerializeField] private float interval;
    [SerializeField] private float dotSize;
    [SerializeField] private Transform pivot;
    [SerializeField] private float startDistance;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private LayerMask groundLayerMask;

    [SerializeField]
    private Vector3 controllerParabla;

    private Ray parablaRay;

    private PlayerActions playerActions;
    private Vector3 _startPosition;
    private Camera _camera;
    private GameObject[] _dots;

    private void Start()
    {
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        playerActions = new();
        playerActions.Vacuum.VacuumPos.performed += OnParabola;
        playerActions.Vacuum.VacuumMouse.performed += OnParabolaMouse;
        playerActions.Enable();
    }

    private void OnDestroy()
    {
        playerActions.Disable();
    }

    private void OnParabola(InputAction.CallbackContext context)
    {
        Vector3 mouseWorldPosition = context.ReadValue<Vector2>();
        Vector3 direction = playerActions.Vacuum.VacuumPos.ReadValue<Vector2>().sqrMagnitude != 0
? playerActions.Vacuum.VacuumPos.ReadValue<Vector2>().normalized :
mouseWorldPosition - pivot.position;

        controllerParabla = direction;

        parablaRay = new Ray(pivot.position, direction);
    }

    private void OnParabolaMouse(InputAction.CallbackContext context)
    {
        if(_camera == null)
        {
            _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }

        controllerParabla = (_camera.ScreenToWorldPoint(Input.mousePosition) - pivot.position).normalized;
        parablaRay = new Ray(_startPosition, (_camera.ScreenToWorldPoint(Input.mousePosition) - _startPosition).normalized);
    }

    public void GenerateParabola()
    {
        DestroyParabola();
        
        var startDirection = controllerParabla;
        _startPosition = pivot.position + startDirection * startDistance;
        
        var groundHitPosition = GetGroundHitPosition();
        var direction = (groundHitPosition - _startPosition).normalized;
        var distance = Vector3.Distance(_startPosition, groundHitPosition);
        var count = Mathf.CeilToInt(distance / interval);
        _dots = new GameObject[count];
        for (var i = 0; i < count; i++)
        {
            var position = _startPosition + direction * (interval * i);
            _dots[i] = Instantiate(dotPrefab, position, Quaternion.identity);
            _dots[i].transform.SetParent(transform);
            _dots[i].transform.position = position;
            _dots[i].transform.localScale = Vector3.one * dotSize;
        }
    }

    public void DestroyParabola()
    {
        if (_dots == null) { return; }

        foreach (var dot in _dots)
        {
            Destroy(dot);
        }
    }

    private Vector3 GetGroundHitPosition()
    {
        if (_camera == null)
        {
            _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }

        var ray = parablaRay;
        var hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, groundLayerMask);
        if (hit.collider == null)
        {
            return ray.origin + ray.direction * 100;
        }
        
        return hit.point;
    }
}
