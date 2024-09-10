using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShopData
{
    public readonly int ItemPrice;
    public readonly string itemId;
    public readonly string itemName;

    public ItemShopData(int itemPrice, string itemId, string itemName)
    {
        ItemPrice = itemPrice;
        this.itemId = itemId;
        this.itemName = itemName;
    }
}
