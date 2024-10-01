using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlyerDateAccess : IPlayerRepository
{

    private PlayerDatas playerDatas;

    public PlyerDateAccess(PlayerDatas playerDatas)
    {
        this.playerDatas = playerDatas;
    }

    public int GetHealth()
    {
        return playerDatas.CurrentHealth;
    }

    public int GetMoney()
    {
        return playerDatas.PlayerMoney;
    }

    public void SetHealth(int health)
    {
        playerDatas.CurrentHealth = health;
    }

    public void SetMoney(int money)
    {
        playerDatas.PlayerMoney = money;
    }
}
