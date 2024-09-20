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
    [SerializeField]    // ���߂�Ώۂ𔽓]����
    private bool isInvert;
    public bool IsInvert => isInvert;
    [SerializeField]    // �^�C���}�b�v��ID
    private int tileID;
    public int TileID => tileID;
}