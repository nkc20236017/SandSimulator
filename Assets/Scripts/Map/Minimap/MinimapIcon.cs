using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
	[SerializeField] private Camera _minimapCamera;
	
	private GameObject[] _targetIcons;

	/// <summary>
	/// アイコンを表示させるターゲットの設定
	/// </summary>
	/// <param name="targetIcons">アイコンを表示させるターゲット</param>
	public void SetTargets(params GameObject[] targetIcons)
	{
		if (_targetIcons != null)
		{
			foreach (var target in _targetIcons)
			{
				Destroy(target);
			}
		}
		
		_targetIcons = new GameObject[targetIcons.Length];
		for (var i = 0; i < targetIcons.Length; i++)
		{
			_targetIcons[i] = Instantiate(targetIcons[i], transform);
			_targetIcons[i].transform.position = _minimapCamera.WorldToViewportPoint(targetIcons[i].transform.position);
		}
	}

	private void Update()
	{
		if (_targetIcons == null) { return; }
		if (_targetIcons.Length == 0) { return; }
		
		foreach (var target in _targetIcons)
		{
			if (target == null) { continue; }
			
			if (IsInCamera(target))
			{
				
			}
			else
			{
				
			}
		}
	}
	
	private bool IsInCamera(GameObject targetIcon)
	{
		var viewportPoint = _minimapCamera.WorldToViewportPoint(targetIcon.transform.position);
		return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
	}
}
