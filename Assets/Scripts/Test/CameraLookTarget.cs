using UnityEngine;

public class CameraLookTarget : MonoBehaviour
{
	[SerializeField] private Transform target;
	
	private void Update()
	{
		transform.LookAt(target);
	}
}

