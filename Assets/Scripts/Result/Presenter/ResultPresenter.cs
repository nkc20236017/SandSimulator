using System;
using System.Threading;
using UnityEngine;

public class ResultPresenter : MonoBehaviour, IOutputResultUI
{

    [SerializeField]
    private TotalResultPresenter resultPresenter;
    [SerializeField]
    private ResultAnimationPresenter animationPresenter;

    private CancellationToken token;

    private void Start()
    {
        var cts = new CancellationTokenSource();

        // 2. CancellationToken‚ðŽæ“¾  
        token = cts.Token;
        cts.Cancel();
    }

    public async void ResultUI(ResultOutPutData outPutData)
    {


        resultPresenter.gameObject.SetActive(false);
        resultPresenter.TotalUI(outPutData);
        try
        {
            await animationPresenter.AnimationPaper(outPutData, token);
        }
        catch(OperationCanceledException)
        {
            animationPresenter.gameObject.SetActive(false);
            resultPresenter.gameObject.SetActive(true);
        }
        animationPresenter.gameObject.SetActive(false);
        resultPresenter.gameObject.SetActive(true);
    }
}
