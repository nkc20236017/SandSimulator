﻿using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class DecisionAngleDetectPlayer : FsmDecision
{
	[Header("Detection Config")]
	[SerializeField] private Transform pivot;
	[SerializeField, MinValue(0)] private float radius;
	[SerializeField, MinValue(0), MaxValue(180)] private float angle;
	[SerializeField, MinValue(0), MaxValue(360)] private float direction;
	[SerializeField] private LayerMask playerLayerMask;
	[SerializeField] private LayerMask obstacleLayerMask;
	
	[Header("Mark Config")]
	[SerializeField] private SpriteRenderer markSpriteRenderer;
	[SerializeField] private Sprite findMarkSprite;
	[SerializeField] private Sprite lostMarkSprite;
	[SerializeField] private float showMarkTime;
	
	[Header("Debug Config")]
	[SerializeField] private bool debugMode;
	
	private bool _isPlayerDetected;
	private Coroutine _markCoroutine;
	private EnemyBrain _enemyBrain;

	private void Awake()
	{
		_enemyBrain = GetComponent<EnemyBrain>();
	}

	public override bool Decide()
	{
		return DetectPlayer();
	}
	
	private bool DetectPlayer()
	{
		// TODO: デバッグモードの時はプレイヤーを検知しないようにする
		
		var playerCollider = Physics2D.OverlapCircle(pivot.position, radius, playerLayerMask);
		if (playerCollider != null)
		{
			var player = playerCollider.transform;
			var distance = (pivot.position - player.position).sqrMagnitude;
			if (distance > radius * radius)
			{
				if (_isPlayerDetected)
				{
					_isPlayerDetected = false;
					_enemyBrain.Player = null;
					ShowMark(lostMarkSprite);
				}
				
				return false;
			}
			
			var circumference = GetNewCell(-direction * Mathf.Deg2Rad, radius);

			var direction1 = circumference - pivot.position;
			var direction2 = player.position - pivot.position;

			var circumferenceToPlayerAngle = Vector2.Angle(direction1, direction2);
			if (circumferenceToPlayerAngle <= angle)
			{
				if (IsObstaclePivot(player))
				{
					if (_isPlayerDetected)
					{
						_isPlayerDetected = false;
						_enemyBrain.Player = null;
						ShowMark(lostMarkSprite);
						// TODO: プレイヤーを見失った場合、見失った場所まで移動する
					}

					return false;
				}

				if (!_isPlayerDetected)
				{
					_isPlayerDetected = true;
					_enemyBrain.Player = player;
					ShowMark(findMarkSprite);
				}

				return true;
			}

			// TODO: プレイヤーが死んでいる場合はプレイヤーを検知しない（無視する）
			if (_isPlayerDetected && IsObstaclePivot(_enemyBrain.Player))
			{
				_enemyBrain.Player = null;
				_isPlayerDetected = false;
				ShowMark(lostMarkSprite);
			}

			return false;
		}

		if (_enemyBrain.Player != null && _isPlayerDetected)
		{
			_enemyBrain.Player = null;
			_isPlayerDetected = false;
			ShowMark(lostMarkSprite);
		}

		return false;
	}

	private bool IsObstaclePivot(Transform target)
	{
		var hit = Physics2D.Linecast(pivot.position, target.position, obstacleLayerMask);
		return hit.collider != null;
	}
	
	// private bool IsObstacleCollider(Transform target)
	// {
	// 	
	// }
	
	private void ShowMark(Sprite mark)
	{
		markSpriteRenderer.gameObject.SetActive(true);
		markSpriteRenderer.sprite = mark;
		if (_markCoroutine != null)
		{
			StopCoroutine(_markCoroutine);
		}
		
		_markCoroutine = StartCoroutine(HideMark());
	}
        
	private IEnumerator HideMark()
	{
		yield return new WaitForSeconds(showMarkTime);
        
		markSpriteRenderer.sprite = null;
		markSpriteRenderer.gameObject.SetActive(false);
	}

	private void OnDrawGizmos()
	{
		if (!debugMode) { return; }
		if (pivot == null) { return; }
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(pivot.position, radius);
		
		Gizmos.color = Color.green;
		var circumference = GetNewCell(-direction * Mathf.Deg2Rad, radius);

		var angleInRadians = angle * Mathf.Deg2Rad;
		var direction2 = circumference - pivot.position;
		var directionAngle = Mathf.Atan2(direction2.y, direction2.x);

		var newCell1 = GetNewCell(directionAngle - angleInRadians, radius);
		Gizmos.DrawLine(pivot.position, newCell1);

		var newCell2 = GetNewCell(directionAngle + angleInRadians, radius);
		Gizmos.DrawLine(pivot.position, newCell2);
		
		// TODO: プレイヤーを検知した時のギズモ表示する
		if (_enemyBrain == null) { return; }
		if (_enemyBrain.Player == null) { return; }
		
		Gizmos.color = Color.red;
		Gizmos.DrawLine(pivot.position, _enemyBrain.Player.position);
	}
	
	private Vector3 GetNewCell(float f, float chordLength)
	{
		var newX = chordLength * Mathf.Cos(f);
		var newY = chordLength * Mathf.Sin(f);
		var newDirection = new Vector3(newX, newY, 0);
		var newCell = pivot.position + newDirection;
		return newCell;
	}
}