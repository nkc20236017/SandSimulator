using UnityEngine;
using WorldCreation;

public class OreDecisionData : ScriptableObject
{
    [SerializeField]    // �΂�
    private int bias;
    public int Bias => bias;
    [SerializeField]    // �z�Γ��m�̊Ԋu
    private float space;
    public float Space => space;
    [SerializeField]    // �z�΂��Ƃ̏��
    private PrimevalOre[] primevalOres;
}