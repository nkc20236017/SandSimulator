// System.Collections.Genericを使用 / Use System.Collections.Generic
using System.Collections.Generic;
// UnityEngine.InputSystemを使用 / Use UnityEngine.InputSystem
using UnityEngine;
// UnityEngine.InputSystemを使用 / Use UnityEngine.InputSystem
using UnityEngine.InputSystem;
// NaughtyAttributesを使用 / Use NaughtyAttributes
using NaughtyAttributes;

// IKVacuumクラスを定義しMonoBehaviourを継承 / Define the IKVacuum class and inherit
public class IKVacuum : MonoBehaviour
{
    // グループIK Settingsを定義 / Define the Group IK Settings
    [Header("IK Settings")]
    // プレイヤーを指定 / Specify the player
    [SerializeField] private Transform _player;
    // ボーンのプレハブを指定 / Specify the bone prefab
    [SerializeField] private GameObject _bonePrefab;
    // ラインレンダラーのプレハブを指定 / Specify the line renderer prefab
    [SerializeField] private LineRenderer _linePrefab;
    // 最初のポイントを指定 / Specify the first point
    [SerializeField] private Transform _firstPoint;
    // ターゲットポイントを指定 / Specify the target point
    [SerializeField] private Transform _targetPoint;
    // ラインのマテリアルを指定 / Specify the line material
    [SerializeField] private Material _lineMaterial;

    // グループIK Configを定義 / Define the Group IK Config
    [Header("IK Config")]
    // 間接の数を指定する。最小値は0 / Specify the number of joints. Minimum value is 0
    [Tooltip("間接の数")][Min(0)]
    [SerializeField] private int _maxIteration = 5;
    // 線の長さを指定 / Specify the length of the line
    [Tooltip("線の長さ")]
    [SerializeField] private float _length = 1f;
    // 線の太さを指定。最小値は0、最大値は1 / Specify the thickness of the line. Minimum value is 0, maximum value is 1
    [Tooltip("線の太さ")][MinValue(0f), MaxValue(1f)]
    [SerializeField] private float _lineWidth = 0.1f;
    // 重力を指定 / Specify the gravity
    [Tooltip("重力")]
    [SerializeField] private Vector3 _gravity = new(0, -9.81f, 0);
    // バキュームの範囲を指定。最小値は0 / Specify the range of the vacuum. Minimum value is 0
    [Tooltip("バキュームの範囲")][MinValue(0f)]
    [SerializeField] private float _vacuumRange = 1f;

    // 関節間の長さ / Length between joints
    private float _axesLength;
    // プレイヤーからカーソルの方向 / Direction from player to cursor
    private Vector3 _direction;
    // 各関節間の長さ / Length between each joint
    private List<float> _lengths = new();
    // 各関節の位置 / Position of each joint
    private List<Vector3> _positions = new();
    // 各関節のTransform / Transform of each joint
    private List<Transform> _axises = new();
    // 各関節間のラインレンダラー / Line renderer between each joint
    private List<LineRenderer> _lineRenderers = new();
    // カーソルの位置を取得するためのカメラ / Camera to get the position of the cursor
    private Camera _camera;
    // プレイヤーのアクション / Player actions
    private PlayerActions _playerActions;
    // プレイヤーのバキュームアクション / Player's vacuum actions
    private PlayerActions.VacuumActions VacuumActions => _playerActions.Vacuum;

    /// <summary>
    /// ボーンを生成 / Generate bones
    /// </summary>
    private void CreateIK()
    {
        // 最初のポイントを追加 / Add the first point
        _axises.Add(_firstPoint);
        // 親を設定 / Set the parent
        Transform parent = _firstPoint;
        // 各関節間の長さを計算 / Calculate the length between each joint
        _axesLength = _length / _maxIteration;
        // 間接の数だけボーンを生成 / Generate bones for the number of joints
        for (int i = 0; i < _maxIteration; i++)
        {
            // ボーンを生成 / Generate bone
            GameObject newPoint = Instantiate(_bonePrefab, parent.position + Vector3.up * _axesLength, Quaternion.identity);
            // ボーンの親を設定 / Set the parent of the bone
            newPoint.transform.SetParent(parent);
            // ボーンをリストに追加 / Add the bone to the list
            _axises.Add(newPoint.transform);
            // 親を更新 / Update the parent
            parent = newPoint.transform;
        }

        // 各関節の分だけ実行する / Execute for each joint
        for (var i = 0; i < _axises.Count - 1; i++)
        {
            // 各関節間の長さを計算 / Calculate the length between each joint
            _lengths.Add(Vector3.Distance(_axises[i].position, _axises[i + 1].position));
        }

        // 各関節の分だけ実行する / Execute for each joint
        foreach (var axis in _axises)
        {
            // 各関節の位置をリストに追加 / Add the position of each joint to the list
            _positions.Add(axis.position);
        }

        // 各関節の分だけ実行する / Execute for each joint
        for (var i = 0; i < _axises.Count - 1; i++)
        {
            // 各関節感に描画するラインレンダラーを生成 / Generate a line renderer to draw between each joint
            LineRenderer lineRenderer = Instantiate(_linePrefab, _axises[i]);
            // ラインのマテリアルを設定 / Set the material of the line
            lineRenderer.material = _lineMaterial;
            // ラインの太さを設定 / Set the thickness of the line
            lineRenderer.startWidth = _lineWidth;
            // ラインの太さを設定 / Set the thickness of the line
            lineRenderer.endWidth = _lineWidth;
            // ラインレンダラーをリストに追加 / Add the line renderer to the list
            _lineRenderers.Add(lineRenderer);
        }
    }

