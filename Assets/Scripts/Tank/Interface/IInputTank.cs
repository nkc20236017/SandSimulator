using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IInputTank 
{
    void InputAddTank(BlockType type , int amount = 1);
    bool FiringTank();
    void LeftSelectTank();
    void RightSelectTank();
    void InputAddTank(TileBase tileBase , int amount = 1);
    bool TamkMaxSignal();
    BlockType GetSelectType();

}
