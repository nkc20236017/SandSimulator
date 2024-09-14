using UnityEngine;

public class DecisionAttackRange : FsmDecision
{
	[Header("Decision Config")]
	[SerializeField] private float attackRange;
	[SerializeField] private LayerMask playerLayerMask;
	
	public override bool Decide()
	{
		return PlayerInAttackRange();
	}
	
	private bool PlayerInAttackRange()
	{
		var playerCollider = Physics2D.OverlapCircle(transform.position, attackRange, playerLayerMask);
		if (playerCollider != null)
		{
			// TODO: プレイヤーが死んでいる場合はプレイヤーを検知しない（無視する）
			return true;
		}
		return false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}
}
