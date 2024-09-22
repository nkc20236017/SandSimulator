using System.Collections.Generic;
using System.Linq;
using VContainer;

public class MockShopDataAccess : IShopCommand
{
    private DemoShopDataBase ShopDataBase;
    private List<ShopData> shopDatas;
    private IEquipRepository EquipRepository;

    [Inject]
    public MockShopDataAccess(DemoShopDataBase shopDataBase
        , IEquipRepository equipRepository)
    {
        ShopDataBase = shopDataBase;
        EquipRepository = equipRepository;
        shopDatas = new();
    }

    public bool ShopBuyCheck(string id)
    {

        if (shopDatas.Any()== false)
        {
            return false;
        }

        var shopData = shopDatas
            .Where(shopData=>shopData.Equipment.EquipId==id)
            .FirstOrDefault();

        return shopData.SoldOut;
    }

    public void ShopBuyCommand(string id)
    {
        var Equip = EquipRepository.FindData(id);
        shopDatas.Add(new ShopData(Equip));
        var shopData = shopDatas
            .Where(shopData => shopData.Equipment.EquipId == id)
            .FirstOrDefault();

        shopData.ShopBuy();

    }
}

