using UnityEngine;
using NaughtyAttributes;

public class ActionAttack : FsmAction
{
	[Header("Auto Jump Config")]
	[SerializeField] private bool autoJump;
	[ShowIf(nameof(autoJump))]
	[SerializeField] private int autoJumpHeight;
	
	[Header("Ground Config")]
	[SerializeField] private bool matchTheColliderDirectionGround;
	[HideIf(nameof(matchTheColliderDirectionGround))]
	[SerializeField] private float radius;
	[SerializeField] private LayerMask groundLayerMask;
	
	private BoxCollider2D _boxCollider2D;
	private Rigidbody2D _rigidbody2D;
	private EnemyBrain _enemyBrain;
	private IChunkInformation _chunkInformation;

	public override void Action()
	{
		AutoBlockJump();
		Movement();
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = new Vector2(_enemyBrain.Direction.x * _enemyBrain.Status.attackSpeed, _rigidbody2D.velocity.y);
	}
	
	private void AutoBlockJump()
	{
		if (!autoJump) { return; }
		if (_rigidbody2D.velocity.y is > 0.001f or < -0.001f) { return; }
		if (_enemyBrain.Direction.x == 0) { return; }
		if (!IsGround()) { return; }

		var x = _enemyBrain.Direction.x >= 0 ? _boxCollider2D.bounds.max.x + 0.25f : _boxCollider2D.bounds.min.x - 0.25f;
		for (var y = 1; y <= autoJumpHeight; y++)
		{
			var position = new Vector2(x, _boxCollider2D.bounds.min.y + y - 1);
			var tilemap = _chunkInformation.GetChunkTilemap(position);
			var tilemap2 = _chunkInformation.GetChunkTilemap(position + Vector2.up);
			if (tilemap == null) { continue; }
			if (tilemap2 == null) { continue; }
			
			var localPosition = _chunkInformation.WorldToChunk(position);
			var localPosition2 = _chunkInformation.WorldToChunk(position + Vector2.up);
			if (!tilemap.HasTile(localPosition) || tilemap2.HasTile(localPosition2)) { continue; }

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
		var hit = matchTheColliderDirectionGround ? Physics2D.CircleCast(position, _boxCollider2D.size.x / 2 - 0.1f, Vector2.down, 0.1f, groundLayerMask) : Physics2D.CircleCast(position, radius, Vector2.down, 0.1f, groundLayerMask);
		return hit.collider != null;
	}
	
	private bool IsWall(float minY)
	{
		if (_rigidbody2D.velocity.y is > 0.01f or < -0.01f) { return false; }
		if (_enemyBrain.Direction.x == 0) { return false; }
		
		var x = _enemyBrain.Direction.x >= 0 ? _boxCollider2D.bounds.max.x + 0.25f : _boxCollider2D.bounds.min.x - 0.25f;
		var maxY = _boxCollider2D.bounds.size.y - minY;
		for (var y = minY + 1; y <= maxY; y++)
		{
			var position = new Vector2(x, _boxCollider2D.bounds.min.y + y);
			var tilemap = _chunkInformation.GetChunkTilemap(position);
			if (tilemap == null) { continue; }
			
			var localPosition = _chunkInformation.WorldToChunk(position);
			if (!tilemap.HasTile(localPosition)) { continue; }
			
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
				
				var localPosition = _chunkInformation.WorldToChunk(position);
				if (!tilemap.HasTile(localPosition)) { continue; }

				return true;
			}
		}

		return false;
	}

	private void OnEnable()
	{
		var worldMapManager = FindObjectOfType<WorldMapManager>();
		_chunkInformation = worldMapManager.GetComponent<IChunkInformation>();
		
		_boxCollider2D = GetComponent<BoxCollider2D>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_enemyBrain = GetComponent<EnemyBrain>();
	}
}
