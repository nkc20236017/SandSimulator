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
    /// �`�����N�P�ʂ̍��W
    /// </summary>
    public Vector2Int GameChunkPosition => _gameChunkPosition;

    /// <summary>
    /// �`�����N�̑傫��
    /// </summary>
    public Vector2Int Size
    {
        get
        {
            return new Vector2Int(_grid.GetLength(0), _grid.GetLength(1));
        }
    }

    /// <summary>
    /// �`�����N�̊Ǘ����Ă���^�C���}�b�v
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
    /// �w��̃O���b�h�ɂ���u���b�N��ID��Ԃ�
    /// </summary>
    /// <param name="position">�O���b�h�̈ʒu</param>
    /// <returns>�u���b�NID</returns>
    public int GetBlockID(Vector2Int position)
    {
        return _grid[position.x, position.y];
    }

    /// <summary>
    /// �w��̃O���b�h�ɂ���u���b�N��ID��Ԃ�
    /// </summary>
    /// <param name="x">�O���b�h��X�ʒu</param>
    /// <param name="y">�O���b�h��Y�ʒu</param>
    /// <returns>�u���b�NID</returns>
    public int GetBlockID(int x, int y)
    {
        return _grid[x, y];
    }

    /// <summary>
    /// �w��̃O���b�h�Ƀu���b�N��z�u����
    /// </summary>
    /// <param name="x">�O���b�h��X�ʒu</param>
    /// <param name="y">�O���b�h��Y�ʒu</param>
    /// <param name="id">�u���b�NID</param>
    public void SetBlock(int x, int y, int id)
    {
        if (!IsChunkInside(new(x, y))) { return; }
        _grid[x, y] = id;
    }
    /// <summary>
    /// �w��̃O���b�h�Ƀu���b�N��z�u����
    /// </summary>
    /// <param name="position">�O���b�h�̈ʒu</param>
    /// <param name="id">�u���b�NID</param>
    public void SetBlock(Vector2Int position, int id)
    {
        if (!IsChunkInside(position)) { return; }
        _grid[position.x, position.y] = id;
    }

    /// <summary>
    /// �n�w�̔ԍ���ݒ肵�܂�
    /// </summary>
    /// <param name="x">�ݒ肷��O���b�hX�ʒu</param>
    /// <param name="y">�ݒ肷��O���b�hY�ʒu</param>
    /// <param name="index">�n�w�̔ԍ�</param>
    public void SetLayerIndex(int x, int y, int index)
    {
        _layerIndex[x, y] = index;
    }
    /// <summary>
    /// �n�w�̔ԍ����擾���܂�
    /// </summary>
    /// <param name="x">�擾����O���b�hX�ʒu</param>
    /// <param name="y">�擾����O���b�hY�ʒu</param>
    /// <returns></returns>
    public int GetLayerIndex(int x, int y)
    {
        return _layerIndex[x, y];
    }
    /// <summary>
    /// �n�w�̔ԍ����擾���܂�
    /// </summary>
    /// <param name="x">�擾����O���b�hX�ʒu</param>
    /// <param name="y">�擾����O���b�hY�ʒu</param>
    /// <returns></returns>
    public int GetLayerIndex(Vector2Int index)
    {
        return _layerIndex[index.x, index.y];
    }

    /// <summary>
    /// ���_�̂�����l�����Ȃ��`�����N���W�����[���h���W�֕ϊ�
    /// </summary>
    /// <param name="x">�Ώۂ̃`�����N���WX</param>
    /// <param name="y">�Ώۂ̃`�����N���WY</param>
    /// <returns>���[���h���W</returns>
    public Vector2Int RawGameChunkPositionToWorldPosition(int x, int y)
    {
        return RawGameChunkPositionToWorldPosition(new(x, y));
    }
    /// <summary>
    /// ���_�̂�����l�����Ȃ��`�����N���W�����[���h���W�֕ϊ�
    /// </summary>
    /// <param name="position">�Ώۂ̃`�����N���W</param>
    /// <returns>���[���h���W</returns>
    public Vector2Int RawGameChunkPositionToWorldPosition(Vector2Int position)
    {
        return new
        (
            _gameChunkPosition.x * Size.x + position.x,
            _gameChunkPosition.y * Size.y + position.y
        );
    }

    /// <summary>
    /// ���[���h���W�����_�̂�����l�����Ȃ��`�����N���W�ɕϊ�����
    /// </summary>
    /// <param name="position">�Ώۂ̃��[���h���W</param>
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
    /// �ォ�珢������Q�[���I�u�W�F�N�g�̐����擾����
    /// </summary>
    /// <returns></returns>
    public int GetSummonLaterObjectCount()
    {
        return _summonLaterObjects.Count;
    }

    public bool IsInsideChunkPosition(Vector2Int position, int margin = 0)
    {
        // �`�����N��X�ŏ����傫����
        bool isMinX = Size.x * GameChunkPosition.x - margin <= position.x;
        // �`�����N��X�ő��菬������
        bool isMaxX = position.x <= Size.x * GameChunkPosition.x + Size.x + margin;
        // �`�����N��Y�ŏ����傫����
        bool isMinY = Size.y * GameChunkPosition.y - margin <= position.y;
        // �`�����N��Y�ő��菬������
        bool isMaxY = position.y <= Size.y * GameChunkPosition.y + Size.y + margin;

        return isMinX && isMinY && isMaxX && isMaxY;
    }

    /// <summary>
    /// �w��̍��W���`�����N�͈͓̔��ɓ����Ă��邩���ׂ�
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
