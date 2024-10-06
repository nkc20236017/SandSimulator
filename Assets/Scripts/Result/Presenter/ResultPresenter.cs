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
        playerActions.UI.UISelect.performed += OnUiClick;
        playerActions.Enable();
        token = tokenSource.Token;
    }


    private void OnDisable()
    {
        playerActions.Disable();
    }

    private void OnUiClick(InputAction.CallbackContext context)
    {
        if(!demofast)
        {
            demofast = true;
            tokenSource.Cancel();
        }
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
