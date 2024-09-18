using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerEquipDataBase",menuName = "Data/PlayerEquipDataBase")]
public class PlayerEquipDataBase : ScriptableObject
{
    public List<EquipData> equipData;

    private void OnEnable()
    {
        equipData.Clear();
    }

}
