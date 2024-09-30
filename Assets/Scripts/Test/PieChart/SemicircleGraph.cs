using System;
using System.Linq;
using UnityEngine;

public class SemicircleGraph : MonoBehaviour
{
	[Header("Data Config")]
	[SerializeField] private BlockDatas blockDatas;
	[SerializeField] private Semicircle _semicirclePrefab;
	
	[Header("Semicircle Chart Config")]
	[SerializeField] private PieChartData[] pieChartDatas;
	[SerializeField] private int maxPieCount;
	[SerializeField] private int maxTankValue;
	
	private Semicircle[] _pies;

	private void Start()
	{
		SemicircleGenerate();
	}

	private void Update()
	{
		var totalValue = pieChartDatas.Sum(data => data.value);
		if (totalValue > maxTankValue) { return; }
		
		var pieCount = Mathf.Min(maxPieCount, pieChartDatas.Length);
		
		if (_pies.Length < pieCount)
		{
			SemicircleGenerate();
		}
		
		SetPie(pieCount, totalValue);
	}

	private void SetPie(int pieCount, int totalValue)
	{
		var maxAngle = Mathf.Min(180f, totalValue / (float)maxTankValue * 180f);
		var angle = 0f;
		for (var i = 0; i < pieCount; i++)
		{
			var pie = _pies[i];
			var data = pieChartDatas[i];
			if (data.value <= 0) { continue; }
			
			var ratio = data.value / (float)totalValue;
			var pieAngle = ratio * maxAngle;
			var sprite = data.blockType == BlockType.Ore ? blockDatas.GetOre(pieChartDatas[i].oreType).sprite : blockDatas.GetBlock(data.blockType).sprite;
			pie.SetBlockType(data.blockType, sprite);
			pie.SetValue(data.value);
			pie.SetRotation(angle);
			pie.SetFillAmount(data.value / (float)maxTankValue * 0.5f);
			angle += pieAngle;
		}
	}

	private void SemicircleGenerate()
	{
		ClearPies();
		
		_pies = new Semicircle[maxPieCount];
		for (var i = 0; i < maxPieCount; i++)
		{
			_pies[i] = Instantiate(_semicirclePrefab, transform);
		}
	}
	
	private void ClearPies()
	{
		foreach (GameObject child in transform)
		{
			Destroy(child);
		}
	}
}

[Serializable]
public class PieChartData
{
	public BlockType blockType;
	public OreType oreType;
	public MagicOreType magicOreType;
	public int value;
}