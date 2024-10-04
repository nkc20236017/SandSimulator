using MackySoft.Navigathena.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DemoInputSelectScene : MonoBehaviour
{
    private ISceneIdentifier loadScene = new BuiltInSceneIdentifier("LoadProgressScene");
    private ISceneIdentifier gameScene = new BuiltInSceneIdentifier("MainScene");


    public void OnSelectStage()
    {
        if (SceneManager.GetSceneByName("LoadScene").isLoaded)
        {
            return;
        }

        AudioManager.Instance.PlaySFX("DecisionSE");
        GlobalSceneNavigator.Instance.Push(gameScene, new LoadSceneDirector(loadScene));
        gameObject.SetActive(false);
    }
}
