using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerRepository
{
    int GetHealth();
    void SetHealth(int health);
    void SetMoney(int money);
    int GetMoney();
}
