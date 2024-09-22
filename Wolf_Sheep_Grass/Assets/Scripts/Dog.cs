using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dog : Animal
{
    public Dog(GameObject _animalObject, Tile _currentTile, int _step) : base(_animalObject, _currentTile, _step)
    {
    }

    // 攻擊狼
    // tileFromWolf = 狼所在的格子
    public void Attack(Tile tileFromWolf)
    {
        if (tileFromWolf == null)
            return;

        int dis = pathFinding.GetDistance(currentTile, tileFromWolf);

        // dis = 10 or dis = 14 代表目標在自己周圍
        // 10 代表上、下、左、右
        // 14 代表左上、右上、左下、右下
        // 從 GameManager 的 wolves 清單尋找對應的狼從清單刪除並破壞物件
        if (dis == 10 || dis == 14)
        {
            for(int i = GameManager.Instance.wolves.Count - 1; i >= 0; i--)
            {
                if (GameManager.Instance.wolves[i].currentTile == tileFromWolf)
                {
                    GameManager.Instance.tilesFromWolf.Remove(GameManager.Instance.wolves[i].currentTile);
                    Object.Destroy(GameManager.Instance.wolves[i].animalObject);
                    GameManager.Instance.wolves.Remove(GameManager.Instance.wolves[i]);

                    GameManager.Instance.dogAttackCount++;
                }
            }
        }
    }
}
