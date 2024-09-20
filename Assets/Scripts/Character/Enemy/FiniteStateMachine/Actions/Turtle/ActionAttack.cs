using UnityEngine;

public class ActionAttack : FsmAction
{
	[Header("Attack Config")]
	[SerializeField] private float attackInterval;
	
	private float _attackTimer;
	private Vector3 _moveDirection;
	private Rigidbody2D _rigidbody2D;
	private EnemyBrain _enemyBrain;

	private void Awake()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_enemyBrain = GetComponent<EnemyBrain>();
	}

	public override void Action()
	{
		Attack();
	}
	
	private void Attack()
	{
		Flip();
		
		_attackTimer += Time.deltaTime;
		if (_attackTimer < attackInterval) { return; }
		
		_attackTimer = 0f;
		Movement();
	}
	
	private void Flip()
	{
		var direction = _enemyBrain.Player.position - transform.position;
		_moveDirection = direction.x >= 0 ? Vector3.right : Vector3.left;
		transform.localScale = new Vector3(_moveDirection.x, 1, 1);
	}

	private void Movement()
	{
		_rigidbody2D.AddForce(_moveDirection * _enemyBrain.Status.attackSpeed, ForceMode2D.Impulse);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag("Player")) { return; }

		other.GetComponent<PlayerHealth>().TakeDamage(_enemyBrain.Status.attack);
		_rigidbody2D.velocity = Vector2.zero;
	}
}
