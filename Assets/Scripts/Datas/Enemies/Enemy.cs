using System;
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
	public string id;
	public string name;
	[ShowAssetPreview] public Sprite sprite;
	
	[Header("Status")]
	public EnemyStatus[] status;
}

[Serializable]
public class EnemyStatus
{
	[SerializeField] private string name;
	public int depth;
	
	[Header("Status")]
	public int hp;
	public int attack;
	public int speed;
}