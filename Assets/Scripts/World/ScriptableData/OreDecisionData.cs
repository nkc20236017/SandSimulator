using UnityEngine;
using WorldCreation;

public class OreDecisionData : ScriptableObject
{
    [SerializeField]    // •Î‚è
    private int bias;
    public int Bias => bias;
    [SerializeField]    // zÎ“¯m‚ÌŠÔŠu
    private float space;
    public float Space => space;
    [SerializeField]    // zÎ‚²‚Æ‚Ìî•ñ
    private PrimevalOre[] primevalOres;
}