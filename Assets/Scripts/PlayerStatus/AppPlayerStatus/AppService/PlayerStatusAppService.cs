using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusAppService : ILevelUp
{

    private IStatusModifier statusModifier;

    public PlayerStatusAppService(IStatusModifier statusModifier)
    {
        this.statusModifier = statusModifier;
    }

    public void LevelUp()
    {
        //statusModifier.AddModifier(new ModifierData());
    }
}
