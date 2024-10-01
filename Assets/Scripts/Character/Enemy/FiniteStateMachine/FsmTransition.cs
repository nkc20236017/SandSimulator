using System;

[Serializable]
public class FsmTransition
{
	public FsmDecision Decision;
	public string TrueState;
	public string FalseState;
}
