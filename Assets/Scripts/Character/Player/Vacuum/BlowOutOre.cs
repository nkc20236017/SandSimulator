using UnityEngine;

public class BlowOutOre : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] private float speed;
	[SerializeField] private int attackPower;
	
	private Vector2 _direction;
	private Rigidbody2D _rigidbody2D;
	
	private void Awake()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		Movement();
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = _direction * speed;
	}
	
	public void SetDirection(Vector2 direction)
	{
		_direction = direction;
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.TryGetComponent<IDamagable>(out var damagable)) { return; }
		
		damagable.TakeDamage(attackPower);
		Destroy(gameObject);
	}
}

