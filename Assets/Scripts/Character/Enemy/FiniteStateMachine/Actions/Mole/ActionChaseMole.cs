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
		Movement();
		Rotate();
	}

	private void Movement()
	{
		// プレイヤーの位置に向かって移動する
		var moveDirection = (_enemyBrain.TargetPosition - transform.position).normalized;
		var movement = moveDirection * (_enemyBrain.Status.attackSpeed * Time.deltaTime);
		movement.z = 0f;
		if (Vector3.Distance(transform.position, _enemyBrain.TargetPosition) >= 0.5f)
		{
			_rigidbody2D.MovePosition(transform.position + movement);
		}
	}
	
	private void Rotate()
	{
		// ターゲットの方向に向かって回転させる
		var direction = _enemyBrain.TargetPosition - transform.position;
		var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		var rotation = Mathf.Lerp(transform.rotation.z, angle, 2.5f * Time.deltaTime);
		transform.rotation = Quaternion.Euler(0f, 0f, rotation);
	}

	private void OnEnable()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_enemyBrain = GetComponent<EnemyBrain>();
	}
}

