using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class ExitGateObject : MonoBehaviour
{
    private IGameLoad gameLoad;

    [Inject]
    public void Inject(IGameLoad gameLoad)
    {
        this.gameLoad = gameLoad;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("aa");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            gameLoad.GameLoad();
        }
    }

}
