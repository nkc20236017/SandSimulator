using UnityEngine.Tilemaps;
using UnityEngine;
using WorldCreation;
using System.Collections.Generic;
using System.Drawing;

public class GameChunk
{
    private Tilemap _gameChunkTilemap;
    private Vector2Int _gameChunkPosition;
    private int[,] _grid;
    private int[,] _layerIndex;
    private List<LetterInstantiateObject> _summonLaterObjects = new();
    public List<LetterInstantiateObject> SummonLaterObjects => _summonLaterObjects;

    /// <summary>
    /// チャンク単位の座標
    /// </summary>
    public Vector2Int GameChunkPosition => _gameChunkPosition;

    /// <summary>
    /// チャンクの大きさ
    /// </summary>
    public Vector2Int Size
    {
        get
        {
            return new Vector2Int(_grid.GetLength(0), _grid.GetLength(1));
        }
    }

    /// <summary>
    /// チャンクの管理しているタイルマップ
    /// </summary>
    public Tilemap GameChunkTilemap
    {
        get => _gameChunkTilemap;
        set
        {
            if (_gameChunkTilemap == null)
            {
                _gameChunkTilemap = value;
            }
        }
    }

    public GameChunk(Vector2Int position, Tilemap tilemap, Vector2Int chunkSize)
    {
        _gameChunkTilemap = tilemap;
        _gameChunkPosition = position;
        _grid = new int[chunkSize.x, chunkSize.y];
        _layerIndex = new int[chunkSize.x, chunkSize.y];
    }

    /// <summary>
    /// 指定のグリッドにあるブロックのIDを返す
    /// </summary>
    /// <param name="position">グリッドの位置</param>
    /// <returns>ブロックID</returns>
    public int GetBlockID(Vector2Int position)
    {
        return _grid[position.x, position.y];
    }

    /// <summary>
    /// 指定のグリッドにあるブロックのIDを返す
    /// </summary>
    /// <param name="x">グリッドのX位置</param>
    /// <param name="y">グリッドのY位置</param>
    /// <returns>ブロックID</returns>
    public int GetBlockID(int x, int y)
    {
        return _grid[x, y];
    }

    /// <summary>
    /// 指定のグリッドにブロックを配置する
    /// </summary>
    /// <param name="x">グリッドのX位置</param>
    /// <param name="y">グリッドのY位置</param>
    /// <param name="id">ブロックID</param>
    public void SetBlock(int x, int y, int id)
    {
        if (!IsChunkInside(new(x, y))) { return; }
        _grid[x, y] = id;
    }
    /// <summary>
    /// 指定のグリッドにブロックを配置する
    /// </summary>
    /// <param name="position">グリッドの位置</param>
    /// <param name="id">ブロックID</param>
    public void SetBlock(Vector2Int position, int id)
    {
        if (!IsChunkInside(position)) { return; }
        _grid[position.x, position.y] = id;
    }

    /// <summary>
    /// 地層の番号を設定します
    /// </summary>
    /// <param name="x">設定するグリッドX位置</param>
    /// <param name="y">設定するグリッドY位置</param>
    /// <param name="index">地層の番号</param>
    public void SetLayerIndex(int x, int y, int index)
    {
        _layerIndex[x, y] = index;
    }
    /// <summary>
    /// 地層の番号を取得します
    /// </summary>
    /// <param name="x">取得するグリッドX位置</param>
    /// <param name="y">取得するグリッドY位置</param>
    /// <returns></returns>
    public int GetLayerIndex(int x, int y)
    {
        return _layerIndex[x, y];
    }
    /// <summary>
    /// 地層の番号を取得します
    /// </summary>
    /// <param name="x">取得するグリッドX位置</param>
    /// <param name="y">取得するグリッドY位置</param>
    /// <returns></returns>
    public int GetLayerIndex(Vector2Int index)
    {
        return _layerIndex[index.x, index.y];
    }

    /// <summary>
    /// 原点のずれを考慮しないチャンク座標をワールド座標へ変換
    /// </summary>
    /// <param name="x">対象のチャンク座標X</param>
    /// <param name="y">対象のチャンク座標Y</param>
    /// <returns>ワールド座標</returns>
    public Vector2Int RawGameChunkPositionToWorldPosition(int x, int y)
    {
        return RawGameChunkPositionToWorldPosition(new(x, y));
    }
    /// <summary>
    /// 原点のずれを考慮しないチャンク座標をワールド座標へ変換
    /// </summary>
    /// <param name="position">対象のチャンク座標</param>
    /// <returns>ワールド座標</returns>
    public Vector2Int RawGameChunkPositionToWorldPosition(Vector2Int position)
    {
        return new
        (
            _gameChunkPosition.x * Size.x + position.x,
            _gameChunkPosition.y * Size.y + position.y
        );
    }

    /// <summary>
    /// ワールド座標を原点のずれを考慮しないチャンク座標に変換する
    /// </summary>
    /// <param name="position">対象のワールド座標</param>
    /// <returns></returns>
    public Vector2Int WorldPositionToRawGameChunkPosition(Vector2 position)
    {
        return new
        (
            (int)position.x % Size.x,
            (int)position.y % Size.y
        );
    }

    /// <summary>
    /// 後から召喚するゲームオブジェクトの数を取得する
    /// </summary>
    /// <returns></returns>
    public int GetSummonLaterObjectCount()
    {
        return _summonLaterObjects.Count;
    }

    public bool IsInsideChunkPosition(Vector2Int position, int margin = 0)
    {
        // チャンクのX最小より大きいか
        bool isMinX = Size.x * GameChunkPosition.x - margin <= position.x;
        // チャンクのX最大より小さいか
        bool isMaxX = position.x <= Size.x * GameChunkPosition.x + Size.x + margin;
        // チャンクのY最小より大きいか
        bool isMinY = Size.y * GameChunkPosition.y - margin <= position.y;
        // チャンクのY最大より小さいか
        bool isMaxY = position.y <= Size.y * GameChunkPosition.y + Size.y + margin;

        return isMinX && isMinY && isMaxX && isMaxY;
    }

    /// <summary>
    /// 指定の座標がチャンクの範囲内に入っているか調べる
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool IsChunkInside(Vector2Int position)
    {
        bool isMinGreater = 0 <= position.x && 0 <= position.y;
        bool isMaxSmaller = Size.x > position.x && Size.y > position.y;

        return isMinGreater && isMaxSmaller;
    }
}
