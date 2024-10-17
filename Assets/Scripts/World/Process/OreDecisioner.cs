using Cysharp.Threading.Tasks;
using RandomExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using WorldCreation;

public class OreDecisioner : DistributionDecisioner
{
    private List<Vector2Int> _oreCircleGrids = new();
    private List<Vector2Int> _oreGrids = new();
    private List<Vector2Int> _hitPositions = new();
    private Vector2Int[] _directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public override async UniTask<GameChunk> Execute(CancellationToken token)
    {
        // �����̒n�w�����ɍz�Ώ����擾����
        int indexX = _gameChunk.Size.x / 2;
        int indexY = _gameChunk.Size.y / 2;
        WorldLayer[] worldLayers
            = _createPrinciple
            .LayerDecision
            .WorldLayers;

        Vector2Int[] noisePoints = BlueNoise
        (
            GetWorldScale(),
            worldLayers[_gameChunk.GetLayerIndex(indexX, indexY)].OreDecision
        );
        // �擾�����m�C�Y�������ɍz�΂�z�u����
        foreach (Vector2Int noisePoint in noisePoints)
        {
            // �z�΂̏����擾����
            Vector2Int chunkPosition = _gameChunk.WorldPositionToRawGameChunkPosition(noisePoint);
            OreDecisionData oreDecision
                = worldLayers[_gameChunk.GetLayerIndex(chunkPosition)].OreDecision;
            // �z�΂̎�ނ����肷��
            PrimevalOre chosenOre = GetOre(oreDecision);

            // �z�΂𐶐����Ȃ��ꏊ�̏ꍇ�͎��̃��[�v��
            if (chosenOre.ExposedOreData == null || chosenOre.BuriedOre == null)
            { continue; }

            if (chosenOre.Chipped <= _random.NextFloat(0, 100))
            {
                continue;
            }

            /* ���ݒn����C���̏ꍇ */
            if (_gameChunk.IsInsideChunkPosition(noisePoint) && _gameChunk.GetBlockID(chunkPosition) == _createPrinciple.Blocks.AirIndex)
            {
                // �z�΃I�u�W�F�N�g�𐶐�����\�������
                LetterInstantiateObject letterObject = new
                (
                    oreDecision.OrePrefab,
                    (Vector2)noisePoint,
                    _directions,
                    chosenOre.GroundLayerMask,
                    OnInstantiated,
                    chosenOre.Distance
                );

                _gameChunk.SummonLaterObjects.Add(letterObject);
                continue;
            }

            /* ���ݒn���n���̏ꍇ */
            // �z�u���邽�߂̎��O����
            if (_gameChunk.IsInsideChunkPosition(noisePoint, chosenOre.MaxRadius))
            {
                int radius = _random.NextInt(chosenOre.MinRadius, chosenOre.MaxRadius);
                _oreGrids.AddRange(GetInsideCircleGrid(radius, noisePoint));

                // ���߂��S�Ă̓_���`�����N�ɔz�u����
                foreach (Vector2Int point in _oreGrids)
                {
                    SetBuriedOre(point, chosenOre);
                }
            }
        }

        return await UniTask.RunOnThreadPool(() => _gameChunk);
    }

    private void OnInstantiated(GameObject thisObject, Vector3 normal)
    {
        OreObject oreObject;
        if (thisObject.TryGetComponent(out oreObject))
        {
            WorldLayer[] worldLayers
            = _createPrinciple
            .LayerDecision
            .WorldLayers;

            Vector2Int chunkPosition = _gameChunk.WorldPositionToRawGameChunkPosition(thisObject.transform.position);
            OreDecisionData oreDecision
                = worldLayers[_gameChunk.GetLayerIndex(chunkPosition)].OreDecision;

            float angle = 0;
            // 0, 1
            if ((-0.5f < normal.x && normal.x < 0.5f) && 0.5f < normal.y)
            {
                angle = 0;
            }
            // 1, 0
            else if (0.5f < normal.x && (-0.5f < normal.y && normal.y < 0.5f))
            {
                angle = 270;
            }
            // 0, -1
            else if ((-0.5f < normal.x && normal.x < 0.5f) && normal.y < -0.5f)
            {
                angle = 180;
            }
            // -1, 0
            else if (normal.x < -0.5f && (-0.5f < normal.y && normal.y < 0.5f))
            {
                angle = 90;
            }
            else
            {
                // �z�΂�ݒu���Ȃ�
                return;
            }

            // �z�΂̃f�[�^���Z�b�g�ł���ꍇ�ݒ肷��
            oreObject.SetOre(GetOre(oreDecision).ExposedOreData, _random.NextInt(1, 4), angle);
        }
    }

    private void SetBuriedOre(Vector2Int point, PrimevalOre primevalOre)
    {
        if (_gameChunk.IsInsideChunkPosition(point))
        {
            Vector2Int chunkPoint = _gameChunk.WorldPositionToRawGameChunkPosition((Vector2)point);

            if (_gameChunk.GetBlockID(chunkPoint) == _createPrinciple.Blocks.AirIndex)
            {
                return;
            }

            int id = _createPrinciple.Blocks.GetBlockID(primevalOre.BuriedOre);
            // �`�����N�͈͓̔��ł���Ώ�������
            _gameChunk.SetBlock(chunkPoint, id);
        }
    }

    private PrimevalOre GetOre(OreDecisionData oreDecision)
    {
        // �z�Ώ����m�����ɕ��ёւ�
        PrimevalOre[] primevalOres = oreDecision
            .PrimevalOres
            .OrderByDescending(ore => ore.Probability)
            .ToArray();

        foreach (PrimevalOre ore in primevalOres)
        {
            // 100%�̒��Ƀq�b�g�����ꍇ�Y���̍z�΂��̗p����
            if (ore.Probability >= _random.NextFloat(0, 100))
            {
                return ore;
            }
        }

        // �S�Ă̊m�������蔲�����ꍇ�f�t�H���g�̃f�[�^��n��
        return new PrimevalOre();
    }

    private Vector2Int[] GetInsideCircleGrid(int radius, Vector2Int center)
    {
        _oreCircleGrids.Clear();

        for (int y = center.y - radius; y <= center.y + radius; y++)
        {
            for (int x = center.x - radius; x <= center.x + radius; x++)
            {
                Vector2Int point = new Vector2Int(x, y);
                Vector2Int relativePoint = point - center;
                if (relativePoint.sqrMagnitude <= radius * radius)
                {
                    _oreCircleGrids.Add(point);
                }
            }
        }

        return _oreCircleGrids.ToArray();
    }
}
