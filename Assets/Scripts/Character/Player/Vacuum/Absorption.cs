using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Absorption : MonoBehaviour
{
    [Header("Tile Config")]
    [SerializeField] private Tilemap _tilemap;

    [Header("Suction Config")]
    [SerializeField] private Transform _suctionPivot;
    [SerializeField] private float _suctionAngle;
    [SerializeField] private float _suctionDistance;
    [SerializeField] private float _deleteDistance;

    [Header("Debug Config")]
    [SerializeField] private bool _debugMode;

    private Camera _mainCamera;

    public List<Vector3Int> AbsorbedTilePositions { get; private set; } = new();

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        RotateToCursorDirection();

        if (Input.GetMouseButtonUp(0))
        {
            AbsorbedTilePositions.Clear();
        }

        if (Input.GetMouseButton(0))
        {
            GetAbsorbedTilePositions();
            AbsorbTiles();
        }
    }

    private void RotateToCursorDirection()
    {
        var mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var direction = mouseWorldPosition - _suctionPivot.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (_suctionPivot.parent.localScale.x < 0)
        {
            angle += 180;
        }

        _suctionPivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void AbsorbTiles()
    {
        AbsorbedTilePositions = AbsorbedTilePositions.OrderBy(x => Random.value).ToList();

        foreach (var tilePosition in AbsorbedTilePositions)
        {
            var tile = _tilemap.GetTile(tilePosition);
            if (tile == null) { continue; }

            var direction = (_suctionPivot.position - _tilemap.GetCellCenterWorld(tilePosition)).normalized;
            var newPosition = new Vector3Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), 0);
            var newTilePosition = tilePosition + newPosition;
            _tilemap.SetTile(newTilePosition, tile);
            _tilemap.SetTile(tilePosition, null);

            if (Vector3.Distance(_tilemap.GetCellCenterWorld(newTilePosition), _suctionPivot.position) <= _deleteDistance)
            {
                _tilemap.SetTile(newTilePosition, null);
            }
        }
    }

    private void GetAbsorbedTilePositions()
    {
        AbsorbedTilePositions.Clear();
        var bounds = new BoundsInt(_tilemap.WorldToCell(_suctionPivot.position) - new Vector3Int((int)_suctionDistance, (int)_suctionDistance, 0), new Vector3Int((int)_suctionDistance * 2, (int)_suctionDistance * 2, 1));
        foreach (var tilePosition in bounds.allPositionsWithin)
        {
            var mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var centerCell = (Vector3)_tilemap.WorldToCell(mouseWorldPosition);

            var direction1 = (Vector3)_tilemap.WorldToCell(tilePosition) - _tilemap.WorldToCell(_suctionPivot.position);
            var direction2 = (Vector3)_tilemap.WorldToCell(centerCell) - _tilemap.WorldToCell(_suctionPivot.position);
            var angle = Vector3.Angle(direction1, direction2);

            var distance = Vector3.Distance(_tilemap.GetCellCenterWorld(tilePosition), _suctionPivot.position);

            if (angle <= _suctionAngle && distance < _suctionDistance && distance > _deleteDistance)
            {
                AbsorbedTilePositions.Add(tilePosition);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!_debugMode) { return; }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_suctionPivot.position, _suctionDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_suctionPivot.position, _deleteDistance);

        Gizmos.color = Color.green;
        var camera = Camera.main;
        if (camera == null) { return; }

        var mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        var centerCell = (Vector3)_tilemap.WorldToCell(mouseWorldPosition);

        var angleInRadians = _suctionAngle * Mathf.Deg2Rad;
        var direction2 = centerCell - _suctionPivot.position;
        var angle = Mathf.Atan2(direction2.y, direction2.x);

        var newAngle1 = angle - angleInRadians;
        var newX1 = _suctionDistance * Mathf.Cos(newAngle1);
        var newY1 = _suctionDistance * Mathf.Sin(newAngle1);
        var newDirection1 = new Vector3(newX1, newY1, 0);
        var newCell1 = _suctionPivot.position + newDirection1;
        Gizmos.DrawLine(_suctionPivot.position, newCell1);

        var newAngle2 = angle + angleInRadians;
        var newX2 = _suctionDistance * Mathf.Cos(newAngle2);
        var newY2 = _suctionDistance * Mathf.Sin(newAngle2);
        var newDirection2 = new Vector3(newX2, newY2, 0);
        var newCell2 = _suctionPivot.position + newDirection2;
        Gizmos.DrawLine(_suctionPivot.position, newCell2);
    }
}
