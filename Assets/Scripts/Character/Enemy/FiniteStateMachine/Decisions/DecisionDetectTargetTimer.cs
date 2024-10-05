using UnityEngine;

public class DecisionDetectTargetTimer : FsmDecision
{
	[Header("Config")]
	[SerializeField] private float _detectInterval;
	
	private float _timer;
	private EnemyBrain _enemyBrain;
	
	public override bool Decide()
	{
		return DetectTargetTimer();
	}
	
	private bool DetectTargetTimer()
	{
		if (_enemyBrain.Target == null) { return false; }
		
		if (Vector3.Distance(_enemyBrain.Target.position, transform.position) <= 0.5f)
		{
			_timer += Time.deltaTime;
			if (_timer >= _detectInterval)
			{
				_timer = 0f;
				return true;
			}
		}
		else
		{
			_timer = 0f;
		}
		
		return false;
	}
	
	private void OnEnable()
	{
		_enemyBrain = GetComponent<EnemyBrain>();
	}
}

