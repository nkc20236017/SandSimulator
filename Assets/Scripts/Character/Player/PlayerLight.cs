using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerLight : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] private Tilemap tilemap;
	
	private SpriteRenderer _spriteRenderer;

	private void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		var cellPosition = tilemap.WorldToCell(transform.position);
		var tile = tilemap.GetTile(cellPosition);
		if (tile == null) { return; }

		_spriteRenderer.color = tilemap.GetColor(cellPosition);
	}
}
