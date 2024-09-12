using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BlockType
{
	Sand,
	Mud,
	Stone,
	Water,
	Lava
}

[CreateAssetMenu(fileName = "BlockDatas", menuName = "ScriptableObjects/Datas/BlockDatas")]
public class BlockDatas : ScriptableObject
{
	[SerializeField] private TileData[] tileDatas;
	
	public TileData[] TileDatas => tileDatas;
}

[Serializable]
public class TileData
{
	public BlockType type;
	public TileBase tile;
	[HideInInspector] public List<Vector3Int> tilePositions = new();
}
