using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    // 尋找路徑，並回傳路徑清單
    public void FindPath(Tile _startTile, Tile _targetTile, out List<Tile> _path)
    {
        _path = new List<Tile>();

        if (_targetTile == null)
        {
            Debug.LogWarning("Target Tile is Null !");
            return;
        }

        Tile startTile = _startTile;
        Tile targetTile = _targetTile;

        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);

        while (openSet.Count > 0)
        {
            Tile tile = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < tile.fCost || openSet[i].fCost == tile.fCost)
                {
                    if (openSet[i].hCost < tile.hCost)
                        tile = openSet[i];
                }
            }

            openSet.Remove(tile);
            closedSet.Add(tile);

            if (tile == targetTile)
            {
                // RetracePath(startTile, targetTile);
                _path = RetracePath(startTile, targetTile);
                return;
            }

            foreach (Tile neighbour in GetNeighbours(tile))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = tile.gCost + GetDistance(tile, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetTile);
                    neighbour.parent = tile;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    public List<Tile> GetNeighbours(Tile _tile)
    {
        List<Tile> neighbours = new List<Tile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                // _tile.tileX = 0 ~ xSize -1, _tile.tileY = 0 ~ ySize -1
                int checkX = _tile.tileX + x;
                int checkY = _tile.tileY + y;

                // 地圖 = 20 * 20
                // 因此 20 > checkX > 0 為有效範圍, 20 > checkY > 0 為有效範圍
                if (checkX >= 0 && checkX < MapManager.Instance.XSize && 
                    checkY >= 0 && checkY < MapManager.Instance.YSize)
                {
                    // 將周圍找到的格子加入 neighbours List
                    neighbours.Add(MapManager.Instance.tiles[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    private List<Tile> RetracePath(Tile _startTile, Tile _endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = _endTile;

        while (currentTile != _startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Reverse();

        return path;
    }

    public int GetDistance(Tile _tileA, Tile _tileB)
    {
        int disX = Mathf.Abs(_tileA.tileX - _tileB.tileX);
        int disY = Mathf.Abs(_tileA.tileY - _tileB.tileY);

        if (disX > disY)
            return 14 * disY + 10 * (disX - disY);
        return 14 * disX + 10 * (disY - disX);
    }
}
