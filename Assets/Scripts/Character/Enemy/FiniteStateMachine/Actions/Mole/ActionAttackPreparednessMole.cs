using UnityEngine;

public class ActionAttackPreparednessMole : FsmAction
{
	private EnemyBrain _enemyBrain;
	
	public override void Action()
	{
		if (_enemyBrain.Target == null) { return; }
		
		// ターゲットの方向に向かって回転させる
		var direction = _enemyBrain.Target.position - transform.position;
		var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0f, 0f, angle);
	}
	
	private void OnEnable()
	{
		_enemyBrain = GetComponent<EnemyBrain>();
	}
}

