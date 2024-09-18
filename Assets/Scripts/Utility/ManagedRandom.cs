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

        // 現在の乱数状態を保持するために別変数へ状態を保存
        Random.State stateTemp = Random.state;
        // 乱数状態を指定されたシード値で初期化
        Random.InitState(seed);
        _state = Random.state;
        // 保持した乱数状態を戻す
        Random.state = stateTemp;
    }

    /// <summary>
    /// シード値を元に乱数を生成し取得する
    /// </summary>
    public int Range(int min, int max)
    {
        // 現在の乱数状態を保持してこの実態の乱数状態を適用
        Random.State stateTemp = Random.state;
        Random.state = _state;
        // 乱数取得
        int result = Random.Range(min, max);
        // 状態を更新
        _state = Random.state;
        Random.state = stateTemp;
        _usageCount++;

        return result;
    }

    /// <summary>
    /// シード値を元に乱数を生成し取得する
    /// </summary>
    public float Range(float min, float max)
    {
        // 現在の乱数状態を保持してこの実態の乱数状態を適用
        Random.State stateTemp = Random.state;
        Random.state = _state;
        // 乱数取得
        float result = Random.Range(min, max);
        // 状態を更新
        _state = Random.state;
        Random.state = stateTemp;
        _usageCount++;

        return result;
    }

    /// <summary>
    /// 特定の回数乱数を生成した時の値を取得します
    /// </summary>
    public int Order(int orderIndex, int min, int max)
    {
        // 現在の乱数状態を保持してこの実態の乱数状態を適用
        Random.State stateTemp = Random.state;
        // 乱数状態を指定されたシード値で初期化
        Random.InitState(_seed);
        // 乱数取得
        int result = 0;
        for (int i = 0; i < orderIndex + 1; i++)
        {
            result = Random.Range(min, max);
        }

        Random.state = stateTemp;
        return result;
    }

    /// <summary>
    /// 特定の回数乱数を生成した時の値を取得します
    /// </summary>
    public float Order(int orderIndex, float min, float max)
    {
        // 現在の乱数状態を保持してこの実態の乱数状態を適用
        Random.State stateTemp = Random.state;
        // 乱数状態を指定されたシード値で初期化
        Random.InitState(_seed);
        // 乱数取得
        float result = 0;
        for (int i = 0; i < orderIndex; i++)
        {
            result = Random.Range(min, max);
        }

        Random.state = stateTemp;
        return result;
    }
}