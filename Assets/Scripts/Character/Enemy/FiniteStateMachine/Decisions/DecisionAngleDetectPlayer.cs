using System;
using UnityEngine;
using UnityEngine.Serialization;
using NaughtyAttributes;

public class DecisionAngleDetectPlayer : FsmDecision
{
	[Header("Detection Config")]
	[SerializeField] private Transform pivot;
	[FormerlySerializedAs("range")] [SerializeField, MinValue(0)] private float radius;
	[SerializeField, MinValue(0), MaxValue(180)] private float angle;
	[SerializeField, MinValue(0), MaxValue(360)] private float direction;
	[SerializeField] private LayerMask playerLayerMask;
	[SerializeField] private LayerMask obstacleLayerMask;
	
	[Header("Debug Config")]
	[SerializeField] private bool debugMode;
	
	private bool _isPlayerDetected;
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
		
		var playerCollider = Physics2D.OverlapCircle(transform.position, radius, playerLayerMask);
		if (playerCollider == null) { return false; }
		
		var player = playerCollider.transform;
		var circumference = GetNewCell(-direction * Mathf.Deg2Rad, radius);
		
		var direction1 = circumference - pivot.position;
		var direction2 = player.position - pivot.position;
		
		var circumferenceToPlayerAngle = Vector2.Angle(direction1, direction2);
		if (circumferenceToPlayerAngle > angle) { return false; }

		// TODO: プレイヤーが死んでいる場合はプレイヤーを検知しない（無視する）
		if (IsObstaclePivot(player))
		{
			if (!_isPlayerDetected)
			{
				_isPlayerDetected = true;
				// TODO: プレイヤーを見失ったアクションを付ける
				// TODO: プレイヤーを見失った場合、見失った場所まで移動する
			}

			return false;
		}
			
		_enemyBrain.Player = player;
		if (_isPlayerDetected)
		{
			_isPlayerDetected = false;
			// TODO: プレイヤーを発見したアクションを付ける
		}
		return true;
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

	private void OnDrawGizmosSelected()
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
