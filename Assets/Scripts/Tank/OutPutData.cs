using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutPutData 
{
    public readonly float sandVaule;
    public readonly float madVaule;
    public readonly int maxVaule;
    public readonly float totalVaule;

    public OutPutData(float sandVaule, float madVaule, float totalVaule)
    {
        this.sandVaule = sandVaule;
        this.madVaule = madVaule;
        this.totalVaule = totalVaule;
    }
}
