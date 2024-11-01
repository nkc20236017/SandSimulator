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

    [SerializeField]
    private float waiteTime;

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
        await UniTask.Delay(TimeSpan.FromSeconds(waiteTime), cancellationToken: cancellationToken);
        intensity.enabled = false;
        videoPlayer.Play();
    }

    private async UniTask VideoStop()
    {
        videoPlayer.Stop();
        intensity.enabled = true;

        await VideoPlayer();

    }

    public void LoopPointReached(VideoPlayer vp)
    {
        VideoStop().Forget();
    }



}
