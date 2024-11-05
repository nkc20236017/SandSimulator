using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BlockData", menuName = "ScriptableObjects/Data/Blocks/BlockData")]
public class BlockData : ScriptableObject
{
	[SerializeField] private Block[] blocks;

	public Block[] Block => blocks;
	
	public Block GetBlock(BlockType type)
	{
		return blocks.FirstOrDefault(block => block.type == type);
	}
	
	public Block GetBlock(Sprite sprite)
	{
		return blocks.FirstOrDefault(block => block.sprite == sprite);
	}
	
	public Block GetBlock(TileBase tile)
	{
		return blocks.FirstOrDefault(block => block.tile == tile);
	}

	public Ore GetOre(BlockType blockType)
	{
		return blocks.FirstOrDefault(block => block.type == blockType) as Ore;
	}
}
