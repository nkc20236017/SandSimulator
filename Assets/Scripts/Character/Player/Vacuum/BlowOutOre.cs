using System;
using UnityEngine;

public class BlowOutOre : MonoBehaviour
{
	[SerializeField] private float _speed;
	[SerializeField] private float invincibleTime; // TODO: 消す可能性（無敵時間）
	
	private int _attackPower;
	private float _invincibleTimer; // TODO: 消す可能性（無敵時間）
	private bool _isInvincible = true; // TODO: 消す可能性（無敵時間）
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
	
	private void Update() // TODO: 消す可能性（無敵時間）
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
		_rigidbody2D.gravityScale = gravity;
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
			Destroy(gameObject); // TODO: 消す可能性（無敵時間）
			return; // TODO: 消す可能性（無敵時間）
		}
		if (_isInvincible) { return; } // TODO: 消す可能性（無敵時間）
		
		Destroy(gameObject);
	}
}

