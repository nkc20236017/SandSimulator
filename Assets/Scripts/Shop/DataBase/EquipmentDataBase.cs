using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EquipmentDataDataBase", menuName = "Data/EquipmentDataBase")]
public class EquipmentDataBase : ScriptableObject
{
    public List<EquipmentData> equipmentData;
}
