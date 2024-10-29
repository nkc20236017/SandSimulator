using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Turtle", menuName = "ScriptableObjects/Data/Characters/Enemies/New Turtle")]
public class Turtle : Enemy
{
	[Header("Ore Config")]
	public TurtleOre[] turtleOres;

	public Ore DropOre()
	{
		var totalRate = turtleOres.Sum(turtleOre => turtleOre.rate);
		var random = Random.Range(0, totalRate);
		var rate = 0;
		foreach (var turtleOre in turtleOres)
		{
			rate += turtleOre.rate;
			if (random < rate)
			{
				return turtleOre.ore;
			}
		}
		
		return null;
	}
}

[Serializable]
public class TurtleOre
{
	public Ore ore;
	public int rate;
}