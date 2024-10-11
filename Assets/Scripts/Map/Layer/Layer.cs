using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Layer", menuName = "ScriptableObjects/Layer")]
public class Layer : ScriptableObject
{
	public LayerData[] layerData;
}

[Serializable]
public class LayerData
{
	public string layerName;
	public int degreeOfRisk;
}
