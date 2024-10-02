using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class ExitGateObject : MonoBehaviour
{
    private IGameLoad gameLoad;

    public void Inject(IGameLoad gameLoad)
    {
        this.gameLoad = gameLoad;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            gameLoad.GameLoad();
        }
    }

}
