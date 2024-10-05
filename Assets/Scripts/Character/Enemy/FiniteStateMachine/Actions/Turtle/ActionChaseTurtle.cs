using UnityEngine;

public class ActionChaseTurtle : FsmAction
{
	private Rigidbody2D _rigidbody2D;
	private EnemyBrain _enemyBrain;

	private void OnEnable()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_enemyBrain = GetComponent<EnemyBrain>();
	}

	public override void Action()
	{
		_rigidbody2D.velocity = new Vector2(0f, _rigidbody2D.velocity.y);
		Flip();
	}
	
	private void Flip()
	{
		if (_enemyBrain.Target == null) { return; }
		
		var direction = _enemyBrain.Target.position - transform.position;
		var dir = direction.x >= 0 ? Vector3.right : Vector3.left;
		transform.localScale = new Vector3(dir.x, 1, 1);
		_enemyBrain.Direction = dir;
	}
}
