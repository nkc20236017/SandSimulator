using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Data",menuName = "Data/Data")]
public class MineralData : ScriptableObject
{
    public Sprite sprite;
    public MineralType itemType;
}
