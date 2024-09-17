using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopData
{
    public EquipData Equipment;
    public bool SoldOut;

    public ShopData(EquipData equipment)
    {
        Equipment = equipment;
    }

    public void ShopBuy()
    {
        SoldOut = true;
    }

}
