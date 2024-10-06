using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;

public class PlayerTank : IInputTank, IGameLoad
{
    private int selectIndex;
    private ITankRepository tankRepository;
    private ExitGate exitGate;
    private Dictionary<Block, MineralTank> itemTankDictionary = new();
    private IOutResultUI outPutTank;
    private readonly float MaxTank = 4000;
    private int currentItemAmount = 1;
    private BlockType currentBlockType;
    private bool maxSignal;

    private bool fast;

    [Inject]
    public PlayerTank(IOutResultUI outPutTank, ITankRepository tankRepository)
    {
        this.outPutTank = outPutTank;
        this.tankRepository = tankRepository;
    }

    public void InputAddTank(TileBase tileBase)
    {
        var mineralItem = tankRepository.Find(tileBase);
        for (int i = 0; i < mineralItem.vacuumAmount; i++)
        {
            AddItem(mineralItem);
        }
    }

    public void InputAddTank(BlockType type)
    {
        var mineralItem = tankRepository.Find(type);
        for (int i = 0; i < mineralItem.vacuumAmount; i++)
        {
            AddItem(mineralItem);
        }
    }

    public void RemoveTank()
    {
        var block = tankRepository.Find(currentBlockType);
        if (block == null) { return; }

        for (int i = 0; i < block.vacuumAmount; i++)
        {
            RemoveItem(block);
        }

    }

    public void AddItem(Block mineralData)
    {
        if (currentItemAmount >= MaxTank)
        {
            maxSignal = true;
            return;
        }
        //maxSignalを修正
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

        if (fast == false)
        {
            currentBlockType = mineralData.type;
            outPutTank.OutputSelectTank(new(mineralData.type, mineralData.resultSprite));
            fast = true;
        }
    }

    public void RemoveItem(Block mineralData)
    {
        if (currentItemAmount <= MaxTank)
        {
            maxSignal = false;
        }

        if (itemTankDictionary.TryGetValue(mineralData, out MineralTank vaule))
        {
            if (vaule.mineralAmount <= 1)
            {
                itemTankDictionary.Remove(mineralData);
                currentItemAmount--;
                SelectTank(itemTankDictionary.Keys.Count);
                if (itemTankDictionary.Keys.Count <= 0)
                {
                    outPutTank.OutputSelectTank(new(BlockType.None, null));
                    currentBlockType = BlockType.None;
                    fast = false;
                }
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

        foreach (KeyValuePair<Block, MineralTank> pair in itemTankDictionary)
        {
            selectIndex++;
            if (select == selectIndex)
            {
                this.currentBlockType = pair.Key.type;
                outPutTank.OutputSelectTank(new OutPutSelectData(currentBlockType, pair.Key.resultSprite));
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
        selectIndex = Mathf.Clamp(selectIndex, 0, itemTankDictionary.Keys.Count);
        selectIndex--;
        if(selectIndex ==  0)
        {
            selectIndex = itemTankDictionary.Keys.Count;
        }
        SelectTank(selectIndex);
    }

    public void RightSelectTank()
    {
        selectIndex = Mathf.Clamp(selectIndex, 1, itemTankDictionary.Keys.Count+1);
        selectIndex++;

        if(selectIndex > itemTankDictionary.Keys.Count)
        {
            selectIndex = 1;
        }
        SelectTank(selectIndex);
    }

    public BlockType GetSelectType()
    {
        return currentBlockType;
    }

}
