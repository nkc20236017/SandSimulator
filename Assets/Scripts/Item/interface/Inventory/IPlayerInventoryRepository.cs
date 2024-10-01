using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInventoryRepository
{
    List<EquipData> FindAll();
}
