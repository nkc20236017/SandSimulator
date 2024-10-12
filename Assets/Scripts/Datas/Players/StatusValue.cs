using System.Collections.Generic;

[System.Serializable]
public class StatusValue
{
    public PlayerStatusType statusType;
    public int baseValue;

    private List<int> modifiers = new();

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
