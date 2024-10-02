using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextController : MonoBehaviour
{
    [SerializeField]
    [Header("動かしたいテキスト")]
    private Text text;

    [SerializeField]
    [Header("フェード時間")]
    private float time;

    [SerializeField]
    [Header("もともとの色")]
    private Color startcolor;

    [SerializeField]
    [Header("変更後の色")]
    private Color newcolor;

    void Start()
    {
        Fade();
    }

    private void Fade()
    {
        text.DOColor(newcolor, time).OnComplete(Back);
    }

    private void Back()
    {
        text.DOColor(startcolor, time).OnComplete(Fade);
    }

    private void OnDestroy()
    {
        text.DOKill();
    }
}
