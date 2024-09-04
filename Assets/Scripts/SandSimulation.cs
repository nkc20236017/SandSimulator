using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SandSimulation : Singleton<SandSimulation>
{
    [Header("Renderer Config")]
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private GameObject player;
    
    [Header("Brush Config")]
    [SerializeField] private TileType tileType;
    [SerializeField] private float brushSize = 5;
    
    [Header("Chunk Config")]
    [SerializeField] private GameObject chunkParent;
    [SerializeField] private Chunk chunkPrefab;
    [SerializeField] private Vector2Int chunkSize;
    [SerializeField] private int mainChunkCount;
    [SerializeField] private int chunkCount;
    
    [Header("Map Config")]
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private Texture2D mapTexture;

    private Sprite map;
    private List<Chunk> _chunks = new();
    private List<SpriteRenderer> _chunkRenderers = new();
    private Vector3Int[] _chunkPositions;
    
    private void Start()
    {
        MapGenerate();

        _chunkPositions = GenerateSpiralCoordinates(chunkCount);

        foreach (var position in _chunkPositions)
        {
            var chunk = Instantiate(chunkPrefab, position, Quaternion.identity, chunkParent.transform);
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
        var newChunkPositions = GenerateSpiralCoordinates(chunkCount);
        if (newChunkPositions.SequenceEqual(_chunkPositions)) { return; }

        for (var i = 0; i < chunkCount; i++)
        {
            _chunks[i].transform.position = newChunkPositions[i];
            _chunks[i].UpdateChunkSprite(map.texture);
        }

        _chunkPositions = newChunkPositions;
    }

    private void MapGenerate()
    {
        if (mapTexture != null)
        {
            map = Sprite.Create(mapTexture, new Rect(0, 0, mapTexture.width, mapTexture.height), Vector2.one * 0.5f);
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = map;
            return;
        }
        
        var texture = new Texture2D(mapSize.x, mapSize.y);
        var colors = new Color[mapSize.x * mapSize.y];
        for (var i = 0; i < colors.Length; i++)
        {
            var x = i % mapSize.x;
            var y = i / mapSize.x;
            var value = Mathf.PerlinNoise(x / 10f, y / 10f);
            colors[i] = value < 0.5f ? Color.black : Color.white;
        }
        texture.SetPixels(colors);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
		
        map = Sprite.Create(texture, new Rect(0, 0, mapSize.x, mapSize.y), Vector2.one * 0.5f);
    }

    private Vector3Int[] GenerateSpiralCoordinates(int n)
    {
        var positions = new Vector3Int[n];

        int x = 0, y = 0;
        int dx = 1, dy = 0;
        var segmentLength = 1;
        var steps = 0;
        var segmentPassed = 0;
        
        var playerPosition = player.transform.position;
        var centerPosition = new Vector3(playerPosition.x / chunkSize.x, playerPosition.y / chunkSize.y, 0f);
        centerPosition = Vector3Int.RoundToInt(centerPosition);
        centerPosition = new Vector3(centerPosition.x * chunkSize.x, centerPosition.y * chunkSize.y, 0f);

        for (var i = 0; i < n; i++)
        {
            positions[i] = Vector3Int.RoundToInt(centerPosition + new Vector3(x, y));
            x += dx;
            y += dy;
            steps++;

            // セグメントが終わると方向を変える
            if (steps != segmentLength) { continue; }

            steps = 0;
            segmentPassed++;

            // 方向を時計回りに変更
            var temp = dx;
            dx = -dy;
            dy = temp;

            // 2セグメント毎にセグメントの長さを増加
            if (segmentPassed != 2) { continue; }

            segmentPassed = 0;
            segmentLength++;
        }

        return positions;
    }
}
