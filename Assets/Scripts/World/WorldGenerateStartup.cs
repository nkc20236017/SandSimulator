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
                // �`�����N�̃^�C���}�b�v�𐶐����A�R���|�[�l���g���擾����
                // TilemapRenderer��Tilemap��RequireComponent���Ă��邽��Tilemap�����邱�Ƃ��ۏ؂����
                Vector3 position = new Vector3(x * sizeX, y * sizeY);
                Tilemap newChunkTilemap
                    = Instantiate(chunkTilemapRenderer, position, Quaternion.identity, tilemapParent)
                        .GetComponent<Tilemap>();

                // ���[�h�ɕK�v�ȃf�[�^���쐬����
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