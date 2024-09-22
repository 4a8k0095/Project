using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lamb : Sheep
{
    public Lamb(GameObject _animalObject, Tile _currentTile, int _step) : base(_animalObject, _currentTile, _step)
    {
        Bonus = 10;
    }

    protected override void GrowUp()
    {
        GameManager.Instance.lambEatEnoughCount++;
        GameManager.Instance.LambGrowUp(this, currentTile);
        Object.Destroy(animalObject);
    }
}