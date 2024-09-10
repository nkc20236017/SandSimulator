using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public struct QuadTreeTilemap
{
    private const int MaxDepth = 7;
    private const int MaxTilesPerNode = 10;

    private class QuadTreeNode
    {
        public Rect bounds;
        public List<Vector3Int> tilePositions;
        public QuadTreeNode[] children;

        public QuadTreeNode(Rect bounds)
        {
            this.bounds = bounds;
            tilePositions = new List<Vector3Int>();
            children = null;
        }

        public bool IsLeaf()
        {
            return children == null;
        }
    }

    private QuadTreeNode root;
    private Tilemap tilemap;

    public QuadTreeTilemap(Tilemap tilemap)
    {
        this.tilemap = tilemap;
        root = new QuadTreeNode(new Rect(0, 0, 10000, 10000));
        BuildQuadTree();
    }

    private void BuildQuadTree()
    {
        var bounds = tilemap.cellBounds;
        var allTiles = tilemap.GetTilesBlock(bounds);

        for (var x = 0; x < bounds.size.x; x++)
        {
            for (var y = 0; y < bounds.size.y; y++)
            {
                var tile = allTiles[x + y * bounds.size.x];
                if (tile == null) { continue; }

                var tilePosition = new Vector3Int(bounds.x + x, bounds.y + y, 0);
                InsertTile(root, tilePosition, 0);
            }
        }
    }

    private void InsertTile(QuadTreeNode node, Vector3Int tilePosition, int depth)
    {
        if (!node.bounds.Contains(new Vector2(tilePosition.x, tilePosition.y))) { return; }

        if (node.IsLeaf())
        {
            node.tilePositions.Add(tilePosition);

            if (node.tilePositions.Count > MaxTilesPerNode && depth < MaxDepth)
            {
                Split(node);
            }
        }
        else
        {
            for (var i = 0; i < 4; i++)
            {
                InsertTile(node.children[i], tilePosition, depth + 1);
            }
        }
    }

    private static void Split(QuadTreeNode node)
    {
        var halfWidth = node.bounds.width / 2;
        var halfHeight = node.bounds.height / 2;

        node.children = new QuadTreeNode[4];
        node.children[0] = new QuadTreeNode(new Rect(node.bounds.x, node.bounds.y, halfWidth, halfHeight));
        node.children[1] = new QuadTreeNode(new Rect(node.bounds.x + halfWidth, node.bounds.y, halfWidth, halfHeight));
        node.children[2] = new QuadTreeNode(new Rect(node.bounds.x, node.bounds.y + halfHeight, halfWidth, halfHeight));
        node.children[3] = new QuadTreeNode(new Rect(node.bounds.x + halfWidth, node.bounds.y + halfHeight, halfWidth, halfHeight));

        foreach (var tilePosition in node.tilePositions)
        {
            for (var i = 0; i < 4; i++)
            {
                if (!node.children[i].bounds.Contains(new Vector2(tilePosition.x, tilePosition.y))) { continue; }

                node.children[i].tilePositions.Add(tilePosition);
                break;
            }
        }

        node.tilePositions.Clear();
    }

    public List<Vector3Int> GetTilesInArea(Rect area)
    {
        var result = new List<Vector3Int>();
        QueryArea(root, area, result);
        return result;
    }

    private static void QueryArea(QuadTreeNode node, Rect area, List<Vector3Int> result)
    {
        if (!node.bounds.Overlaps(area)) { return; }

        if (node.IsLeaf())
        {
            result.AddRange(node.tilePositions.Where(tilePosition => area.Contains(new Vector2(tilePosition.x, tilePosition.y))));
        }
        else
        {
            for (var i = 0; i < 4; i++)
            {
                QueryArea(node.children[i], area, result);
            }
        }
    }
}