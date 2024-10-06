using UnityEngine;

public class ActionAttackPreparednessMole : FsmAction
{
	private EnemyBrain _enemyBrain;
	
	public override void Action()
	{
		// ターゲットの方向に向かって回転させる
		var direction = _enemyBrain.TargetPosition - transform.position;
		var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		var rotation = Mathf.Lerp(transform.rotation.z, angle, 2.5f * Time.deltaTime);
		transform.rotation = Quaternion.Euler(0f, 0f, rotation);
	}
	
	private void OnEnable()
	{
		_enemyBrain = GetComponent<EnemyBrain>();
	}
}

