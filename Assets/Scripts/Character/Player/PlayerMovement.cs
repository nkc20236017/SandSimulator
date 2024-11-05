using UnityEngine;
using NaughtyAttributes;

public class PlayerMovement : MonoBehaviour, IWorldGenerateWaitable
{
	[Header("Movement Settings")]
	[Tooltip("プレイヤーの移動速度")] [Min(0f)]
	[SerializeField] private float _moveSpeed;
	[Tooltip("プレイヤーのジャンプ力")] [Min(0f)]
	[SerializeField] private float _jumpForce;
	
	[Header("Ground Config")]
	[SerializeField] private LayerMask _groundLayerMask;
	
	[Header("Ground Settings")]
	[Tooltip("半径をコライダーに合わせるかどうか")]
	[SerializeField] private bool _isColliderRadius;
	[Tooltip("地面判定の半径")] [Min(0f)] [HideIf(nameof(_isColliderRadius))]
	[SerializeField] private float _radius;
	[Tooltip("地面判定の高さ")] [Min(0f)]
	[SerializeField] private float _height;
	
	[Header("Auto Jump Settings")]
	[Tooltip("自動ジャンプの許可")]
	[SerializeField] private bool _canAutoJump = true;
	[Tooltip("自動ジャンプできるブロックの高さ")] [ShowIf(nameof(_canAutoJump))]
	[SerializeField] private int _autoJumpHeight = 1;
	
	[Header("Knock back Settings")]
	[Tooltip("ノックバック時間")] [Min(0f)]
	[SerializeField] private float _knockBackTime;
	
	private float _knockBackTimer;
	private bool _canMove = true;
	private bool _isJumping;
	private Vector2 _moveDirection;
	private BoxCollider2D _boxCollider2D;
	private Rigidbody2D _rigidbody2D;
	private PlayerActions _playerActions;

	public IChunkInformation ChunkInformation { get; private set; }
	private PlayerActions.MovementActions MovementActions => _playerActions.Movement;

	private void Awake()
	{
		_playerActions = new PlayerActions();
		_boxCollider2D = GetComponent<BoxCollider2D>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		MovementActions.Jump.started += _ => Jump();
		MovementActions.Jump.canceled += _ => JumpCancel();
    }

	private void FixedUpdate()
	{
		if (!_canMove) { return; }

		AutoBlockJump();
		Movement();
	}
	
	/// <summary>
	/// 移動処理
	/// </summary>
	private void Movement()
	{
		float x = _moveDirection.x * (_moveSpeed * Time.fixedDeltaTime);
		var calculatedMoveForce = new Vector2(x, _rigidbody2D.velocity.y);
		_rigidbody2D.velocity = calculatedMoveForce;
	}
	
    private void Update()
    {
	    KnockBack();
	    InputMovement();
    }

    /// <summary>
    /// ノックバック処理
    /// </summary>
	private void KnockBack()
	{
		if (_canMove) { return; }

		_knockBackTimer += Time.deltaTime;
		if (_knockBackTimer < _knockBackTime) { return; }

		_canMove = true;
		_knockBackTimer = 0;
	}

    /// <summary>
    /// 移動方向をアクションから取得
    /// </summary>
	private void InputMovement()
	{
		_moveDirection = MovementActions.Move.ReadValue<Vector2>();
	}

    /// <summary>
    /// ジャンプ処理
    /// </summary>
	private void Jump()
	{
		if (!_canMove) { return; }
		if (!IsGround()) { return; }

		_isJumping = true;
		
		var calculatedJumpForce = Vector2.up * (_jumpForce * Time.fixedDeltaTime);
		_rigidbody2D.velocity = calculatedJumpForce;
	}
	
    /// <summary>
    /// ジャンプキャンセル処理
    /// </summary>
	private void JumpCancel()
	{
		_isJumping = false;
		// 上昇中の場合、上昇速度を半減させる
		if (_rigidbody2D.velocity.y > 0)
		{
			_rigidbody2D.velocity *= Vector2.up * 0.5f;
		}
	}
	
