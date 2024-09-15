using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShopData
{
    public readonly int ItemPrice;
    public readonly string itemId;
    public readonly string itemName;
    public readonly bool Soldout;

    public ItemShopData(EquipmentData equipment)
    {
        ItemPrice = equipment.ItemPrice;
        itemId = equipment.ItemId;
        itemName = equipment.ItemName;
        Soldout = equipment.Soldout;
    }
}
