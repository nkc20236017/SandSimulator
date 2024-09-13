using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class PlayerTank : IInputTank
{
    private ITankRepository tankRepository;
    private Dictionary<Block, MineralTank> itemTankDictionary = new();
    private IOutPutTank outPutTank;
    private readonly float MaxTank =4000;
    private int currentItemAmount;

    [Inject]
    public PlayerTank(IOutPutTank outPutTank,  ITankRepository tankRepository)
    {
        this.outPutTank = outPutTank;
        this.tankRepository = tankRepository;
    }

    public void InputAddTank(BlockType type)
    {
        var mineralItem = tankRepository.Find(type);
        AddItem(mineralItem);
    }

    public void InputRemoveTank(BlockType type)
    {
        var mineralItem = tankRepository.Find(type);
        RemoveItem(mineralItem);
    }

    public void AddItem(Block mineralData)
    {

        if (currentItemAmount >= MaxTank)
        {
            Debug.Log("アイテムがいっぱいです");
            return;
        }
        currentItemAmount++;
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

    public void RemoveItem(Block mineralData)
    {
        if (itemTankDictionary.TryGetValue(mineralData, out MineralTank vaule))
        {
            if (vaule.mineralAmount <= 1)
            {
                Debug.Log("アイテムリストから削除");
                itemTankDictionary.Remove(mineralData);
                currentItemAmount--;
            }
            else
            {
                vaule.MineralRemove();
                currentItemAmount--;
            }
            TankCalculation(vaule);
        }

    }

    public void TankCalculation(MineralTank itemData)
    {

        float totalValue = currentItemAmount;

        float totalRatio = totalValue / MaxTank;
        float itemRatio = itemData.mineralAmount / totalValue;

        var outputTank = new OutPutData(itemRatio, totalRatio, itemData.mineralData.type
            ,itemData.mineralData.sprite);
        Debug.Log(totalValue);
        outPutTank.OutputTank(outputTank);
    }

}
