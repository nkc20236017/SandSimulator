using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ShopDataBase", menuName = "Data/ShopDataBase")]
public class DemoShopDataBase : ScriptableObject
{
    public List<ShopData> shopDatas;
}
