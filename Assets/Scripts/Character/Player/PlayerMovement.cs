using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement Config")]
	[SerializeField] private float speed;
	[SerializeField] private float jumpForce;
	
	[Header("Ground Check")]
	[SerializeField] private bool isColliderRadius;
	[SerializeField] private float radius;
	[SerializeField] private LayerMask groundLayerMask;
	
	[Header("Auto Jump Config")]
	[SerializeField] private bool canAutoJump = true;
	[ShowIf(nameof(canAutoJump))]
	[SerializeField] private int autoJumpHeight = 1;
	
	[Header("Knockback Config")]
	[SerializeField] private float knockbackTime;
		
	private static readonly int IsJump = Animator.StringToHash("isJump");
	private static readonly int XVelocity = Animator.StringToHash("xVelocity");
	private float _knockbackTimer;
	private Vector2 _moveDirection;
	private BoxCollider2D _boxCollider2D;
	private Rigidbody2D _rigidbody2D;
	private Animator _animator;
	private SpriteRenderer _spriteRenderer;
	private Camera _camera;
	private PlayerActions _playerActions;
	private IChunkInformation _chunkInformation;

	public bool CanMove { get; set; } = true;
	public bool IsMoveFlip { get; set; } = true;
	private PlayerActions.MovementActions MovementActions => _playerActions.Movement;

	private void Awake()
	{
		_playerActions = new PlayerActions();
		_animator = transform.Find("Model").GetComponent<Animator>();
		_boxCollider2D = GetComponent<BoxCollider2D>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	private void Start()
	{
		MovementActions.Jump.started += _ => Jump();
		MovementActions.Jump.canceled += _ => JumpCancel();
    }

	private void FixedUpdate()
	{
		Animation();

		if (!CanMove) { return; }

		if (canAutoJump)
		{
			AutoBlockJump();
		}
		Movement();
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = new Vector2(_moveDirection.x * speed, _rigidbody2D.velocity.y);
	}


    private void Update()
	{
		if (!CanMove)
		{
			_knockbackTimer += Time.deltaTime;
			if (_knockbackTimer >= knockbackTime)
			{
				CanMove = true;
				_knockbackTimer = 0;
			}
		}
		
		InputMovement();
		IsGround();
	}

	private void InputMovement()
	{
		_moveDirection = MovementActions.Move.ReadValue<Vector2>();
		
		Flip();
	}

	private void Jump()
	{
		if (!CanMove) { return; }
		if (!IsGround()) { return; }
		
		_rigidbody2D.velocity = Vector2.up * jumpForce * Time.fixedDeltaTime;
	}
	
	private void JumpCancel()
	{
		if (_rigidbody2D.velocity.y > 0)
		{
			_rigidbody2D.velocity *= Vector2.up * 0.5f;
		}
	}
	
	private void AutoBlockJump()
	{
		if (_rigidbody2D.velocity.y is > 0.001f or < -0.001f) { return; }
		if (_moveDirection.x == 0) { return; }
		if (!IsGround()) { return; }

		var x = _moveDirection.x >= 0 ? _boxCollider2D.bounds.max.x + 0.25f : _boxCollider2D.bounds.min.x - 0.25f;
		for (var y = 1; y <= autoJumpHeight; y++)
		{
			var position = new Vector2(x, _boxCollider2D.bounds.min.y + y - 1);
			var tilemap = _chunkInformation.GetChunkTilemap(position);
			if (tilemap == null) { continue; }
			
			var cellPosition = _chunkInformation.WorldToChunk(position);
			if (!tilemap.HasTile(cellPosition) || tilemap.HasTile(cellPosition + Vector3Int.up)) { continue; }

			if (IsWall(y) || IsHeavenly(y))
			{
				if (y == autoJumpHeight) { return; }

				continue;
			}

			transform.position += new Vector3(0.1f, y + 0.1f, 0);
			return;
		}
	}
	
	private bool IsGround()
	{
		if (_rigidbody2D.velocity.y is > 0.001f or < -0.001f) { return false; }
		
		var x = _boxCollider2D.bounds.center.x;
		var y = _boxCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		var hit = isColliderRadius ? Physics2D.CircleCast(position, _boxCollider2D.size.x / 2 - 0.1f, Vector2.down, 0.1f, groundLayerMask) : Physics2D.CircleCast(position, radius, Vector2.down, 0.1f, groundLayerMask);
		return hit.collider != null;
	}
	
	private bool IsWall(float minY)
	{
		if (_rigidbody2D.velocity.y is > 0.01f or < -0.01f) { return false; }
		if (_moveDirection.x == 0) { return false; }
		
		var x = _moveDirection.x >= 0 ? _boxCollider2D.bounds.max.x + 0.25f : _boxCollider2D.bounds.min.x - 0.25f;
		var maxY = _boxCollider2D.bounds.size.y - minY;
		for (var y = minY + 1; y <= maxY; y++)
		{
			var position = new Vector2(x, _boxCollider2D.bounds.min.y + y);
			var tilemap = _chunkInformation.GetChunkTilemap(position);
			if (tilemap == null) { return true; }
			
			var cellPosition = _chunkInformation.WorldToChunk(position);
			if (!tilemap.HasTile(cellPosition)) { continue; }
			
			return true;
		}
		
		return false;
	}
	
	private bool IsHeavenly(float height)
	{
		var minX = _boxCollider2D.bounds.min.x - 0.25f;
		var maxX = _boxCollider2D.bounds.max.x + 1.25f;
		for (var y = 1; y <= height; y++)
		{
			for (var x = minX; x <= maxX; x++)
			{
				var position = new Vector2(x, _boxCollider2D.bounds.max.y + y);
				var tilemap = _chunkInformation.GetChunkTilemap(position);
				if (tilemap == null) { continue; }
				
				var cellPosition = _chunkInformation.WorldToChunk(position);
				if (!tilemap.HasTile(cellPosition)) { continue; }

				return true;
			}
		}

		return false;
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
			if (_camera == null)
			{
				_camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
			}
			
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
	
	public void Knockback(Vector2 direction)
	{
		_rigidbody2D.velocity = direction;
		CanMove = false;
	}

	private void Animation()
	{
        _animator.SetBool(IsJump, !IsGround());
        _animator.SetFloat(XVelocity, Mathf.Abs(_rigidbody2D.velocity.x));
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
	}
	
	private void OnEnable()
	{
		_playerActions.Enable();
		
		var worldMapManager = FindObjectOfType<WorldMapManager>();
		_chunkInformation = worldMapManager.GetComponent<IChunkInformation>();
		
		_camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
	}
	
	private void OnDisable()
	{
		_playerActions.Disable();
	}
}
