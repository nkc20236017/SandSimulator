using UnityEngine;
using VContainer;
using VContainer.Unity;

public class TankScope : LifetimeScope
{
    [SerializeField]
    private BlockDatas MineralDataBase;
    [SerializeField]
    private SemicircleGraph ui;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(MineralDataBase);
        builder.Register<PlayerTank>(Lifetime.Singleton)
            .As<IInputTank>()
            .As<IGameLoad>();
        builder.Register<MineralDataAccess>(Lifetime.Singleton)
            .As<ITankRepository>();
        builder.RegisterComponent(ui)
            .As<IOutResultUI>();
    }
}
