using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierData
{
    public readonly PlayerStatusType StatusType;
    public readonly int ModifierAmount;

    public ModifierData(PlayerStatusType statusType, int modifierAmount)
    {
        this.StatusType = statusType;
        this.ModifierAmount = modifierAmount;
    }

}
