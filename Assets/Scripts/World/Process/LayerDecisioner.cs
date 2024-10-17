using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using RandomExtensions;

namespace WorldCreation
{
    public class LayerDecisioner : WorldDecisionerBase
    {
        private int[] _layerBorderRangeHeights; // �\���F[���E���0, ���E���0, ���E���1, ���E���1, ...]
        private int[] _layerHeights;
        private int[] _layerNoise;

        public override void Initalize(GameChunk chunk, WorldCreatePrinciple createPrinciple, IRandom managedRandom)
        {
            // ���������������s
            base.Initalize(chunk, createPrinciple, managedRandom);

            FindingLayerBorder(_createPrinciple.LayerDecision);
        }

        public override async UniTask<GameChunk> Execute(CancellationToken token)
        {
            // �n�w�p�̃f�[�^�擾
            LayerDecisionData layerDecision = _createPrinciple.LayerDecision;
            // ���݂̃`�����N�̉��Ə�̍��W���擾
            int worldLowerHeight = _gameChunk.Size.y * _gameChunk.GameChunkPosition.y;
            int worldUpperHeight = worldLowerHeight + (_gameChunk.Size.y - 1);

            // �n�w���܂����ł���ꏊ�̎擾
            int[] straddles = _layerBorderRangeHeights
                .Where(y => worldLowerHeight <= y && y <= worldUpperHeight)
                .ToArray();

            // �`�����N�̉������ǂ̒n�w�ɑ��݂��Ă��邩�擾����
            int layerIndex = Array.IndexOf
            (
                _layerHeights
                    .Concat(new int[] { worldLowerHeight })
                    .OrderByDescending(y => y)
                    .ToArray(),
                worldLowerHeight
            );

            // �`�����N���܂����ł��Ȃ���Βn�w��ID�œh��Ԃ�
            if (straddles.Length == 0)
            {
                for (int y = 0; y < _gameChunk.Size.y; y++)
                {
                    for (int x = 0; x < _gameChunk.Size.x; x++)
                    {
                        TileBase material = layerDecision.WorldLayers[layerIndex].MaterialTile;
                        _gameChunk.SetBlock
                        (
                            x,
                            y,
                            _createPrinciple.Blocks.GetBlockID(material)
                        );

                        // �`�����N�̒n�w������������
                        _gameChunk.SetLayerIndex(x, y, layerIndex);
                    }
                }

                Debug.Log("<color=#00ff00ff>�n�w�̐��������I��</color>");
                return await UniTask.RunOnThreadPool(() => _gameChunk);
            }

            // �`�����N���ׂ��ł����ꍇ�n�w�̘c�݂𐶐�����
            for (int y = 0; y < _gameChunk.Size.y; y++)
            {
                for (int x = 0; x < _gameChunk.Size.x; x++)
                {
                    // �n�w�̋��E���̈ʒu���擾����
                    Vector2Int worldPosition = _gameChunk.RawGameChunkPositionToWorldPosition(x, y);
                    int borderHeight = GetBorder(_gameChunk, layerDecision, worldPosition.x, layerIndex - 1);

                    borderHeight
                        = _layerHeights[layerIndex - 1]
                        - (int)layerDecision.BorderDistortionPower
                        + borderHeight;

                    TileBase material;
                    if (borderHeight > worldPosition.y)
                    {
                        // �n�w�̋��E��艺�ł���Ε��ʂ̃^�C��
                        material = layerDecision.WorldLayers[layerIndex].MaterialTile;
                        _gameChunk.SetBlock
                        (
                            x,
                            y,
                            _createPrinciple.Blocks.GetBlockID(material)
                        );

                        // �`�����N�̒n�w������������
                        _gameChunk.SetLayerIndex(x, y, layerIndex);
                    }
                    else
                    {
                        material = layerDecision.WorldLayers[layerIndex - 1].MaterialTile;
                        // �n�w�̋��E��艺�ł���Ύ��̃^�C��
                        _gameChunk.SetBlock
                        (
                            x,
                            y,
                            _createPrinciple.Blocks.GetBlockID(material)
                        );

                        // �`�����N�̒n�w������������
                        _gameChunk.SetLayerIndex(x, y, layerIndex - 1);
                    }
                }
            }

            Debug.Log("<color=#00ff00ff>�n�w�̐��������I��</color>");
            return await UniTask.RunOnThreadPool(() => _gameChunk);
        }

        // TODO: �n�w�̔����ʃ��\�b�h�ō��

        private int GetBorder(GameChunk chunk, LayerDecisionData layerDecision, int x, int layerNumber)
        {
            int[] layerNoise = new int[layerDecision.LayerRatios.Length];
            for (int i = 0; i < layerNoise.Length; i++)
            {
                layerNoise[i] = _random.NextInt(Int16.MinValue, Int16.MaxValue);
            }

            return (int)
            (
                Mathf.PerlinNoise1D(x * layerDecision.BorderAmplitude + layerNoise[layerNumber])
                    * layerDecision.BorderDistortionPower
            );
        }

        private void FindingLayerBorder(LayerDecisionData layerDecision)
        {
            int layerMaxRatio = GetWorldScale().y;
            // �n�w�̐����̔z����쐬
            _layerHeights = new int[layerDecision.LayerRatios.Length];
            _layerBorderRangeHeights = new int[layerDecision.LayerRatios.Length * 2];

            for (int i = 0; i < layerDecision.LayerRatios.Length; i++)
            {
                // ���ꂼ��̒n�w�̊��荇�������߂�
                _layerBorderRangeHeights[i]
                    = (int)(layerMaxRatio
                    * (1 - layerDecision.LayerRatios[i]));

                _layerBorderRangeHeights[_layerBorderRangeHeights.Length - (i + 1)]
                    = _layerBorderRangeHeights[i]
                    + (int)layerDecision.BorderDistortionPower;

                _layerHeights[i]
                    = _layerBorderRangeHeights[i]
                    + (int)layerDecision.BorderDistortionPower;

                layerMaxRatio = _layerHeights[i];
            }
            // �n�w�̋��E�����������ɕ��ѕς���
            _layerBorderRangeHeights = _layerBorderRangeHeights.OrderBy(i => i).ToArray();
        }
    }
}
