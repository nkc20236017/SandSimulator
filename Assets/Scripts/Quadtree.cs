using UnityEngine;
using UnityEngine.Tilemaps;

public class Quadtree : MonoBehaviour
{
	[SerializeField] private Tilemap tilemap;
	
	private Camera _camera;
	private TilesUpdate _tilesUpdate;

	private void Start()
	{
		_camera = Camera.main;
		_tilesUpdate = TilesUpdate.Instance;
	}

	private void OnDrawGizmos()
	{
		// 四分木を使ってタイルの位置を取得し描画する
		var bounds = tilemap.cellBounds.allPositionsWithin;
		foreach (var position in bounds)
		{
			var worldPosition = tilemap.GetCellCenterWorld(position);
			if (!tilemap.HasTile(position)) { continue; }
			
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(worldPosition, Vector3.one);
		}
	}
}

