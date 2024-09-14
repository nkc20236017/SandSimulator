using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BlockDatas", menuName = "ScriptableObjects/Datas/BlockDatas")]
public class BlockDatas : ScriptableObject
{
	[SerializeField] private Block[] blocks;

	public Block[] Block => blocks;
	
	public Block GetBlock(BlockType type)
	{
		return blocks.FirstOrDefault(block => block.type == type);
	}

	public Block GetBlock(TileBase tile)
	{
		return blocks.FirstOrDefault(block => block.tile == tile);
	}
	
	public Ore GetOre(OreType oreType)
	{
		return blocks.FirstOrDefault(block => block.type == BlockType.Ore && ((Ore) block).oreType == oreType) as Ore;
	}
}

