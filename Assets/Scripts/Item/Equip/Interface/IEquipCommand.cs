using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipCommand 
{
    void Equip(EquipData equipData);
    void UnEquip(EquipData equipData);
}
