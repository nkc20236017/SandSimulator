using UnityEngine;

[CreateAssetMenu(fileName = "New cave combine data 0", menuName = "Config/Cave")]
public class CaveCombineData : ScriptableObject
{
    [SerializeField]    // �m�C�Y�̑傫��
    private Vector2 scale;
    public Vector2 Scale => scale;
    [SerializeField]    // ��ƂȂ�ꏊ�̑傫��
    private float clodSize;
    public float ClodSize => clodSize;
    [SerializeField]
    private float hollowSize;
    public float HollowSize => hollowSize;
    [SerializeField]    // �u���b�N�Ŗ��߂�悤�ɂ���
    private bool isBackfill;
    public bool IsBackfill => isBackfill;
    [SerializeField]
    private bool isInvert;
    public bool IsInvert => isInvert;
    [SerializeField]    // �^�C���}�b�v��ID
    private int backfillTileID = -1;
    public int BackfillTileID => backfillTileID;
}