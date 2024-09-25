using System;
using UnityEngine;
using UnityEngine.UI;

public class TankPieChart : MonoBehaviour
{
	[Header("Tank Pie Chart Config")]
	[SerializeField] private Image wedgePrefab;
	[SerializeField] private float totalValue;
	[SerializeField] private Wedge[] wedges;
	
	private void Start()
	{
		CreateWedges();
	}

	private void CreateWedges()
	{
		var totalAngle = 0f;
		foreach (var wedge in wedges)
		{
			var wedgeImage = Instantiate(wedgePrefab, transform);
			wedgeImage.color = wedge.color;
			wedgeImage.fillAmount = wedge.value / totalValue;
			wedgeImage.transform.rotation = Quaternion.Euler(0, 0, totalAngle);
			totalAngle += wedge.value / totalValue * 360;
		}
	}
}

[Serializable]
public class Wedge : MonoBehaviour
{
	public float value;
	public Color color;
}