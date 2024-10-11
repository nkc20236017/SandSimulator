using UnityEngine;
using UnityEngine.Tilemaps;
using WorldCreation;

[RequireComponent(typeof(WorldGenerator))]
public class WorldGenerateStartup : MonoBehaviour
{
    [SerializeField]
    private WorldCreatePrinciple worldPrinciple;
    [SerializeField]
    private TilemapRenderer chunkTilemapRenderer;

    private WorldGenerator _worldGenerator;
    private WorldDecisionerBase[] mainWorldDecisions = new WorldDecisionerBase[]
    {
        new LayerDecisioner(),
    };

    private void Awake()
    {
        _worldGenerator = GetComponent<WorldGenerator>();
    }

    private void Start()
    {
        Vector2 chunkSize = chunkTilemapRenderer.chunkCullingBounds;

        for (int y = 0; y < worldPrinciple.WorldSplidCount.y; y++)
        {
            for (int x = 0; x < worldPrinciple.WorldSplidCount.x; x++)
            {
                // �`�����N�̃^�C���}�b�v�𐶐����A�R���|�[�l���g���擾����
                // TilemapRenderer��Tilemap��RequireComponent���Ă��邽��Tilemap�����邱�Ƃ��ۏ؂����
                Tilemap newChunkTilemap
                    = Instantiate(chunkTilemapRenderer)
                    .GetComponent<Tilemap>();

                // ���[�h�ɕK�v�ȃf�[�^���쐬����
                GameChunk gameChunk = new GameChunk
                (
                    new Vector2Int(x, y),
                    newChunkTilemap,
                    new Vector2Int((int)chunkSize.x, (int)chunkSize.y)
                );

                _worldGenerator.ChunksLoad
                (
                    gameChunk,
                    worldPrinciple,
                    mainWorldDecisions
                );
            }
        }
    }
}