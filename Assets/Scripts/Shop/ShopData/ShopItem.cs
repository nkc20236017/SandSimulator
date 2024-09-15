using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem
{
    public readonly ItemData ItemData;
    public bool IsSoldOut{ get; private set; }

    public void Buy()
    {
        IsSoldOut = true;
    }


}
