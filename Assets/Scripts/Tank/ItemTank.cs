using System.Collections.Generic;
using UnityEngine;

public class ItemTank : IInputTank
{
    private Dictionary<ItemType, ItemData> itemTankDictionary = new();
    private IOutPutTank outPutTank;
    private readonly float MaxTank;
    private int currentItemAmount;

    public ItemTank(IOutPutTank outPutTank, int maxTank)
    {
        this.outPutTank = outPutTank;
        this.MaxTank = maxTank;
    }

    public void InputAddTank(ItemType type)
    {
        AddItem(type);
    }

    public void InputRemoveTank(ItemType type)
    {
        RemoveItem(type);
    }

    public void AddItem(ItemType itemType)
    {
        currentItemAmount++;

        if (currentItemAmount > MaxTank)
        {
            Debug.Log("ƒAƒCƒeƒ€‚ª‚¢‚Á‚Ï‚¢‚Å‚·");
            return;
        }

        if (itemTankDictionary.TryGetValue(itemType, out ItemData vaule))
        {
            vaule.ItemAdd();
            TankCalculation(vaule);
        }
        else
        {
            ItemData itemData = new ItemData(itemType);
            itemTankDictionary.Add(itemType, itemData);
            TankCalculation(itemData);
        }
    }

    public void RemoveItem(ItemType itemType)
    {
        if (itemTankDictionary.TryGetValue(itemType, out ItemData vaule))
        {
            if (vaule.ItemAmount <= 1)
            {
                currentItemAmount--;
                itemTankDictionary.Remove(itemType);
            }
        }
        else
        {
            currentItemAmount--;
            vaule.ItemRemove();
        }
    }

    float sandVaule;
    float madVaule;
    public void TankCalculation(ItemData itemData)
    {

        float totalValue = currentItemAmount;

        float totalRatio = totalValue / MaxTank;
        float itemRatio = itemData.ItemAmount / MaxTank;

        //foreach (KeyValuePair<ItemType, ItemData> pair in itemTankDictionary)
        //{
        //    if (pair.Key == ItemType.Sand)
        //    {
        //        sandVaule = pair.Value.ItemAmount;
        //    }
        //    if (pair.Key == ItemType.Mad)
        //    {
        //        madVaule = pair.Value.ItemAmount;
        //    }
        //}
        //float totalValue = sandVaule + madVaule;
        //var totalRatio = totalValue / MaxTank;
        //var sandRatio = sandVaule / totalValue;
        //var madRatio = madVaule / totalValue;

        //var outputTank = new OutPutData(sandRatio, madRatio, totalRatio);

        var outputTank = new OutPutData(itemRatio,totalRatio,itemData.itemType);

        outPutTank.OutputTank(outputTank);

    }

}
