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
    [SerializeField]    // シード値
    private uint seed;

    // チャンクの情報を保持しておく辞書型
    private Dictionary<Vector2Int, GameChunk> _gameChunkDictionary = new();
    private CancellationTokenSource _tokenSource;

    private void Start()
    {
        seed = (uint)Random.Range(uint.MinValue, uint.MaxValue);
    }

    public async UniTask ChunksLoad(GameChunk gameChunk, WorldCreatePrinciple worldCreatePrinciple, WorldDecisionerBase[] worldDecidables)
    {
        // ロード用トークンを発行
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

    }

    public async void InstantiateLaterObjects(LetterInstantiateObject[] summonLetterObjects, IChunkInformation manager)
    {
        // ゲームオブジェクトを設置する
        foreach (LetterInstantiateObject laterObject in summonLetterObjects)
        {
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
            Debug.Log($"instantiate", summonObject);

            laterObject.InitalizeAction?.Invoke(summonObject, nearestHit.normal);

            await UniTask.NextFrame();

            if (summonObject.TryGetComponent<IWorldGenerateWaitable>(out _))
            {
                IWorldGenerateWaitable[] worldGenerateWaiters = summonObject.GetComponents<IWorldGenerateWaitable>();

                foreach (IWorldGenerateWaitable worldGenerateWaiter in worldGenerateWaiters)
                {
                    worldGenerateWaiter.OnGenerated(manager);
                    Debug.Log("initalizeRoot");
                }
            }

        }
    }
}