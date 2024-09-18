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

	private float _currentSpeed;
	private Vector2 _moveDirection;
	private Tilemap tilemap;
	private BoxCollider2D _boxCollider2D;
	private Rigidbody2D _rigidbody2D;
	private SpriteRenderer _spriteRenderer;
	private Camera _camera;
	private PlayerActions _playerActions;
	private IChunkInformation _chunkInformation;
	
	public bool IsMoveFlip { get; set; } = true;
	private PlayerActions.MovementActions MovementActions => _playerActions.Movement;

	private void Awake()
	{
		_playerActions = new PlayerActions();

		_boxCollider2D = GetComponent<BoxCollider2D>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		_chunkInformation = GetComponent<IChunkInformation>();
	}

	private void Start()
	{
		_camera = Camera.main;
		
		MovementActions.Jump.started += _ => Jump();
		MovementActions.Jump.canceled += _ => JumpCancel();
		
		_currentSpeed = speed;

		SetTilemap();
	}

	private void FixedUpdate()
	{
		Movement();
		OneBlockUp();
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = new Vector2(_moveDirection.x * _currentSpeed, _rigidbody2D.velocity.y);
	}

	private void Update()
	{
		SetTilemap();
		InputMovement();
		IsGround();
	}

	private void SetTilemap()
	{
		tilemap = _chunkInformation.GetChunk(transform.position);
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
			_rigidbody2D.velocity *= Vector2.up * 0.5f;
		}
	}
	
	private void OneBlockUp()
	{
		if (_rigidbody2D.velocity.y is > 0.01f or < -0.01f) { return; }
		if (_moveDirection.x == 0) { return; }
		if (!IsGround()) { return; }

		var x = _moveDirection.x > 0 ? _boxCollider2D.bounds.max.x + 0.25f : _boxCollider2D.bounds.min.x - 0.25f;
		var position = new Vector2(x, _boxCollider2D.bounds.min.y + 0.1f);
		var cellPosition = tilemap.WorldToCell(position);
		if (tilemap.HasTile(cellPosition) && !tilemap.HasTile(cellPosition + Vector3Int.up))
		{
			transform.position += new Vector3(0.1f, 1.1f, 0);
		}
	}
	
	private bool IsGround()
	{
		var x = _boxCollider2D.bounds.center.x;
		var y = _boxCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		var hit = isColliderRadius ? Physics2D.CircleCast(position, _boxCollider2D.size.x / 2 - 0.1f, Vector2.down, 0.1f, groundLayerMask) : Physics2D.CircleCast(position, radius, Vector2.down, 0.1f, groundLayerMask);
		return hit.collider != null;
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

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		var boxCollider2D = GetComponent<BoxCollider2D>();
		var x = boxCollider2D.bounds.center.x;
		var y = boxCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		if (isColliderRadius)
		{
			Gizmos.DrawWireSphere(position, boxCollider2D.size.x / 2 - 0.1f);
		}
		else
		{
			Gizmos.DrawWireSphere(position, radius);
		}
		
		Gizmos.color = Color.green;
		if (_moveDirection.x == 0) { return; }
		
		x = _moveDirection.x > 0 ? boxCollider2D.bounds.max.x + 0.25f : boxCollider2D.bounds.min.x - 0.25f;
		position = new Vector2(x, boxCollider2D.bounds.min.y + 0.1f);
		var cellPosition = tilemap.WorldToCell(position);
		Gizmos.DrawWireCube(tilemap.GetCellCenterWorld(cellPosition), new Vector3(1, 1, 0));
		Gizmos.DrawWireCube(tilemap.GetCellCenterWorld(cellPosition + Vector3Int.up), new Vector3(1, 1, 0));
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
