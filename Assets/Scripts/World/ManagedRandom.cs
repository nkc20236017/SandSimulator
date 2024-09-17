using System.Collections.Generic;
using UnityEngine;

public struct ManagedRandom
{
    private Random.State _state;

    public ManagedRandom(int seed)
    {
        // ���݂̗�����Ԃ�ێ����邽�߂ɕʕϐ��֏�Ԃ�ۑ�
        Random.State stateTemp = Random.state;
        // ������Ԃ��w�肳�ꂽ�V�[�h�l�ŏ�����
        Random.InitState(seed);
        _state = Random.state;
        // �ێ�����������Ԃ�߂�
        Random.state = stateTemp;
    }

    /// <summary>
    /// �V�[�h�l�����ɗ����𐶐����擾����
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public int Range(int min, int max)
    {
        // ���݂̗�����Ԃ�ێ����Ă��̎��Ԃ̗�����Ԃ�K�p
        Random.State stateTemp = Random.state;
        Random.state = _state;
        // �����擾
        int result = Random.Range(min, max);
        // ��Ԃ��X�V
        _state = Random.state;
        Random.state = stateTemp;

        return result;
    }

    /// <summary>
    /// �V�[�h�l�����ɗ����𐶐����擾����
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public float Range(float min, float max)
    {
        // ���݂̗�����Ԃ�ێ����Ă��̎��Ԃ̗�����Ԃ�K�p
        Random.State stateTemp = Random.state;
        Random.state = _state;
        // �����擾
        float result = Random.Range(min, max);
        // ��Ԃ��X�V
        _state = Random.state;
        Random.state = stateTemp;

        return result;
    }
}