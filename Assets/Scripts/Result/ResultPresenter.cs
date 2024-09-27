using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPresenter : MonoBehaviour, IOutputResultUI
{
    [SerializeField]
    private GameObject uiPrefab;

    [SerializeField]
    private Text totaleMoney;

    public void ResultUI(ResultOutPutData outPutData)
    {

        for (int i = 0; i < outPutData.mineralTank.Count; i++)
        {

            var resultMoney = outPutData.mineralTank[i].mineralAmount*outPutData.mineralTank[i].mineralData.price;
            var uiObject = Instantiate(uiPrefab,transform);
            uiObject.GetComponent<ResultUI>().SetUpUII(outPutData.mineralTank[i].mineralData.sprite
                , outPutData.mineralTank[i].mineralAmount.ToString(),resultMoney.ToString());
        }

        totaleMoney.text = outPutData.mineralTotaleAmount.ToString();

    }
}
