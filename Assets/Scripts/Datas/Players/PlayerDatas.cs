using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerData",menuName = "ScriptableObjects/Datas/PlayerData")]
public class PlayerDatas : ScriptableObject 
{
    public int MaxHealth;
    public int CurrentHealth;
    public int PlayerMoney;
    public float MaxTankCapacity;
    public float CurrentTankCapacity;
}
