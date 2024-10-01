using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FallingSandSimulation : MonoBehaviour
{
    [Header("Compute Shader Config")]
    [SerializeField] private ComputeShader computeShader;
    
    [Header("Map Config")]
    [SerializeField] private Tilemap tilemap;
    
    [Header("Brush Config")]
    [SerializeField] private TileBase sandTile;
    [SerializeField] private int radius = 5;

    private static readonly int Result = Shader.PropertyToID("Result");
    private static readonly int SandBuffer = Shader.PropertyToID("SandBuffer");
    private static readonly int Width = Shader.PropertyToID("Width");
    private static readonly int Height = Shader.PropertyToID("Height");
    private int Kernel => computeShader.FindKernel("CSMain");
    private int _width;
    private int _height;
    private Camera _camera;
    private ComputeBuffer _computeBuffer;
    private RenderTexture _renderTexture;

    private void Start()
    {
        _camera = Camera.main;

        _width = 100;
        _height = 100;

        InitializeSandSimulation();
    }
    
    private void InitializeSandSimulation()
    {
        _renderTexture = new RenderTexture(_width, _height, 0, RenderTextureFormat.RFloat)
        {
                enableRandomWrite = true
        };
        _renderTexture.Create();

        _computeBuffer = new ComputeBuffer(_width * _height, sizeof(float));
        var initialData = new float[_width * _height];
        _computeBuffer.SetData(initialData);

        computeShader.SetTexture(Kernel, Result, _renderTexture);
        computeShader.SetBuffer(Kernel, SandBuffer, _computeBuffer);
        computeShader.SetInt(Width, _width);
        computeShader.SetInt(Height, _height);
    }

    private void Update()
    {
        HandleTileInput();
        HandleNullInput();
        UpdateSandSimulation();
        UpdateTilemap();
    }

    private void HandleTileInput()
    {
        if (!Input.GetMouseButton(0)) { return; }

        var worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        var cellPosition = tilemap.WorldToCell(worldPosition);

        SpawnTileAtPosition(cellPosition.x, cellPosition.y, 1f);
    }
    
    private void HandleNullInput()
    {
        if (!Input.GetMouseButton(1)) { return; }

        var worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        var cellPosition = tilemap.WorldToCell(worldPosition);

        SpawnTileAtPosition(cellPosition.x, cellPosition.y, 0f);
    }

    private void SpawnTileAtPosition(int centerX, int centerY, float amount)
    {
        var sandData = new float[_width * _height];
        _computeBuffer.GetData(sandData);

        for (var x = -radius; x <= radius; x++)
        {
            for (var y = -radius; y <= radius; y++)
            {
                if (x * x + y * y > radius * radius) { continue; }

                var spawnX = centerX + x;
                var spawnY = centerY + y;

                if (spawnX < 0 || spawnX >= _width || spawnY < 0 || spawnY >= _height) { continue; }

                var index = spawnY * _width + spawnX;
                sandData[index] = amount;
            }
        }

        _computeBuffer.SetData(sandData);
    }

    private void UpdateSandSimulation()
    {
        computeShader.Dispatch(Kernel, _width / 8, _height / 8, 1);
    }

    private void UpdateTilemap()
    {
        var sandData = new float[_width * _height];
        _computeBuffer.GetData(sandData);

        var tilePositions = new List<Vector3Int>();
        var tileBases = new List<TileBase>();

        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                var index = y * _width + x;
                tilePositions.Add(new Vector3Int(x, y, 0));
                tileBases.Add(Mathf.Approximately(sandData[index], 1f) ? sandTile : null);
            }
        }

        tilemap.SetTiles(tilePositions.ToArray(), tileBases.ToArray());
    }

    private void OnDestroy()
    {
        _computeBuffer?.Release();
        if (_renderTexture != null)
        {
            _renderTexture.Release();
        }
    }
}