using UnityEngine;

public class ActionChaseMole : FsmAction
{
	private EnemyBrain _enemyBrain;
	
	public override void Action()
	{
		Chase();
	}

	private void Chase()
	{
		if (_enemyBrain.Target == null) { return; }

		Movement();
	}

	private void Movement()
	{
		// プレイヤーの位置に向かって移動する
		var moveDirection = (_enemyBrain.Target.position - transform.position).normalized;
		var movement = moveDirection * (_enemyBrain.Status.attackSpeed * Time.deltaTime);
		if (Vector3.Distance(transform.position, _enemyBrain.Target.position) >= 0.5f)
		{
			transform.Translate(movement);
		}
	}

	private void OnEnable()
	{
		_enemyBrain = GetComponent<EnemyBrain>();
	}
}

