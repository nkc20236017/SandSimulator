using UnityEngine;

[CreateAssetMenu(fileName = "New cave combine data 0", menuName = "Config/Cave")]
public class CaveCombineData : ScriptableObject
{
    [SerializeField]    // ノイズの大きさ
    private Vector2 scale;
    public Vector2 Scale => scale;
    [SerializeField]    // 塊となる場所の大きさ
    private float clodSize;
    public float ClodSize => clodSize;
    [SerializeField]
    private float hollowSize;
    public float HollowSize => hollowSize;
    [SerializeField]    // 埋める対象を反転する
    private bool isInvert;
    public bool IsInvert => isInvert;
    [SerializeField]    // タイルマップのID
    private int tileID;
    public int TileID => tileID;
}