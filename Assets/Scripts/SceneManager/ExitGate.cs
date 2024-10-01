using MackySoft.Navigathena.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGate 
{
    private readonly Dictionary<Block, MineralTank> resultDic;
    private ISceneIdentifier loadScene = new BuiltInSceneIdentifier("LoadScene");
    private ISceneIdentifier gameScene = new BuiltInSceneIdentifier("ResultScene");

    public ExitGate(Dictionary<Block, MineralTank> result)
    {
        this.resultDic = result;
    }

    public void ResultScene()
    {
        GlobalSceneNavigator.Instance.Push(gameScene, new LoadSceneDirector(loadScene),data:new SceneResultData(resultDic));
    }



}
