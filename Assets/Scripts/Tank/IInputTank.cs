using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputTank 
{
    void InputAddTank(ItemType type);
    void InputRemoveTank(ItemType type);
}
