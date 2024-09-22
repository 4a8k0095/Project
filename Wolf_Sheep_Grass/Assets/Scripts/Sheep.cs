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

    // �Y��
    // _grassTile = ��Ҧb����l
    public override void Eat(Tile _grassTile)
    {
        if (_grassTile == null)
            return;

        int dis = pathFinding.GetDistance(currentTile, _grassTile);

        // dis = 10 or dis = 14 �N��ؼЦb�ۤv�P��
        // 10 �N��W�B�U�B���B�k
        // 14 �N���W�B�k�W�B���U�B�k�U
        if (dis == 10 || dis == 14)
        {
            MapManager.Instance.ChangeGrassToGround(_grassTile);
            eatCount++;
            GameManager.Instance.grassEatCount++;

            // ��Y����w�ƶq�����
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