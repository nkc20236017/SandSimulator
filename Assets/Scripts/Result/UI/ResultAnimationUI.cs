using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ResultAnimationUI : MonoBehaviour
{
    [Header("UIÇÃê›íË")]

    [SerializeField]
    private Image resultIcon;
    [SerializeField]
    private Text amountText;
    [SerializeField]
    private Text priceText;

    [Header("DoTweenÇÃê›íË")]
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
    [Header("UniTaskÇÃê›íË")]
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
        this.amountText.text = "Å~" + amountText;
        this.priceText.text = priceText;
    }

    public UniTask AnimationEnterUI(CancellationToken cancellationToken)
    {
        return rectTransform.DOAnchorPos(centerPoint, defaultSpeed)
            .ToUniTask(cancellationToken:cancellationToken);
    }

    public UniTask AnimationStampUI(CancellationToken cancellationToken)
    {
        //ÉXÉ^ÉìÉvÇÃèàóù
        stampObj.SetActive(true);
        return stampObj.transform.DOScale(new Vector2(1, 1), waitTime)
            .ToUniTask(cancellationToken: cancellationToken);
    }

    public UniTask AnimationExitUI(CancellationToken cancellationToken)
    {
        return rectTransform
            .DOAnchorPos(outPoint, defaultSpeed)
            .ToUniTask(cancellationToken:cancellationToken);
    }


}
