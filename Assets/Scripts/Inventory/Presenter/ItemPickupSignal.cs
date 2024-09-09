using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ItemPickupSignal : IPickupSignal,IStartable
{
    public static ItemPickupSignal Instance;

    private readonly IInventoryInputSignal inventoryInputSignal;

    [Inject]
    public ItemPickupSignal(IInventoryInputSignal inventoryInputSignal)
    {
        this.inventoryInputSignal = inventoryInputSignal;
    }

    public void PickupSignal(string itemId)
    {
        inventoryInputSignal.AddItem(itemId);
    }

    public void Start()
    {
        Instance = this;
    }
}
