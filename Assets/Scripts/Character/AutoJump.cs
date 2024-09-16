using UnityEngine;
using UnityEngine.Tilemaps;

public class AutoJump : MonoBehaviour
{
	[SerializeField] private Tilemap tilemap;
	[SerializeField] private LayerMask groundLayerMask;
	
	private BoxCollider2D _boxCollider2D;
	private Rigidbody2D _rigidbody2D;

	private void Awake()
	{
		_boxCollider2D = GetComponent<BoxCollider2D>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
	}

	public void OneBlockUp(Vector3 direction)
	{
		if (_rigidbody2D.velocity.y is > 0.01f or < -0.01f) { return; }
		if (direction.x == 0) { return; }
		if (!IsGround()) { return; }

		var x = direction.x > 0 ? _boxCollider2D.bounds.max.x + 0.25f : _boxCollider2D.bounds.min.x - 0.25f;
		var position = new Vector2(x, _boxCollider2D.bounds.min.y + 0.1f);
		var cellPosition = tilemap.WorldToCell(position);
		if (tilemap.HasTile(cellPosition) && !tilemap.HasTile(cellPosition + Vector3Int.up))
		{
			transform.position += new Vector3(0.1f, 1.1f, 0);
		}
	}
	
	private bool IsGround()
	{
		var x = _boxCollider2D.bounds.center.x;
		var y = _boxCollider2D.bounds.min.y;
		var position = new Vector2(x, y);
		var radius = _boxCollider2D.size.x / 2 - 0.1f;
		var hit = Physics2D.CircleCast(position, radius, Vector2.down, 0.1f, groundLayerMask);
		return hit.collider != null;
	}
}

