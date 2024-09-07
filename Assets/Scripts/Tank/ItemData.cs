using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    public ItemType itemType;
    public int ItemAmount;

    public ItemData(ItemType itemType)
    {
        this.itemType = itemType;
        ItemAmount = 1;
    }

    public void ItemAdd()
    {
        ItemAmount++;
    }

    public void ItemRemove()
    {
        ItemAmount--;
    }

}
