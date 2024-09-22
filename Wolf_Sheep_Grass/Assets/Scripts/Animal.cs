using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal
{
    // 儲存全地圖的格子
    private Tile[,] tiles;

    // 儲存找到的路徑
    private List<Tile> path = new List<Tile>();

    protected PathFinding pathFinding;

    // 儲存 小羊 羊 狼 狗 等角色的GameObject
    public GameObject animalObject;
    // 儲存角色當前所在的格子
    public Tile currentTile;
    // 角色移動步數
    protected int step;

    public Animal(GameObject _animalObject, Tile _currentTile, int _step)
    {
        animalObject = _animalObject;
        currentTile = _currentTile;
        step = _step;

        pathFinding = new PathFinding();
        tiles = MapManager.Instance.tiles;
    }

    public virtual void Eat(Tile targetTile)
    {
        Debug.LogWarning("使用時，請覆寫實作內容");
    }

    // 尋找最近的獵物
    // 小羊 羊 => 草
    // 狼 => 小羊 羊
    // 狗 => 狼
    public void FindClosestPrey(List<Tile> _tilesFromPrey, out Tile tileFromPrey)
    {
        int tempDis = int.MaxValue;

        tileFromPrey = null;
        Tile targetTile = null;

        // 比對獵物清單，找出最近的獵物
        for (int i = 0; i < _tilesFromPrey.Count; i++)
        {
            Tile tempTileFromPrey = tiles[_tilesFromPrey[i].tileX, _tilesFromPrey[i].tileY];
            List<Tile> targetTileNeighbours = pathFinding.GetNeighbours(tempTileFromPrey);
            foreach (Tile targetNeighbour in targetTileNeighbours)
            {
                int dis = pathFinding.GetDistance(currentTile, targetNeighbour);
                if (dis < tempDis)
                {
                    tempDis = dis;
                    targetTile = targetNeighbour;
                    tileFromPrey = tempTileFromPrey;
                }
            }
        }

        // 規劃到最近的獵物的路徑
        pathFinding.FindPath(currentTile, targetTile, out path);
    }

    private int playSpeed = 1;
    // 移動
    public IEnumerator Move()
    {
        if (GameManager.Instance.playSpeed == GameManager.PlaySpeed.normal)
            playSpeed = 1;
        else
            playSpeed = 10;

        for (int i = 0; i < step; i++)
        {
            if (path.Count <= 0 )
                break;

            // 更新當前所在的 Tile
            currentTile.walkable = true;

            currentTile = path[0];
            currentTile.walkable = false;
            animalObject.transform.position = currentTile.worldPosition;

            path.Remove(path[0]);
            yield return new WaitForSeconds(0.1f / playSpeed);
        }
    }
}