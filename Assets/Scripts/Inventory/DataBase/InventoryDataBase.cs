using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDataBase",menuName = "Data/InventoryDataBase")]
public class InventoryDataBase : ScriptableObject
{
    public Dictionary<ItemData, ItemInventory> DataBase = new();
}
