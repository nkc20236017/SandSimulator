using NUnit.Framework;
using UnityEngine;

public class TankTest
{
    [Test]
    public void Tank()
    {
        IOutResultUI tankOutPut = new TankUITest();
        //IInputTank inputTank = new ItemTank(tankOutPut, 100);

        //    inputTank.InputAddTank(ItemType.Sand);
        

        //    inputTank.InputRemoveTank(ItemType.Sand);
      
    }
}


public class TankUITest : IOutResultUI
{
    private float Maxhigt = 365;
    private float Totale;

    public void OutputTank(OutPutResultData outPutData)
    {
        Totale = outPutData.totalRatio * Maxhigt;


        Debug.Log(outPutData.itemRatio + "の値");
        Debug.Log(outPutData.totalRatio + "トータル");


    }
}