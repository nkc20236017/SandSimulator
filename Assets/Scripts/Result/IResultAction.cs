using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResultAction
{
    void ResultStart(Dictionary<Block, MineralTank> result);
}
