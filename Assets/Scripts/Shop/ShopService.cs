using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class ShopService : IShopBuy
{
    private IShopRepository shopRepository;
    private IItemInputSignal itemInputSignal;
    private IShopOutPut shopOutPut;
    private int playerMoney;

    [Inject]
    public ShopService(
        IShopRepository shopRepository, IItemInputSignal itemInputSignal
        , int playerMoney, IShopOutPut shopOutPut)
    {
        this.shopOutPut = shopOutPut;
        this.shopRepository = shopRepository;
        this.itemInputSignal = itemInputSignal;
        this.playerMoney = playerMoney;
        this.shopOutPut = shopOutPut;
    }

    public void ItemBuy(string itemid)
    {
        OutputData shopOutPutData;
        var shopItem = shopRepository.FindTrader(itemid);
        if(playerMoney < shopItem.ItemPrice)
        {
            Debug.Log(playerMoney+"‚¨‹à‚ª‘«‚è‚È‚¢");
            shopOutPutData = new OutputData(true,playerMoney);
            shopOutPut.OutPut(shopOutPutData);
            return;
        }

        playerMoney -= shopItem.ItemPrice;
        itemInputSignal.AddItem(shopItem.itemId);
        shopOutPutData = new OutputData(false,playerMoney);
        shopOutPut.OutPut(shopOutPutData);
    }

    public int GetMoney() => playerMoney;

}
