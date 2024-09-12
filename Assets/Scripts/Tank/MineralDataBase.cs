using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMineralDataBase", menuName = "Data/MineralDataBase")]
public class MineralDataBase : ScriptableObject
{
    public List<MineralData> data;
}

