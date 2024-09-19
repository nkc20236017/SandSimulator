using UnityEngine;

[CreateAssetMenu(fileName = "New cave layer 0", menuName = "Config/CaveLayer")]
public class CaveLayerData : ScriptableObject
{
    private Vector2 scale;
    private float clodSize;
    private bool isInvert;
    private int tileID;
}