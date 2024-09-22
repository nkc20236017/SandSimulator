using VContainer;

public class ItemApplicationService : IInventoryCommand
{
    private IItemRepository itemRepository;
    private IInputInventory inventoryService;

    [Inject]
    public ItemApplicationService
        (
        IItemRepository itemRepository, IInputInventory inventoryService
        )
    {
        this.itemRepository = itemRepository;
        this.inventoryService = inventoryService;
    }

    public void AddCommand(string id)
    {
        var item = itemRepository.FindEquipData(id);
        inventoryService.AddToInventory(item);
    }

    public void RemoveCommand(string id)
    {
        var item = itemRepository.FindEquipData(id);
        inventoryService.RemoveFromInventory(item);
    }
}
