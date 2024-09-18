using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerEquipRepository
{
    EquipData FindData(string equipId);
    EquipId FindEquipId(string equipId);
    void AddEquip(string equipId);
    void RemoveEquip(string equipId);
}
