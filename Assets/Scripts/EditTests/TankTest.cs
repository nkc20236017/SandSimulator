using NUnit.Framework;
using UnityEngine;

public class TankTest
{
    [Test]
    public void Tank()
    {
        IOutPutTank tankOutPut = new TankUITest();
        IInputTank inputTank = new ItemTank(tankOutPut, 100);

        for (int i = 0; i < 300; i++)
        {
            inputTank.InputAddTank(ItemType.Sand);
        }

    }
}


public class TankUITest : IOutPutTank
{
    private float Maxhigt = 365;
    private float Totale;

    public void OutputTank(OutPutData outPutData)
    {
        Totale = outPutData.totalRatio * Maxhigt;


        Debug.Log(outPutData.itemRatio + "の値");
        Debug.Log(outPutData.totalRatio + "トータル");


    }
}