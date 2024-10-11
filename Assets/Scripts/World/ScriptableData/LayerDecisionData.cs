using System.Linq;
using UnityEngine;
using WorldCreation;

public class LayerDecisionData
{
    [Header("Layer Propertys")]

    [SerializeField]    // �����_���l�̐U�ꕝ
    [Range(0f, 1f)]
    private float borderAmplitude;
    public float BorderAmplitude => borderAmplitude;
    [SerializeField]    // �n�w�̊���
    [Range(0f, 1f)]
    private float[] layerRatios;
    public float[] LayerRatios => layerRatios;
    [SerializeField]    // �n�w�̋��E���̘c��
    private float borderDistortionPower;
    public float BorderDistortionPower => borderDistortionPower;
    [SerializeField]    // ���ꂼ��̒n�w�̏��
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
    /// �n�w�̐�����v���Ȃ��ꍇ�ɏC������
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
    /// �n�w�̊�����100%�ɒ�������
    /// </summary>
    private void DebugValidateLayerRatio()
    {
        // �ύX���ꂽ�l���擾����
        (int changedIndex, bool isChanged) = GetChangedValue(ref layerRatios, ref layerRatiosOld);
        if (!isChanged) { return; }

        float ratioTotal = layerRatios.Sum();
        // �z��̒ǉ��ɂ��ω��������ꍇ�V�����쐬���ꂽ�v�f���c��̐����ɂ���
        if (changedIndex == -1)
        {
            float layerRespite = 1 - (ratioTotal - layerRatios[layerRatios.Length - 1]);
            if (0 <= layerRespite && layerRespite <= 1)
            {
                layerRatios[layerRatios.Length - 1] = layerRespite;
            }
            return;
        }

        // ���v�l�̕��ς��擾
        float changedRespite = 1 - layerRatios[changedIndex];
        int ignore = layerRatios
            .Where(_ => Mathf.Approximately(0, _))
            .ToArray()
            .Length + 1;
        float otherTotal = 1 - ratioTotal;
        float changedQuantity = otherTotal / (layerRatios.Length - ignore);

        // 1���傫����Α��̒l��������
        for (int i = 0; i < layerRatios.Length; i++)
        {
            // �ύX���ꂽ�v�f�����b�N����Ă���v�f�͒��߂��Ȃ�
            if (changedIndex >= i) { continue; }
            layerRatios[i] += changedQuantity;
            layerRatios[i] = Mathf.Clamp01(layerRatios[i]);
        }

        layerRatiosOld = layerRatios.ToArray();
    }

    private (int value, bool isChanged) GetChangedValue(ref float[] current, ref float[] old)
    {
        // �f�[�^���폜����Ă���ΌÂ����̂𓯊�
        if (current.Length == 0)
        {
            old = new float[0];
            return (0, false);
        }
        // ���݂ƌÂ����̂��قڈꏏ�ł���ΏI��
        if (old.Length != 0 && current.SequenceEqual(old))
        {
            return (0, false);
        }
        // ���݂Ɖߋ��̔z��̒������قȂ�ΐV�K�쐬
        if (old.Length != current.Length)
        {
            old = current.ToArray();
        }

        float ratioTotal = current.Sum();
        // ���v���ق�1�ł���ΏI��
        if (Mathf.Approximately(1, ratioTotal) == true)
        {
            return (0, false);
        }

        // �ω�������ω������ꏊ���擾����
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