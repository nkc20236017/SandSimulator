using UnityEngine;
using UnityEngine.InputSystem;

public class Vacuum : MonoBehaviour
{
	[Header("Vacuum Settings")]
	[SerializeField] private VacuumData _vacuumData;
	[SerializeField] private BlockData _blockData;
	[SerializeField] private Transform _pivot;
	
	private Camera _camera;
	private PlayerActions _playerActions;

	public Vector3 Direction { get; private set; }
	public PlayerActions.VacuumActions VacuumActions => _playerActions.Vacuum;
	public BlockData BlockData => _blockData;
	public VacuumData VacuumData => _vacuumData;
	public Transform Pivot => _pivot;
	public ISoundSourceable SoundSource { get; private set; }
	
	private void OnEnable()
	{
		_playerActions.Enable();
		
		var soundSource = FindObjectOfType<SoundSource>();
		SoundSource = soundSource.GetComponent<ISoundSourceable>();
		SoundSource.SetInstantiation("Eject");
		
		_camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		
		_playerActions = new PlayerActions();
		VacuumActions.VacuumPos.performed += OnGamepad;
		VacuumActions.VacuumMouse.performed += OnMouse;
	}

	private void Update()
	{
		RotateVacuum();
	}

	private void RotateVacuum()
	{
		float directionAngle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
		if (_pivot.parent.localScale.x < 0)
		{
			directionAngle += 180;
		}

		var angle = new Vector3(0, 0, directionAngle);
		_pivot.rotation = Quaternion.Euler(angle);
	}

	/// <summary>
	/// ゲームパッドの入力を取得して、吸引方向を決定する
	/// </summary>
	/// <param name="context"></param>
	private void OnGamepad(InputAction.CallbackContext context)
	{
		Vector2 vacuumPos = VacuumActions.VacuumPos.ReadValue<Vector2>();
		if (vacuumPos.sqrMagnitude == 0) { return; }
		
		Direction = vacuumPos.normalized;
	}
	
	/// <summary>
	/// マウスの位置を取得して、吸引方向を決定する
	/// </summary>
	/// <param name="context"></param>
	private void OnMouse(InputAction.CallbackContext context)
	{
		var position = context.ReadValue<Vector2>();
		if (_camera == null)
		{
			_camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		}
		
		Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(position);
		Vector3 direction = (mouseWorldPosition - _pivot.position).normalized;
		direction.z = 0;

		Direction = direction;
	}

	private void OnDisable()
	{
		_playerActions.Disable();
	}
}
