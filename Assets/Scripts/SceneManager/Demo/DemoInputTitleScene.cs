using MackySoft.Navigathena.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DemoInputTitleScene : MonoBehaviour
{
    private ISceneIdentifier loadScene = new BuiltInSceneIdentifier("LoadScene");
    private ISceneIdentifier gameScene = new BuiltInSceneIdentifier("SelectScene");
    private bool fastSelect;
    private PlayerActions inputActions;

    private void Start()
    {
        inputActions = new PlayerActions();
        inputActions.UI.UISelect.performed += OnMouseButton;
        inputActions.Enable();
    }

    private void OnMouseButton(InputAction.CallbackContext context)
    {
        if (fastSelect) { return; }

        AudioManager.Instance.PlaySFX("DecisionSE");
        fastSelect = true;
        GlobalSceneNavigator.Instance.Push(gameScene, new LoadSceneDirector(loadScene));
    }
}
