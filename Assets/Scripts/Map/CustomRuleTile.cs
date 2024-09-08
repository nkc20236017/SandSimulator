using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Custom Rule Tile")]
public class CustomRuleTile : RuleTile
{
	public TileBase SomeSpecificTile;
	
	public override bool RuleMatch(int neighbor, TileBase otherTile)
	{
		if (neighbor == TilingRuleOutput.Neighbor.This)
		{
			if (otherTile == SomeSpecificTile)
			{
				return true;
			}
		}

		return base.RuleMatch(neighbor, otherTile);
	}
}
