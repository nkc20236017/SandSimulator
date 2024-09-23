using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipCommand 
{
    void Equip(string id);
    void UnEquip(string id);
}
