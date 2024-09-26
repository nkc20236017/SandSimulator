using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPresenter : MonoBehaviour, IOutputResultUI
{



    public void ResultUI(ResultOutPutData outPutData)
    {
        Debug.Log(outPutData.mineralTotaleAmount + "ÉgÅ[É^Éã");


        for (int i = 0; i < outPutData.mineralTank.Count; i++)
        {
            Debug.Log(outPutData.mineralTank[i].mineralData.price);
        }

    }
}
