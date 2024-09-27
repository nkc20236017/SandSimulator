using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New MapCustomRuleTile", menuName = "Tiles/MapCustomRuleTile")]
public class MapCustomRuleTile : RuleTile<MapCustomRuleTile.Neighbor> 
{
    public bool alwaysConnectTile;
    public TileBase[] tilesToConnect;
    public bool alwaysConnectTilemap;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int This = 1;
        public const int NotThis = 2;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) 
    {
        switch (neighbor) 
        {
            case Neighbor.This: return CheckThis(tile);
            case Neighbor.NotThis: return CheckNotThis(tile);
        }
        
        return base.RuleMatch(neighbor, tile);
    }
    
    private bool CheckThis(TileBase tile)
    {
        if (!alwaysConnectTile)
        {
            return tile == this;
        }
        
        return tile == this || tilesToConnect.Contains(tile);
    }
    
    private bool CheckNotThis(TileBase tile) 
    {
        return tile != this && !tilesToConnect.Contains(tile);
    }
}