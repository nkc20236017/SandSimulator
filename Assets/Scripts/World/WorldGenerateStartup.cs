using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorldCreation;

[RequireComponent(typeof(WorldGenerator))]
public class WorldGenerateStartup : MonoBehaviour
{
    [SerializeField]    // ���[���h�̐������[��
    private WorldCreatePrinciple worldPrinciple;
    [SerializeField]    // �^�C���}�b�v�̐e
    private Transform tilemapParent;
    [SerializeField]    // �`�����N�v���n�u
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

                // �`�����N�̃^�C���}�b�v�𐶐����A�R���|�[�l���g���擾����
                // TilemapRenderer��Tilemap��RequireComponent���Ă��邽��Tilemap�����邱�Ƃ��ۏ؂����
                Vector3 position = new Vector3(x * sizeX, y * sizeY);
                Tilemap newChunkTilemap
                    = Instantiate(chunkTilemapRenderer, position, Quaternion.identity, tilemapParent)
                        .GetComponent<Tilemap>();

                // ���[�h�ɕK�v�ȃf�[�^���쐬����
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

        // ��������I�u�W�F�N�g�ɗ^�������n��
        WorldMapManager mapManager = new
        (
            gameChunks,
            new Vector2Int((int)chunkTilemapRenderer.chunkCullingBounds.x, (int)chunkTilemapRenderer.chunkCullingBounds.y),
            Vector2.zero
        );

        foreach (GameChunk gameChunk in gameChunks)
        {
            // �Q�[���I�u�W�F�N�g����������
            _worldGenerator.InstantiateLaterObjects(gameChunk.SummonLaterObjects.ToArray(), mapManager);
        }
    }
}