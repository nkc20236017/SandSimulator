using MackySoft.Navigathena.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DemoInputSelectScene : MonoBehaviour, IPointerDownHandler
{
    private ISceneIdentifier loadScene = new BuiltInSceneIdentifier("LoadProgressScene");
    private ISceneIdentifier gameScene = new BuiltInSceneIdentifier("TileMapScene");

    public void OnPointerDown(PointerEventData eventData)
    {
        GlobalSceneNavigator.Instance.Push(gameScene, new LoadSceneDirector(loadScene));
    }
}
