using UnityEngine;

public class DecisionsTimer : FsmDecision
{
	[SerializeField] private float interval;
	
	private float _timer;
	
	public override bool Decide()
	{
		return Timer();
	}
	
	private bool Timer()
	{
		_timer += Time.deltaTime;
		if (_timer < interval) { return false; }

		_timer = 0;
		return true;
	}
}

