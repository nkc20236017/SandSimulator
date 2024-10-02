using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextController : MonoBehaviour
{
    [SerializeField]
    [Header("�����������e�L�X�g")]
    private Text text;

    [SerializeField]
    [Header("�t�F�[�h����")]
    private float time;

    [SerializeField]
    [Header("���Ƃ��Ƃ̐F")]
    private Color startcolor;

    [SerializeField]
    [Header("�ύX��̐F")]
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
