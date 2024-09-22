using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ShopDi : LifetimeScope
{
    [SerializeField]
    private ShopPresenter shopPresenter;
    [SerializeField]
    private DemoPlayerDataObject demoPlayerDataObject;
    [SerializeField]
    private DemoShopDataBase demoShopDataBase;
    [SerializeField]
    private EquipDataBase equipmentDataBase;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<ShopService>(Lifetime.Singleton)
            .As<IInputShop>();
        builder.Register<EquipDataBaseAccess>(Lifetime.Singleton)
            .As<IEquipRepository>();
        builder.Register<MockShopDataAccess>(Lifetime.Singleton)
            .As<IShopCommand>();
        builder.Register<DemoPlayerDataAccess>(Lifetime.Singleton)
            .As<IDemoPlayeRepository>();
        builder.RegisterComponent(demoPlayerDataObject);
        builder.RegisterComponent(equipmentDataBase);
        builder.RegisterComponent(demoShopDataBase);
        builder.RegisterComponent(shopPresenter)
            .As<IOutPutShop>();
    }
}
