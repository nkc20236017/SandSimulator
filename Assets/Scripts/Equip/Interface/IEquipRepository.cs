using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipRepository 
{
    EquipData FindData(string equipId);
    void AddEquip(string equipId);
    void RemoveEquip(string equipId);
}
