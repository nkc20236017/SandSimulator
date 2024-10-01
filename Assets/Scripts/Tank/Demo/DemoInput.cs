using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class DemoInputItemTank : MonoBehaviour
{

    private IInputTank itemTank;
    private IGameLoad gameLoad;
    [SerializeField]
    private MainGameEntoryPoint gameEntoryPoint;

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

        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            gameEntoryPoint.SetProgress(new ProgressData(1f, "ロード中", "100%"));
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (itemTank.FiringTank())
            {
                Debug.Log("出します");
            }
            else
            {
                Debug.Log("出ません");
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
