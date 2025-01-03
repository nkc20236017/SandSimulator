﻿using System.Linq;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
	[Header("Enemy Config")]
	[SerializeField] private Enemy enemy;
	
	[Header("Finite State Machine")]
	[SerializeField] private string initState;
	[SerializeField] private FsmState[] states;
    
	private FsmState _currentState;
    
	public Vector3 Direction { get; set; }
	public EnemyStatus Status { get; private set; }
	public Transform Target { get; set; }
	public Vector3 TargetPosition { get; set; }

	private void Awake()
	{
		SetEnemy(enemy, 0);
	}

	private void Start()
	{
		ChangeState(initState);
	}
	
	public void SetEnemy(Enemy myEnemy, int width)
	{
		Status = myEnemy.status[width];
		GetComponent<EnemyHealth>().CurrentHealth = Status.health;
	}

	private void Update()
	{
		_currentState?.UpdateState(this);
	}
	
	public void ChangeState(string newStateID)
	{
		var newState = GetState(newStateID);
		if (newState == null) { return; }
		_currentState = newState;
	}
	
	private FsmState GetState(string newStateID)
	{
		return states.FirstOrDefault(state => state.ID == newStateID);
	}
}
