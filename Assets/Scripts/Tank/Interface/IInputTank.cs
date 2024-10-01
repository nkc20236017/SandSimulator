using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IInputTank 
{
    void InputAddTank(BlockType type);
    bool FiringTank();
    void LeftSelectTank();
    void RightSelectTank();
    void InputAddTank(TileBase tileBase);
    bool TamkMaxSignal();
    BlockType GetSelectType();

}
