using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public class DataAccess : IItemRepository
{
    private readonly ItemDataBase inventoryDataBase;

    [Inject]
    public DataAccess(ItemDataBase inventoryDataBase)
    {
        this.inventoryDataBase = inventoryDataBase;
    }

    public ItemData FindItem(string itemId)
    {
        return inventoryDataBase
            .itemDatas.Where(item => item.ItemId == itemId)
            .FirstOrDefault();
    }
}
