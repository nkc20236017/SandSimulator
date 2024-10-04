using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using System;
using System.Threading;
using VContainer;

public class ResultEntoryPoint : SceneEntryPointBase
{
    private IResultAction resultAction;

    [Inject]
    public void Inject(IResultAction resultAction)
    {
        this.resultAction = resultAction;
    }

    protected override async UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
        var store = new ProgressDataStore<ProgressData>();
        progress.Report(store.SetData(new ProgressData(0f, "ÉçÅ[ÉhíÜ", "0%")));
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        progress.Report(store.SetData(new ProgressData(1f, "äÆóπ", "100%")));
    }

    protected override UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
    {

        if (reader.TryRead(out SceneResultData resultData))
        {
            resultAction.ResultStart(resultData.result);
        }

        return base.OnEnter(reader, cancellationToken);
    }

}
