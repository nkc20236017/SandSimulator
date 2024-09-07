using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : Singleton<MapGenerator>
{
    [Header("Renderer Config")]
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private GameObject player;
    
    [Header("Brush Config")]
    [SerializeField] private BlockType tileType;
    [SerializeField] private float brushSize = 5;
    
    [Header("Chunk Config")]
    [SerializeField] private GameObject chunkParent;
    [SerializeField] private Chunk chunkPrefab;
    [SerializeField] private int mainChunkCount;
    [SerializeField] private int chunkCount;
    
    [Header("Map Config")]
    [SerializeField] private Texture2D mapTexture;

    private Sprite map;
    private List<Chunk> _chunks = new();
    private List<SpriteRenderer> _chunkRenderers = new();
    private Vector3Int[] _chunkPositions;
    
    private void Start()
    {
        TextureToSprite();
    
        _chunkPositions = GenerateSpiralCoordinates(chunkCount);
        ChunkGenerate();
    }
    
    private void TextureToSprite()
    {
        var texture = new Texture2D(mapTexture.width, mapTexture.height);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
		
        map = Sprite.Create(texture, new Rect(0, 0, mapTexture.width, mapTexture.height), Vector2.one * 0.5f);
    }
    
    private void ChunkGenerate()
    {
        foreach (var position in _chunkPositions)
        {
            var chunk = Instantiate(chunkPrefab, position, Quaternion.identity, chunkParent.transform);
            var chunkPosition = new Vector3Int(position.x + chunk.ChunkSize.x, position.y + chunk.ChunkSize.y, 0);
            chunk.transform.position = chunkPosition;
            _chunkPositions[_chunkPositions.ToList().IndexOf(position)] = chunkPosition;
            chunk.UpdateChunkSprite(map.texture);
            var chunkRenderer = chunk.GetComponent<SpriteRenderer>();
            _chunkRenderers.Add(chunkRenderer);
            _chunks.Add(chunk);
            _chunks[^1].name = $"Chunk {_chunks.Count - 1}";
    
            if (_chunks.Count > mainChunkCount)
            {
                chunkRenderer.enabled = false;
            }
        }
    }
    
    private void Update()
    {
        ChunkUpdate();
    }

    private void ChunkUpdate()
    {
        var newChunkPositions = GenerateSpiralCoordinates(chunkCount);
        if (newChunkPositions.SequenceEqual(_chunkPositions)) { return; }
        
        for (var i = 0; i < chunkCount; i++)
        {
            newChunkPositions[i] = new Vector3Int(newChunkPositions[i].x + _chunks[i].ChunkSize.x, newChunkPositions[i].y + _chunks[i].ChunkSize.y, 0);
            _chunks[i].transform.position = newChunkPositions[i];
            _chunks[i].UpdateChunkSprite(map.texture);
        }
        
        _chunkPositions = newChunkPositions;
    }

    private Vector3Int[] GenerateSpiralCoordinates(int n)
    {
        var positions = new Vector3Int[n];
        int x = 0, y = 0, dx = 1, dy = 0, segmentLength = 1, steps = 0, segmentPassed = 0;

        for (var i = 0; i < n; i++)
        {
            positions[i] = Vector3Int.RoundToInt(player.transform.position + new Vector3(x, y));
            x += dx;
            y += dy;
            steps++;

            if (steps != segmentLength) { continue; }

            steps = 0;
            segmentPassed++;
            (dx, dy) = (-dy, dx);

            if (segmentPassed != 2) { continue; }

            segmentPassed = 0;
            segmentLength++;
        }

        return positions;
    }
}
