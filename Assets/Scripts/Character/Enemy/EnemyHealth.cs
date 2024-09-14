using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    [Header("Config")] 
    [SerializeField] private string enemyID;
    [SerializeField] private float health;
    [SerializeField, Label("Despawn Time (s)")] private float despawnTime;
    
    [Header("Upgrade Health")]
    [SerializeField] private float minUpgradeHealth;
    [SerializeField] private float maxUpgradeHealth;
    
    [Header("Health Bar")]
    [SerializeField] private GameObject healthBarObject;
    [SerializeField] private Image healthBar;

    private readonly int _deadHash = Animator.StringToHash("Dead");
    private float _maxHealth;
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private EnemyBrain _enemyBrain;

    public float DespawnTime => despawnTime;
    public float CurrentDespawnTime { get; set; }
    public float CurrentHealth { get; private set; }
    public string EnemyID => enemyID;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _enemyBrain = GetComponent<EnemyBrain>();
    }

    private void Start()
    {
        _maxHealth = health;
        CurrentHealth = health;
        CurrentDespawnTime = DespawnTime;
        
        // TODO: 一定時間プレイヤーから離れていたらデスポーンさせる
        if (DespawnTime != 0f)
        {
            Destroy(gameObject, CurrentDespawnTime);
        }

        // TODO: プレイヤーのレベルに合わせて敵を強化する
    }

    private void Update()
    {
        // TODO: プレイヤーが死亡したらデスポーンする
        
        // TODO: 敵の体力バーを追加する（ダメージを受けたら♥のUIを表示させる）
        UpdateEnemyUI();
    }

    private void UpdateEnemyUI()
    {
        if (healthBarObject == null) { return; }
        if (healthBar == null) { return; }
        
        healthBarObject.SetActive(CurrentHealth < _maxHealth);
        
        healthBar.fillAmount = 
            Mathf.Lerp(
                healthBar.fillAmount, 
                CurrentHealth / _maxHealth, 
                10f * Time.fixedDeltaTime
            );
    }
    
    private void UpgradeHealth()
    {
        var upgradeHealth = Random.Range(minUpgradeHealth, maxUpgradeHealth);
        _maxHealth = Mathf.RoundToInt(_maxHealth + upgradeHealth);
        CurrentHealth = _maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        // TODO: ダメージを受けた時効果音
        if (CurrentHealth > 0f) { return; }

        DisableEnemy();
    }

    private void DisableEnemy()
    {
        _animator.SetTrigger(_deadHash);
        _spriteRenderer.sortingOrder = -1;
        _enemyBrain.enabled = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Static;
        // TODO: アイテムドロップ
        Destroy(gameObject);
    }
}

