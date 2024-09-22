using UnityEngine;
using VContainer;

public class ShopService : IInputShop
{
    private IEquipRepository equipRepository;
    private IDemoPlayeRepository demoPlayeRepository;
    private IShopCommand shopCommand;
    private IOutPutShop outPutShop;
    private IEquip equip;

    [Inject]
    public ShopService(IEquipRepository shopRepository, IDemoPlayeRepository demoPlayeRepository
        , IShopCommand shopCommand, IOutPutShop outPutShop, IEquip equip)
    {
        this.demoPlayeRepository = demoPlayeRepository;
        this.equipRepository = shopRepository;
        this.shopCommand = shopCommand;
        this.outPutShop = outPutShop;
        this.equip = equip;
    }

    public void ShopBuy(string equipId)
    {
        var requestShopItem = equipRepository.FindData(equipId);
        var playerData = demoPlayeRepository.Find();
        OutPutData outputData;
        var money = playerData.PlayerMoney;

        if (shopCommand.ShopBuyCheck(equipId) == true)
        {
            equip.Equip(equipId);
            outputData = new OutPutData(equipId,money);
            outPutShop.EquipUI(outputData);
            Debug.Log("バキュームを装備します");
            return;
        }
        //Shopのデータ
        if (playerData.PlayerMoney < requestShopItem.EquipPrice)
        {
            outputData = new OutPutData(equipId, money);
            outPutShop.NotBuyUI(outputData);
            Debug.Log("お金が足りません");
            return;
        }

        money -= requestShopItem.EquipPrice;

        outputData = new OutPutData(equipId,money);
        shopCommand.ShopBuyCommand(equipId);
        outPutShop.ShopUI(outputData);

    }
}
