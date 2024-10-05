using UnityEngine;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class ActionWanderMole : FsmAction
{
	[Header("Wander Config")]
	[SerializeField, MinMaxSlider(0f, 60f)] private Vector2 _wanderTime;
	[SerializeField] private float _radius;
	
	private float _timer;
	private float _randomWanderTime;
	private Vector3 _movePosition;
	private EnemyBrain _enemyBrain;
	private IChunkInformation _chunkInformation;
	
	private void Start()
	{
		_timer = Random.Range(_wanderTime.x, _wanderTime.y);
		GetRandomPointInCircle();
	}
	
	public override void Action()
	{
		Wander();
	}
	
	private void Wander()
	{
		if (IsTilemap(_movePosition))
		{
			GetRandomPointInCircle();
			return;
		}

		Movement();

		_timer -= Time.deltaTime;
		if (_timer <= 0f)
		{
			GetRandomPointInCircle();
			_timer = Random.Range(_wanderTime.x, _wanderTime.y);
		}
	}
        
    private void Movement()
    {
        // ランダムな位置に向かって移動する
        var moveDirection = (_movePosition - transform.position).normalized;
        var movement = moveDirection * (_enemyBrain.Status.speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _movePosition) >= 0.5f)
        {
            transform.Translate(movement);
            
            var direction = _movePosition - transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void GetRandomPointInCircle()
    {
        // 範囲内のランダムな位置を取得する
        var randomAngle = Random.Range(0f, Mathf.PI * 2f);
        var randomDistance = Random.Range(0f, _radius);
        var x = Mathf.Cos(randomAngle) * randomDistance;
        var y = Mathf.Sin(randomAngle) * randomDistance;
        _movePosition = transform.position + new Vector3(x, y, 0f);
    }
    
    private bool IsTilemap(Vector3 position)
    {
        // マップがあるかどうか調べる
		var mapTilemap = _chunkInformation.GetChunkTilemap(position);
		return mapTilemap != null;
    }

    private void OnDrawGizmos()
    {
	    Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, _radius);
		
		if (_movePosition == Vector3.zero) { return; }
		if (_enemyBrain.Target != null) { return; }
		
		Gizmos.DrawLine(transform.position, _movePosition);
		Gizmos.DrawWireSphere(_movePosition, 0.5f);
    }

    private void OnEnable()
	{
		var worldMapManager = FindObjectOfType<WorldMapManager>();
		_chunkInformation = worldMapManager.GetComponent<IChunkInformation>();
		_enemyBrain = GetComponent<EnemyBrain>();
	}
}

