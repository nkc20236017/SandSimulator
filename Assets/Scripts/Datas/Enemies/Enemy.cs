using NaughtyAttributes;
using UnityEngine;

public enum EnemyType
{
	Turtle,
}

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/Datas/New Enemy")]
public class Enemy : ScriptableObject
{
	public EnemyType type;
	public int id;
	public string name;
	[ShowAssetPreview] public Sprite sprite;
	
	[Header("Status")]
	public int maxHp;
	public int attack;
	public int defense;
	public int speed;
	
	[Header("Update Config")]
	[MinMaxSlider(0, 100)] public Vector2 updateInterval;
}

