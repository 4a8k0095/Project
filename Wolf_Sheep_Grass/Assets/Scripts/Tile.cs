using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用於尋路
[System.Serializable]
public class Tile
{
    // 格子x,y座標
    public int tileX;
    public int tileY;
    // 格子是否可通行
    public bool walkable;
    // 格子的世界座標
    public Vector3 worldPosition;

    public int gCost;
    public int hCost;
    public Tile parent;

    // Tile 本身
    public GameObject tileObject;

    public Tile(bool _walkable, Vector3 _worldPos, int _tileX, int _tileY, GameObject _tileObject)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        tileX = _tileX;
        tileY = _tileY;
        tileObject = _tileObject;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
