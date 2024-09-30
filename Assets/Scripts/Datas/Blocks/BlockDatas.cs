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
	
	public Block GetBlock(Sprite sprite)
	{
		return blocks.FirstOrDefault(block => block.sprite == sprite);
	}
	
	public Ore GetOre(OreType oreType)
	{
		return blocks.FirstOrDefault(block => block.type == BlockType.Ore && ((Ore) block).oreType == oreType) as Ore;
	}
	
	public Ore GetOre(Sprite sprite)
	{
		return blocks.FirstOrDefault(block => block.type == BlockType.Ore && block.sprite == sprite) as Ore;
	}
	
	public Ore GetRandomOre()
	{
		var ores = blocks.Where(block => block.type == BlockType.Ore).ToArray();
		return ores[Random.Range(0, ores.Length)] as Ore;
	}
}
