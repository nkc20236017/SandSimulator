using Cysharp.Threading.Tasks;
using MackySoft.Navigathena.SceneManagement.Utilities;
using MackySoft.Navigathena.SceneManagement.VContainer;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using VContainer.Unity;

public class RootSceneEntoryPoint : ScopedSceneEntryPoint
{


    const string kRootSceneName = "RootScene";

    protected override async UniTask<LifetimeScope> EnsureParentScope(CancellationToken cancellationToken)
    {
        // Load root scene.
        if (!SceneManager.GetSceneByName(kRootSceneName).isLoaded)
        {
            await SceneManager.LoadSceneAsync(kRootSceneName, LoadSceneMode.Additive)
            .ToUniTask(cancellationToken: cancellationToken);
        }

        Scene rootScene = SceneManager.GetSceneByName(kRootSceneName);

#if UNITY_EDITOR
        // Reorder root scene.
        EditorSceneManager.MoveSceneBefore(rootScene, gameObject.scene);
#endif

        // Build root LifetimeScope container.
        if (rootScene.TryGetComponentInScene(out LifetimeScope rootLifetimeScope, true) && rootLifetimeScope.Container == null)
        {
            await UniTask.RunOnThreadPool(() => rootLifetimeScope.Build(), cancellationToken: cancellationToken);
        }
        return rootLifetimeScope;
    }
}
