using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wolf : Animal
{
    private int eatCount = 0;
    public Wolf(GameObject _animalObject, Tile _currentTile, int _step) : base(_animalObject, _currentTile, _step)
    {
    }

    // 吃羊
    // tileFromSheep = 羊所在的格子
    public override void Eat(Tile tileFromSheep)
    {
        if (tileFromSheep == null)
        {
            Leave();
        }

        int dis = pathFinding.GetDistance(currentTile, tileFromSheep);

        // dis = 10 or dis = 14 代表目標在自己周圍
        // 10 代表上、下、左、右
        // 14 代表左上、右上、左下、右下
        if (dis == 10 || dis == 14)
        {
            // 從 GameManager 的 sheeps 清單尋找對應的羊從清單刪除並破壞物件並更新清單
            for (int i = GameManager.Instance.sheeps.Count - 1; i >= 0; i--)
            {
                if (GameManager.Instance.sheeps[i].currentTile == tileFromSheep)
                {
                    Sheep sheep = GameManager.Instance.sheeps[i];
                    if (sheep.GetType() == typeof(Lamb))
                        GameManager.Instance.wolfEatLambCount++;
                    else if (sheep.GetType() == typeof(Sheep))
                        GameManager.Instance.wolfEatSheepCount++;

                    GameManager.Instance.tilesFromSheep.Remove(GameManager.Instance.sheeps[i].currentTile);
                    Object.Destroy(GameManager.Instance.sheeps[i].animalObject);
                    GameManager.Instance.sheeps.Remove(GameManager.Instance.sheeps[i]);

                    eatCount++;
                }
            }
        }

        if (eatCount >= 3)
            Leave();
    }

    private void Leave()
    {
        GameManager.Instance.wolves.Remove(this);
        GameManager.Instance.tilesFromWolf.Remove(currentTile);
        Object.Destroy(animalObject);
    }
}
