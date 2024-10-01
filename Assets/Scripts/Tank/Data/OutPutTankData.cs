using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<<< HEAD:Assets/Scripts/Tank/Data/OutPutResultData.cs
public class OutPutResultData 
========
public class OutPutTankData 
>>>>>>>> scenemanager:Assets/Scripts/Tank/Data/OutPutTankData.cs
{
    //Tankのアイテムの割合
    public readonly float itemRatio;
    //Tankのトータルの割合
    public readonly float totalRatio;
    public readonly BlockType itemType;
    public readonly Sprite Sprite;

<<<<<<<< HEAD:Assets/Scripts/Tank/Data/OutPutResultData.cs
    public OutPutResultData(float itemRatio, float totalRatio, BlockType itemType,Sprite sprite)
========
    public OutPutTankData(float itemRatio, float totalRatio, BlockType itemType,Sprite sprite)
>>>>>>>> scenemanager:Assets/Scripts/Tank/Data/OutPutTankData.cs
    {
        this.Sprite = sprite;
        this.itemRatio = itemRatio;
        this.totalRatio = totalRatio;
        this.itemType = itemType;
    }
}
