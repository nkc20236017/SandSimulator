using System.Collections.Generic;
using UnityEngine;
using WorldCreation;

[CreateAssetMenu(fileName = "New ore decision", menuName = "ScriptableObjects/WorldCreatePrinciple/Decision/Ore")]
public class OreDecisionData : ScriptableObject
{
    [SerializeField]    // �΂�
    private int bias;
    public int Bias => bias;
    [SerializeField]    // �z�Γ��m�̊Ԋu
    private int space;
    public int Space => space;
    [SerializeField]
    private GameObject orePrefab;
    public GameObject OrePrefab => orePrefab;
    [SerializeField]    // �z�΂��Ƃ̏��
    private PrimevalOre[] primevalOres;
    public IReadOnlyCollection<PrimevalOre> PrimevalOres => primevalOres;
}