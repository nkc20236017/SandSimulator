using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WorldCreation;
using RandomExtensions;
using Unity.VisualScripting;
using System.Linq;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]    // �V�[�h�l
    private uint seed;

    // �`�����N�̏���ێ����Ă��������^
    private Dictionary<Vector2Int, GameChunk> _gameChunkDictionary = new();
    private CancellationTokenSource _tokenSource;

    private void Start()
    {
        seed = (uint)Random.Range(uint.MinValue, uint.MaxValue);
    }

    public async UniTask ChunksLoad(GameChunk gameChunk, WorldCreatePrinciple worldCreatePrinciple, WorldDecisionerBase[] worldDecidables)
    {
        // ���[�h�p�g�[�N���𔭍s
        _tokenSource = new();

        foreach (WorldDecisionerBase worldDecision in worldDecidables)
        {
            worldDecision.Initalize
            (
                gameChunk,
                worldCreatePrinciple,
                new Xoshiro256StarStarRandom(seed)
            );
            await worldDecision.Execute(_tokenSource.Token);
        }

        // �Q�[���I�u�W�F�N�g��ݒu����
        for (int i = 0; i < gameChunk.GetSummonLaterObjectCount(); i++)
        {
            var laterObject = gameChunk.SummonLaterObjects[i];

            RaycastHit2D[] hits = new RaycastHit2D[laterObject.CheckDirections.Length];

            for (int j = 0; j < laterObject.CheckDirections.Length; j++)
            {
                RaycastHit2D hit = Physics2D.Raycast(laterObject.Position, laterObject.CheckDirections[j], laterObject.CheckDistance, laterObject.GroundLayerMask);
                hits[j] = (hit) ? hit : default;
            }

            RaycastHit2D nearestHit = hits
                .Where(hit => hit)
                .OrderBy(point => (point.point - laterObject.Position).magnitude)
                .FirstOrDefault();

            if (nearestHit == default) { continue; }

            GameObject summonObject = Instantiate(laterObject.Prefab, nearestHit.point, Quaternion.identity, transform);

            laterObject.InitalizeAction?.Invoke(summonObject, nearestHit.normal);

            await UniTask.NextFrame();
        }
    }
}