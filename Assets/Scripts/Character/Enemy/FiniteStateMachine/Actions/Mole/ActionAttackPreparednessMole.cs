using UnityEngine;

public class ActionAttackPreparednessMole : FsmAction
{
	[SerializeField] private float _rotateSpeed = 5f;
	private EnemyBrain _enemyBrain;
	
	public override void Action()
	{
		Rotate();
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
		_enemyBrain = GetComponent<EnemyBrain>();
	}
}

