using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputTank 
{
    void InputAddTank(MineralType type);
    void InputRemoveTank(MineralType type);
}
