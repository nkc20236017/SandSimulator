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
        // TODO: アイテムドロップ（経験値結晶のみ）
        // TODO: ［効果音］エネミー死亡
        // TODO: ［エフェクト］エネミー死亡

        Destroy(gameObject);
    }
}

