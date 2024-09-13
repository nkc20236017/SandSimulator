using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutPutData 
{
    public readonly float itemRatio;
    public readonly float totalRatio;
    public readonly BlockType itemType;
    public readonly Sprite Sprite;

    public OutPutData(float itemRatio, float totalRatio, BlockType itemType,Sprite sprite)
    {
        this.Sprite = sprite;
        this.itemRatio = itemRatio;
        this.totalRatio = totalRatio;
        this.itemType = itemType;
    }
}
