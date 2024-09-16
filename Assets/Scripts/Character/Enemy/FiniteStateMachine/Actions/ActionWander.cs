using UnityEngine;

public class ActionWander : FsmAction
{
	private BoxCollider2D _boxCollider2D;
	private Rigidbody2D _rigidbody2D;
	private EnemyBrain _enemyBrain;

	private void Start()
	{
		_boxCollider2D = GetComponent<BoxCollider2D>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
	}

	public override void Action()
	{
		throw new System.NotImplementedException();
	}
}
