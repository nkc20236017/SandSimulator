using UnityEngine;
using VContainer;
using VContainer.Unity;

public class TankScope : LifetimeScope
{
    [SerializeField]
    private BlockDatas MineralDataBase;
    [SerializeField]
    private TankUI ui;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(MineralDataBase);
        builder.Register<PlayerTank>(Lifetime.Singleton)
            .As<IInputTank>();
        builder.Register<MineralDataAccess>(Lifetime.Singleton)
            .As<ITankRepository>();
        builder.RegisterComponent(ui)
            .As<IOutPutTank>();
    }
}