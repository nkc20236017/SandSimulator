using UnityEngine;
using VContainer;
using VContainer.Unity;

public class DemoDI : LifetimeScope
{
    [SerializeField]
    private ShopPresenter shopPresenter;
    [SerializeField]
    private DemoPlayerDataObject demoPlayerDataObject;
    [SerializeField]
    private DemoShopDataBase demoShopDataBase;
    [SerializeField]
    private EquipDataBase equipmentDataBase;
    [SerializeField]
    private PlayerEquipDataBase playerEquipDataBase;
    [SerializeField]
    private InventoryPresenter inventoryPresenter;

    protected override void Configure(IContainerBuilder builder)
    {

        builder.Register<EquipService>(Lifetime.Singleton)
            .As<IEquip>();
        builder.Register<MyClass>(Lifetime.Singleton)
            .As<IModifiers>();
        builder.Register<ShopService>(Lifetime.Singleton)
            .As<IInputShop>();
        builder.Register<MockShopDataAccess>(Lifetime.Singleton)
            .As<IShopCommand>();
        builder.Register<DemoPlayerDataAccess>(Lifetime.Singleton)
            .As<IDemoPlayeRepository>();
        builder.Register<PlayerEquipDataAccess>(Lifetime.Singleton)
            .As<IPlayerEquipRepository>();
        builder.Register<EquipDataBaseAccess>(Lifetime.Singleton)
            .As<IEquipRepository>();
        builder.Register<ShopDataAccess>(Lifetime.Singleton)
            .As<IShopRepository>();
        builder.Register<ItemDataAccess>(Lifetime.Singleton)
            .As<IItemRepository>();
        builder.Register<InventoryService>(Lifetime.Singleton)
            .As<IInputInventory>()
            .As<IPlayerInventoryRepository>();
        builder.Register<ItemApplicationService>(Lifetime.Singleton)
            .As<IInventoryCommand>();


        builder.RegisterComponent(playerEquipDataBase);
        builder.RegisterComponent(demoPlayerDataObject);
        builder.RegisterComponent(equipmentDataBase);
        builder.RegisterComponent(demoShopDataBase);
        builder.RegisterComponent(shopPresenter)
            .As<IOutPutShop>();
        builder.RegisterComponent(inventoryPresenter)
            .As<IOutPutInventory>();

    }
}

public class MyClass : IModifiers
{
    public void AddModifiers(string id)
    {
        Debug.Log("ã≠âªÇ∑ÇÈÇ±Ç∆");
    }

    public void RemoveModifiers(string id)
    {
        Debug.Log("ã≠âªâèú");
    }
}