using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPresenter : MonoBehaviour, IOutputResultUI
{
    [SerializeField]
    private GameObject uiPrefab;

    public void ResultUI(ResultOutPutData outPutData)
    {

        for (int i = 0; i < outPutData.mineralTank.Count; i++)
        {

            var resultMoney = outPutData.mineralTank[i].mineralAmount*outPutData.mineralTank[i].mineralData.price;
            var uiObject = Instantiate(uiPrefab,transform);
            uiObject.GetComponent<ResultUI>().SetUpUII(outPutData.mineralTank[i].mineralData.sprite
                , outPutData.mineralTank[i].mineralAmount.ToString(),resultMoney.ToString());
        }

    }
}
