using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SignboardAnimetion : MonoBehaviour
{
    [SerializeField]
    [Header("�ړ����鑬�x")]
    private float appearSpeed;

    [SerializeField]
    [Header("�߂鑬�x")]
    private float backSpeed;

    [SerializeField]
    [Header("�ړ��O�̏ꏊ")]
    private Vector2 startPoint;

    [SerializeField]
    [Header("�ڕW�n�_")]
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
