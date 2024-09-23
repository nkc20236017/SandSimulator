using System.Collections.Generic;

public interface IShopRepository
{
    ShopData FindShopData(EquipData equipData);
    List<ShopData> FindAll();
    void ShopBuyCommand(EquipData id);
    bool ShopBuyCheck(EquipData equipData);
}