using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ResultAnimationUI : MonoBehaviour
{
    [Header("UIの設定")]

    [SerializeField]
    private Image resultIcon;
    [SerializeField]
    private Text amountText;
    [SerializeField]
    private Text priceText;

    [Header("DoTweenの設定")]
    [SerializeField]
    private float defaultSpeed;
    [SerializeField]
    private Vector2 centerPoint;
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private GameObject stampObj;
    [SerializeField]
    private Vector2 outPoint;
    [Header("UniTaskの設定")]
    [SerializeField]
    private float timeDelay;

    private RectTransform rectTransform;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    public void SetUpUI(Sprite itemIcon , string amountText,string priceText)
    {
        resultIcon.sprite = itemIcon;
        this.amountText.text = "×" + amountText;
        this.priceText.text = priceText;
    }

    public UniTask AnimationEnterUI(CancellationToken cancellationToken)
    {

        return rectTransform.DOAnchorPos(centerPoint, defaultSpeed)
            .ToUniTask(cancellationToken:cancellationToken);
    }

    public UniTask AnimationStampUI(CancellationToken cancellationToken)
    {
        //スタンプの処理
        stampObj.SetActive(true);
        return stampObj.transform.DOScale(new Vector2(1, 1), waitTime)
            .ToUniTask(cancellationToken: cancellationToken);
    }

    public async UniTask AnimationExitUI(CancellationToken cancellationToken)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);
        await rectTransform
            .DOAnchorPos(outPoint, defaultSpeed)
            .ToUniTask(cancellationToken:cancellationToken);
    }


}
