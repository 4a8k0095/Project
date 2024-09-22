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

    // �Y��
    // tileFromSheep = �ϩҦb����l
    public override void Eat(Tile tileFromSheep)
    {
        if (tileFromSheep == null)
        {
            Leave();
        }

        int dis = pathFinding.GetDistance(currentTile, tileFromSheep);

        // dis = 10 or dis = 14 �N��ؼЦb�ۤv�P��
        // 10 �N��W�B�U�B���B�k
        // 14 �N���W�B�k�W�B���U�B�k�U
        if (dis == 10 || dis == 14)
        {
            // �q GameManager �� sheeps �M��M��������ϱq�M��R���ï}�a����ç�s�M��
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
