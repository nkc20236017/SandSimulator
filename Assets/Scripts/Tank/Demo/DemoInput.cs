using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class DemoInputItemTank : MonoBehaviour
{

    private IInputTank itemTank;
    private IGameLoad gameLoad;

    [Inject]
    public void Inject(IInputTank itemTank, IGameLoad gameLoad)
    {
        this.itemTank = itemTank;
        this.gameLoad = gameLoad;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            itemTank.SelectTank(BlockType.Sand);
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            itemTank.SelectTank(BlockType.Mud);
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (itemTank.FiringTank())
            {
                Debug.Log("èoÇµÇ‹Ç∑");
            }
            else
            {
                Debug.Log("èoÇ‹ÇπÇÒ");
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameLoad.GameLoad();
        }

        if(Input.GetKey(KeyCode.A))
        {
            itemTank.InputAddTank(BlockType.Sand);
        }
        if(Input.GetKey(KeyCode.Q))
        {
            itemTank.InputAddTank(BlockType.Mud);
        }

    }


}
