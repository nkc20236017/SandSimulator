using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MackySoft.Navigathena.SceneManagement;
using Cysharp.Threading.Tasks;
using System;
using MackySoft.Navigathena;
using System.Threading;

public class MainGameEntoryPoint : SceneEntryPointBase
{
    private ProgressData progressData;

    protected override async UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
        var store = new ProgressDataStore<ProgressData>();
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f),cancellationToken:cancellationToken);
        progress.Report(store.SetData(progressData));
        await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: cancellationToken);
        progress.Report(store.SetData(progressData));
        await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: cancellationToken);
        progress.Report(store.SetData(progressData));
        await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: cancellationToken);
        progress.Report(store.SetData(progressData));
        await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: cancellationToken);
        progress.Report(store.SetData(progressData));
    }

    public void SetProgress(ProgressData data)
    {
        progressData = data;
    }

}
