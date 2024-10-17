using System;
using System.Collections.Generic;
using UnityEngine;

public class LetterInstantiateObject
{
    private GameObject prefab;
    public GameObject Prefab => prefab;

    private Vector2 position;
    public Vector2 Position => position;
    private float checkDistance;
    public float CheckDistance => checkDistance;

    private Vector2Int[] checkDirections;
    public Vector2Int[] CheckDirections => checkDirections;

    private LayerMask groundLayerMask;
    public LayerMask GroundLayerMask => groundLayerMask;

    private Action<GameObject, Vector3> initalizeAction;
    public Action<GameObject, Vector3> InitalizeAction => initalizeAction;

    public LetterInstantiateObject(GameObject prefab, Vector3 position, Vector2Int[] checkDirections, LayerMask groundLayerMask, Action<GameObject, Vector3> initalizeAction, float checkDistance)
    {
        this.prefab = prefab;
        this.position = position;
        this.checkDirections = checkDirections;
        this.groundLayerMask = groundLayerMask;
        this.initalizeAction = initalizeAction;
        this.checkDistance = checkDistance;
    }
}