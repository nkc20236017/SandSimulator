using MackySoft.Navigathena.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneResultData : ISceneData
{
    public readonly Dictionary<Block, MineralTank> result;

    public SceneResultData(Dictionary<Block, MineralTank> result)
    {
        this.result = result;
    }
}
