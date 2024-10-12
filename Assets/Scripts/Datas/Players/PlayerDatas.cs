using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerData",menuName = "ScriptableObjects/Datas/PlayerData")]
public class PlayerDatas : ScriptableObject 
{
    public List<StatusValue> statusValues = new List<StatusValue>();

    public int PlayerMoney;
    public float PlayerExpRate;
    public int CurrentExp;
    public int BaseNextLevelExp;

}
