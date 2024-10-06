using UnityEngine;

public class ActionChaseMole : FsmAction
{
	private Rigidbody2D _rigidbody2D;
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
		movement.z = 0f;
		if (Vector3.Distance(transform.position, _enemyBrain.Target.position) >= 0.5f)
		{
			_rigidbody2D.MovePosition(transform.position + movement);
		}
	}

	private void OnEnable()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_enemyBrain = GetComponent<EnemyBrain>();
	}
}

