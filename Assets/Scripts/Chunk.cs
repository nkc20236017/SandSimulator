using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] private Vector2Int ChunkSize;

    private SpriteRenderer _spriteRenderer;
    private SandSimulation _sandSimulation;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateChunkSprite(Texture2D texture)
    {
        var position = Vector3Int.FloorToInt(transform.position);
        var newTexture = new Texture2D(ChunkSize.x, ChunkSize.y);
        
        var width = texture.width / 2 + position.x * 100;
        var height = texture.height / 2 + position.y * 100;
        var pixelColor = texture.GetPixels(width, height, ChunkSize.x, ChunkSize.y);

        newTexture.SetPixels(pixelColor);
        newTexture.filterMode = FilterMode.Point;
        newTexture.Apply();

        _spriteRenderer.sprite = Sprite.Create(newTexture, new Rect(0, 0, ChunkSize.x, ChunkSize.y), Vector2.one * 0.5f);

        var polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
    }
}
