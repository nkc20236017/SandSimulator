using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class DemoInput : MonoBehaviour
{
    private IResultAction resultAction;

    private Dictionary<Block, MineralTank> mineralTanks = new();

    [SerializeField]
    private Block block;

    [Inject]
    public void Injct(IResultAction resultAction)
    {
        this.resultAction = resultAction;
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.H))
        {
            if(mineralTanks.TryGetValue(block, out MineralTank mineralTank))
            {
                mineralTank.MineralAdd();
            Debug.Log("1");
            }
            else
            {
            Debug.Log("2");
                mineralTanks.Add(block, new MineralTank(block));
            }
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            resultAction.ResultStart(mineralTanks);
        }

    }


}
