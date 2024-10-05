using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
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
        // TODO: ［効果音］エネミーダメージ
        AudioManager.Instance.PlaySFX("DamegeSE");
        // TODO: ［エフェクト］エネミーダメージ
        GameObject effectObj = (GameObject)Resources.Load("HitEffect");
        Vector3 effectPos = 
            new Vector3
            (transform.position.x,
            transform.position.y,
            transform.position.z - 1);
        Instantiate(effectObj, effectPos, Quaternion.identity);

        if (CurrentHealth > 0f) { return; }

        DisableEnemy();
    }

    private void DisableEnemy()
    {
        // _animator.SetTrigger(_deadHash);
        // TODO: アイテムドロップ（経験値結晶のみ）
        // TODO: ［効果音］エネミー死亡
        // TODO: ［エフェクト］エネミー死亡

        Destroy(gameObject);
    }
}

