using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

public class LoadSceneEfect : MonoBehaviour
{

    [SerializeField]
    private float fadeDuration = 0.5f;

    [SerializeField]
    private CanvasGroup canvasGroup;

    public UniTask StartTransition(CancellationToken cancellationToken)
    {
        return canvasGroup.DOFade(1f, fadeDuration)
            .ToUniTask(cancellationToken: cancellationToken);
    }

    public UniTask EndTransition(CancellationToken cancellationToken)
    {
        return canvasGroup.DOFade(0f, fadeDuration)
            .ToUniTask(cancellationToken: cancellationToken);
    }

}
