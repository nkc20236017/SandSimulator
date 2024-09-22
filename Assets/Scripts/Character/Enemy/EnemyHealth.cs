using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    // [Header("Health Bar")]
    // [SerializeField] private GameObject healthBarObject;
    // [SerializeField] private Image healthBar;

    // private float _maxHealth;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private EnemyBrain _enemyBrain;
    
    public float CurrentHealth { get; set; }

    private void Awake()
    {
        // _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _enemyBrain = GetComponent<EnemyBrain>();
    }

    // private void Update()
    // {
    //     // TODO: 敵の体力バーを追加する（ダメージを受けたら♥のUIを表示させる） ━━━━━━━━　♥♥♥♥♥♥♥♥♥♥
    //     // UpdateEnemyUI();
    // }

    // private void UpdateEnemyUI()
    // {
    //     if (healthBarObject == null) { return; }
    //     if (healthBar == null) { return; }
    //     
    //     healthBarObject.SetActive(_currentHealth < _maxHealth);
    //     
    //     healthBar.fillAmount = 
    //         Mathf.Lerp(
    //             healthBar.fillAmount, 
    //             _currentHealth / _maxHealth, 
    //             10f * Time.fixedDeltaTime
    //         );
    // }
    
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        // TODO: ダメージを受けた時効果音
        if (CurrentHealth > 0f) { return; }

        DisableEnemy();
    }

    private void DisableEnemy()
    {
        // _animator.SetTrigger(_deadHash);
        _spriteRenderer.sortingOrder = -1;
        _enemyBrain.enabled = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Static;
        // TODO: アイテムドロップ（経験値結晶のみ）
        Destroy(gameObject);
    }
}

