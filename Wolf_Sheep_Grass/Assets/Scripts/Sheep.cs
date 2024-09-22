using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sheep : Animal
{
    protected int eatCount = 0;
    private int bonus = 5;
    protected int Bonus
    {
        get { return bonus; }
        set { bonus = value; }
    }

    public Sheep(GameObject _animalObject, Tile _currentTile, int _step) : base(_animalObject, _currentTile, _step)
    {
    }

    // 吃草
    // _grassTile = 草所在的格子
    public override void Eat(Tile _grassTile)
    {
        if (_grassTile == null)
            return;

        int dis = pathFinding.GetDistance(currentTile, _grassTile);

        // dis = 10 or dis = 14 代表目標在自己周圍
        // 10 代表上、下、左、右
        // 14 代表左上、右上、左下、右下
        if (dis == 10 || dis == 14)
        {
            MapManager.Instance.ChangeGrassToGround(_grassTile);
            eatCount++;
            GameManager.Instance.grassEatCount++;

            // 草吃到指定數量後執行
            if (eatCount >= 3)
            {
                GrowUp();
            }
        }
    }

    protected virtual void GrowUp()
    {
        GameManager.Instance.sheepEatEnoughCount++;
        GameManager.Instance.coins += bonus;

        GameManager.Instance.sheeps.Remove(this);
        GameManager.Instance.tilesFromSheep.Remove(this.currentTile);
        Object.Destroy(animalObject);
    }
}