    /// <summary>
    /// 自動ジャンプ処理
    /// </summary>
	private void AutoBlockJump()
	{
		if (_isJumping) { return; }
		if (!_canAutoJump) { return; }
		if (_rigidbody2D.velocity.y is > 0.001f or < -0.001f) { return; }
		if (_moveDirection.x == 0) { return; }
		if (!IsGround()) { return; }
		
		var x = _moveDirection.x >= 0 ? _boxCollider2D.bounds.max.x + 0.25f : _boxCollider2D.bounds.min.x - 0.25f;
		for (var y = 1; y <= _autoJumpHeight; y++)
		{
			var position = new Vector2(x, _boxCollider2D.bounds.min.y + y - 1);
			var tilemap = ChunkInformation.GetChunkTilemap(position);
			if (tilemap == null) { continue; }
			
			var cellPosition = ChunkInformation.WorldToChunk(position);
			if (!tilemap.HasTile(cellPosition) || tilemap.HasTile(cellPosition + Vector3Int.up)) { continue; }

			if (IsWall(y) || IsHeavenly(y))
			{
				if (y == _autoJumpHeight) { return; }

				continue;
			}

			transform.position += new Vector3(0.1f, y + 0.1f, 0);
			return;
		}
	}
	
	/// <summary>
	/// 地面に接地しているかどうか
	/// </summary>
	public bool IsGround()
	{
		Bounds bounds = _boxCollider2D.bounds;
		float x = bounds.center.x;
		float y = bounds.min.y + _height;
		var position = new Vector2(x, y);
		RaycastHit2D hit = _isColliderRadius ? Physics2D.CircleCast(position, _boxCollider2D.size.x / 2 - 0.1f, Vector2.down, 0.1f, _groundLayerMask) : Physics2D.CircleCast(position, _radius, Vector2.down, 0.1f, _groundLayerMask);
		return hit.collider != null;
	}
	
	/// <summary>
	/// 壁に接しているかどうか
	/// </summary>
	/// <param name="minY">足元から除く高さ</param>
	private bool IsWall(float minY)
	{
		if (_rigidbody2D.velocity.y is > 0.01f or < -0.01f) { return false; }
		if (_moveDirection.x == 0) { return false; }
		
		float x = _moveDirection.x >= 0 ? _boxCollider2D.bounds.max.x + 0.25f : _boxCollider2D.bounds.min.x - 0.25f;
		float maxY = _boxCollider2D.bounds.size.y - minY;
		for (float y = minY + 1; y <= maxY; y++)
		{
			var position = new Vector2(x, _boxCollider2D.bounds.min.y + y);
			var tilemap = ChunkInformation.GetChunkTilemap(position);
			if (tilemap == null) { return true; }
			
			var cellPosition = ChunkInformation.WorldToChunk(position);
			if (!tilemap.HasTile(cellPosition)) { continue; }
			
			return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// 天井に接しているかどうか
	/// </summary>
	/// <param name="height">高さ</param>
	private bool IsHeavenly(float height)
	{
		var minX = _boxCollider2D.bounds.min.x - 0.25f;
		var maxX = _boxCollider2D.bounds.max.x + 1.25f;
		for (var y = 1; y <= height; y++)
		{
			for (var x = minX; x <= maxX; x++)
			{
				var position = new Vector2(x, _boxCollider2D.bounds.max.y + y);
				var tilemap = ChunkInformation.GetChunkTilemap(position);
				if (tilemap == null) { continue; }
				
				var cellPosition = ChunkInformation.WorldToChunk(position);
				if (!tilemap.HasTile(cellPosition)) { continue; }

				return true;
			}
		}

		return false;
	}
	
	/// <summary>
	/// ノックバックの力を加える
	/// </summary>
	/// <param name="direction">飛ばす方向</param>
	public void KnockBackAddForce(Vector2 direction)
	{
		_rigidbody2D.velocity = direction;
		_canMove = false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		var boxCollider2D = GetComponent<BoxCollider2D>();
		var x = boxCollider2D.bounds.center.x;
		var y = boxCollider2D.bounds.min.y + _height;
		var position = new Vector2(x, y);
		if (_isColliderRadius)
		{
			Gizmos.DrawWireSphere(position, boxCollider2D.size.x / 2 - 0.01f);
		}
		else
		{
			Gizmos.DrawWireSphere(position, _radius);
		}
	}
	
	private void OnEnable()
	{
		_playerActions.Enable();
	}
	
	private void OnDisable()
	{
		_playerActions.Disable();
	}

	public void OnGenerated(IChunkInformation worldMapManager)
	{
		ChunkInformation = worldMapManager;
	}
}
