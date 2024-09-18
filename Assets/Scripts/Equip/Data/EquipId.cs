using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipId 
{
    public readonly string Id;

    public EquipId(string id)
    {
        if(id == string.Empty)
        {
            throw new System.NullReferenceException(id + "ïsê≥Ç»ílÇ≈Ç∑");
        }

        this.Id = id;
    }

}
