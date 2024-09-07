using System.Collections.Generic;
using UnityEngine;

public class ItemTank : IInputTank
{
    private Dictionary<ItemType, ItemData> itemTankDictionary = new();
    private IOutPutTank outPutTank;
    private readonly int MaxTank;

    public ItemTank(IOutPutTank outPutTank, int maxTank)
    {
        this.outPutTank = outPutTank;
        this.MaxTank = maxTank;
    }

    public void InputAddTank(ItemType type)
    {
        AddItem(type);
        TankCalculation();
    }

    public void InputRemoveTank(ItemType type)
    {
        RemoveItem(type);
    }

    public void AddItem(ItemType itemType)
    {
        if (itemTankDictionary.TryGetValue(itemType, out ItemData vaule))
        {
            vaule.ItemAdd();
        }
        else
        {
            ItemData itemData = new ItemData(itemType);
            itemTankDictionary.Add(itemType, itemData);
        }
    }

    public void RemoveItem(ItemType itemType)
    {
        if (itemTankDictionary.TryGetValue(itemType, out ItemData vaule))
        {
            if (vaule.ItemAmount <= 1)
            {
                itemTankDictionary.Remove(itemType);
            }
        }
        else
        {
            vaule.ItemRemove();
        }
    }

    float sandVaule;
    float madVaule;
    public void TankCalculation()
    {

        foreach (KeyValuePair<ItemType, ItemData> pair in itemTankDictionary)
        {
            if (pair.Key == ItemType.Sand)
            {
                sandVaule = pair.Value.ItemAmount;
            }
            if (pair.Key == ItemType.Mad)
            {
                madVaule = pair.Value.ItemAmount;
            }
        }
        float totalValue = sandVaule + madVaule;
        var totalRatio = totalValue / MaxTank;
        var sandRatio = sandVaule / totalValue;
        var madRatio = madVaule / totalValue;

        var outputTank = new OutPutData(sandRatio, madRatio, totalRatio);

        outPutTank.OutputTank(outputTank);

    }

}
