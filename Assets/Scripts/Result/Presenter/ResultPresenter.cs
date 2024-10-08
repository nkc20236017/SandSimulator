using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResultPresenter : MonoBehaviour, IOutputResultUI
{

    [SerializeField]
    private TotalResultPresenter resultPresenter;
    [SerializeField]
    private ResultAnimationPresenter animationPresenter;
    [SerializeField]
    private GameObject sceneButton;
    [SerializeField]
    private GameObject totalePrise;
    //ˆêŽž“I‚È
    private bool demofast;
    private CancellationToken token;
    private CancellationTokenSource tokenSource;
    private PlayerActions playerActions;


    private void Start()
    {
        tokenSource = new CancellationTokenSource();
        playerActions = new PlayerActions();
        token = tokenSource.Token;
        playerActions.UI.UISelect.performed += OnUISelect;
        playerActions.Enable();
    }

    private void OnDisable()
    {
        playerActions.Disable();
    }

    private void OnUISelect(InputAction.CallbackContext context)
    {
        if (demofast) { return; }
        demofast = true;
        tokenSource.Cancel();

    }

    public async void ResultUI(ResultOutPutData outPutData)
    {
        resultPresenter.gameObject.SetActive(false);
        resultPresenter.TotalUI(outPutData);
        try
        {
            await animationPresenter.AnimationPaper(outPutData, token);
        }
        catch (OperationCanceledException)
        {
            animationPresenter.gameObject.SetActive(false);
            resultPresenter.gameObject.SetActive(true);
            sceneButton.SetActive(true);
            totalePrise.SetActive(true);
        }
        AudioManager.Instance.StopBGM("Coinloop");
        AudioManager.Instance.PlaySFX("CoinSE");
        animationPresenter.gameObject.SetActive(false);
        resultPresenter.gameObject.SetActive(true);
        sceneButton.SetActive(true);
        totalePrise.SetActive(true);
    }

    private void OnDestroy()
    {
        tokenSource.Cancel();
    }

}
