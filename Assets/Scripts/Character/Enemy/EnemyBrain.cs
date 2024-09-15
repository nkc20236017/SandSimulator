using System.Linq;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] private string initState;
	[SerializeField] private FsmState[] states;
    
	private FsmState _currentState;
    
	public Transform Player { get; set; }
    
	private void Start()
	{
		ChangeState(initState);
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
