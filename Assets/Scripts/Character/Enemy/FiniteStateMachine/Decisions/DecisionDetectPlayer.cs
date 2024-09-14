using System;
using UnityEngine;

public class DecisionDetectPlayer : FsmDecision
{
	[Header("Detection Config")]
	[SerializeField] private float detectionRange;
	[SerializeField] private LayerMask playerLayerMask;
	[SerializeField] private LayerMask obstacleLayerMask;
	
	// TODO: プレイヤーを発見・見失った時にアクションを付ける
	// TODO: プレイヤーを見失った場合、見失った場所まで移動する
	
	public override bool Decide()
	{
		return DetectPlayer();
	}
	
	private bool DetectPlayer()
	{
		// TODO: デバッグモードの時はプレイヤーを検知しないようにする
		
		var playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayerMask);
		if (playerCollider != null)
		{
			// TODO: プレイヤーが死んでいる場合はプレイヤーを検知しない（無視する）
			// 
			return true;
		}
		return false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, detectionRange);
		
		// TODO: プレイヤーを検知した時のギズモ表示する
	}
}
