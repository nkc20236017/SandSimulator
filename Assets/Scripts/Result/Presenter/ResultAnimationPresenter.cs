using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ResultAnimationPresenter : MonoBehaviour
{
    [SerializeField]
    private GameObject paperAnimationObject;
    [SerializeField]
    private Transform paperTransform;

    public async UniTask AnimationPaper(ResultOutPutData outPutData , CancellationToken cancellationToken)
    {
        for (int i = 0; i < outPutData.mineralTank.Count; i++)
        {
            var paperObject = Instantiate(paperAnimationObject,paperTransform);
            var animationPaper = paperObject.GetComponent<ResultAnimationUI>();
            var mineral = outPutData.mineralTank[i];
            var mineralPrice = mineral.mineralData.price * mineral.mineralAmount;
            animationPaper.SetUpUI(mineral.mineralData.resultSprite, mineral.mineralAmount.ToString(), mineralPrice.ToString());
            await animationPaper.AnimationEnterUI(cancellationToken);
            await animationPaper.AnimationStampUI(cancellationToken);
            await animationPaper.AnimationExitUI(cancellationToken);
        }
    }
}
