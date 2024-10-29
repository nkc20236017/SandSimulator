using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

public class TankScope : LifetimeScope
{
    [FormerlySerializedAs("MineralDataBase")] [SerializeField]
    private BlockData _mineralDataBase;
    [SerializeField]
    private SemicircleGraph ui;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_mineralDataBase);
        builder.Register<PlayerTank>(Lifetime.Singleton)
            .As<IInputTank>()
            .As<IGameLoad>();
        builder.Register<MineralDataAccess>(Lifetime.Singleton)
            .As<ITankRepository>();
        builder.RegisterComponent(ui)
            .As<IOutResultUI>();
    }
}
