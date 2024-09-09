using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class DemoDI : LifetimeScope
{
    [SerializeField]
    private InventoryPresenter inventoryPresenter;

    [SerializeField]
    private ItemDataBase inventoryData;

    [SerializeField]
    private InventoryDataBase inventoryDataBase;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<InventoryService>(Lifetime.Singleton)
            .As<IInventoryInputSignal>()
            .As<IInventoryRemoveSignal>();
        builder.RegisterEntryPoint<ItemPickupSignal>(Lifetime.Singleton)
            .As<IPickupSignal>();
        builder.Register<DataAccess>(Lifetime.Singleton)
            .As<IItemRepository>();
        builder.Register<InventoryDataAccess>(Lifetime.Singleton)
            .As<IInventoryRepository>();
        builder.RegisterEntryPoint<ItemRemoveSignal>(Lifetime.Singleton)
            .As<IRemoveSignal>();

        builder.RegisterComponent(inventoryData);
        builder.RegisterComponent(inventoryDataBase);
        builder.RegisterComponent(inventoryPresenter)
            .As<IOutPutInventoryUI>();
    }
}
