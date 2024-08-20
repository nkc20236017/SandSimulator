using UnityEngine;

public class Test : MonoBehaviour
{
	public float radius = 5f;
	public float distance = 3f;
	public float range = 2f;

	private void OnDrawGizmos()
	{
		var pivot = transform.position;

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(pivot, radius);
		
		Gizmos.color = Color.blue;
		// pivotからmouseWorldPosition方向(direction)にdistance先からdirection方向に左右直角(perpendicular)にdistance右から左のrange先までの線を引く
		var camera = Camera.main;
		if (camera == null) { return; }

		var mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
		var direction = (mouseWorldPosition - pivot).normalized;
		var perpendicular = new Vector3(-direction.y, direction.x, 0);
		var left = pivot + direction * distance + perpendicular * range;
		var right = pivot + direction * distance - perpendicular * range;
		Gizmos.DrawLine(left, right);
	}
}
