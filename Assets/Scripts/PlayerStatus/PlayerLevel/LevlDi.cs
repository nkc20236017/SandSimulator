using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class LevlDi : LifetimeScope
{
    [SerializeField]
    private LevelPresenter levelPresenter;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<LevelService>(Lifetime.Singleton)
            .As<IExp>();
        builder.RegisterComponent(levelPresenter)
            .As<IExpOutPut>();
    }
}
