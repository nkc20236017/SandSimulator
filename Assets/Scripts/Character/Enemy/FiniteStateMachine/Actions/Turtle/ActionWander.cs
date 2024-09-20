using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class ActionWander : FsmAction
{
	[Header("Datas Config")]
	[SerializeField] private Tilemap _tilemap;
	[SerializeField] private LayerMask groundLayerMask;
	
	[Header("Ground Config")]
	[SerializeField] private bool matchTheColliderDirectionGround;
	[HideIf(nameof(matchTheColliderDirectionGround))]
	[SerializeField] private float radius;
	
	[Header("Auto Jump Config")]
	[SerializeField] private bool autoJump;
	[ShowIf(nameof(autoJump))]
	[SerializeField] private int autoJumpHeight;
	
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
		AutoBlockJump();
		if (IsHole() && IsGround())
		{
			Flip();
		}
		Movement();
	}
	
	private void AutoBlockJump()
	{
		if (_rigidbody2D.velocity.y is > 0.001f or < -0.001f) { return; }
		if (_moveDirection.x == 0) { return; }
		if (!IsGround()) { return; }

		var x = _moveDirection.x >= 0 ? _boxCollider2D.bounds.max.x + 0.25f : _boxCollider2D.bounds.min.x - 0.25f;
		var canflip = false;
		for (var y = 1; y <= autoJumpHeight; y++)
		{
			var position = new Vector2(x, _boxCollider2D.bounds.min.y + y - 1);
			var cellPosition = _tilemap.WorldToCell(position);
			if (!_tilemap.HasTile(cellPosition) || _tilemap.HasTile(cellPosition + Vector3Int.up)) { continue; }

			if (IsWall(y) || IsHeavenly(y))
			{
				if (y == autoJumpHeight)
				{
					Flip();
					return;
				}

				canflip = true;
				continue;
			}

			transform.position += new Vector3(0.1f, y + 0.1f, 0);
			return;
		}

		if (IsWall(0) || canflip)
		{
			Flip();
		}
	}
	
	private bool IsGround()
	{
		if (_rigidbody2D.velocity.y is > 0.001f or < -0.001f) { return false; }
		
		var x = _boxCollider2D.bounds.center.x;
		var y = _boxCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		var hit = matchTheColliderDirectionGround ? Physics2D.CircleCast(position, _boxCollider2D.size.x / 2 - 0.1f, Vector2.down, 0.1f, groundLayerMask) : Physics2D.CircleCast(position, radius, Vector2.down, 0.1f, groundLayerMask);
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
			var cellPosition = _tilemap.WorldToCell(position);
			if (!_tilemap.HasTile(cellPosition)) { continue; }
			
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
				var cellPosition = _tilemap.WorldToCell(position);
				if (!_tilemap.HasTile(cellPosition)) { continue; }

				return true;
			}
		}

		return false;
	}
	
	private void Flip()
	{
		_moveDirection = _moveDirection == Vector3.left ? Vector3.right : Vector3.left;
		transform.localScale = new Vector3(_moveDirection.x, 1, 1);
	}

	private bool IsHole()
	{
		if (_rigidbody2D.velocity.y is > 0.01f or < -0.01f) { return false; }
		if (_moveDirection.x == 0) { return false; }
		
		var x = _moveDirection.x >= 0 ? _boxCollider2D.bounds.max.x : _boxCollider2D.bounds.min.x - _boxCollider2D.size.x;
		var position = new Vector2(x, _boxCollider2D.bounds.min.y - _boxCollider2D.size.y);
		var cellPosition = _tilemap.WorldToCell(position);
		var size = Vector3Int.CeilToInt(_boxCollider2D.size);
		var bounds = new BoundsInt(cellPosition.x, cellPosition.y, 1, size.x, size.y, 1);
		var isHole = false;
		foreach (var pos in bounds.allPositionsWithin)
		{
			if (!_tilemap.HasTile(_tilemap.WorldToCell(pos))) { continue; }

			return false;
		}
		
		return true;
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = new Vector2(_moveDirection.x * _enemyBrain.Status.speed, _rigidbody2D.velocity.y);
	}

	private void OnDrawGizmos()
	{
		if (_tilemap == null) { return; }
		
		var boxCollider2D = GetComponent<BoxCollider2D>();
		if (boxCollider2D == null) { return; }
		
		var x = boxCollider2D.bounds.center.x;
		var y = boxCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		Gizmos.DrawWireSphere(position, boxCollider2D.size.x / 2 - 0.1f);
		
		// 穴のギズモ表示
		x = _moveDirection.x >= 0 ? boxCollider2D.bounds.max.x : boxCollider2D.bounds.min.x - boxCollider2D.size.x;
		position = new Vector2(x, boxCollider2D.bounds.min.y - boxCollider2D.size.y);
		var cellPosition = _tilemap.WorldToCell(position);
		var size = Vector3Int.CeilToInt(boxCollider2D.size);
		var bounds = new BoundsInt(cellPosition.x, cellPosition.y, 1, size.x, size.y, 1);
		var isHole = false;
		foreach (var pos in bounds.allPositionsWithin)
		{
			if (!_tilemap.HasTile(_tilemap.WorldToCell(pos))) { continue; }
			
			isHole = true;
			break;
		}
		Gizmos.color = isHole ? Color.red : Color.green;
		Gizmos.DrawWireCube(bounds.center, bounds.size);
		
		x = _moveDirection.x >= 0 ? boxCollider2D.bounds.max.x + 0.25f : boxCollider2D.bounds.min.x - 0.25f;
		
		// ジャンプのギズモ表示
		for (var minY = 0; minY < autoJumpHeight; minY++)
		{
			for (y = minY + 1; y <= boxCollider2D.bounds.size.y - minY; y++)
			{
				position = new Vector2(x, boxCollider2D.bounds.min.y + y);
				cellPosition = _tilemap.WorldToCell(position);
				Gizmos.color = _tilemap.HasTile(cellPosition) ? Color.red : Color.blue;
				Gizmos.DrawWireCube(_tilemap.GetCellCenterWorld(cellPosition), Vector3.one);
			}
			
			position = new Vector2(x, boxCollider2D.bounds.min.y + minY);
			var cellPosition0 = _tilemap.WorldToCell(position);
			Gizmos.color = _tilemap.HasTile(cellPosition0) ? Color.red : Color.green;
			Gizmos.DrawWireCube(_tilemap.GetCellCenterWorld(cellPosition0), Vector3.one);
		}
		
		// 天井のギズモ表示
		var minX = boxCollider2D.bounds.min.x - 0.25f;
		var maxX = boxCollider2D.bounds.max.x + 1.25f;
		for (y = 1; y <= autoJumpHeight; y++)
		{
			for (x = minX; x <= maxX; x++)
			{
				position = new Vector2(x, boxCollider2D.bounds.max.y + y);
				cellPosition = _tilemap.WorldToCell(position);
				Gizmos.color = _tilemap.HasTile(cellPosition) ? Color.yellow : Color.green;
				Gizmos.DrawWireCube(_tilemap.GetCellCenterWorld(cellPosition), Vector3.one);
			}
		}
	}
}
