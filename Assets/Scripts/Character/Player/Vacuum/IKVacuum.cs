using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [Tooltip("関節の太さ")][MinValue(0f), MaxValue(1f)]
    [SerializeField] private float _jointWidth = 0.61f;
    [Tooltip("線の太さ")][MinValue(0f), MaxValue(1f)]
    [SerializeField] private float _lineWidth = 0.1f;
    [Tooltip("重力")]
    [SerializeField] private Vector3 _gravity = new(0, -9.81f, 0);
    [Tooltip("バキュームの範囲")][MinValue(0f)]
    [SerializeField] private float _vacuumRange = 1f;

    private float _axesLength;
    private Vector3 _direction;
    private List<float> _lengths = new();
    private List<Vector3> _positions = new();
    private List<Transform> _axises = new();
    private List<LineRenderer> _lineRenderers = new();
    private Camera _camera;
    private PlayerActions _playerActions;
    private PlayerActions.VacuumActions VacuumActions => _playerActions.Vacuum;

    private void OnEnable()
    {
        _camera = Camera.main;

        _playerActions = new PlayerActions();
        _playerActions.Enable();
        VacuumActions.VacuumPos.performed += OnGamepad;
        VacuumActions.VacuumMouse.performed += OnMouse;

        // バキュームを生成 / Generate the vacuum
        CreateIK();
    }

    /// <summary>
    /// ボーンを生成 / Generate bones
    /// </summary>
    private void CreateIK()
    {
        // コードの幅を設定 / Set the scale of the joint
        var codeScale = new Vector3(_jointWidth, _jointWidth, _jointWidth);
        _firstPoint.localScale = codeScale;
        
        // 最初のポイントを追加 / Add the first point
        _axises.Add(_firstPoint);
        Transform parent = _firstPoint;
        _axesLength = _length / _maxIteration;
        for (var i = 0; i < _maxIteration; i++)
        {
            // ボーンを生成 / Generate bone
            GameObject newPoint = Instantiate(_bonePrefab, parent.position + Vector3.up * _axesLength, Quaternion.identity);
            newPoint.transform.localScale = codeScale;
            newPoint.transform.SetParent(parent);
            _axises.Add(newPoint.transform);
            parent = newPoint.transform;
        }

        for (var i = 0; i < _axises.Count - 1; i++)
        {
            // 各関節間の長さを計算 / Calculate the length between each joint
            _lengths.Add(Vector3.Distance(_axises[i].position, _axises[i + 1].position));
        }

        foreach (var axis in _axises)
        {
            // 各関節の位置をリストに追加 / Add the position of each joint to the list
            _positions.Add(axis.position);
        }

        for (var i = 0; i < _axises.Count - 1; i++)
        {
            // 各関節感に描画するラインレンダラーを生成 / Generate a line renderer to draw between each joint
            LineRenderer lineRenderer = Instantiate(_linePrefab, _axises[i]);
            lineRenderer.material = _lineMaterial;
            lineRenderer.startWidth = _lineWidth;
            lineRenderer.endWidth = _lineWidth;
            _lineRenderers.Add(lineRenderer);
        }
    }

    private void Update()
    {
        // バキュームの移動 / Vacuum movement
        VacuumMovement();
        // バキュームの回転 / Vacuum rotation
        VacuumRotate();
        // 各関節の位置を更新 / Update positions of each joint
        UpdatePositions();
        // 逆運動学を適用 / Apply inverse kinematics
        ApplyIK();
        // 重力を適用 / Apply gravity
        ApplyGravity();
        // 各関節の回転を更新 / Update rotations of each joint
        UpdateRotations();
        // ラインレンダラーを更新 / Update line renderers
        UpdateLineRenderers();
        // 各関節間の長さを更新 / Update lengths between each joint
        UpdateLengths();
    }

    private void UpdatePositions()
    {
        for (var i = 0; i < _axises.Count; i++)
        {
            // 各関節の位置を更新 / Update the position of each joint
            _positions[i] = _axises[i].position;
        }
    }

    private void ApplyIK()
    {
        Vector3 basePosition = _positions[0];
        Vector3 targetPosition = _targetPoint.position;
        var prevDistance = 0.0f;

        for (var iter = 0; iter < _maxIteration; iter++)
        {
            float distance = Vector3.Distance(_positions[^1], targetPosition);
            float change = Mathf.Abs(distance - prevDistance);
            prevDistance = distance;
            if (distance < 1e-6 || change < 1e-8) { break; }

            _positions[^1] = targetPosition;
            for (var i = _positions.Count - 1; i >= 1; i--)
            {
                Vector3 direction = (_positions[i] - _positions[i - 1]).normalized;
                _positions[i - 1] = _positions[i] - direction * _lengths[i - 1];
            }

            _positions[0] = basePosition;
            for (var i = 0; i <= _positions.Count - 2; i++)
            {
                Vector3 direction = (_positions[i + 1] - _positions[i]).normalized;
                _positions[i + 1] = _positions[i] + direction * _lengths[i];
            }
        }
    }

    private void ApplyGravity()
    {
        for (var i = 1; i < _positions.Count; i++)
        {
            // 重力を適用 / Apply gravity
            _positions[i] += _gravity * (Time.deltaTime * Time.deltaTime);
        }
    }

    private void UpdateRotations()
    {
        for (var i = 0; i < _positions.Count - 1; i++)
        {
            Vector3 origin = _axises[i].position;
            Vector3 current = _axises[i + 1].position;
            Vector3 target = _positions[i + 1];
            // 2つのベクトルの回転を計算 / Calculate the rotation of two vectors
            Quaternion delta = GetDeltaRotation(origin, current, target);
            _axises[i].rotation = delta * _axises[i].rotation;
        }
    }

    private void UpdateLineRenderers()
    {
        for (var i = 0; i < _lineRenderers.Count; i++)
        {
            // ラインレンダラーの位置を更新 / Update the position of the line renderer
            _lineRenderers[i].SetPosition(0, _axises[i].position + new Vector3(0, 0, -0.25f));
            _lineRenderers[i].SetPosition(1, _axises[i + 1].position + new Vector3(0, 0, -0.25f));
        }
    }

    private void UpdateLengths()
    {
        for (var i = 0; i < _axises.Count - 1; i++)
        {
            // 各関節間の長さを更新 / Update the length between each joint
            _lengths[i] = Vector3.Distance(_axises[i].position, _axises[i + 1].position);
        }
    }

    /// <summary>
    /// バキュームの移動 / Vacuum movement
    /// </summary>
    private void VacuumMovement()
    {
        _targetPoint.position = _player.position + _direction * _vacuumRange;
    }

    /// <summary>
    /// ゲームパッドの入力を取得して、吸引方向を決定する / Get the input from the gamepad and determine the suction direction
    /// </summary>
    /// <param name="context"></param>
    private void OnGamepad(InputAction.CallbackContext context)
    {
        var vacuumPos = VacuumActions.VacuumPos.ReadValue<Vector2>();
        if (vacuumPos.sqrMagnitude == 0) { return; }

        _direction = vacuumPos.normalized;
    }

    /// <summary>
    /// マウスの位置を取得して、吸引方向を決定する / Get the position of the mouse and determine the suction direction
    /// </summary>
    /// <param name="context"></param>
    private void OnMouse(InputAction.CallbackContext context)
    {
        if (_camera == null)
        {
            _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }

        var position = context.ReadValue<Vector2>();
        Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(position);
        Vector3 direction = mouseWorldPosition - _player.position;
        direction.z = 0;

        _direction = direction.normalized;
    }

    /// <summary>
    /// バキュームの向きを設定 / Set the direction of the vacuum
    /// </summary>
    private void VacuumRotate()
    {
        Vector3 direction = (_targetPoint.position - _player.position).normalized;
        direction.z = 0;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _targetPoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    /// <summary>
    /// 2つのベクトルの回転を計算 / Calculate the rotation of two vectors
    /// </summary>
    /// <param name="origin">原点 / Origin</param>
    /// <param name="current">現在の位置 / Current position</param>
    /// <param name="target">目標の位置 / Target position</param>
    private static Quaternion GetDeltaRotation(Vector3 origin, Vector3 current, Vector3 target)
    {
        Vector3 beforeDirection = (current - origin).normalized;
        Vector3 afterDirection = (target - origin).normalized;
        return Quaternion.FromToRotation(beforeDirection, afterDirection);
    }

    private void OnDisable()
    {
        _playerActions.Disable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // バキュームの移動範囲を描画 / Draw the vacuum movement range
        Gizmos.DrawWireSphere(_player.position, _vacuumRange);
    }

    private void OnValidate()
    {
        // バキュームの範囲を設定 / Set the range of the vacuum
        _vacuumRange = Mathf.Min(_length, _vacuumRange);
    }
}