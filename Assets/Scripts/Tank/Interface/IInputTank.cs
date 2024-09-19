using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IInputTank 
{
    void InputAddTank(BlockType type);
    void FiringTank();
    void SelectTank(BlockType blockType);
    void InputAddTank(TileBase tileBase);
}
