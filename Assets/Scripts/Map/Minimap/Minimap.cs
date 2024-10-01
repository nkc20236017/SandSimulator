using UnityEngine;

public class Minimap : MonoBehaviour
{
	[SerializeField] private Camera _minimapCamera;
	
	private GameObject[] _target;

	/// <summary>
	/// アイコンを表示させるターゲットの設定
	/// </summary>
	/// <param name="target">アイコンを表示させるターゲット</param>
	public void SetTargets(params GameObject[] target)
	{
		_target = target;
	}

	private void Update()
	{
		if (_target == null) { return; }
		if (_target.Length == 0) { return; }
		
		foreach (var target in _target)
		{
			if (target == null) { continue; }
			
			if (!IsInCamera(target))
			{
				
			}
		}
	}
	
	private bool IsInCamera(GameObject target)
	{
		var viewportPoint = _minimapCamera.WorldToViewportPoint(target.transform.position);
		return viewportPoint.x >= -135 && viewportPoint.x <= 135 && viewportPoint.y >= -135 && viewportPoint.y <= 135;
	}
}

