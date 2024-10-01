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

    private CancellationToken token;
    private CancellationTokenSource tokenSource;


    private void Start()
    {
        tokenSource = new CancellationTokenSource();

        token = tokenSource.Token;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
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
        }
        animationPresenter.gameObject.SetActive(false);
        resultPresenter.gameObject.SetActive(true);
        sceneButton.SetActive(true);
    }
}
