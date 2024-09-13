using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputTank 
{
    void InputAddTank(BlockType type);
    void InputRemoveTank(BlockType type);
}
