using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralTank
{
    public MineralData mineralData;
    public int mineralAmount;

    public MineralTank(MineralData mineralData)
    {
        this.mineralData = mineralData;
        mineralAmount = 1;
    }

    public void MineralAdd()
    {
        mineralAmount++;
    }

    public void MineralRemove()
    {
        mineralAmount--;
    }

}
