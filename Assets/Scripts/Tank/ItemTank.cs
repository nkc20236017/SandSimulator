using System.Collections.Generic;
using UnityEngine;

public class ItemTank : IInputTank
{
    private ITankRepository tankRepository;
    private Dictionary<MineralData, MineralTank> itemTankDictionary = new();
    private IOutPutTank outPutTank;
    private readonly float MaxTank;
    private int currentItemAmount;

    public ItemTank(IOutPutTank outPutTank, int maxTank, ITankRepository tankRepository)
    {
        this.outPutTank = outPutTank;
        this.MaxTank = maxTank;
        this.tankRepository = tankRepository;
    }

    public void InputAddTank(ItemType type)
    {
        currentItemAmount++;
        var mineralItem = tankRepository.Find(type);
        AddItem(mineralItem);
    }

    public void InputRemoveTank(ItemType type)
    {
        var mineralItem = tankRepository.Find(type);
        RemoveItem(mineralItem);
    }

    public void AddItem(MineralData mineralData)
    {

        if (currentItemAmount > MaxTank)
        {
            Debug.Log("アイテムがいっぱいです");
            return;
        }

        if (itemTankDictionary.TryGetValue(mineralData, out MineralTank vaule))
        {
            vaule.MineralAdd();
            TankCalculation(vaule);
        }
        else
        {
            MineralTank itemData = new MineralTank(mineralData);
            itemTankDictionary.Add(mineralData, itemData);
            TankCalculation(itemData);
        }
    }

    public void RemoveItem(MineralData mineralData)
    {
        if (itemTankDictionary.TryGetValue(mineralData, out MineralTank vaule))
        {
            if (vaule.mineralAmount <= 1)
            {
                Debug.Log("アイテムリストから削除");
                itemTankDictionary.Remove(mineralData);
            }
            else
            {
                vaule.MineralRemove();
            }
            TankCalculation(vaule);
        currentItemAmount--;
        }

    }

    public void TankCalculation(MineralTank itemData)
    {

        float totalValue = currentItemAmount;

        float totalRatio = totalValue / MaxTank;
        float itemRatio = itemData.mineralAmount / totalValue;

        var outputTank = new OutPutData(itemRatio, totalRatio, itemData.mineralData.itemType
            ,itemData.mineralData.sprite);

        outPutTank.OutputTank(outputTank);

        Debug.Log("アイテムの量"+totalValue);

    }

}
