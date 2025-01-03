﻿using UnityEngine;

public class DecisionAttackTarget : FsmDecision
{
	[Header("Decision Attack Target")]
	[SerializeField] private bool checkWall;
	[SerializeField] private bool canKnockback;
	[SerializeField] private LayerMask targetLayerMask;
	
	private BoxCollider2D _boxCollider2D;
	private Rigidbody2D _rigidbody2D;
	private EnemyBrain _enemyBrain;
	private IChunkInformation _chunkInformation;
	
	public override bool Decide()
	{
		return IsTargetAttack();
	}
	
	private bool IsTargetAttack()
	{
		if (checkWall && IsWall(2))
		{
			_rigidbody2D.velocity = new Vector2(0f, _rigidbody2D.velocity.y);
			return true;
		}
		
		var point = (Vector2)transform.position + _boxCollider2D.offset;
		var hits = Physics2D.OverlapBoxAll(point, _boxCollider2D.size, 0, targetLayerMask);
		if (hits.Length == 0) { return false; }

		_rigidbody2D.velocity = new Vector2(0f, _rigidbody2D.velocity.y);
		foreach (var hit in hits)
		{
			var playerHealth = hit.GetComponent<IDamageable>();
			playerHealth?.TakeDamage(_enemyBrain.Status.attack);

			if (canKnockback)
			{
				var targetMovement = hit.GetComponent<PlayerMovement>();
				if (targetMovement != null)
				{
					var direction = hit.transform.position - transform.position;
					targetMovement.Knockback(direction.normalized * (_enemyBrain.Status.attackSpeed / 2));
				}
			}
		}
		
		return true;
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
			if (tilemap == null) { return true; }

			var localPosition = _chunkInformation.WorldToChunk(position);
			if (!tilemap.HasTile(localPosition)) { continue; }
			
			return true;
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

