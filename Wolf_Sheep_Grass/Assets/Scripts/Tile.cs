using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �Ω�M��
[System.Serializable]
public class Tile
{
    // ��lx,y�y��
    public int tileX;
    public int tileY;
    // ��l�O�_�i�q��
    public bool walkable;
    // ��l���@�ɮy��
    public Vector3 worldPosition;

    public int gCost;
    public int hCost;
    public Tile parent;

    // Tile ����
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
