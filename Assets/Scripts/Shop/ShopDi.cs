using System.Collections;
using System.Collections.Generic;
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
    private EquipmentDataBase equipmentDataBase;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<DemoShopService>(Lifetime.Singleton)
            .As<IInputShop>();
        //builder.Register<MockEquipDataAccess>(Lifetime.Singleton)
        //    .As<IEquipRepository>();
        builder.Register<MockShopDataAccess>(Lifetime.Singleton)
            .As<IShopCommand>();
        builder.Register<MockPlayerDataAccess>(Lifetime.Singleton)
            .As<IDemoPlayeRepository>();
        builder.RegisterComponent(demoPlayerDataObject);
        builder.RegisterComponent(equipmentDataBase);
        builder.RegisterComponent(demoShopDataBase);
        builder.RegisterComponent(shopPresenter)
            .As<IOutPutShop>();
    }
}
