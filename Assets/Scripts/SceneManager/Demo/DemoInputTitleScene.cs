using MackySoft.Navigathena.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoInputTitleScene : MonoBehaviour
{
    private ISceneIdentifier loadScene = new BuiltInSceneIdentifier("LoadScene");
    private ISceneIdentifier gameScene = new BuiltInSceneIdentifier("SelectScene");
    private bool fastSelect;

    private void Awake()
    {
        if(!SceneManager.GetSceneByName("RootScene").isLoaded)
        {
            SceneManager.LoadScene("RootScene",LoadSceneMode.Additive );
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (fastSelect) { return; }
            AudioManager.Instance.PlaySFX("DecisionSE");
            fastSelect = true;
            GlobalSceneNavigator.Instance.Push(gameScene, new LoadSceneDirector(loadScene));
        }
    }
}
