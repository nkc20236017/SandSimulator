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

    public void Button()
    {
        itemTank.InputAddTank(BlockType.Sand);
    }


}
