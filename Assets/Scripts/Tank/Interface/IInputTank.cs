using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IInputTank 
{
    bool InputAddTank(BlockType type);
    bool FiringTank();
    void SelectTank(BlockType blockType);
    bool InputAddTank(TileBase tileBase);
}
