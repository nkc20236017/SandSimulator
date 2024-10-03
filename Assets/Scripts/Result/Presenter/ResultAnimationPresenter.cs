using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ResultAnimationPresenter : MonoBehaviour
{
    [SerializeField]
    private GameObject paperAnimationObject;
    [SerializeField]
    private Transform paperTransform;
    [SerializeField]
    private GameObject resultObject;
    [SerializeField]
    private Text resultText;



    public async UniTask AnimationPaper(ResultOutPutData outPutData, CancellationToken cancellationToken)
    {
        int resultPrice = 0;
        for (int i = 0; i < outPutData.mineralTank.Count; i++)
        {
            AudioManager.Instance.PlaySFX("PaperSE");
            var paperObject = Instantiate(paperAnimationObject, paperTransform);
            var animationPaper = paperObject.GetComponent<ResultAnimationUI>();
            var mineral = outPutData.mineralTank[i];
            var mineralPrice = mineral.mineralData.price * mineral.mineralAmount;
            resultPrice += mineralPrice;
            animationPaper.SetUpUI(mineral.mineralData.resultSprite, mineral.mineralAmount.ToString(), mineralPrice.ToString());
            await animationPaper.AnimationEnterUI(cancellationToken);
            await animationPaper.AnimationStampUI(cancellationToken);
            AudioManager.Instance.PlaySFX("StampNormalSE");
            await animationPaper.AnimationExitUI(cancellationToken);
        }
        await DOVirtual.Int
    (
    from: 0,
    to: resultPrice,
    duration: 1,
    onVirtualUpdate: (totalePrice) =>
    {
        resultObject.SetActive(true);
        resultText.text = totalePrice.ToString();
    }
    ).ToUniTask(cancellationToken:cancellationToken);
    }
}
