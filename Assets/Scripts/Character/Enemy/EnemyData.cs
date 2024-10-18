using System;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
	[SerializeField] private EnemyConfigData[] enemyConfigData;
	
	public EnemyConfigData[] EnemyConfigData => enemyConfigData;
	public BoxCollider2D _boxCollider2D;
	public Rigidbody2D _rigidbody2D;
	public EnemyBrain _enemyBrain;

	private void OnEnable()
	{
		_boxCollider2D = GetComponent<BoxCollider2D>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_enemyBrain = GetComponent<EnemyBrain>();
	}
}

[Serializable]
public class EnemyConfigData
{
	public string ID;
	public EnemyConfig Config;
}