using DG.Tweening;
using System.Collections;
using UnityEngine;

public class HealOre : MonoBehaviour
{
    [SerializeField]
    [Header("�_�ł܂ő҂���")]
    private float waitTime;
    [SerializeField]
    [Header("�_�ŉ�")]
    private int count;
    [SerializeField]
    [Header("tween����")]
    private float time;
    [Header("�񕜗�")]
    public int healPoint;
    [SerializeField]
    [Header("�X�v���C�g")]
    SpriteRenderer sprite;
    int x;

    void Start()
    {
        StartCoroutine(ColorChenge());
    }

    private IEnumerator ColorChenge()
    {
        yield return new WaitForSeconds(waitTime);

        //�_�ŏ���
        //����͊ȈՓI��DOColor�Ń��l��ύX
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
