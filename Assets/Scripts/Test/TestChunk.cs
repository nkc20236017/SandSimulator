using UnityEngine;
using UnityEngine.Tilemaps;

public class TestChunk : MonoBehaviour
{
    [SerializeField] private Transform mapGrid;
    [SerializeField] private Tilemap mapTilemap;
    [SerializeField] private TileBase tileBase;
    
    private Tilemap _tilemap;

    private void Start()
    {
        var tilemap = new GameObject();
        tilemap.transform.SetParent(mapGrid);
        _tilemap = tilemap.AddComponent<Tilemap>();
        tilemap.AddComponent<TilemapRenderer>();
        const int size = 10000;
        var posArray = new Vector3Int[size];
        var tileArray = new TileBase[size];
        for (var i = 0; i < size; i++)
        {
            posArray[i] = new Vector3Int(i % 100, i / 100, 0);
            tileArray[i] = tileBase;
        }
        _tilemap.SetTiles(posArray, tileArray);
    }

    private void GenerateMapTile()
    {
        const int size = 1000;
        var posArray = new Vector3Int[size];
        var tileArray = new TileBase[size];
        for (var i = 0; i < size; i++)
        {
            posArray[i] = new Vector3Int(i % size, i / size, 0);
            tileArray[i] = tileBase;
        }
        mapTilemap.SetTiles(posArray, tileArray);
        Instantiate(mapTilemap, mapGrid);
    }
}