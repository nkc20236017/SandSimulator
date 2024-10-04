using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TotalResultPresenter : MonoBehaviour
{
    [SerializeField]
    private GameObject uiPrefab;
    [SerializeField]
    private Text resultText;

    public void TotalUI(ResultOutPutData outPutData)
    {
        int totaleAmount =0;
        for (int i = 0; i < outPutData.mineralTank.Count; i++)
        {
            var resultMoney = outPutData.mineralTank[i].mineralAmount * outPutData.mineralTank[i].mineralData.price;
            var uiObject = Instantiate(uiPrefab, transform);
            totaleAmount += resultMoney;
            uiObject.GetComponent<ResultUI>().SetUpUI(outPutData.mineralTank[i].mineralData.resultSprite
                , outPutData.mineralTank[i].mineralAmount.ToString(), resultMoney.ToString());
        }
        resultText.text = totaleAmount.ToString();
    }


}
