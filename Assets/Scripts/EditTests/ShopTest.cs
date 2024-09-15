using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class ShopTest
{
    [Test]
    public void ShopTests()
    {
        var itemInventory = new ItemInventoy() ;
        var shopRepository = new MockShopDataAccess();
        var shopOutput = new MockShopUI();
        var shopService = new ShopService(shopRepository,itemInventory,100,shopOutput);
        shopService.ItemBuy("001");
    }
}

public class MockShopDataAccess : IShopRepository
{
    public ItemShopData FindTrader(string itemId)
    {
        return null;
    }

    public int GetMoney()
    {
        return 0;
    }
}

public class ItemInventoy : IItemInputSignal
{
    public void AddItem(string itemId)
    {
        Debug.Log("�A�C�e����ǉ�������");
    }
}

public class MockShopUI : IShopOutPut
{
    public void OutPut(OutputData outputData)
    {
        Debug.Log(outputData.PlayerMoney);
        Debug.Log(outputData.CanBuy);
    }
}
