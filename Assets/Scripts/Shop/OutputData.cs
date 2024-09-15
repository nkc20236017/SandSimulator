using UnityEngine;

public class OutputData
{
    public readonly bool CanBuy;
    public readonly int PlayerMoney;
    public OutputData(bool canBuy, int playerMoney)
    {
        this.CanBuy = canBuy;
        this.PlayerMoney = playerMoney;
    }
}
