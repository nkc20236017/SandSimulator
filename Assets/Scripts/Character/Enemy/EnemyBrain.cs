using System.Linq;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
	[Header("Enemy Config")]
	[SerializeField] private Enemy enemy;
	
	[Header("Finite State Machine")]
	[SerializeField] private string initState;
	[SerializeField] private FsmState[] states;
    
	private FsmState _currentState;
    
	public Enemy Enemy { get; set; }
	public Transform Player { get; set; }
    
	private void Start()
	{
		SetEnemy(enemy);
		
		ChangeState(initState);
	}
	
	public void SetEnemy(Enemy enemy)
	{
		Enemy = enemy;
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
