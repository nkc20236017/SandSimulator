using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class InventoryDataAccess : IInventoryRepository
{

    private InventoryDataBase inventoryDataBase;
    private Dictionary<ItemData, ItemInventory> inventoryDictionary;

    [Inject]
    public InventoryDataAccess(InventoryDataBase inventoryDataBase)
    {
        this.inventoryDataBase = inventoryDataBase;
        inventoryDictionary = this.inventoryDataBase.DataBase;
    }

    public void AddToInventory(ItemData item)
    {
        if(inventoryDictionary.TryGetValue(item,out var inventory))
        {
            inventory.AddStack();
        }
        else
        {
            inventory = new ItemInventory(item);
            inventoryDictionary.Add(item, inventory);
        }
    }

    public List<ItemInventory> FindAll()
    {
        var inventoryList = new List<ItemInventory>();

        foreach(var item in inventoryDictionary.Values)
        {
            inventoryList.Add(item);
        }

        return inventoryList;

    }

    public void RemoveFromInventory(ItemData item)
    {
        if(inventoryDictionary.TryGetValue(item, out var inventory))
        {
            if(inventory.StackSize <= 1)
            {
                inventoryDictionary.Remove(item);
            }
            else
            {
                inventory.RemoveStack();
            }
        }
    }
}