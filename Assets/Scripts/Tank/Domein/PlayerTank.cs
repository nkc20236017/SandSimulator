using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;

public class PlayerTank : IInputTank ,IGameLoad
{
    private int selectIndex;
    private ITankRepository tankRepository;
    private ExitGate exitGate;
    private Dictionary<Block, MineralTank> itemTankDictionary = new();
    private IOutResultUI outPutTank;
    private readonly float MaxTank = 4000;
    private int currentItemAmount =1;
    private BlockType currentBlockType;
    private bool maxSignal;

    [Inject]
    public PlayerTank(IOutResultUI outPutTank, ITankRepository tankRepository)
    {
        this.outPutTank = outPutTank;
        this.tankRepository = tankRepository;
    }

    public void InputAddTank(TileBase tileBase)
    {
        var mineralItem = tankRepository.Find(tileBase);
        AddItem(mineralItem);
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
            Debug.Log("�A�C�e���������ς��ł�");
            maxSignal = true;
            return ;
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
        maxSignal = false;
    }

    public void RemoveItem(Block mineralData)
    {
        if (itemTankDictionary.TryGetValue(mineralData, out MineralTank vaule))
        {
            if (vaule.mineralAmount <= 1)
            {
                Debug.Log("タンクを削除");
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

        var outputTank = new OutPutTankData(itemRatio, totalRatio, itemData.mineralData.type
            , itemData.mineralData.sprite);
        outPutTank.OutputTank(outputTank);
    }

    public bool FiringTank()
    {
        var block = tankRepository.Find(currentBlockType);
        RemoveItem(block);

        var item = itemTankDictionary.
            Where(item => item.Key.type == block.type).
            FirstOrDefault();

        if (item.Key != null)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void SelectTank(int select)
    {
        int selectIndex = 0;

        select = Mathf.Clamp(select, 0, itemTankDictionary.Keys.Count);

        foreach(KeyValuePair<Block,MineralTank> pair in itemTankDictionary)
        {
            selectIndex++;
            if(select == selectIndex)
            {
                this.currentBlockType = pair.Key.type;
                outPutTank.OutputSelectTank(new OutPutSelectData(currentBlockType));
            }
        }

    }

    public bool TamkMaxSignal()
    {
        return maxSignal;
    }

    public void GameLoad()
    {
        exitGate = new(itemTankDictionary);
        exitGate.ResultScene();
    }

    public void LeftSelectTank()
    {
        selectIndex = Mathf.Clamp(selectIndex,1,itemTankDictionary.Keys.Count);
        selectIndex--;
        SelectTank(selectIndex);
        Debug.Log(selectIndex);
    }

    public void RightSelectTank()
    {
        selectIndex = Mathf.Clamp(selectIndex,1,itemTankDictionary.Keys.Count);
        selectIndex++;
        SelectTank(selectIndex);
        Debug.Log(selectIndex);
    }
}
