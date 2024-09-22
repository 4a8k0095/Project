using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dog : Animal
{
    public Dog(GameObject _animalObject, Tile _currentTile, int _step) : base(_animalObject, _currentTile, _step)
    {
    }

    // �����T
    // tileFromWolf = �T�Ҧb����l
    public void Attack(Tile tileFromWolf)
    {
        if (tileFromWolf == null)
            return;

        int dis = pathFinding.GetDistance(currentTile, tileFromWolf);

        // dis = 10 or dis = 14 �N��ؼЦb�ۤv�P��
        // 10 �N��W�B�U�B���B�k
        // 14 �N���W�B�k�W�B���U�B�k�U
        // �q GameManager �� wolves �M��M��������T�q�M��R���ï}�a����
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
