using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class DIResult : LifetimeScope
{
    [SerializeField]
    private ResultPresenter resultPresenter;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<ResultService>(Lifetime.Singleton)
            .As<IResultAction>();
        builder.RegisterComponent(resultPresenter)
            .As<IOutputResultUI>();
    }
}
