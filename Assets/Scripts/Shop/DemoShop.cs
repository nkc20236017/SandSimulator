using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DemoShop 
{
    private IShopRepository shopRepository;

    public DemoShop(IShopRepository shopRepository)
    {
        this.shopRepository = shopRepository;
    }

    public void ShopBuy(string itemId)
    {
        var itemData = shopRepository.FindTrader(itemId);
        var playerMoney = shopRepository.GetMoney();

        playerMoney -= itemData.ItemPrice;

    }


}
