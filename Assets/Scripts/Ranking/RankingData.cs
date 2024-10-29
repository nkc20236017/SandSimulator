using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RankingData
{
    public List<int> Ranks;

    public RankingData()
    {
        Ranks = new List<int>();
    }
}