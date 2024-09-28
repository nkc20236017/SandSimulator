using System;
using System.Linq;
using UnityEngine;

public class PieChart : MonoBehaviour
{
	[Header("Data Config")]
	[SerializeField] private BlockDatas blockDatas;
	[SerializeField] private Pie piePrefab;
	
	[Header("Pie Chart Config")]
	[SerializeField] private PieChartData[] pieChartDatas;
	[SerializeField] private int maxPieCount;
	[SerializeField] private int maxTankValue;
	
	private Pie[] _pies;

	private void Start()
	{
		_pies = new Pie[maxPieCount];
		for (var i = 0; i < maxPieCount; i++)
		{
			_pies[i] = Instantiate(piePrefab, transform);
		}
	}

	private void Update()
	{
		if (pieChartDatas.Length == 0) { return; }
		if (_pies.Sum(pie => pie.Value) >= maxTankValue) { return; }
		
		for (var i = 0; i < pieChartDatas.Length; i++)
		{
			if (i >= maxPieCount) { break; }
			
			if (pieChartDatas[i].value == 0)
			{
				_pies[i].ClearPie();
				continue;
			}
			
			var blocktype = pieChartDatas[i].blockType;
			Sprite sprite;
			sprite = blocktype == BlockType.Ore ? blockDatas.GetOre(pieChartDatas[i].oreType).sprite : blockDatas.GetBlock(blocktype).sprite;
			_pies[i].SetBlockType(blocktype, sprite);
			_pies[i].SetValue(pieChartDatas[i].value);
			var value = pieChartDatas.Take(i).Sum(pie => pie.value);
			_pies[i].SetRotation(value * 180 / maxTankValue);
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