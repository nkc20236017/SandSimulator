using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ShopDataBase", menuName = "Data/ShopDataBase")]
public class ShopDataBase : ScriptableObject
{
    public List<ShopData> shopDatas;

    private void OnEnable()
    {
        for (int i = 0; i < shopDatas.Count; i++)
        {
            shopDatas[i].ShopCler();
        }
    }

}
