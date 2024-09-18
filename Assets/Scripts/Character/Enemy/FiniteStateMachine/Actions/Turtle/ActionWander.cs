using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class ActionWander : FsmAction
{
	[Header("Datas Config")]
	[SerializeField] private Tilemap tilemap;
	[SerializeField] private LayerMask groundLayerMask;
	
	[Header("Ground Config")]
	[SerializeField] private bool matchTheColliderDirectionGround;
	[HideIf(nameof(matchTheColliderDirectionGround))]
	[SerializeField] private float radius;
	
	[Header("Auto Jump Config")]
	[SerializeField] private bool autoJump;
	[ShowIf(nameof(autoJump))]
	[SerializeField] private int autoJumpHeight;

	[Header("Wall Config")]
	[SerializeField] private int wallHeight;
	
	[Header("Fall Config")]
	[SerializeField] private bool matchTheColliderDirectionFall;
	[HideIf(nameof(matchTheColliderDirectionFall))]
	[SerializeField] private Vector2Int fallDirection;
	
	[Header("InvincibleTime Config")]
	[SerializeField] private float invincibleTime;
	
	private Vector3 _moveDirection;
	private BoxCollider2D _boxCollider2D;
	private Rigidbody2D _rigidbody2D;
	private EnemyBrain _enemyBrain;

	private void Start()
	{
		_boxCollider2D = GetComponent<BoxCollider2D>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_enemyBrain = GetComponent<EnemyBrain>();
		
		var random = Random.Range(0, 2);
		_moveDirection = random == 0 ? Vector3.left : Vector3.right;
		transform.localScale = new Vector3(_moveDirection.x, 1, 1);
	}

	public override void Action()
	{
		Wander();
	}
	
	private void Wander()
	{
		OneBlockUp();
		Flip();
		Movement();
	}
	
	private void OneBlockUp()
	{
		if (_rigidbody2D.velocity.y is > 0.01f or < -0.01f) { return; }
		if (_moveDirection.x == 0) { return; }
		if (!IsGround()) { return; }

		var x = _moveDirection.x >= 0 ? _boxCollider2D.bounds.max.x + 0.25f : _boxCollider2D.bounds.min.x - 0.25f;
		var position = new Vector2(x, _boxCollider2D.bounds.min.y + autoJumpHeight - 1);
		var cellPosition = tilemap.WorldToCell(position);
		if (tilemap.HasTile(cellPosition) && !tilemap.HasTile(cellPosition + Vector3Int.up))
		{
			transform.position += new Vector3(0.25f, autoJumpHeight + 0.25f, 0);
		}
	}
	
	private bool IsGround()
	{
		var x = _boxCollider2D.bounds.center.x;
		var y = _boxCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		var hit = matchTheColliderDirectionGround ? Physics2D.CircleCast(position, _boxCollider2D.size.x / 2 - 0.1f, Vector2.down, 0.1f, groundLayerMask) : Physics2D.CircleCast(position, radius, Vector2.down, 0.1f, groundLayerMask);
		return hit.collider != null;
	}
	
	private void Flip()
	{
		if (!IsHole() && !IsWall()) { return; }

		_moveDirection = _moveDirection == Vector3.left ? Vector3.right : Vector3.left;
		transform.localScale = new Vector3(_moveDirection.x, 1, 1);
	}

	private bool IsHole()
	{
		if (_rigidbody2D.velocity.y is > 0.01f or < -0.01f) { return false; }
		if (_moveDirection.x == 0) { return false; }
		
		var x = _moveDirection.x >= 0 ? _boxCollider2D.bounds.max.x + _boxCollider2D.bounds.size.x / 2 + 0.25f : _boxCollider2D.bounds.min.x - _boxCollider2D.bounds.size.x / 2 - 0.25f;
		var position = new Vector2(x, _boxCollider2D.bounds.min.y - _boxCollider2D.bounds.size.y / 2 - 0.1f);
		var cellPosition = tilemap.WorldToCell(position);
		var bounds = new BoundsInt(cellPosition, Vector3Int.RoundToInt(_boxCollider2D.size));
		return tilemap.GetTilesBlock(bounds) == null;
	}

	private bool IsWall()
	{
		if (_rigidbody2D.velocity.y is > 0.01f or < -0.01f) { return false; }
		if (_moveDirection.x == 0) { return false; }
		
		var x = _moveDirection.x >= 0 ? _boxCollider2D.bounds.max.x + 0.25f : _boxCollider2D.bounds.min.x - 0.25f;
		var position = new Vector2(x, _boxCollider2D.bounds.min.y + wallHeight - 1);
		var cellPosition = tilemap.WorldToCell(position);
		return tilemap.HasTile(cellPosition);
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = new Vector2(_moveDirection.x * _enemyBrain.Enemy.status[0].speed, _rigidbody2D.velocity.y);
	}

	private void OnDrawGizmos()
	{
		var boxCollider2D = GetComponent<BoxCollider2D>();
		if (boxCollider2D == null) { return; }
		
		var x = boxCollider2D.bounds.center.x;
		var y = boxCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		Gizmos.DrawWireSphere(position, boxCollider2D.size.x / 2 - 0.1f);
		// if (_moveDirection.x == 0) { return; }
		
		x = _moveDirection.x >= 0 ? boxCollider2D.bounds.max.x + 0.25f : boxCollider2D.bounds.min.x - 0.25f;
		
		// ジャンプのギズモ表示
		position = new Vector2(x, boxCollider2D.bounds.min.y + autoJumpHeight - 1);
		var cellPosition = tilemap.WorldToCell(position);
		Gizmos.color = tilemap.HasTile(cellPosition) && !tilemap.HasTile(cellPosition + Vector3Int.up) ? Color.red : Color.green;
		Gizmos.DrawWireCube(tilemap.GetCellCenterWorld(cellPosition), Vector3.one);
		
		// 穴のギズモ表示
		position = new Vector2(x, boxCollider2D.bounds.min.y - boxCollider2D.size.y);
		cellPosition = tilemap.WorldToCell(position);
		var size = Vector3Int.CeilToInt(boxCollider2D.size);
		var bounds = new BoundsInt(cellPosition.x, cellPosition.y, 1, size.x, size.y, 1);
		var isHole = tilemap.GetTilesBlock(bounds) == null;
		Gizmos.color = isHole ? Color.red : Color.green;
		Gizmos.DrawWireCube(bounds.center, bounds.size);
		
		// x = _moveDirection.x >= 0 ? boxCollider2D.bounds.max.x + 1 : boxCollider2D.bounds.min.x - 1;
		
		// 壁のギズモ表示
		position = new Vector2(x, boxCollider2D.bounds.min.y + wallHeight - 1);
		cellPosition = tilemap.WorldToCell(position);
		Gizmos.color = tilemap.HasTile(cellPosition) ? Color.red : Color.green;
		Gizmos.DrawWireCube(tilemap.GetCellCenterWorld(cellPosition), Vector3.one);
	}
}
