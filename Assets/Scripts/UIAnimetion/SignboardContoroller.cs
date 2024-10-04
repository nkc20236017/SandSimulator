using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SignboardAnimetion : MonoBehaviour
{
    [SerializeField]
    [Header("移動する速度")]
    private float appearSpeed;

    [SerializeField]
    [Header("戻る速度")]
    private float backSpeed;

    [SerializeField]
    [Header("移動前の場所")]
    private Vector2 startPoint;

    [SerializeField]
    [Header("目標地点")]
    private Vector2 movePoint;

    private RectTransform recPos;

    void Awake()
    {
        recPos = GetComponent<RectTransform>();
    }

    public void AppearUI()
    {
        transform.DOKill();
        recPos.DOAnchorPos(movePoint, appearSpeed).SetEase(Ease.OutExpo);
    }

    public void BackUI()
    {
        transform.DOKill();
        recPos.DOAnchorPos(startPoint, backSpeed);
    }

    public void OnOutLine()
    {
        Outline outline = GetComponent<Outline>();
        outline.enabled = true;
    }

    public void KillOutLine()
    {
        Outline outline = GetComponent<Outline>();
        outline.enabled = false;
    }
}
