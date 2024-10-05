using UnityEngine;

public class Digging : MonoBehaviour
{
	[Header("Digging Config")]
	[SerializeField] private float _radius;
	
	private IChunkInformation _chunkInformation;

	private void Update()
	{
		var bounds = new BoundsInt(Vector3Int.RoundToInt(transform.position) - new Vector3Int((int)_radius, (int)_radius, 0), new Vector3Int((int)_radius * 2, (int)_radius * 2, 1));
		foreach (var position in bounds.allPositionsWithin)
		{
			var distance = Vector3.Distance(transform.position, position);
			if (distance > _radius) { continue; }
			
			var tilemap = _chunkInformation.GetChunkTilemap(new Vector2(position.x, position.y));
			if (tilemap == null) { continue; }
			
			var localPosition = _chunkInformation.WorldToChunk(new Vector2(position.x, position.y));
			if (!tilemap.HasTile(localPosition)) { continue; }
			
			tilemap.SetTile(localPosition, null);
		}
	}

	private void OnEnable()
	{
		var worldMapManager = FindObjectOfType<WorldMapManager>();
		_chunkInformation = worldMapManager.GetComponent<IChunkInformation>();
	}
	
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, _radius);
	}
}
