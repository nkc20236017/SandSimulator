using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public class ItemDataAccess : IItemRepository
{
    private EquipDataBase EquipDataBase;

    [Inject]
    public ItemDataAccess(EquipDataBase equipDataBase)
    {
        EquipDataBase = equipDataBase;
    }


    public EquipData FindEquipData(string equipId)
    {
        return EquipDataBase.equipmentData
            .Where(item => item.EquipId == equipId)
            .FirstOrDefault();
    }
}
