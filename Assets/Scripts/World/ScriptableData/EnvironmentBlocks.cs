using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New environment block", menuName = "ScriptableObjects/WorldCreatePrinciple/Block")]
public class EnvironmentBlocks : ScriptableObject
{
    [SerializeField]
    private TileBase[] blocks;
    [SerializeField]
    private int airIndex;
    public int AirIndex => airIndex;

    public int GetBlockID(TileBase tile)
    {
        return Array.IndexOf(blocks, tile);
    }

    public TileBase GetBlock(int index)
    {
        return blocks[index];
    }
}