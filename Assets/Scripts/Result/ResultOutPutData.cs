using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultOutPutData 
{
    public readonly List<MineralTank> mineralTank;
    public readonly float mineralTotaleAmount;

    public ResultOutPutData(List<MineralTank> mineralTank, float mineralTotaleAmount)
    {
        this.mineralTank = mineralTank;
        this.mineralTotaleAmount = mineralTotaleAmount;
    }
}
