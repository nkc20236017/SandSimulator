using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SelectEntoryPoint : SceneEntryPointBase
{
    protected override async UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
        var store = new ProgressDataStore<ProgressData>();
        progress.Report(store.SetData(new ProgressData(0f,"ÉçÅ[ÉhíÜ","0%")));
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        progress.Report(store.SetData(new ProgressData(1f, "äÆóπ", "100%")));

    }
}
