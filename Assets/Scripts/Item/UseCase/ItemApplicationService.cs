using UnityEngine;
using VContainer;

public class ItemApplicationService : IInventoryCommand ,IEquipCommand ,IShopCommand
{
    private IItemRepository itemRepository;
    private IInputInventory inventoryService;
    private IEquip equipService;
    private IInputShop inputShop;

    [Inject]
    public ItemApplicationService
        (
        IItemRepository itemRepository, IInputInventory inventoryService
, IEquip equipService, IInputShop inputShop

        )
    {
        this.itemRepository = itemRepository;
        this.inventoryService = inventoryService;
        this.equipService = equipService;
        this.inputShop = inputShop;
    }

    public void AddCommand(string id)
    {
        var item = itemRepository.FindEquipData(id);
        inventoryService.AddToInventory(item);
    }

    public void Equip(string id)
    {
        var item = itemRepository.FindEquipData(id);
        equipService.Equip(item);
    }

    public void RemoveCommand(string id)
    {
        var item = itemRepository.FindEquipData(id);
        inventoryService.RemoveFromInventory(item);
    }

    public void ShopBuy(string id)
    {
        var item = itemRepository.FindEquipData(id);
        inputShop.ShopBuy(item);
    }

    public void UnEquip(string id)
    {
        var item = itemRepository.FindEquipData(id);
        equipService.UnEquip(item);
    }
}
