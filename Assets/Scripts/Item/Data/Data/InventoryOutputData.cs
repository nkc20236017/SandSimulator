using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOutputData
{
    public readonly List<EquipData> equipData;

    public InventoryOutputData(List<EquipData> equipData)
    {
        this.equipData = equipData;
    }
}
