using System;
using System.Threading;
using UnityEngine;

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


    private void Start()
    {
        tokenSource = new CancellationTokenSource();

        token = tokenSource.Token;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)&&!demofast)
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
