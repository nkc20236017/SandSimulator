﻿using System;
using UnityEngine;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class ActionWanderTurtle : FsmAction
{
	[Header("Datas Config")]
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
	
	[Header("Generate Ore Config")]
	[SerializeField] private Turtle turtle;
	[SerializeField] private Transform[] oreParents;
	[SerializeField] private OreObject orePrefab;

	private Vector3 _moveDirection;
	private BoxCollider2D _boxCollider2D;
	private Rigidbody2D _rigidbody2D;
	private EnemyBrain _enemyBrain;
	private IChunkInformation _chunkInformation;

	private void Start()
	{
		var random = Random.Range(0, 2);
		_moveDirection = random == 0 ? Vector3.left : Vector3.right;
		transform.localScale = new Vector3(_moveDirection.x, 1, 1);

		GenerateOres();
	}

	private void GenerateOres()
	{
		var random = Random.Range(1, oreParents.Length);
		for (var i = 0; i < random; i++)
		{
			var ore = Instantiate(orePrefab, oreParents[i]);
			ore.gameObject.SetActive(true);
			ore.CanFall = false;
			var randomSize = Random.Range(1, 4);
			ore.SetOre(turtle.DropOre(), randomSize, 0);
			// oreParents[i].SetParent(ore.transform);
		}
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
		var canFlip = false;
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
				if (y == autoJumpHeight)
				{
					Flip();
					return;
				}

				canFlip = true;
				continue;
			}

			transform.position += new Vector3(0.1f, y + 0.1f, 0);
			return;
		}

		if (IsWall(0) || canFlip)
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
			var tilemap = _chunkInformation.GetChunkTilemap(position);
			if (tilemap == null) { return true; }
			
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
		var tilemap = _chunkInformation.GetChunkTilemap(position);
		if (tilemap == null) { return false; }
		
		var cellPosition = tilemap.WorldToCell(position);
		var size = Vector3Int.CeilToInt(_boxCollider2D.size);
		var bounds = new BoundsInt(cellPosition.x, cellPosition.y, 1, size.x, size.y, 1);
		foreach (var pos in bounds.allPositionsWithin)
		{
			tilemap = _chunkInformation.GetChunkTilemap(new Vector2(pos.x, pos.y));
			if (tilemap == null) { continue; }
			
			var localPosition = _chunkInformation.WorldToChunk(new Vector2(pos.x, pos.y));
			if (!tilemap.HasTile(localPosition)) { continue; }

			return false;
		}
		
		return true;
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = new Vector2(_moveDirection.x * _enemyBrain.Status.speed, _rigidbody2D.velocity.y);
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
