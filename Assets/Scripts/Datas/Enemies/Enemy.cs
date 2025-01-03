﻿using System;
using UnityEngine;
using NaughtyAttributes;

public enum EnemyType
{
	Mole,
	Turtle,
	Anmonite,
	Wolf,
}

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/Datas/Enemies/New Enemy")]
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
	public int health;
	public float speed;
	public int attack;
	public float attackSpeed;
}