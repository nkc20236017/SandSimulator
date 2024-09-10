using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement Config")]
	[SerializeField] private float speed;
	[SerializeField] private float jumpForce;
	
	[Header("Ground Check")]
	[SerializeField] private bool isColliderRadius;
	[SerializeField] private float radius;
	[SerializeField] private LayerMask groundLayerMask;
	[SerializeField] private Tilemap tilemap;

	private float _currentSpeed;
	private Vector2 _moveDirection;
	private BoxCollider2D _boxCollider2D;
	private Rigidbody2D _rigidbody2D;
	private SpriteRenderer _spriteRenderer;
	private Camera _camera;
	private PlayerActions _playerActions;
	private PlayerActions.MovementActions MovementActions => _playerActions.Movement;
	
	public bool IsMoveFlip { get; set; } = true;

	private void Awake()
	{
		_playerActions = new PlayerActions();

		_boxCollider2D = GetComponent<BoxCollider2D>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	private void Start()
	{
		_camera = Camera.main;
		
		MovementActions.Jump.started += _ => Jump();
		MovementActions.Jump.canceled += _ => JumpCancel();
		
		_currentSpeed = speed;
	}

	private void FixedUpdate()
	{
		Movement();
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = new Vector2(_moveDirection.x * _currentSpeed, _rigidbody2D.velocity.y);
	}

	private void Update()
	{
		InputMovement();
		IsGround();
		OneBlockUp();
	}

	private void InputMovement()
	{
		_moveDirection = MovementActions.Move.ReadValue<Vector2>();
		
		Flip();
	}

	private void Jump()
	{
		if (!IsGround()) { return; }
		
		_rigidbody2D.velocity = Vector2.up * jumpForce;
	}
	
	private void JumpCancel()
	{
		if (_rigidbody2D.velocity.y > 0)
		{
			_rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _rigidbody2D.velocity.y * 0.5f);
		}
	}
	
	private bool IsGround()
	{
		var x = _boxCollider2D.bounds.center.x;
		var y = _boxCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		var hit = Physics2D.CircleCast(position, radius, Vector2.down, 0.1f, groundLayerMask);
		return hit.collider != null;
	}
	
	private void OneBlockUp()
	{
		if (_rigidbody2D.velocity.y != 0) { return; }
		if (_moveDirection == Vector2.zero) { return; }

		var x = _moveDirection.x > 0 ? _boxCollider2D.bounds.max.x : _boxCollider2D.bounds.min.x;
		var position = new Vector2(x, _boxCollider2D.bounds.min.y);
		var cellPosition = tilemap.WorldToCell(position);
		var directionPosition = cellPosition;
		var variate = tilemap.GetTile(directionPosition);
		var immediate = tilemap.GetTile(directionPosition + Vector3Int.up);
		if (immediate == null && variate != null)
		{
			transform.position += new Vector3(_moveDirection.x * 0.01f, Vector3.up.y);
		}
	}
	
	private void Flip()
	{
		if (IsMoveFlip)
		{
			_spriteRenderer.flipX = _moveDirection.x switch
			{
					> 0 => false,
					< 0 => true,
					_ => _spriteRenderer.flipX,
			};
		}
		else
		{
			var worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
			if (transform.position.x < worldPosition.x)
			{
				_spriteRenderer.flipX = false;
			}
			else if (transform.position.x > worldPosition.x)
			{
				_spriteRenderer.flipX = true;
			}
		}
	}
	
	public void SetSpeed(float speed)
	{
		_currentSpeed = speed;
	}
	
	public void ResetSpeed()
	{
		_currentSpeed = speed;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		var boxCollider2D = GetComponent<BoxCollider2D>();
		var x = boxCollider2D.bounds.center.x;
		var y = boxCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		if (isColliderRadius)
		{
			Gizmos.DrawWireSphere(position, boxCollider2D.size.x / 2);
		}
		else
		{
			Gizmos.DrawWireSphere(position, radius);
		}
		
		Gizmos.color = Color.green;
		if (_moveDirection == Vector2.zero) { return; }
		
		x = _moveDirection.x > 0 ? boxCollider2D.bounds.max.x : boxCollider2D.bounds.min.x;
		position = new Vector2(x, boxCollider2D.bounds.min.y);
		var cellPosition = tilemap.WorldToCell(position);
		var directionPosition = cellPosition + Vector3Int.RoundToInt(_moveDirection);
		Gizmos.DrawWireCube(tilemap.GetCellCenterWorld(directionPosition), Vector3.one);
		Gizmos.DrawWireCube(tilemap.GetCellCenterWorld(directionPosition + Vector3Int.up), Vector3.one);
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
