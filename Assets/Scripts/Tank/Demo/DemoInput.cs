using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class DemoInput : MonoBehaviour
{

    private IInputTank itemTank;

    [Inject]
    public void Inject(IInputTank itemTank)
    {
        this.itemTank = itemTank;
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
            itemTank.FiringTank();
        }

        if(Input.GetKey(KeyCode.A))
        {
            itemTank.InputAddTank(BlockType.Sand);
            itemTank.InputAddTank(BlockType.Mud);
        }


    }


}
