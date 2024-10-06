using UnityEngine;

public class DecisionFindTarget : FsmDecision
{
	[Header("Find Target Config")]
	[SerializeField] private float _radius;
	[SerializeField] private LayerMask _targetLayerMask;
	
	private EnemyBrain _enemyBrain;
	
	public override bool Decide()
	{
		return FindTarget();
	}
	
	private bool FindTarget()
	{
		var colliders = Physics2D.OverlapCircle(transform.position, _radius, _targetLayerMask);
		return colliders != null;
	}
	
	private void OnEnable()
	{
		_enemyBrain = GetComponent<EnemyBrain>();
	}
	
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _radius);
	}
}

