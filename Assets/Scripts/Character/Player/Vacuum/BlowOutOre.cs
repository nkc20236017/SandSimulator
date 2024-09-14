using System;
using UnityEngine;

public class BlowOutOre : MonoBehaviour
{
	[SerializeField] private float _speed;
	[SerializeField] private float despawnTime = 300;
	[SerializeField] private float invincibleTime;
	
	private int _attackPower;
	private float _invincibleTimer;
	private bool _isInvincible = true;
	private Vector2 _direction;
	private BoxCollider2D _boxCollider2D;
	private Rigidbody2D _rigidbody2D;
	private SpriteRenderer _spriteRenderer;
	
	private void Awake()
	{
		_boxCollider2D = GetComponent<BoxCollider2D>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		Destroy(gameObject, despawnTime);
	}

	private void Update()
	{
		if (!_isInvincible) { return; }

		_invincibleTimer += Time.deltaTime;
		if (_invincibleTimer < invincibleTime) { return; }

		_isInvincible = false;
	}
	
	public void SetOre(int attackPower, int gravity, Vector2 direction, Sprite sprite)
	{
		_attackPower = attackPower;
		_speed /= gravity;
		_direction = direction;
		_spriteRenderer.sprite = sprite;
		
		_boxCollider2D.size = sprite.bounds.size;

		Movement();
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = _direction * _speed;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.CompareTag("Player")) { return; }
		
		if (other.collider.TryGetComponent<IDamagable>(out var target))
		{
			target.TakeDamage(_attackPower);
			Destroy(gameObject);
			return;
		}
		if (_isInvincible) { return; }
		
		Destroy(gameObject);
	}
}

