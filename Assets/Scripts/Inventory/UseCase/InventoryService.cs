using System.Collections.Generic;
using VContainer;

public class InventoryService : IInventoryInputSignal , IInventoryRemoveSignal
{
    private IItemRepository itemRepository;
    private IOutPutInventoryUI inventoryUI;
    private IInventoryRepository inventoryRepository;

    [Inject]
    public InventoryService(IItemRepository itemRepository, IOutPutInventoryUI inventoryUI,
        IInventoryRepository inventoryRepository)
    {
        this.itemRepository = itemRepository;
        this.inventoryUI = inventoryUI;
        this.inventoryRepository = inventoryRepository;
    }

    public void AddItem(string itemId)
    {
        var item = itemRepository.FindItem(itemId);
        inventoryRepository.AddToInventory(item);
        OutputUI(itemId);
    }

    public void RemoveItem(string itemId)
    {
        var item = itemRepository.FindItem(itemId);
        inventoryRepository.RemoveFromInventory(item);
        OutputUI(itemId);
    }

    private void OutputUI(string itemId)
    {
        inventoryUI.OutPut(itemId);
    }

}
