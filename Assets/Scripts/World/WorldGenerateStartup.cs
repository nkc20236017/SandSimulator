using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorldCreation;

[RequireComponent(typeof(WorldGenerator))]
public class WorldGenerateStartup : MonoBehaviour
{
    [SerializeField]    // ワールドの生成ルール
    private WorldCreatePrinciple worldPrinciple;
    [SerializeField]    // タイルマップの親
    private Transform tilemapParent;
    [SerializeField]    // チャンクプレハブ
    private TilemapRenderer chunkTilemapRenderer;

    private WorldGenerator _worldGenerator;
    private bool _isQuitting = false;
    private WorldDecisionerBase[] mainWorldDecisions = new WorldDecisionerBase[]
    {
        new LayerDecisioner(),
        new CaveDecisioner(),
        new OreDecisioner(),
        new TileInstaller()
    };

    private void Awake()
    {
        _worldGenerator = GetComponent<WorldGenerator>();
    }

    private void Start()
    {
        Vector2 chunkSize = chunkTilemapRenderer.chunkCullingBounds;

        SetupProcess((int)chunkSize.x, (int)chunkSize.y);
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    private async void SetupProcess(int sizeX, int sizeY)
    {
        GameChunk[,] gameChunks
            = new GameChunk[worldPrinciple.WorldSplidCount.x, worldPrinciple.WorldSplidCount.y];

        for (int y = 0; y < worldPrinciple.WorldSplidCount.y; y++)
        {
            for (int x = 0; x < worldPrinciple.WorldSplidCount.x; x++)
            {
                if (_isQuitting)
                {
                    return;
                }

                // チャンクのタイルマップを生成し、コンポーネントを取得する
                // TilemapRendererはTilemapをRequireComponentしているためTilemapがあることが保証される
                Vector3 position = new Vector3(x * sizeX, y * sizeY);
                Tilemap newChunkTilemap
                    = Instantiate(chunkTilemapRenderer, position, Quaternion.identity, tilemapParent)
                        .GetComponent<Tilemap>();

                // ロードに必要なデータを作成する
                gameChunks[x, y] = new GameChunk
                (
                    new Vector2Int(x, y),
                    newChunkTilemap,
                    new Vector2Int(sizeX, sizeY)
                );

                await _worldGenerator.ChunksLoad
                (
                    gameChunks[x, y],
                    worldPrinciple,
                    mainWorldDecisions
                );
            }
        }

        // 召喚するオブジェクトに与える情報を渡す
        WorldMapManager mapManager = new
        (
            gameChunks,
            new Vector2Int((int)chunkTilemapRenderer.chunkCullingBounds.x, (int)chunkTilemapRenderer.chunkCullingBounds.y),
            Vector2.zero
        );

        foreach (GameChunk gameChunk in gameChunks)
        {
            // ゲームオブジェクトを召喚する
            _worldGenerator.InstantiateLaterObjects(gameChunk.SummonLaterObjects.ToArray(), mapManager);
        }
    }
}