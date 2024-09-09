using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ItemRemoveSignal : IStartable, IRemoveSignal
{
    public static ItemRemoveSignal Instance;
    private readonly IItemRemoveSignal inventoryRemoveSignal;

    [Inject]
    public ItemRemoveSignal(IItemRemoveSignal inventoryRemoveSignal)
    {
        this.inventoryRemoveSignal = inventoryRemoveSignal;
    }

    public void Start()
    {
        Instance = this;
    }
    public void Remove(string itemId)
    {
        inventoryRemoveSignal.RemoveItem(itemId);
    }

}
