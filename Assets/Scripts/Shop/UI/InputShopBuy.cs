using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class InputShopBuy : MonoBehaviour
{
    private IShopBuy shopBuy;

    [SerializeField]
    private ItemData itemData;

    [Inject]
    public void Inject(IShopBuy shopBuy)
    {
        this.shopBuy = shopBuy;
    }

    public void ShopBuy()
    {
        shopBuy.ItemBuy(itemData.ItemId);
    }

}
