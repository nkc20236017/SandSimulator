using UnityEngine;
using WorldCreation;

[CreateAssetMenu(fileName = "New cave decision", menuName = "ScriptableObjects/WorldCreatePrinciple/Decision/Cave")]
public class CaveDecisionData : ScriptableObject
{
    [SerializeField]
    private CaveProcedures[] caveProceduresGathering;
    public CaveProcedures[] CaveProceduresGathering => caveProceduresGathering;
}