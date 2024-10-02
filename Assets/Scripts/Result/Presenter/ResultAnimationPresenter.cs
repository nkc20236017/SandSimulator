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
    private Text resultText;

    public async UniTask AnimationPaper(ResultOutPutData outPutData, CancellationToken cancellationToken)
    {
        int resultPrie = 0;
        for (int i = 0; i < outPutData.mineralTank.Count; i++)
        {
            var paperObject = Instantiate(paperAnimationObject, paperTransform);
            var animationPaper = paperObject.GetComponent<ResultAnimationUI>();
            var mineral = outPutData.mineralTank[i];
            var mineralPrice = mineral.mineralData.price * mineral.mineralAmount;
            resultPrie += mineralPrice;
            animationPaper.SetUpUI(mineral.mineralData.resultSprite, mineral.mineralAmount.ToString(), mineralPrice.ToString());
            await animationPaper.AnimationEnterUI(cancellationToken);
            await animationPaper.AnimationStampUI(cancellationToken);
            await animationPaper.AnimationExitUI(cancellationToken);
        }

        resultText.text = resultPrie.ToString();
    }
}
