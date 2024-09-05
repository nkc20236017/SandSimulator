using UnityEngine;
using NaughtyAttributes;

public enum BlockType
{
	Sand,
	Mud,
	Water,
	Fire,
	Smoke,
	Cristal
}

[CreateAssetMenu(fileName = "New Block", menuName = "ScriptableObjects/New Block")]
public class Block : ScriptableObject
{
	[Header("Block Config")]
	[SerializeField] private string blockName;
	[SerializeField] private BlockType blockType;
	[SerializeField, ShowAssetPreview] private Sprite[] sprites;
	[SerializeField, TextArea(3, 10)] private string description;
	[SerializeField] private int hardness;
	
	public string BlockName => blockName;
	public BlockType BlockType => blockType;
	public string Description => description;
	public int Hardness => hardness;
	
	public Texture2D GetRandomTexture()
	{
		return sprites[Random.Range(0, sprites.Length)].texture;
	}
}
