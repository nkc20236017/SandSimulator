using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    private RankingPresenter rankingPresenter;
    [SerializeField]
    private GameObject sceneButton;
    [SerializeField]
    private GameObject totalePrise;

    [SerializeField]
    private GameObject resultObject;
    [SerializeField]
    private Transform endPoint;

    //ˆêŽž“I‚È
    private bool demofast;
    private CancellationToken token;
    private CancellationToken token2;
    private CancellationTokenSource tokenSource;
    private CancellationTokenSource tokenSource2;
    private PlayerActions playerActions;
    private ResultOutPutData resultOutPutData;


    private void Start()
    {
        tokenSource = new CancellationTokenSource();
        tokenSource2 = new CancellationTokenSource();
        token2 = tokenSource2.Token;
        playerActions = new PlayerActions();
        token = tokenSource.Token;
        playerActions.UI.UISelect.started += OnUISelect;
        playerActions.Enable();
    }

    private void OnDisable()
    {
        tokenSource2.Cancel();
        playerActions.Disable();
    }

    private void OnUISelect(InputAction.CallbackContext context)
    {
        tokenSource.Cancel();
        if (!demofast) { return; }
        NextRanking();
        if (!rankingPresenter.rankingEnd) { return; }
        NextScene();
    }

    public async void ResultUI(ResultOutPutData outPutData)
    {
        resultOutPutData = outPutData;
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
            totalePrise.SetActive(true);
            await UniTask.Delay(1,cancellationToken : token2);
            demofast = true;
        }
        AudioManager.Instance.StopBGM("Coinloop");
        AudioManager.Instance.PlaySFX("CoinSE");
        animationPresenter.gameObject.SetActive(false);
        resultPresenter.gameObject.SetActive(true);
        totalePrise.SetActive(true);
    }

    private async void NextRanking()
    {
        resultObject.transform.DOMoveX(endPoint.position.x, 1);
        await rankingPresenter.ShowRanking(resultOutPutData);
        sceneButton.SetActive(true);

    }

    private void NextScene()
    {
        sceneButton.GetComponent<ResultSceneButton>().SceneLoad();
    }

    private void OnDestroy()
    {
        tokenSource.Cancel();
    }

}
