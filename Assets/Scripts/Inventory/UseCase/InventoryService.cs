using System.Collections.Generic;
using VContainer;

public class InventoryService 
{
    private IOutPutInventoryUI inventoryUI;
    private IInventoryRepository inventoryRepository;

    [Inject]
    public InventoryService(IOutPutInventoryUI inventoryUI,
        IInventoryRepository inventoryRepository)
    {
        this.inventoryUI = inventoryUI;
        this.inventoryRepository = inventoryRepository;
    }

    public void AddItem(ItemData itemData)
    {
        inventoryRepository.AddToInventory(itemData);
        inventoryUI.OutPut();
    }

    public void RemoveItem(ItemData itemData)
    {
        inventoryRepository.RemoveFromInventory(itemData);
        inventoryUI.OutPut();
    }
}
