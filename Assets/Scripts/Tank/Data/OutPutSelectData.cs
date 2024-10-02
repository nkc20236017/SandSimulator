using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutPutSelectData
{
    public readonly BlockType type;
    public readonly Sprite SelectSprite;

    public OutPutSelectData(BlockType type, Sprite selectSprite)
    {
        this.type = type;
        SelectSprite = selectSprite;
    }

}
