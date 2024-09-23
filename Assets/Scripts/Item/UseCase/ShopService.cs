using UnityEngine;
using VContainer;

public class ShopService : IInputShop
{
    private IDemoPlayeRepository demoPlayeRepository;
    private IShopRepository shopRepository;
    private IOutPutShop outPutShop;
    private IEquip equip;
    private IInputInventory inputInventory;

    [Inject]
    public ShopService( IDemoPlayeRepository demoPlayeRepository, IShopRepository shopRepository,
        IOutPutShop outPutShop, IEquip equip, IInputInventory inventoryCommand)
    {
        this.demoPlayeRepository = demoPlayeRepository;
        this.shopRepository = shopRepository;
        this.outPutShop = outPutShop;
        this.equip = equip;
        this.inputInventory = inventoryCommand;
    }

    public void ShopBuy(EquipData equipData)
    {
        var playerData = demoPlayeRepository.Find();
        var money = playerData.PlayerMoney;
        OutPutData outputData;

        if (shopRepository.ShopBuyCheck(equipData) == true)
        {
            equip.Equip(equipData);
            Debug.Log("バキュームを装備します");
            return;
        }
        //Shopのデータ
        if (playerData.PlayerMoney < equipData.EquipPrice)
        {
            outputData = new OutPutData(equipData, money);
            outPutShop.NotBuyUI(outputData);
            Debug.Log("お金が足りません");
            return;
        }

        money -= equipData.EquipPrice;

        shopRepository.ShopBuyCommand(equipData);
        outputData = new OutPutData(equipData,money);
        inputInventory.AddToInventory(equipData);
        outPutShop.ShopUI(outputData);

    }
}
