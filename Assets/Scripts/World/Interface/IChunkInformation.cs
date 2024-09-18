using UnityEngine;
using UnityEngine.Tilemaps;

public interface IChunkInformation
{
    /// <summary>
    /// �w��̍��W���ʒu���Ă�����W���擾����
    /// </summary>
    /// <param name="position">���[���h���W</param>
    /// <returns>�`�����N�̃^�C���}�b�v�I�u�W�F�N�g</returns>
    public Tilemap GetChunk(Vector2 position);

    /// <summary>
    /// �w��̍��W���ʒu���Ă�����W���擾����
    /// </summary>
    /// <param name="position">���[���h���W</param>
    /// <returns>�`�����N�̃^�C���}�b�v�I�u�W�F�N�g</returns>
    public Tilemap GetChunk(Vector2 position, Vector2Int chunkVector);

    /// <summary>
    /// �w��̍��W�̑w�ԍ����擾����
    /// </summary>
    /// <param name="position">���[���h���W</param>
    /// <returns>�w�̔ԍ�(�オ0)</returns>
    public int GetLayer(Vector2 position);
}