    /// <summary>
    /// Unityイベント関数updateを定義 / Define the Unity event function update
    /// 繰り返し実行される / Repeatedly executed
    /// </summary>
    private void Update()
    {
        // バキュームの移動 / Vacuum movement
        VacuumMovement();
        // バキュームの回転 / Vacuum rotation
        VacuumRotate();

        // 各関節の分だけ実行する / Execute for each joint
        for (var i = 0; i < _axises.Count; i++)
        {
            // 各関節の位置をリストに追加 / Add the position of each joint to the list
            _positions[i] = _axises[i].position;
        }

        // ベースの位置を設定 / Set the base position
        Vector3 basePosition = _positions[0];
        // ターゲットの位置を設定 / Set the target position
        Vector3 targetPosition = _targetPoint.position;
        // 前回の距離を設定 / Set the previous distance
        var prevDistance = 0.0f;
        // 最大繰り返し回数だけ実行する / Execute for the maximum number of iterations
        for (var iter = 0; iter < _maxIteration; iter++)
        {
            // ターゲットと最後の関節の距離を計算 / Calculate the distance between the target and the last joint
            float distance = Vector3.Distance(_positions[^1], targetPosition);
            // 変化量を計算 / Calculate the change
            float change = Mathf.Abs(distance - prevDistance);
            // 前回の距離を設定 / Set the previous distance
            prevDistance = distance;
            // 一定の距離以下か変化量が一定の値以下の場合、ループを抜ける / If the distance is less than a certain distance or the change is less than a certain value, exit the loop
            if (distance < 1e-6 || change < 1e-8) { break; }

            // ターゲットの位置を更新 / Update the position of the target
            _positions[^1] = targetPosition;
            // 各関数の分だけ実行する / Execute for each function
            for (var i = _positions.Count - 1; i >= 1; i--)
            {
                // 各関節間の方向を計算 / Calculate the direction between each joint
                Vector3 direction = (_positions[i] - _positions[i - 1]).normalized;
                // 各関節の位置を更新 / Update the position of each joint
                _positions[i - 1] = _positions[i] - direction * _lengths[i - 1];
            }

            // ベースの位置を更新 / Update the base position
            _positions[0] = basePosition;
            // 各関節の分だけ実行する / Execute for each joint
            for (var i = 0; i <= _positions.Count - 2; i++)
            {
                // 各関節間の方向を計算 / Calculate the direction between each joint
                Vector3 direction = (_positions[i + 1] - _positions[i]).normalized;
                // 各関節の位置を更新 / Update the position of each joint
                _positions[i + 1] = _positions[i] + direction * _lengths[i];
            }

            // 重力を適用 / Apply gravity
            for (var i = 1; i < _positions.Count; i++)
            {
                // 各関節の位置を更新 / Update the position of each joint
                _positions[i] += _gravity * (Time.deltaTime * Time.deltaTime);
            }
        }

        // 各ボーンの回転を計算 / Calculate the rotation of each bone
        for (var i = 0; i < _positions.Count - 1; i++)
        {
            // 各関節の位置を取得 / Get the position of each joint
            Vector3 origin = _axises[i].position;
            // 各関節の位置を取得 / Get the position of each joint
            Vector3 current = _axises[i + 1].position;
            // 各関節の位置を取得 / Get the position of each joint
            Vector3 target = _positions[i + 1];
            // 各関節の回転を計算 / Calculate the rotation of each joint
            Quaternion delta = GetDeltaRotation(origin, current, target);
            // 各関節の回転を更新 / Update the rotation of each joint
            _axises[i].rotation = delta * _axises[i].rotation;
        }

        // ボーンの位置を更新 / Update the position of the bones
        for (var i = 0; i < _lineRenderers.Count; i++)
        {
            // ボーンの位置を更新 / Update the position of the bones
            _lineRenderers[i].SetPosition(0, _axises[i].position + new Vector3(0, 0, -0.25f));
            // ボーンの位置を更新 / Update the position of the bones
            _lineRenderers[i].SetPosition(1, _axises[i + 1].position + new Vector3(0, 0, -0.25f));
        }

        // 各ボーン間の長さを更新 / Update the length between each bone
        for (var i = 0; i < _axises.Count - 1; i++)
        {
            // 各関節間の長さを計算 / Calculate the length between each joint
            _lengths[i] = Vector3.Distance(_axises[i].position, _axises[i + 1].position);
        }
    }

