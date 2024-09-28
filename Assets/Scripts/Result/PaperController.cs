using UnityEngine;
using DG.Tweening;
using UnityEditor;
using System.Collections;

public class PaperController : MonoBehaviour
{
    [SerializeField]
    [Header("�ʏ펞�̑��x")]
    private float defaultSpeed;

    [SerializeField]
    [Header("�X�L�b�v���̑��x")]
    private float highSpeed;

    [SerializeField]
    [Header("��~����ꏊ")]
    private Vector2 centerPoint;

    [SerializeField]
    [Header("�ҋ@����")]
    private float waitTime;

    [SerializeField]
    [Header("�X�^���v�̃I�u�W�F�N�g")]
    private GameObject stampObj;

    [SerializeField]
    [Header("��~��Ɉړ�����ꏊ")]
    private Vector2 outPoint;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        rectTransform.DOAnchorPos(centerPoint, defaultSpeed).OnComplete(OnStamp);
    }

    private void OnStamp()
    {
        //�X�^���v�̏���
        stampObj.SetActive(true);
        stampObj.transform.DOScale(new Vector2(1,1), waitTime);
    }

    private void OnOut()
    {
        rectTransform.DOAnchorPos(outPoint, defaultSpeed).OnComplete(DestroyObj);
    }

    private void DestroyObj()
    {
        Destroy(gameObject);
    }
}
