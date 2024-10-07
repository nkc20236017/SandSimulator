using DG.Tweening;
using System.Collections;
using UnityEngine;

public class HealOre : MonoBehaviour
{
    [SerializeField]
    [Header("点滅まで待つ時間")]
    private float waitTime;
    [SerializeField]
    [Header("点滅回数")]
    private int count;
    [SerializeField]
    [Header("tween時間")]
    private float time;
    [Header("回復量")]
    public int healPoint;
    [SerializeField]
    [Header("スプライト")]
    SpriteRenderer sprite;
    int x;

    void Start()
    {
        StartCoroutine(ColorChenge());
    }

    private IEnumerator ColorChenge()
    {
        yield return new WaitForSeconds(waitTime);

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
        sprite.DOKill();
        Destroy(gameObject);
    }
}
