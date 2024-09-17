using UnityEngine;
using VContainer;

public class DemoShopService : IInputShop
{
    private IEquipRepository equipRepository;
    private IDemoPlayeRepository demoPlayeRepository;
    private IShopCommand shopCommand;
    private IOutPutShop outPutShop;
    private IEquip equip;

    [Inject]
    public DemoShopService(IEquipRepository shopRepository, IDemoPlayeRepository demoPlayeRepository
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
        OutPutData outputData;

        if (shopCommand.ShopBuyCheck(equipId) == true)
        {
            equip.Equip(equipId);
            outputData = new OutPutData(equipId);
            outPutShop.Equip(outputData);
            Debug.Log("バキュームを装備します");
            return;
        }
        //Shopのデータ
        var requestShopItem = equipRepository.FindData(equipId);
        var playerData = demoPlayeRepository.Find();
        if (playerData.PlayerMoney < requestShopItem.EquipPrice)
        {
            outputData = new OutPutData(equipId);
            outPutShop.NotBuy(outputData);
            Debug.Log("お金が足りません");
            return;
        }

        outputData = new OutPutData(equipId);
        shopCommand.ShopBuyCommand(equipId);
        outPutShop.ShopUI(outputData);

    }
}
