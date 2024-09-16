using UnityEngine;
using VContainer;

public class DemoShopService : IInputShop
{
    private IEquipRepository shopRepository;
    private IDemoPlayeRepository demoPlayeRepository;
    private IShopCommand shopCommand;
    private IOutPutShop outPutShop;

    [Inject]
    public DemoShopService(IEquipRepository shopRepository, IDemoPlayeRepository demoPlayeRepository
        , IShopCommand shopCommand, IOutPutShop outPutShop)
    {
        this.demoPlayeRepository = demoPlayeRepository;
        this.shopRepository = shopRepository;
        this.shopCommand = shopCommand;
        this.outPutShop = outPutShop;
    }

    public ShopServiceData GetShopServiceData(string id)
    {
        return shopRepository.FindServiceData(id);
    }

    public void ShopBuy(string equipId)
    {
        OutPutData outputData;

        if (shopCommand.ShopBuyCheck(equipId) == true)
        {
            outputData = new OutPutData(equipId);
            outPutShop.Equip(outputData);
            Debug.Log("バキュームを装備します");
            return;
        }
        //Shopのデータ
        var requestShopItem = shopRepository.FindServiceData(equipId);
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
