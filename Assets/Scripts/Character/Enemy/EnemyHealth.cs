using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float CurrentHealth { get; set; }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        // TODO: ［効果音］エネミーダメージ
        AudioManager.Instance.PlaySFX("DamegeSE");

        if (CurrentHealth > 0f) { return; }

        DisableEnemy();
    }

    private void DisableEnemy()
    {
        // _animator.SetTrigger(_deadHash);
        // TODO: ［効果音］エネミー死亡
        GameObject effectobj = (GameObject)Resources.Load("EnemyDieEffect");
        Vector2 effectPos = new Vector2(transform.position.x, transform.position.y);
        Instantiate(effectobj, effectPos, Quaternion.identity);
        // TODO: ［エフェクト］エネミー死亡

        Destroy(gameObject);
    }
}

