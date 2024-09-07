using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System;

public class TankTest 
{
    [Test]
    public void Tank()
    {
        IOutPutTank tankOutPut = new TankUITest();
        IInputTank inputTank = new ItemTank(tankOutPut , 100);
        
        for (int i = 0; i < 1; i++)
        {
            inputTank.InputAddTank(ItemType.Sand);
            inputTank.InputAddTank(ItemType.Mad);
        }

    }
}

public class TankOutPutMock : IOutPutTank
{
    public void OutputTank(OutPutData outPutData)
    {
        Debug.Log("砂の値"+outPutData.sandVaule);
        Debug.Log("泥の値"+outPutData.madVaule);
        Debug.Log("トータルの値"+outPutData.totalVaule);
    }
}

public class TankUITest : IOutPutTank
{
    private float Maxhigt = 100;
    private float Totale;

    public void OutputTank(OutPutData outPutData)
    {
        Totale = outPutData.totalVaule*Maxhigt;
        var sandVaule = Totale * outPutData.sandVaule ;
        var madeVaule = Totale * outPutData.madVaule;

        Debug.Log(madeVaule + "泥の値");
        Debug.Log(sandVaule + "砂の値");
        Debug.Log(Totale + "トータル");


    }
}