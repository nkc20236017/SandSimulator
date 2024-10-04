using MackySoft.Navigathena.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DemoInputSelectScene : MonoBehaviour
{
    private ISceneIdentifier loadScene = new BuiltInSceneIdentifier("LoadProgressScene");
    private ISceneIdentifier gameScene = new BuiltInSceneIdentifier("MainScene");

    public void OnSelectStage()
    {
        AudioManager.Instance.PlaySFX("DecisionSE");
        GlobalSceneNavigator.Instance.Push(gameScene, new LoadSceneDirector(loadScene));
    }
}
