using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class ActionWander : FsmAction
{
	[SerializeField] private Tilemap tilemap;
	[SerializeField] private LayerMask groundLayerMask;
	[SerializeField] private bool isColliderRadius;
	[SerializeField] private float radius;
	
	private Vector3 _moveDirection;
	private BoxCollider2D _boxCollider2D;
	private Rigidbody2D _rigidbody2D;
	private EnemyBrain _enemyBrain;

	private void Start()
	{
		_boxCollider2D = GetComponent<BoxCollider2D>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
		
		var random = Random.Range(0, 2);
		_moveDirection = random == 0 ? Vector3.left : Vector3.right;
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
		if (IsHole())
		{
			_moveDirection = _moveDirection == Vector3.left ? Vector3.right : Vector3.left;
		}
	}

	private bool IsHole()
	{
		if (_rigidbody2D.velocity.y is > 0.01f or < -0.01f) { return false; }
		if (_moveDirection.x == 0) { return false; }
		
		var x = _moveDirection.x > 0 ? _boxCollider2D.bounds.max.x + _boxCollider2D.bounds.size.x / 2 + 0.25f : _boxCollider2D.bounds.min.x - _boxCollider2D.bounds.size.x / 2 - 0.25f;
		var position = new Vector2(x, _boxCollider2D.bounds.min.y - _boxCollider2D.bounds.size.y / 2 - 0.1f);
		var cellPosition = tilemap.WorldToCell(position);
		var bounds = new BoundsInt(cellPosition, Vector3Int.RoundToInt(_boxCollider2D.size));
		return tilemap.GetTilesBlock(bounds) == null;
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = new Vector2(_moveDirection.x * _enemyBrain.Enemy.status[0].speed, _rigidbody2D.velocity.y);
	}

	private void OnDrawGizmos()
	{
		if (!isColliderRadius) { return; }
		
		var boxCollider2D = GetComponent<BoxCollider2D>();
		if (boxCollider2D == null) { return; }
		
		var x = boxCollider2D.bounds.center.x;
		var y = boxCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		Gizmos.DrawWireSphere(position, boxCollider2D.size.x / 2 - 0.1f);
		// if (_moveDirection.x == 0) { return; }
		
		x = _moveDirection.x >= 0 ? boxCollider2D.bounds.max.x + 0.25f : boxCollider2D.bounds.min.x - 0.25f;
		position = new Vector2(x, boxCollider2D.bounds.min.y - boxCollider2D.bounds.size.y);
		var cellPosition = Vector3Int.RoundToInt(tilemap.WorldToCell(position));
		var bounds = new BoundsInt(cellPosition, Vector3Int.RoundToInt(boxCollider2D.size));
		Gizmos.color = tilemap.GetTilesBlock(bounds) == null ? Color.red : Color.green;
		Gizmos.DrawWireCube(bounds.center, bounds.size);
	}
}
