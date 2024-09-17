using System.Collections.Generic;
using UnityEngine;

public struct ManagedRandom
{
    private Random.State _state;

    public ManagedRandom(int seed)
    {
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
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
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

        return result;
    }

    /// <summary>
    /// シード値を元に乱数を生成し取得する
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
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

        return result;
    }
}