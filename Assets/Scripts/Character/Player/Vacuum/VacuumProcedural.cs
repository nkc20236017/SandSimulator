using UnityEngine;

public class VacuumProcedural : MonoBehaviour
{
    [Header("Procedural Config")]
    [SerializeField] private Transform _startPivot;
    [SerializeField] private Transform _endPivot;

    [Header("Procedural Settings")]
    [SerializeField, Min(0)] private int _boneCount;
    [SerializeField] private Vector2[] _bonePositions;
    
    private void OnValidate()
    {
        if (_boneCount != _bonePositions.Length)
        {
            System.Array.Resize(ref _bonePositions, _boneCount);
        }
    }
}
