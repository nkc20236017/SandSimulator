using UnityEngine;
using UnityEngine.InputSystem;

public class Vacuum : MonoBehaviour
{
	[Header("Vacuum Settings")]
	[SerializeField] private BlockDatas _blockDatas;
	[SerializeField] private Transform _pivot;
	
	private Camera _camera;
	private PlayerActions _playerActions;
	private PlayerActions.VacuumActions VacuumActions => _playerActions.Vacuum;

	public Vector3 _direction { get; private set; }
	public BlockDatas BlockDatas => _blockDatas;
	public Transform Pivot => _pivot;
	
	private void Start()
	{
		_camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		
		_playerActions = new PlayerActions();
		VacuumActions.VacuumPos.performed += OnGamepad;
		VacuumActions.VacuumMouse.performed += OnMouse;
	}

	private void Update()
	{
		RotateToCursorDirection();
	}

	private void RotateToCursorDirection()
	{
		float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
		if (_pivot.parent.localScale.x < 0)
		{
			angle += 180;
		}

		_pivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}

	private void OnGamepad(InputAction.CallbackContext context)
	{
		Vector2 vacuumPos = VacuumActions.VacuumPos.ReadValue<Vector2>();
		if (vacuumPos.sqrMagnitude == 0) { return; }
		
		_direction = vacuumPos.normalized;
	}
	
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

		_direction = direction;

		return _direction;
	}

	private void OnEnable()
	{
		_playerActions.Enable();
	}

	private void OnDisable()
	{
		_playerActions.Disable();
	}
}
