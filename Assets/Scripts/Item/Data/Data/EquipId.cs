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
            throw new System.NullReferenceException(id + "�s���Ȓl�ł�");
        }

        this.Id = id;
    }

}
