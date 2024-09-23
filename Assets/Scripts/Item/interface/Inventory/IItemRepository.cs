using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemRepository
{
    EquipData FindEquipData(string equipId);
}
