using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal
{
    // �x�s���a�Ϫ���l
    private Tile[,] tiles;

    // �x�s��쪺���|
    private List<Tile> path = new List<Tile>();

    protected PathFinding pathFinding;

    // �x�s �p�� �� �T �� �����⪺GameObject
    public GameObject animalObject;
    // �x�s�����e�Ҧb����l
    public Tile currentTile;
    // ���Ⲿ�ʨB��
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
        Debug.LogWarning("�ϥήɡA���мg��@���e");
    }

    // �M��̪��y��
    // �p�� �� => ��
    // �T => �p�� ��
    // �� => �T
    public void FindClosestPrey(List<Tile> _tilesFromPrey, out Tile tileFromPrey)
    {
        int tempDis = int.MaxValue;

        tileFromPrey = null;
        Tile targetTile = null;

        // ����y���M��A��X�̪��y��
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

        // �W����̪��y�������|
        pathFinding.FindPath(currentTile, targetTile, out path);
    }

    private int playSpeed = 1;
    // ����
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

            // ��s��e�Ҧb�� Tile
            currentTile.walkable = true;

            currentTile = path[0];
            currentTile.walkable = false;
            animalObject.transform.position = currentTile.worldPosition;

            path.Remove(path[0]);
            yield return new WaitForSeconds(0.1f / playSpeed);
        }
    }
}