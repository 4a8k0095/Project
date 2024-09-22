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
        round.text = ": " + GameManager.Instance.Round;
        grassCount.text = "瓜计秖: " + MapManager.Instance.grassTiles.Count;
        sheepCount.text = "瓜ο计秖: " + GameManager.Instance.sheeps.Count;
        wolfCount.text = "瓜疶计秖: " + GameManager.Instance.wolves.Count;
        eatGrassCount.text = "奔羆计: " + GameManager.Instance.grassEatCount;
        lambEatEnoughCount.text = "ο埂Ω计: " + GameManager.Instance.lambEatEnoughCount;
        sheepEatEnoughCount.text = "ο埂Ω计: " + GameManager.Instance.sheepEatEnoughCount;
        wolfEatLambCount.text = "疶奔οΩ计: " + GameManager.Instance.wolfEatLambCount;
        wolfEatSheepCount.text = "疶奔οΩ计: " + GameManager.Instance.wolfEatSheepCount;
        dogAttackCount.text = "ю阑疶Ω计: " + GameManager.Instance.dogAttackCount;
        coins.text = "刽计: " + GameManager.Instance.coins;
    }
}
