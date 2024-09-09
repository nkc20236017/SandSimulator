using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item Data",menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public string ItemId;
    public Sprite itemIcom;

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        ItemId = AssetDatabase.AssetPathToGUID(path);
#endif
    }
}
