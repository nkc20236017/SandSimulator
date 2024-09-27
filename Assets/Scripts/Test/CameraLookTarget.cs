using UnityEngine;

public class CameraLookTarget : MonoBehaviour
{
	[SerializeField] private Transform target;
	
	private void Update()
	{
		if (target == null) { return; }
		
		transform.position = target.position + new Vector3(0, 0, -10);
	}
}
