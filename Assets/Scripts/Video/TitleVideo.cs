using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleVideo : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;
    [SerializeField]
    private PostProcessLayer intensity;

    [SerializeField]
    private RawImage rawImage;

    private InputAction inputAction;

    private CancellationTokenSource cancellationTokenSource;

    private CancellationToken cancellationToken;

    private void Awake()
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
    }

    private void OnDestroy()
    {
        cancellationTokenSource.Cancel();
    }

    private async void Start()
    {

        videoPlayer.loopPointReached += LoopPointReached;

        inputAction = new("Any", InputActionType.Button, "<Gamepad>/*");

        inputAction.performed += OnAny;

        inputAction.Enable();



        await VideoPlayer();
    }

    private void OnAny(InputAction.CallbackContext callbackContext)
    {
        VideoStop().Forget();
    }

    private async UniTask VideoPlayer()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(60), cancellationToken: cancellationToken);
        intensity.enabled = false;
        videoPlayer.Play();
        await rawImage.DOFade(1, 1).ToUniTask(cancellationToken: cancellationToken);
    }

    private async UniTask VideoStop()
    {
        videoPlayer.Stop();
        intensity.enabled = true;
        await rawImage.DOFade(0, 1).ToUniTask(cancellationToken: cancellationToken);

        await VideoPlayer();

    }

    public void LoopPointReached(VideoPlayer vp)
    {
        VideoStop().Forget();
    }



}
