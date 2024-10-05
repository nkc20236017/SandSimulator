using System;
using UnityEngine;

public class BlowOutOre : MonoBehaviour, IDetectSoundable
{
	[SerializeField] private float _speed;
	[SerializeField] private float plusRadius;
	[SerializeField] private float despawnTime = 300;
	[SerializeField] private float invincibleTime;
	
	private int _attackPower;
	private float _invincibleTimer;
	private bool _isInvincible = true;
	private Vector2 _direction;
	private CircleCollider2D _circleCollider2D;
	private Rigidbody2D _rigidbody2D;
	private SpriteRenderer _spriteRenderer;
	
	public bool IsDetectSound { get; set; }
	
	private void Awake()
	{
		_circleCollider2D = GetComponent<CircleCollider2D>();
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
		
		_circleCollider2D.radius = sprite.bounds.size.x / 2 + plusRadius;

		Movement();
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = _direction * _speed;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.CompareTag("Player")) { return; }
		
		IsDetectSound = true;
		if (other.collider.TryGetComponent<IDamageable>(out var target))
		{
			target.TakeDamage(_attackPower);
			Destroy();
			return;
		}
		if (_isInvincible) { return; }
		
		Destroy();
	}

	private void Destroy()
	{
		// TODO: ［エフェクト］鉱石破壊
		GameObject effectobj = (GameObject)Resources.Load("OreEfect");
		Vector2 effectPos = new Vector2(transform.position.x,transform.position.y);
		Instantiate(effectobj, effectPos, Quaternion.identity);
		Destroy(gameObject);
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		if (other.collider.CompareTag("Player")) { return; }

		IsDetectSound = false;
	}
}

