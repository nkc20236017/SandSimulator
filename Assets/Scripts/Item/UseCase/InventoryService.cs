using System.Collections.Generic;
using VContainer;

public class InventoryService : IInputInventory , IPlayerInventoryRepository
{
    private List<EquipData> InventoryList = new();
    private IOutPutInventory outPutInventory;

    [Inject]
    public InventoryService(IOutPutInventory outPutInventory)
    {
        this.outPutInventory = outPutInventory;
    }

    public void AddToInventory(EquipData equipData)
    {
        InventoryList.Add(equipData);
        outPutInventory.OutPutUI(new InventoryOutputData(FindAll()));
    }

    public List<EquipData> FindAll()
    {
        return InventoryList;
    }

    public void RemoveFromInventory(EquipData equipData)
    {
        InventoryList.Remove(equipData);
        outPutInventory.OutPutUI(new InventoryOutputData(FindAll()));
    }
}
