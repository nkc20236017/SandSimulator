using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipRepository
{
    ShopServiceData FindServiceData(string id);
    EquipmentData FindData(string id);
}
