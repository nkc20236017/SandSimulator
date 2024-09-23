using UnityEngine;
using NaughtyAttributes;

public class DecisionWidthDetectPlayer : MonoBehaviour
{
	[Header("DecisionAttackTarget Config")]
	[SerializeField] private Transform pivot;
	[SerializeField, MinValue(0)] private float radius;
	[SerializeField, MinValue(0)] private float range;
	[SerializeField, MinValue(0), MaxValue(360)] private float direction;
	
	[Header("Debug Config")]
	[SerializeField] private bool debugMode;
	
	private void OnDrawGizmosSelected()
	{
		if (!debugMode) { return; }
		if (pivot == null) { return; }
		
		range = Mathf.Clamp(range, 0, radius);
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(pivot.position, radius);
		
		var circumference = GetNewCell(-direction * Mathf.Deg2Rad, range);

		var dir = circumference - pivot.position;
		
		var targetPosition = pivot.position + dir.normalized * 0;
		Gizmos.DrawLine(pivot.position, targetPosition);
		
		var angle = Mathf.Atan2(dir.y, dir.x);
		
		var chordLength = Mathf.Sqrt(Mathf.Pow(0, 2) + Mathf.Pow(range, 2));
		var angle2 = Mathf.Acos(0 / chordLength);
		var newCell1 = GetNewCell(angle - angle2, chordLength);
		var newCell2 = GetNewCell(angle + angle2, chordLength);

		Gizmos.color = Color.red;
		Gizmos.DrawLine(newCell1, newCell2);
		Gizmos.DrawWireSphere(pivot.position, chordLength);
		
		var chordLength2 = Mathf.Sqrt(Mathf.Pow(0, 2) + Mathf.Pow(range, 2));
		var angle3 = Mathf.Acos(0 / chordLength2);
		var h = radius * (1 - Mathf.Cos(angle3));
		var y = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(range, 2)) - (radius - h) - 0;
		
		var newX3 = y * Mathf.Cos(angle);
		var newY3 = y * Mathf.Sin(angle);
		var newDirection3 = new Vector3(newX3, newY3, 0);
		
		var newCell3 = newCell1 + newDirection3;
		Gizmos.DrawLine(newCell1, newCell3);
		
		var newCell4 = newCell2 + newDirection3;
		Gizmos.DrawLine(newCell2, newCell4);
	}

	private Vector3 GetNewCell(float angle, float chordLength)
	{
		var newX = chordLength * Mathf.Cos(angle);
		var newY = chordLength * Mathf.Sin(angle);
		var newDirection = new Vector3(newX, newY, 0);
		var newCell = pivot.position + newDirection;
		return newCell;
	}
}

