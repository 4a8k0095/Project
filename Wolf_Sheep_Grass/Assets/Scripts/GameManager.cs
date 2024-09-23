using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Animal Prefab")]
    [SerializeField] private GameObject lambPrefab;
    [SerializeField] private GameObject sheepPrefab;
    [SerializeField] private GameObject dogPrefab;
    [SerializeField] private GameObject wolfPrefab;

    [Header("Animal Properties")]
    [SerializeField] private int lambStep = 1;
    [SerializeField] private int sheepStep = 2;
    [SerializeField] private int dogStep = 4;
    [SerializeField] private int wolfStep = 3;

    [Header("Initial Game Setting")]
    [SerializeField] private int initialSheepCount = 10;
    public int InitialSheepCount
    {
        get { return initialSheepCount; }
    }

    [SerializeField] private int initialSpawnWolfRound = 6;
    public int InitialSpawnWolfRound
    {
        get { return initialSpawnWolfRound; }
    }

    private const int roundToSpawnLamb = 5;

    [SerializeField] private int sheepsLimit = 20;

    [SerializeField] private int wolfLimit = 1;

    [SerializeField] private int grassLimit = 15;
    public int GrassLimit
    {
        get { return grassLimit; }
    }

    [Header("Game Properties")]
    [SerializeField] private int round = 0;
    public int Round
    {
        get { return round; }
    }
    public int coins = 0;
    public int grassEatCount = 0;
    public int lambEatEnoughCount = 0;
    public int sheepEatEnoughCount = 0;
    public int dogAttackCount = 0;
    public int wolfEatLambCount = 0;
    public int wolfEatSheepCount = 0;

    [Header("Animals and Tiles List")]
    [SerializeField] public List<Sheep> sheeps = new List<Sheep>();
    [SerializeField] public List<Tile> tilesFromSheep = new List<Tile>();

    [SerializeField] private List<Dog> dogs = new List<Dog>();

    [SerializeField] public List<Wolf> wolves = new List<Wolf>();
    [SerializeField] public List<Tile> tilesFromWolf = new List<Tile>();

    private bool action = false;

    public enum PlaySpeed
    {
        normal,
        fast
    }
    public PlaySpeed playSpeed = PlaySpeed.normal;

    private void Awake()
    {
        if (Instance == null)
            Instance = FindObjectOfType<GameManager>();
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            PlayToEnd();
        }

        if (Input.GetKeyDown(KeyCode.Space) && !action)
        {
            EnterNextRound();
        }
    }

    public void PlayToEnd()
    {
        StartCoroutine(PlayToHundredRound());
        playSpeed = PlaySpeed.fast;
    }

    public void EnterNextRound()
    {
        StartCoroutine(Action());
        playSpeed = PlaySpeed.normal;
    }

    private IEnumerator PlayToHundredRound()
    {
        int runTimes = 100 - round;
        for(int i = 0; i < runTimes; i++)
        {
            yield return StartCoroutine(Action());
        }
    }

    // ��ʶ��� ��or�p�� => �T => ��
    private IEnumerator Action()
    {
        round++;
        // ���`�Ƥj��10��3��
        // ���`�Ƥp��10��5��
        if (sheeps.Count >= 10)
        {
            MapManager.Instance.SpawnGrass(3);
        }
        else if (sheeps.Count < 10)
        {
            MapManager.Instance.SpawnGrass(5);
        }

        // �Ϥj��10�C5�^�X��1�p��
        // �Ϥp�󵥩�10�C5�^�X��2�p��
        if (round % roundToSpawnLamb == 0)
        {
            if (sheeps.Count > 10)
            {
                SpawnLamb(MapManager.Instance.tiles, 1);
            }
            else if (sheeps.Count <= 10)
            {
                SpawnLamb(MapManager.Instance.tiles, 2);
            }
        }

        // �� ���
        for (int i = 0; i < sheeps.Count; i++)
        {
            // �M��̪��y�� (��)
            sheeps[i].FindClosestPrey(MapManager.Instance.grassTiles, out Tile targetTile);
            // ����
            yield return StartCoroutine(sheeps[i].Move());
            // ��s�ثe�Ҧb��l
            tilesFromSheep[i] = sheeps[i].currentTile;
            // �Y��
            sheeps[i].Eat(targetTile);
        }

        // �T ���
        for (int i = 0; i < wolves.Count; i++)
        {
            // �M��̪��y�� (��or�p��)
            wolves[i].FindClosestPrey(tilesFromSheep, out Tile targetTile);
            // ����
            yield return StartCoroutine(wolves[i].Move());
            // ��s�ثe�Ҧb��l
            tilesFromWolf[i] = wolves[i].currentTile;
            // �Y��
            wolves[i].Eat(targetTile);
        }

        // �� ���
        for (int i = 0; i < dogs.Count; i++)
        {
            // �M��̪��y�� (�T)
            dogs[i].FindClosestPrey(tilesFromWolf, out Tile targetTile);
            // ����
            yield return StartCoroutine(dogs[i].Move());
            // �����T
            dogs[i].Attack(targetTile);
        }
        action = false;
    }

    // �Ыؤp��
    public void SpawnLamb(Tile[,] _tiles, int _count)
    {
        if (sheeps.Count >= sheepsLimit)
        {
            return;
        }
        else
        {
            if (_count > sheepsLimit - sheeps.Count)
                _count = sheepsLimit - sheeps.Count;

            int lambCount = 0;

            while (lambCount < _count)
            {
                // ���o�a�ϤW�H�����Ŧa�Ыؤp��
                Tile targetTile = MyUtility.GetRandomTile(_tiles);
                GameObject lambInstance = Instantiate(lambPrefab, targetTile.tileObject.transform.position, Quaternion.identity);
                Lamb lamb = new Lamb(lambInstance, targetTile, lambStep);

                // �N�p�ϩM�p�ϩҦb����l�[�J�M��
                sheeps.Add(lamb);
                tilesFromSheep.Add(lamb.currentTile);
                lambCount++;
            }
        }
    }

    // ��p�ϦY����w�ƶq�����Q�I�s
    public void LambGrowUp(Sheep _sheep, Tile _tile)
    {
        // �ǤJ�����p�G�O�p�ϡA�Ыؤ@���Ϧb�ۦP��m
        if (_sheep.GetType() == typeof(Lamb))
        {
            GameObject sheepInstance = Instantiate(sheepPrefab, _tile.tileObject.transform.position, Quaternion.identity);
            Sheep sheep = new Sheep(sheepInstance, _tile, sheepStep);

            int index = sheeps.FindIndex(x => x == _sheep);
            sheeps[index] = sheep;
        }
    }

    // �Ыئ�
    public void SpawnSheep(Tile[,] _tiles, int _count)
    {
        if (sheeps.Count >= sheepsLimit)
        {
            return;
        }
        else
        {
            if (_count > sheepsLimit - sheeps.Count)
                _count = sheepsLimit - sheeps.Count;

            int sheepCount = 0;

            while (sheepCount < _count)
            {
                // ���o�a�ϤW�H�����Ŧa�Ыئ�
                Tile targetTile = MyUtility.GetRandomTile(_tiles);
                GameObject sheepInstance = Instantiate(sheepPrefab, targetTile.tileObject.transform.position, Quaternion.identity);
                Sheep sheep = new Sheep(sheepInstance, targetTile, sheepStep);

                // �N�ϩM�ϩҦb����l�[�J�M��
                sheeps.Add(sheep);
                tilesFromSheep.Add(sheep.currentTile);
                sheepCount++;
            }
        }
    }

    // �Ыت�
    public void SpawnDog(Tile[,] _tiles)
    {
        // ���o�a�ϤW�H�����Ŧa�Ыت�
        Tile targetTile = MyUtility.GetRandomTile(_tiles);
        GameObject dogInstance = Instantiate(dogPrefab, targetTile.tileObject.transform.position, Quaternion.identity);
        Dog dog = new Dog(dogInstance, targetTile, dogStep);

        // �N���[�J�M��
        dogs.Add(dog);
    }

    // �ЫدT
    public IEnumerator SpawnWolf(Tile[,] _tiles, int _roundToSpawn)
    {
        int spawnRound = round + _roundToSpawn;

        // ��F����w�^�X�~�����
        yield return new WaitUntil(() => round == spawnRound);

        // �H���^�X�ơA�Ψӫ��w�U�@���T�X�^�X��X�{
        int randomRound = MyUtility.GetRandomNum(6, 10);

        // �a�ϤW�S�T�Ыؤ@��
        if (wolves.Count < wolfLimit)
        {
            Tile targetTile = MyUtility.GetRandomTile(_tiles);
            GameObject wolfInstance = Instantiate(wolfPrefab, targetTile.tileObject.transform.position, Quaternion.identity);
            Wolf wolf = new Wolf(wolfInstance, targetTile, wolfStep);
            wolves.Add(wolf);
            tilesFromWolf.Add(wolf.currentTile);

            StartCoroutine(SpawnWolf(MapManager.Instance.tiles, randomRound));
        }
        // �a�ϤW���T���ЫءA�����I�s�U�@���ͦ�
        else
        {
            StartCoroutine(SpawnWolf(MapManager.Instance.tiles, randomRound));
        }
    }
}
