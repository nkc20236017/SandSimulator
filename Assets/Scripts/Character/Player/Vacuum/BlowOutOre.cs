using UnityEngine;

public class BlowOutOre : MonoBehaviour
{
	private int _attackPower;
	private float _speed;
	private Vector2 _direction;
	private Rigidbody2D _rigidbody2D;
	private SpriteRenderer _spriteRenderer;
	
	private void Awake()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	public void SetOre(int attackPower, float speed, Vector2 direction, Sprite sprite)
	{
		_attackPower = attackPower;
		_speed = speed;
		_direction = direction;
		_spriteRenderer.sprite = sprite;

		gameObject.AddComponent<PolygonCollider2D>();

		Movement();
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = _direction * (_speed * Time.deltaTime * 100);
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.TryGetComponent<IDamagable>(out var target)) { return; }
		
		target.TakeDamage(_attackPower);
		Destroy(gameObject);
	}
}

