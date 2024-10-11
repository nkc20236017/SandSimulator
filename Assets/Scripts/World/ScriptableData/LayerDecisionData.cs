using System.Linq;
using UnityEngine;
using WorldCreation;

public class LayerDecisionData
{
    [Header("Layer Propertys")]

    [SerializeField]    // ランダム値の振れ幅
    [Range(0f, 1f)]
    private float borderAmplitude;
    public float BorderAmplitude => borderAmplitude;
    [SerializeField]    // 地層の割合
    [Range(0f, 1f)]
    private float[] layerRatios;
    public float[] LayerRatios => layerRatios;
    [SerializeField]    // 地層の境界線の歪み
    private float borderDistortionPower;
    public float BorderDistortionPower => borderDistortionPower;
    [SerializeField]    // それぞれの地層の状態
    private WorldLayer[] worldLayers;
    public WorldLayer[] WorldLayers => worldLayers;


#if UNITY_EDITOR
    private float[] layerRatiosOld = new float[0];

    private void OnValidate()
    {
        DebugValidateNumberOfLayer();

        DebugValidateLayerRatio();
    }

    /// <summary>
    /// 地層の数が一致しない場合に修正する
    /// </summary>
    private void DebugValidateNumberOfLayer()
    {
        if (worldLayers.Length != layerRatios.Length + 1)
        {
            WorldLayer[] worldLayersTemp = new WorldLayer[layerRatios.Length + 1];
            for (int i = 0; i < layerRatios.Length + 1; i++)
            {
                if (worldLayers.Length <= i) { break; }
                worldLayersTemp[i] = worldLayers[i];
            }
            worldLayers = worldLayersTemp;
        }
    }

    /// <summary>
    /// 地層の割合を100%に調整する
    /// </summary>
    private void DebugValidateLayerRatio()
    {
        // 変更された値を取得する
        (int changedIndex, bool isChanged) = GetChangedValue(ref layerRatios, ref layerRatiosOld);
        if (!isChanged) { return; }

        float ratioTotal = layerRatios.Sum();
        // 配列の追加による変化だった場合新しく作成された要素を残りの数字にする
        if (changedIndex == -1)
        {
            float layerRespite = 1 - (ratioTotal - layerRatios[layerRatios.Length - 1]);
            if (0 <= layerRespite && layerRespite <= 1)
            {
                layerRatios[layerRatios.Length - 1] = layerRespite;
            }
            return;
        }

        // 合計値の平均を取得
        float changedRespite = 1 - layerRatios[changedIndex];
        int ignore = layerRatios
            .Where(_ => Mathf.Approximately(0, _))
            .ToArray()
            .Length + 1;
        float otherTotal = 1 - ratioTotal;
        float changedQuantity = otherTotal / (layerRatios.Length - ignore);

        // 1より大きければ他の値を下げる
        for (int i = 0; i < layerRatios.Length; i++)
        {
            // 変更された要素かロックされている要素は調節しない
            if (changedIndex >= i) { continue; }
            layerRatios[i] += changedQuantity;
            layerRatios[i] = Mathf.Clamp01(layerRatios[i]);
        }

        layerRatiosOld = layerRatios.ToArray();
    }

    private (int value, bool isChanged) GetChangedValue(ref float[] current, ref float[] old)
    {
        // データが削除されていれば古いものを同期
        if (current.Length == 0)
        {
            old = new float[0];
            return (0, false);
        }
        // 現在と古いものがほぼ一緒であれば終了
        if (old.Length != 0 && current.SequenceEqual(old))
        {
            return (0, false);
        }
        // 現在と過去の配列の長さが異なれば新規作成
        if (old.Length != current.Length)
        {
            old = current.ToArray();
        }

        float ratioTotal = current.Sum();
        // 合計がほぼ1であれば終了
        if (Mathf.Approximately(1, ratioTotal) == true)
        {
            return (0, false);
        }

        // 変化したら変化した場所を取得する
        int changedIndex = -1;
        for (int i = 0; i < current.Length; i++)
        {
            if (Mathf.Approximately(current[i], old[i]) == false)
            {
                changedIndex = i;
                break;
            }
        }

        return (changedIndex, true);
    }
#endif
}