using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public class ShopDataAccess : IShopRepository
{
    private ShopDataBase shopDataBase;

    [Inject]
    public ShopDataAccess(ShopDataBase ShopDataBase)
    {
        this.shopDataBase = ShopDataBase;
    }

    public List<ShopData> FindAll()
    {
        return shopDataBase.shopDatas;
    }

    public ShopData FindShopData(EquipData equipData)
    {
        return shopDataBase.shopDatas
            .Where(shopData => shopData.Equipment.EquipId == equipData.EquipId)
            .FirstOrDefault();
    }

    public bool ShopBuyCheck(EquipData equipData)
    {
        if (shopDataBase.shopDatas.Any() == false)
        {
            return false;
        }

        var shopData = shopDataBase.shopDatas
            .Where(shopData => shopData.Equipment.EquipId == equipData.EquipId)
            .FirstOrDefault();

        if(shopData == null)
        {
            return false;
        }

        return shopData.SoldOut;
    }

    public void ShopBuyCommand(EquipData equipData)
    {
        var shopData = shopDataBase.shopDatas
            .Where(shopData => shopData.Equipment.EquipId == equipData.EquipId)
            .FirstOrDefault();

        shopData.ShopBuy();

    }
}
