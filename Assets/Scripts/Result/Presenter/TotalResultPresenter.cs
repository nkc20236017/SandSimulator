using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TotalResultPresenter : MonoBehaviour
{
    [SerializeField]
    private GameObject uiPrefab;


    public void TotalUI(ResultOutPutData outPutData)
    {

        for (int i = 0; i < outPutData.mineralTank.Count; i++)
        {
            var resultMoney = outPutData.mineralTank[i].mineralAmount * outPutData.mineralTank[i].mineralData.price;
            var uiObject = Instantiate(uiPrefab, transform);
            uiObject.GetComponent<ResultUI>().SetUpUI(outPutData.mineralTank[i].mineralData.resultSprite
                , outPutData.mineralTank[i].mineralAmount.ToString(), resultMoney.ToString());
        }

    }


}
