using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnvironmentBlocks : ScriptableObject
{
    [SerializeField]
    private TileBase[] blocks;

    public int GetBlockID(TileBase tile)
    {
        return Array.IndexOf(blocks, tile);
    }

    public TileBase GetBlock(int index)
    {
        return blocks[index];
    }
}