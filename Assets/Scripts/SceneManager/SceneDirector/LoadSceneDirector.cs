using Cysharp.Threading.Tasks;
using DG.Tweening;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.Utilities;
using MackySoft.Navigathena.Transitions;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneDirector : ITransitionDirector
{
    private ISceneIdentifier loadSceneIdentifier;

    public LoadSceneDirector(ISceneIdentifier loadSceneIdentifier)
    {
        this.loadSceneIdentifier = loadSceneIdentifier;
    }

    public ITransitionHandle CreateHandle()
    {
        return new LoadSceneHandle(loadSceneIdentifier);
    }

    class LoadSceneHandle : ITransitionHandle , IProgress<IProgressDataStore>
    {
        private readonly ISceneIdentifier loadSceneIdentifier;

        private ISceneHandle loadSceneHandle;

        private LoadSceneEfectData loadSceneEfect;

        public LoadSceneHandle(ISceneIdentifier loadSceneIdentifier)
        {
            this.loadSceneIdentifier = loadSceneIdentifier;
        }


        public async UniTask Start(CancellationToken cancellation = default)
        {
            loadSceneHandle = loadSceneIdentifier.CreateHandle();
            Scene scene = await loadSceneHandle.Load();
            if (!scene.TryGetComponentInScene(out loadSceneEfect, true))
            {
                throw new InvalidOperationException($"Scene '{scene.name}' does not have a {nameof(LoadSceneEfectData)} component.");
            }
            await loadSceneEfect.CanvasGroup.DOFade(1,0.5f)
                .ToUniTask(cancellationToken: cancellation);

        }

        public async UniTask End(CancellationToken cancellation = default)
        {
            await UniTask.WaitUntil(() => loadSceneEfect.Slider.value ==1f);
            
            await loadSceneEfect.CanvasGroup
                .DOFade(0,0.5f)
                .ToUniTask(cancellationToken: cancellation);
            await loadSceneHandle.Unload();

            loadSceneEfect = null;
            loadSceneHandle = null;

        }

        void IProgress<IProgressDataStore>.Report(IProgressDataStore value)
        {
            if(value.TryGetData(out ProgressData data))
            {
                loadSceneEfect.ProglesMesage.text = data.ProgressMesage;
                loadSceneEfect.ProglesText.text = data.ProgressText;
                loadSceneEfect.Slider.value = data.Progress;

            }
        }
    }
}
