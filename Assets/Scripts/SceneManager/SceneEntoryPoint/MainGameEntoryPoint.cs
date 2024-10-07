using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MackySoft.Navigathena.SceneManagement;
using Cysharp.Threading.Tasks;
using System;
using MackySoft.Navigathena;
using System.Threading;
using UniRx;

public class MainGameEntoryPoint : SceneEntryPointBase
{
    private ReactiveProperty<ProgressData> progressData = new();

    protected override async UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
        var store = new ProgressDataStore<ProgressData>();

        await UniTask.Delay(1,cancellationToken: cancellationToken);

        this.ObserveEveryValueChanged(progress => progressData.Value.Progress)
            .Subscribe(progressData =>
            {
                Debug.Log(progressData);
                progress.Report(store.SetData(this.progressData.Value));
            });

    }

    public void SetProgress(ProgressData data)
    {
        progressData.Value = data;
        Debug.Log(progressData.Value.Progress);
    }

}
