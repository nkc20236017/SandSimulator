using UnityEngine;
using WorldCreation;

[CreateAssetMenu(fileName = "New enemy decision", menuName = "ScriptableObjects/WorldCreatePrinciple/Decision/Enemy")]
public class EnemyDecisionData : ScriptableObject
{
    [SerializeField]
    private PrimevalEnemy[] enemyProceduresGathering;
    public PrimevalEnemy[] EnemyProceduresGathering => enemyProceduresGathering;
}