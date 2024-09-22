using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopDataAccess : IShopRepository
{
    private DemoShopDataBase DemoShopDataBase;

    public ShopDataAccess(DemoShopDataBase demoShopDataBase)
    {
        this.DemoShopDataBase = demoShopDataBase;
    }

    public ShopData FindShopData(string Id)
    {
        return DemoShopDataBase.shopDatas
            .Where(shopData => shopData.Equipment.EquipId == Id)
            .FirstOrDefault();
    }
}
