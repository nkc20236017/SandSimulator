using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPresenter : MonoBehaviour , IOutPutShop
{
    [SerializeField]
    private GameObject ShopUIObject;

    [SerializeField]
    private Transform ShopImageParent;

    public void Equip(OutPutData outPutData)
    {
        Debug.Log("����");
    }

    public void NotBuy(OutPutData outPutData)
    {
        Debug.Log("�w���ł��܂���ł���");
    }

    public void ShopUI(OutPutData outPutData)
    {
        Debug.Log("�w�����܂����B");
    }


}
