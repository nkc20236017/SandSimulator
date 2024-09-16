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
        Debug.Log("‘•”õ");
    }

    public void NotBuy(OutPutData outPutData)
    {
        Debug.Log("w“ü‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½");
    }

    public void ShopUI(OutPutData outPutData)
    {
        Debug.Log("w“ü‚µ‚Ü‚µ‚½B");
    }


}
