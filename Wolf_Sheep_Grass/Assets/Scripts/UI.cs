using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Text round;
    [SerializeField] private Text grassCount;
    [SerializeField] private Text sheepCount;
    [SerializeField] private Text wolfCount;
    [SerializeField] private Text eatGrassCount;
    [SerializeField] private Text lambEatEnoughCount;
    [SerializeField] private Text sheepEatEnoughCount;
    [SerializeField] private Text wolfEatLambCount;
    [SerializeField] private Text wolfEatSheepCount;
    [SerializeField] private Text dogAttackCount;
    [SerializeField] private Text coins;

    private void Update()
    {
        round.text = "�^�X: " + GameManager.Instance.Round;
        grassCount.text = "�a�ϤW�󪺼ƶq: " + MapManager.Instance.grassTiles.Count;
        sheepCount.text = "�a�ϤW�Ϫ��ƶq: " + GameManager.Instance.sheeps.Count;
        wolfCount.text = "�a�ϤW�T���ƶq: " + GameManager.Instance.wolves.Count;
        eatGrassCount.text = "�Y�������`��: " + GameManager.Instance.grassEatCount;
        lambEatEnoughCount.text = "�p�ϦY��������: " + GameManager.Instance.lambEatEnoughCount;
        sheepEatEnoughCount.text = "�ϦY��������: " + GameManager.Instance.sheepEatEnoughCount;
        wolfEatLambCount.text = "�T�Y���p�Ϫ�����: " + GameManager.Instance.wolfEatLambCount;
        wolfEatSheepCount.text = "�T�Y���Ϫ�����: " + GameManager.Instance.wolfEatSheepCount;
        dogAttackCount.text = "�������T������: " + GameManager.Instance.dogAttackCount;
        coins.text = "������: " + GameManager.Instance.coins;
    }
}
