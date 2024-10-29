using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class RankingPresenter : MonoBehaviour
{
    private RankingService ranking;

    [SerializeField]
    private Transform textParent;

    [SerializeField]
    private Text[] texts;

    [SerializeField]
    private Transform endPoint;

    public bool rankingEnd;


    private void Awake()
    {
        ranking = new RankingService();
        texts = textParent.GetComponentsInChildren<Text>();
    }

    public async UniTask ShowRanking(ResultOutPutData outPutData)
    {


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
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].text = i + 1 + "E" + data.Ranks[i].ToString();
            }
        }
        
        await this.transform.DOMoveX(endPoint.position.x, 1).ToUniTask();
        rankingEnd = true;
    }
}
