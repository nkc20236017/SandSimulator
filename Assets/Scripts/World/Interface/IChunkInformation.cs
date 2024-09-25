using UnityEngine;
using UnityEngine.Tilemaps;

public interface IChunkInformation
{
    /// <summary>
    /// ���[���h���W����`�����N�̍��W�ɕϊ����܂�
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    public Vector3Int WorldToChunk(Vector2 world);

    /// <summary>
    /// �`�����N�̍��W���烏�[���h���W�ɕϊ����܂�
    /// </summary>
    /// <param name="chunkIndex">�`�����N�̍��W</param>
    /// <param name="tilePosition">�`�����N���̃^�C���̍��W</param>
    /// <returns></returns>
    public Vector3 ChunkToWorld(Vector2Int chunkIndex, Vector3Int tilePosition);

    /// <summary>
    /// �w��̍��W���ʒu���Ă�����W���擾����
    /// </summary>
    /// <param name="position">���[���h���W</param>
    /// <returns>�`�����N�̃^�C���}�b�v�I�u�W�F�N�g</returns>
    public Tilemap GetChunkTilemap(Vector2 position);

    /// <summary>
    /// �w��̍��W���ʒu���Ă�����W���擾����
    /// </summary>
    /// <param name="position">���[���h���W</param>
    /// <returns>�`�����N�̃^�C���}�b�v�I�u�W�F�N�g</returns>
    public Tilemap GetChunkTilemap(Vector2 position, Vector2Int chunkVector);

    /// <summary>
    /// �w��̍��W�̑w�ԍ����擾����
    /// </summary>
    /// <param name="position">���[���h���W</param>
    /// <returns>�w�̔ԍ�(�オ1)</returns>
    public int GetLayer(Vector2 position);
}