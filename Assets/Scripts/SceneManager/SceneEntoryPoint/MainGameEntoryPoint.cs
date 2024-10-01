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

    protected override UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
        var store = new ProgressDataStore<ProgressData>();
        progress.Report(store.SetData(progressData));
        return base.OnInitialize(reader, progress, cancellationToken);
    }

    public void SetProgress(ProgressData data)
    {
        progressData = data;
    }

}
