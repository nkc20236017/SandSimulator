using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class RankingPresenter : MonoBehaviour
{
    private RankingService ranking;

    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

    [SerializeField]
    private Transform textParent;

    [SerializeField]
    private Text[] texts;

    [SerializeField]
    private Transform endPoint;

    public bool rankingEnd;


    private void Awake()
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
        
        ranking = new RankingService();
        texts = textParent.GetComponentsInChildren<Text>();
    }

    private void OnDestroy()
    {
        cancellationTokenSource.Cancel();
    }

    public async UniTask ShowRanking(ResultOutPutData outPutData)
    {
        if (rankingEnd) { return; }

        int totaleAmount = 0;
        for (int i = 0; i < outPutData.mineralTank.Count; i++)
        {
            var resultMoney = outPutData.mineralTank[i].mineralAmount * outPutData.mineralTank[i].mineralData.price;
            totaleAmount += resultMoney;
        }

        ranking.ShowRanking(totaleAmount);

        var data = ranking.GetRank();
        if (data.Ranks.Count > 5)
        {
            for (int i = 0; i < 5; i++)
            {
                texts[i].text = i+1+"E"+data.Ranks[i].ToString();
            }
        }
        else
        {
            for (int i = 0; i < data.Ranks.Count; i++)
            {
                texts[i].text = i + 1 + "E" + data.Ranks[i].ToString();
            }
        }
        
        await this.transform.DOMoveX(endPoint.position.x, 1).ToUniTask(cancellationToken:cancellationToken);
        rankingEnd = true;
    }
}
