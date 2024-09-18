using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EquipmentDataDataBase", menuName = "Data/EquipmentDataBase")]
public class EquipDataBase : ScriptableObject
{
    public List<EquipData> equipmentData;
}
