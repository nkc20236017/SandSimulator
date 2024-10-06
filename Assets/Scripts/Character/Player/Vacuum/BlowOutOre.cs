using UnityEngine;

public class BlowOutOre : MonoBehaviour
{
	[SerializeField] private float _speed;
	[SerializeField] private float plusRadius;
	[SerializeField] private float despawnTime = 300;
	[SerializeField] private float invincibleTime;
	
	private int _attackPower;
	private float _invincibleTimer;
	private bool _isInvincible = true;
	private Vector2 _direction;
	private Color _color;
	private CircleCollider2D _circleCollider2D;
	private Rigidbody2D _rigidbody2D;
	private SpriteRenderer _spriteRenderer;
	private ISoundSourceable _soundSource;
	
	
	private void Awake()
	{
		var soundSource = FindObjectOfType<SoundSource>();
		_soundSource = soundSource.GetComponent<ISoundSourceable>();
		_soundSource.SetInstantiation("BlowOutOre");
		
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
	
	public void SetOre(Ore ore, Vector2 direction)
	{
		_attackPower = ore.attackPower;
		_speed /= ore.weightPerSize[0];
		_direction = direction;
		_spriteRenderer.sprite = ore.oreSprites[0];
		_color = ore.color;
		
		_circleCollider2D.radius = ore.oreSprites[0].bounds.size.x / 2 + plusRadius;

		Movement();
	}
	
	private void Movement()
	{
		_rigidbody2D.velocity = _direction * _speed;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.CompareTag("Player")) { return; }
		
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
		_soundSource.InstantiateSound("BlowOutOre", transform.position);
		// TODO: ［エフェクト］鉱石破壊
		AudioManager.Instance.PlaySFX("BreakSE");
		GameObject effectobj = (GameObject)Resources.Load("OreEfect");
		Vector2 effectPos = new Vector2(transform.position.x,transform.position.y);
		Instantiate(effectobj, effectPos, Quaternion.identity);
		Destroy(gameObject);
	}
}
