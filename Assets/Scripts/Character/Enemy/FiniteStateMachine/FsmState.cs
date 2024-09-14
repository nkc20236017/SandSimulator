using System;

[Serializable]
public class FsmState
{
	public string ID;
	public FsmAction[] Actions;
	public FsmTransition[] Transitions;
    
	public void UpdateState(EnemyBrain enemyBrain)
	{
		ExecuteActions();
		ExecuteTransitions(enemyBrain);
	}
    
	private void ExecuteActions()
	{
		// すべてのアクションを実行する
		foreach (var action in Actions)
		{
			action.Action();
		}
	}

	private void ExecuteTransitions(EnemyBrain enemyBrain)
	{
		if (Transitions is not { Length: > 0 }) { return; }
        
		foreach (var transition in Transitions)
		{
			var decision = transition.Decision.Decide();
			enemyBrain.ChangeState(decision ? transition.TrueState : transition.FalseState);
		}
	}
}

