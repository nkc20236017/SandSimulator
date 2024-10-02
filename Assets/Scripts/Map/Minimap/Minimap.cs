using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
	[Header("Minimap Camera")]
	[SerializeField] private MinimapIcon _minimapIcon;
	[SerializeField] private GameObject _targetIconPrefab;
	[SerializeField] private Camera _minimapCamera;
	
	private List<GameObject> _targets = new();
	private List<GameObject> _targetIcons = new();

	/// <summary>
	/// アイコンを表示させるターゲットを追加
	/// </summary>
	/// <param name="minimapIconType">アイコンの種類</param>
	/// <param name="target">アイコンを表示させるターゲット</param>
	public void AddTargetIcons(MinimapIconType minimapIconType, GameObject target)
	{
		Sprite icon = _minimapIcon.GetIcon(minimapIconType);
		
		_targets.Add(target);
		var minimapIcon = Instantiate(_targetIconPrefab, target.transform);
		minimapIcon.GetComponent<Canvas>().worldCamera = Camera.main;
		minimapIcon.GetComponentInChildren<Image>().sprite = icon;
		_targetIcons.Add(minimapIcon);
	}
	
	/// <summary>
	/// 全てのアイコンを削除
	/// </summary>
	private void AllClearMinimapIcon()
	{
		if (_targetIcons == null) { return; }
		if (_targetIcons.Count == 0) { return; }
		
		foreach (var target in _targetIcons)
		{
			Destroy(target);
		}
		_targets.Clear();
		_targetIcons.Clear();
	}

	private void Update()
	{
		if (_targetIcons == null) { return; }
		if (_targetIcons.Count == 0) { return; }
		
		foreach (var target in _targetIcons)
		{
			if (target == null)
			{
				_targetIcons.Remove(target);
				continue;
			}

			if (IsInCamera(_targets[_targetIcons.IndexOf(target)]))
			{
				var position = _targets[_targetIcons.IndexOf(target)].transform.position;
				position.z = 0;
				target.transform.position = position;
			}
			else
			{
				var direction = _targets[_targetIcons.IndexOf(target)].transform.position - _minimapCamera.transform.position;
				var distance = Mathf.Min(_minimapCamera.orthographicSize, direction.magnitude);
				direction = direction.normalized * distance;
				var position = _minimapCamera.transform.position + direction;
				position.z = 0;
				target.transform.position = position;
			}
		}
	}
	
	private bool IsInCamera(GameObject target)
	{
		var viewportPoint = _minimapCamera.WorldToViewportPoint(target.transform.position);
		return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
	}
}
