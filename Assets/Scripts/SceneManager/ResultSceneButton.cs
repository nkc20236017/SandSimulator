using MackySoft.Navigathena.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSceneButton : MonoBehaviour
{
    private ISceneIdentifier loadScene = new BuiltInSceneIdentifier("LoadScene");
    private ISceneIdentifier gameScene = new BuiltInSceneIdentifier("TitleScene");

    public void SceneLoad()
    {
        AudioManager.Instance.PlaySFX("DecisionSE");
        GlobalSceneNavigator.Instance.Push(gameScene, new LoadSceneDirector(loadScene));
    }

}
