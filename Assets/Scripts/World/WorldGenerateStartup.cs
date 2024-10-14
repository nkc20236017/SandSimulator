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
    private WorldDecisionerBase[] mainWorldDecisions = new WorldDecisionerBase[]
    {
        new LayerDecisioner(),
        new CaveDecisioner(),
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

    private async void SetupProcess(int sizeX, int sizeY)
    {
        for (int y = 0; y < worldPrinciple.WorldSplidCount.y; y++)
        {
            for (int x = 0; x < worldPrinciple.WorldSplidCount.x; x++)
            {
                // チャンクのタイルマップを生成し、コンポーネントを取得する
                // TilemapRendererはTilemapをRequireComponentしているためTilemapがあることが保証される
                Vector3 position = new Vector3(x * sizeX, y * sizeY);
                Tilemap newChunkTilemap
                    = Instantiate(chunkTilemapRenderer, position, Quaternion.identity, tilemapParent)
                        .GetComponent<Tilemap>();

                // ロードに必要なデータを作成する
                GameChunk gameChunk = new GameChunk
                (
                    new Vector2Int(x, y),
                    newChunkTilemap,
                    new Vector2Int(sizeX, sizeY)
                );

                await _worldGenerator.ChunksLoad
                (
                    gameChunk,
                    worldPrinciple,
                    mainWorldDecisions
                );
            }
        }
    }
}