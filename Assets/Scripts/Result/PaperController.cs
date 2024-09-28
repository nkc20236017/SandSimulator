using UnityEngine;
using DG.Tweening;
using UnityEditor;
using System.Collections;

public class PaperController : MonoBehaviour
{
    [SerializeField]
    [Header("通常時の速度")]
    private float defaultSpeed;

    [SerializeField]
    [Header("スキップ時の速度")]
    private float highSpeed;

    [SerializeField]
    [Header("停止する場所")]
    private Vector2 centerPoint;

    [SerializeField]
    [Header("待機時間")]
    private float waitTime;

    [SerializeField]
    [Header("スタンプのオブジェクト")]
    private GameObject stampObj;

    [SerializeField]
    [Header("停止後に移動する場所")]
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
        //スタンプの処理
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