    /// <summary>
    /// バキュームの移動 / Vacuum movement
    /// </summary>
    private void VacuumMovement()
    {
        // バキュームの移動制限 / Restrict the movement of the vacuum
        _targetPoint.position = _player.position + _direction * _vacuumRange;
    }

    /// <summary>
    /// ゲームパッドの入力を取得して、吸引方向を決定する / Get the input from the gamepad and determine the suction direction
    /// </summary>
    /// <param name="context"></param>
    private void OnGamepad(InputAction.CallbackContext context)
    {
        // バキュームの位置を取得 / Get the position of the vacuum
        var vacuumPos = VacuumActions.VacuumPos.ReadValue<Vector2>();
        // バキュームの位置が0の場合、戻る / Return if the vacuum position is 0
        if (vacuumPos.sqrMagnitude == 0) { return; }

        // バキュームの位置を正規化 / Normalize the position of the vacuum
        _direction = vacuumPos.normalized;
    }

    /// <summary>
    /// マウスの位置を取得して、吸引方向を決定する / Get the position of the mouse and determine the suction direction
    /// </summary>
    /// <param name="context"></param>
    private void OnMouse(InputAction.CallbackContext context)
    {
        // カメラがnullの場合、カメラを取得 / Get the camera if it is null
        if (_camera == null)
        {
            // カメラを取得 / Get the camera
            _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }

        // マウスの位置を取得 / Get the position of the mouse
        var position = context.ReadValue<Vector2>();
        // マウスの位置をワールド座標に変換 / Convert the position of the mouse to world coordinates
        Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(position);
        // プレイヤーからマウスの位置を取得 / Get the position of the mouse from the player
        Vector3 direction = mouseWorldPosition - _player.position;
        // プレイヤーからマウスの位置を取得 / Get the position of the mouse from the player
        direction.z = 0;

        // バキュームの位置を正規化 / Normalize the position of the vacuum
        _direction = direction.normalized;
    }

    /// <summary>
    /// バキュームの向きを設定 / Set the direction of the vacuum
    /// </summary>
    private void VacuumRotate()
    {
        // バキュームの向きを設定 / Set the direction of the vacuum
        Vector3 direction = (_targetPoint.position - _player.position).normalized;
        // バキュームの向きを設定 / Set the direction of the vacuum
        direction.z = 0;
        // バキュームの向きを設定 / Set the direction of the vacuum
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // バキュームの向きを設定 / Set the direction of the vacuum
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
        // 2つのベクトルの回転を計算 / Calculate the rotation of two vectors
        Vector3 beforeDirection = (current - origin).normalized;
        // 2つのベクトルの回転を計算 / Calculate the rotation of two vectors
        Vector3 afterDirection = (target - origin).normalized;
        // 2つのベクトルの回転を計算 / Calculate the rotation of two vectors
        return Quaternion.FromToRotation(beforeDirection, afterDirection);
    }
    
    /// <summary>
    /// Unityイベント関数enableを定義 / Define the Unity event function enable
    /// </summary>
    private void OnEnable()
    {
        // カメラを取得 / Get the camera
        _camera = Camera.main;
        
        // プレイヤーのアクションを取得 / Get the player's actions
        _playerActions = new PlayerActions();
        // プレイヤーのアクションを有効にする / Enable the player's actions
        _playerActions.Enable();
        // プレイヤーのバキュームアクションを有効にする / Enable the player's vacuum actions
        VacuumActions.VacuumPos.performed += OnGamepad;
        // プレイヤーのバキュームアクションを有効にする / Enable the player's vacuum actions
        VacuumActions.VacuumMouse.performed += OnMouse;
        
        // バキュームを生成 / Generate the vacuum
        CreateIK();
    }

    /// <summary>
    /// Unityイベント関数disableを定義 / Define the Unity event function disable
    /// </summary>
    private void OnDisable()
    {
        // プレイヤーのバキュームアクションを無効にする / Disable the player's vacuum actions
        _playerActions.Disable();
    }
    
    /// <summary>
    /// Unityイベント関数OnDrawGizmosを定義 / Define the Unity event function OnDrawGizmos
    /// </summary>
    private void OnDrawGizmos()
    {
        // ギズモの色を設定 / Set the color of the gizmo
        Gizmos.color = Color.green;
        // バキュームの移動範囲を描画 / Draw the vacuum movement range
        Gizmos.DrawWireSphere(_player.position, _vacuumRange);
    }
    
    /// <summary>
    /// Unityイベント関数OnValidateを定義 / Define the Unity event function OnValidate
    /// </summary>
    private void OnValidate()
    {
        // バキュームの範囲を設定 / Set the range of the vacuum
        _vacuumRange = Mathf.Min(_length, _vacuumRange);
    }
}
