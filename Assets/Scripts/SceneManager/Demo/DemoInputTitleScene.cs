using MackySoft.Navigathena.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoInputTitleScene : MonoBehaviour
{
    private ISceneIdentifier loadScene = new BuiltInSceneIdentifier("LoadScene");
    private ISceneIdentifier gameScene = new BuiltInSceneIdentifier("SelectScene");

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GlobalSceneNavigator.Instance.Push(gameScene,new LoadSceneDirector(loadScene));
        }
    }
}