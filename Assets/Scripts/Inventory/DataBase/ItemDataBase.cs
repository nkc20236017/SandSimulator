using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewItemDataBase",menuName = "Data/DataBase")]
public class ItemDataBase : ScriptableObject
{
    public List<ItemData> itemDatas = new List<ItemData>();
}
