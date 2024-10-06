using UnityEngine;

public class ActionChaseMole : FsmAction
{
	[SerializeField] private float _rotateSpeed = 5f;
	
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
		// 移動している方向に滑らかに回転する
		var moveDirection = (_enemyBrain.TargetPosition - transform.position).normalized;
		var angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
		var rotation = Mathf.LerpAngle(transform.eulerAngles.z, angle, Time.deltaTime * _rotateSpeed);
		transform.rotation = Quaternion.Euler(0f, 0f, rotation);
	}

	private void OnEnable()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_enemyBrain = GetComponent<EnemyBrain>();
	}
}

