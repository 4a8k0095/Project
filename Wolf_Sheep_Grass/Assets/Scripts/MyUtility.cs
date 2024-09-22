using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUtility : MonoBehaviour
{
    static System.Random random;

    public static Tile GetRandomTile(Tile[,] _tiles)
    {
        random = new System.Random(Guid.NewGuid().GetHashCode());

        Tile targetTile;

        do
        {
            int randomX = random.Next(MapManager.Instance.XSize);
            int randomY = random.Next(MapManager.Instance.YSize);

            targetTile = _tiles[randomX, randomY];
        }
        while (!targetTile.walkable);

        targetTile.walkable = false;
        return targetTile;
    }

    public static int GetRandomNum(int min, int max)
    {
        random = new System.Random(Guid.NewGuid().GetHashCode());

        return random.Next(min, max + 1);
    }
}