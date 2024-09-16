using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopData
{
    public EquipmentData Equipment;
    public bool SoldOut { get; private set; }

    public ShopData(EquipmentData equipment)
    {
        Equipment = equipment;
    }

    public void ShopBuy()
    {
        SoldOut = true;
    }

}
