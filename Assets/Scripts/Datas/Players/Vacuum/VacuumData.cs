using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "VacuumData", menuName = "ScriptableObjects/Data/Characters/Players/VacuumData")]
public class VacuumData : ScriptableObject
{
	[Header("Vacuum Settings")]
	public string VacuumName;
	[ShowAssetPreview] public Sprite VacuumSprite;
	public int Price;
	
	[Header("Absorption Settings")]
	[Tooltip("吸引角度(0~180°)")] [Range(0f, 180f)]
	public float AbsorptionAngle;
	[Tooltip("吸引距離")] [Min(0f)]
	public float AbsorptionDistance;
	[Tooltip("削除距離")] [Min(0f)]
	public float AbsorptionDeleteDistance;
	[Tooltip("吸い込み速度")] [Min(1f)]
	public float AbsorptionSpeed;
	
	[Header("Eject Settings")]
	[Tooltip("吐き出し範囲")] [Min(0f)]
	public float EjectRadius;
	[Tooltip("吐き出し距離")] [Min(0f)]
	public float EjectDistance;
	[Tooltip("吐き出し範囲の幅")] [Min(0f)]
	public float EjectRange;
	[Tooltip("吐き出し速度")] [Min(1f)]
	public float EjectSpeed = 1;
	[Tooltip("吐き出し高さ")] [Min(0)]
	public int EjectUp = 3;
}
