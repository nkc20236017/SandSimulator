using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class DemoInputTank : MonoBehaviour
{
    private IResultAction resultAction;

    private Dictionary<Block, MineralTank> mineralTanks = new();

    [SerializeField]
    private Block block;
    [SerializeField]
    private Block block2;

    [Inject]
    public void Injct(IResultAction resultAction)
    {
        this.resultAction = resultAction;
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.H))
        {
            AddItem(block);
        }
        if (Input.GetKey(KeyCode.G))
        {
            AddItem(block2);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            resultAction.ResultStart(mineralTanks);
        }

    }

    private void AddItem(Block block2)
    {
        if (mineralTanks.TryGetValue(block2, out MineralTank mineralTank))
        {
            mineralTank.MineralAdd();
            Debug.Log("既存のアイテムの量を追加");
        }
        else
        {
            Debug.Log("新アイテムを追加");
            mineralTanks.Add(block2, new MineralTank(block2));
        }
    }
}
