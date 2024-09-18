using System.Collections.Generic;
using UnityEngine;

public struct ManagedRandom
{
    private int _seed;
    private int _usageCount;
    private Random.State _state;
    public int UsageCount => _usageCount;

    public ManagedRandom(int seed)
    {
        _seed = seed;
        _usageCount = 0;

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
        _usageCount++;

        return result;
    }

    /// <summary>
    /// �V�[�h�l�����ɗ����𐶐����擾����
    /// </summary>
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
        _usageCount++;

        return result;
    }

    /// <summary>
    /// ����̉񐔗����𐶐��������̒l���擾���܂�
    /// </summary>
    public int Order(int orderIndex, int min, int max)
    {
        // ���݂̗�����Ԃ�ێ����Ă��̎��Ԃ̗�����Ԃ�K�p
        Random.State stateTemp = Random.state;
        // ������Ԃ��w�肳�ꂽ�V�[�h�l�ŏ�����
        Random.InitState(_seed);
        // �����擾
        int result = 0;
        for (int i = 0; i < orderIndex + 1; i++)
        {
            result = Random.Range(min, max);
        }

        Random.state = stateTemp;
        return result;
    }

    /// <summary>
    /// ����̉񐔗����𐶐��������̒l���擾���܂�
    /// </summary>
    public float Order(int orderIndex, float min, float max)
    {
        // ���݂̗�����Ԃ�ێ����Ă��̎��Ԃ̗�����Ԃ�K�p
        Random.State stateTemp = Random.state;
        // ������Ԃ��w�肳�ꂽ�V�[�h�l�ŏ�����
        Random.InitState(_seed);
        // �����擾
        float result = 0;
        for (int i = 0; i < orderIndex; i++)
        {
            result = Random.Range(min, max);
        }

        Random.state = stateTemp;
        return result;
    }
}