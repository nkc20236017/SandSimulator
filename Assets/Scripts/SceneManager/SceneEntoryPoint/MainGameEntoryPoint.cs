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
    protected override async UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
        var store = new ProgressDataStore<ProgressData>();
        progress.Report(store.SetData(new ProgressData(0f, "ÉçÅ[ÉhíÜ", "0%")));
        await UniTask.Delay(TimeSpan.FromSeconds(1.2f));
        progress.Report(store.SetData(new ProgressData(1f, "äÆóπ", "100%")));

    }
}
