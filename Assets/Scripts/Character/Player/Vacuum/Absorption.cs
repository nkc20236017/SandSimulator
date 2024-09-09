using System;
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
    [SerializeField] private Transform pivot;
    [SerializeField, Range(0f, 180f)] private float _suctionAngle;
    [SerializeField, Min(0f)] private float _suctionDistance;
    [SerializeField, Min(0f)] private float _deleteDistance;

    [Header("Debug Config")]
    [SerializeField] private bool _debugMode;

    [Header("Delete Config")]
    [SerializeField] private bool matchTheSizeOfTheCollider;

    private Camera _mainCamera;
    private PlayerActions _playerActions;

    public List<Vector3Int> AbsorbedTilePositions { get; private set; } = new();

    private void Awake()
    {
        _playerActions = new PlayerActions();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        
        // _playerActions.Vacuum.Absorption.performed += _ => GetAbsorbedTilePositions();
        // _playerActions.Vacuum.Absorption.canceled += _ => AbsorbedTilePositions.Clear();
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
        var direction = mouseWorldPosition - pivot.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (pivot.parent.localScale.x < 0)
        {
            angle += 180;
        }

        pivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void GetAbsorbedTilePositions()
    {
        AbsorbedTilePositions.Clear();
        var bounds = new BoundsInt(_tilemap.WorldToCell(pivot.position) - new Vector3Int((int)_suctionDistance, (int)_suctionDistance, 0), new Vector3Int((int)_suctionDistance * 2, (int)_suctionDistance * 2, 1));
        var getTilesBlock = _tilemap.GetTilesBlock(bounds);
        getTilesBlock = getTilesBlock.Where(x => x != null).ToArray();
        if (getTilesBlock.Length == 0) { return; }
        
        foreach (var tilePosition in bounds.allPositionsWithin)
        {
            if (_tilemap.GetTile(tilePosition) == null) { continue; }
            
            var mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var centerCell = (Vector3)_tilemap.WorldToCell(mouseWorldPosition);

            var direction1 = (Vector3)_tilemap.WorldToCell(tilePosition) - _tilemap.WorldToCell(pivot.position);
            var direction2 = (Vector3)_tilemap.WorldToCell(centerCell) - _tilemap.WorldToCell(pivot.position);
            var angle = Vector3.Angle(direction1, direction2);

            var distance = Vector3.Distance(_tilemap.GetCellCenterWorld(tilePosition), pivot.position);

            if (angle <= _suctionAngle && distance <= _suctionDistance && distance > _deleteDistance)
            {
                AbsorbedTilePositions.Add(tilePosition);
            }
        }
    }
    
    private void AbsorbTiles()
    {
        AbsorbedTilePositions = AbsorbedTilePositions.OrderBy(_ => Random.value).ToList();

        foreach (var tilePosition in AbsorbedTilePositions)
        {
            var mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var deletePivotDirection = (mouseWorldPosition - pivot.position).normalized;
            var deletePivotPosition = deletePivotDirection * _deleteDistance;
            var direction = deletePivotPosition - pivot.position;
            var newTilePosition = Vector3Int.RoundToInt(tilePosition + direction.normalized);
            newTilePosition = _tilemap.WorldToCell(newTilePosition);
            newTilePosition = Vector3Int.RoundToInt((tilePosition - pivot.position).normalized * _deleteDistance + pivot.position);
            Debug.DrawLine(_tilemap.WorldToCell(tilePosition), _tilemap.WorldToCell(newTilePosition), Color.red, 1f);
            if (_tilemap.GetTile(newTilePosition) != null) { continue; }
            
            var tile = _tilemap.GetTile(tilePosition);
            
            _tilemap.SetTile(newTilePosition, tile);
            _tilemap.SetTile(tilePosition, null);
            
            _tilemap.SetColliderType(newTilePosition, Tile.ColliderType.None);

            if (Vector3.Distance(_tilemap.GetCellCenterWorld(newTilePosition), pivot.position) <= _deleteDistance)
            {
                _tilemap.SetTile(newTilePosition, null);
            }
        }
    }

    private void OnDrawGizmos()
    {
        _deleteDistance = Mathf.Min(_suctionDistance, _deleteDistance);
        _suctionDistance = Mathf.Max(_deleteDistance, _suctionDistance);
        
        if (!_debugMode) { return; }

        if (matchTheSizeOfTheCollider)
        {
            var boxCollider2D = GetComponentInParent<BoxCollider2D>();
            if (boxCollider2D != null)
            {
                _suctionDistance = pivot.transform.position.y - boxCollider2D.bounds.min.y;
                var left = pivot.transform.position.x - boxCollider2D.bounds.min.x;
                var right = boxCollider2D.bounds.max.x - pivot.transform.position.x;
                _deleteDistance = Mathf.Max(left, right);
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pivot.position, _suctionDistance);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pivot.position, _deleteDistance);

        Gizmos.color = Color.green;
        var camera = Camera.main;
        if (camera == null) { return; }

        var mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        var centerCell = (Vector3)_tilemap.WorldToCell(mouseWorldPosition);

        var angleInRadians = _suctionAngle * Mathf.Deg2Rad;
        var direction2 = centerCell - pivot.position;
        var angle = Mathf.Atan2(direction2.y, direction2.x);

        var newCell1 = GetNewCell(angle - angleInRadians, _suctionDistance);
        Gizmos.DrawLine(pivot.position, newCell1);

        var newCell2 = GetNewCell(angle + angleInRadians, _suctionDistance);
        Gizmos.DrawLine(pivot.position, newCell2);
    }
    
    private Vector3 GetNewCell(float angle, float chordLength)
    {
        var newX = chordLength * Mathf.Cos(angle);
        var newY = chordLength * Mathf.Sin(angle);
        var newDirection = new Vector3(newX, newY, 0);
        var newCell = pivot.position + newDirection;
        return newCell;
    }
    
    private void OnEnable()
    {
        _playerActions.Enable();
    }
    
    private void OnDisable()
    {
        _playerActions.Disable();
    }
}
