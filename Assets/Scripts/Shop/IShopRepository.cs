public interface IShopRepository
{
    ItemShopData FindTrader(string itemId);
    int GetMoney();
}

