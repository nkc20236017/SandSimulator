using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Experimental.GraphView.GraphView;

[Serializable]
public struct WorldLayer
{
    [SerializeField]
    private TileBase fillingTile;               // ���߂�p�[�e�B�N���^�C��(�󂾂ƍ폜����)
    [SerializeField]
    private Vector2Int minImpactAreaPosition;   // ���̃��C���[���e����^����G���A�̍ŏ����W
    [SerializeField]
    private Vector2Int maxImpactAreaPosition;   // ���̃��C���[���e����^����G���A�̍ő���W
    [SerializeField]
    private Vector2 seed;                       // �������郏�[���h�̃V�[�h�l
    [SerializeField]
    private float frequency;                    // �ό`����p�x(�l��傫������Ɖ򂪍ׂ����Ȃ�)
    [SerializeField, Range(0f, 1f)]
    private float extent;                       // ��؂�l(�l��傫������ƒʘH�̕����L���Ȃ�)

    public TileBase FillingTile { get => fillingTile; }
    public Vector2Int MinImpactAreaPosition { get => minImpactAreaPosition; }
    public Vector2Int MaxImpactAreaPosition { get => maxImpactAreaPosition; }
    public Vector2 Seed { get => seed; }
    public float Frequency { get => frequency; }
    public float Extent { get => extent; }
}

public class DemoVisualize : MonoBehaviour
{
    [SerializeField]
    private int solo;
    [SerializeField]
    private WorldLayer[] _worldLayers;
    [SerializeField]
    private Tilemap worldTilemap;
    [SerializeField]
    private TileBase tileBase;

    private IWorldGeneratable worldGenerator;

    private void OnValidate()
    {
        worldGenerator = new CaveGenerater(worldTilemap);
    }

    private void Start()
    {
        worldGenerator = new CaveGenerater(worldTilemap);
        worldGenerator.Execute(_worldLayers);
    }

    [ContextMenu("World/Generate")]
    public void Generate()
    {
        WorldLayer[] worldLayers = _worldLayers;
        if (0 <= solo)
        {
            worldLayers = new WorldLayer[] { _worldLayers[solo] };
        }
        Reset();
        worldGenerator.Execute(worldLayers);
    }

    [ContextMenu("World/Reset")]
    public void Reset()
    {
        // �J�n���W�ƏI�����W�𖾎��I�ɐ錾
        Vector2Int StartPosition = _worldLayers[0].MinImpactAreaPosition;
        Vector2Int EndPosition = _worldLayers[0].MinImpactAreaPosition + _worldLayers[0].MaxImpactAreaPosition;

        for (int x = StartPosition.x; x < EndPosition.x; x++)
        {
            for (int y = StartPosition.y; y < EndPosition.y; y++)
            {
                worldTilemap.SetTile(new Vector3Int(x, y), tileBase);
            }
        }
    }
}
