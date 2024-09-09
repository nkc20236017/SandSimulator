using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class ItemApplicationService : IItemInputSignal,IItemRemoveSignal
{
    private InventoryService inventoryService;
    private IItemRepository itemRepository;

    [Inject]
    public ItemApplicationService(InventoryService inventoryService, IItemRepository itemRepository)
    {
        this.inventoryService = inventoryService;
        this.itemRepository = itemRepository;
    }

    public void AddItem(string itemId)
    {
        var item = itemRepository.FindItem(itemId);
        inventoryService.AddItem(item);
    }

    public void RemoveItem(string itemId)
    {
        var item = itemRepository.FindItem(itemId);
        inventoryService.RemoveItem(item);
    }
}
