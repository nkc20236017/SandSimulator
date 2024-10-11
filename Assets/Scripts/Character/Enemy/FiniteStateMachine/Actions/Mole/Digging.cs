using System.Linq;
using UnityEngine;

public class Digging : MonoBehaviour
{
	[Header("Digging Config")]
	[SerializeField] private BlockDatas _blockDatas;
	[SerializeField] private float _radius;
	[SerializeField] private float _diggingInterval;
	
	private int _numberExecutions;
	private float _timer;
	private EnemyBrain _enemyBrain;

	private void Update()
	{
		_timer += Time.deltaTime;
		if (_timer < _diggingInterval) { return; }
		
		_numberExecutions++;
		_timer = 0f;
		var bounds = new BoundsInt(Vector3Int.RoundToInt(transform.position) - new Vector3Int((int)_radius, (int)_radius, 0), new Vector3Int((int)_radius * 2, (int)_radius * 2, 1));
		foreach (var position in bounds.allPositionsWithin)
		{
			var distance = Vector3.Distance(transform.position, position);
			if (distance > _radius) { continue; }
			
			var tilemap = _enemyBrain.ChunkInformation.GetChunkTilemap(new Vector2(position.x, position.y));
			if (tilemap == null) { continue; }
			
			var localPosition = _enemyBrain.ChunkInformation.WorldToChunk(new Vector2(position.x, position.y));
			if (!tilemap.HasTile(localPosition)) { continue; }
			var tile = tilemap.GetTile(localPosition);
			var isContinue = _blockDatas.Block.Where(tileData => tileData.tile == tile).Any(tileData => _numberExecutions % tileData.weight != 0);
			if (isContinue) { continue; }
			
			tilemap.SetTile(localPosition, null);
		}
	}
	
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, _radius);
	}

	private void OnEnable()
	{
		_enemyBrain = GetComponent<EnemyBrain>();
	}
}
