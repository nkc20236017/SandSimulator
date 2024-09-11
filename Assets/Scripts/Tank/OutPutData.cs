using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutPutData 
{
    public readonly float itemRatio;
    public readonly float totalRatio;
    public readonly ItemType itemType;

    public OutPutData(float itemRatio, float totalRatio, ItemType itemType)
    {
        this.itemRatio = itemRatio;
        this.totalRatio = totalRatio;
        this.itemType = itemType;
    }
}
