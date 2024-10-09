using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.BaseCommands.Import.Commit;

[System.Serializable]
public class StatusValue
{

    private readonly int baseValue;

    public StatusValue(int baseValue)
    {
        this.baseValue = baseValue;
    }

    private List<int> modifiers = new ();

    public int GetValue()
    {
        int finalValue = baseValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }


    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }

}
