using System;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] private Vector2Int ChunkSize;

    private Texture2D _texture;
    private SpriteRenderer _spriteRenderer;
    private MapGenerator _mapGenerator;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // ChunkUpdate();
    }
    
    private void ChunkUpdate()
    {
        var position = Vector3Int.FloorToInt(transform.position);
        var width = _texture.width / 2 + position.x * 100;
        var height = _texture.height / 2 + position.y * 100;
        var colors = _texture.GetPixels(width, height, ChunkSize.x, ChunkSize.y);

        foreach (var color in colors)
        {
            
        }
    }

    public void UpdateChunkSprite(Texture2D texture)
    {
        var position = Vector3Int.FloorToInt(transform.position);
        _texture = new Texture2D(ChunkSize.x, ChunkSize.y);
        
        var width = texture.width / 2 + position.x * 100;
        var height = texture.height / 2 + position.y * 100;
        var pixelColor = texture.GetPixels(width, height, ChunkSize.x, ChunkSize.y);

        _texture.SetPixels(pixelColor);
        _texture.filterMode = FilterMode.Point;
        _texture.Apply();

        _spriteRenderer.sprite = Sprite.Create(_texture, new Rect(0, 0, ChunkSize.x, ChunkSize.y), Vector2.one * 0.5f);

        // var polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
    }
}
