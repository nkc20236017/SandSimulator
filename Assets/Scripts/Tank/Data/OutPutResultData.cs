using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutPutResultData 
{
    //Tank�̃A�C�e���̊���
    public readonly float itemRatio;
    //Tank�̃g�[�^���̊���
    public readonly float totalRatio;
    public readonly BlockType itemType;
    public readonly Sprite Sprite;

    public OutPutResultData(float itemRatio, float totalRatio, BlockType itemType,Sprite sprite)
    {
        this.Sprite = sprite;
        this.itemRatio = itemRatio;
        this.totalRatio = totalRatio;
        this.itemType = itemType;
    }
}