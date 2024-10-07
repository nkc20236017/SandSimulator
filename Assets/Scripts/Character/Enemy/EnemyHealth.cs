using UnityEngine;
using DG.Tweening;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float CurrentHealth { get; set; }
    [SerializeField]
    private SpriteRenderer sprite;
    [SerializeField]
    private int count;
    [SerializeField]
    private float time;
    private int x;
    private bool isDamege;

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        // TODO: ［効果音］エネミーダメージ
        AudioManager.Instance.PlaySFX("DamegeSE");
        StartCoroutine(ColorChenge());
        if (CurrentHealth > 0f) { return; }
        DisableEnemy();
    }

    private IEnumerator ColorChenge()
    {
        if(isDamege == false) 
        {
            isDamege = true;
            //点滅処理
            //今回は簡易的にDOColorでα値を変更
            for (int z = 0; z < count * 2; z++)
            {
                if (x != 0)
                {
                    yield return new WaitForSeconds(time);
                    sprite.DOColor(new Color(255, 255, 255, x), time);
                    x = 0;
                }
                else
                {
                    yield return new WaitForSeconds(time);
                    sprite.DOColor(new Color(255, 255, 255, x), time);
                    x = 255;
                }
            }
            isDamege = false;
        }
        
    }
        

    private void DisableEnemy()
    {
        // _animator.SetTrigger(_deadHash);
        // TODO: ［効果音］エネミー死亡
        GameObject effectobj = (GameObject)Resources.Load("EnemyDieEffect");
        Vector2 effectPos = new Vector2(transform.position.x, transform.position.y);
        Instantiate(effectobj, effectPos, Quaternion.identity);

        GameObject healobj = (GameObject)Resources.Load("HealOre");
        Vector2 healPos = new Vector2(transform.position.x, transform.position.y);
        Instantiate(healobj, healPos, Quaternion.identity);
        // TODO: ［エフェクト］エネミー死亡

        Destroy(gameObject);
    }
}