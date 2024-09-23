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

    // 行動順序 羊or小羊 => 狼 => 狗
    private IEnumerator Action()
    {
        round++;
        // 羊總數大於10生3草
        // 羊總數小於10生5草
        if (sheeps.Count >= 10)
        {
            MapManager.Instance.SpawnGrass(3);
        }
        else if (sheeps.Count < 10)
        {
            MapManager.Instance.SpawnGrass(5);
        }

        // 羊大於10每5回合生1小羊
        // 羊小於等於10每5回合生2小羊
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

        // 羊 行動
        for (int i = 0; i < sheeps.Count; i++)
        {
            // 尋找最近的獵物 (草)
            sheeps[i].FindClosestPrey(MapManager.Instance.grassTiles, out Tile targetTile);
            // 移動
            yield return StartCoroutine(sheeps[i].Move());
            // 更新目前所在格子
            tilesFromSheep[i] = sheeps[i].currentTile;
            // 吃草
            sheeps[i].Eat(targetTile);
        }

        // 狼 行動
        for (int i = 0; i < wolves.Count; i++)
        {
            // 尋找最近的獵物 (羊or小羊)
            wolves[i].FindClosestPrey(tilesFromSheep, out Tile targetTile);
            // 移動
            yield return StartCoroutine(wolves[i].Move());
            // 更新目前所在格子
            tilesFromWolf[i] = wolves[i].currentTile;
            // 吃羊
            wolves[i].Eat(targetTile);
        }

        // 狗 行動
        for (int i = 0; i < dogs.Count; i++)
        {
            // 尋找最近的獵物 (狼)
            dogs[i].FindClosestPrey(tilesFromWolf, out Tile targetTile);
            // 移動
            yield return StartCoroutine(dogs[i].Move());
            // 攻擊狼
            dogs[i].Attack(targetTile);
        }
        action = false;
    }

    // 創建小羊
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
                // 取得地圖上隨機的空地創建小羊
                Tile targetTile = MyUtility.GetRandomTile(_tiles);
                GameObject lambInstance = Instantiate(lambPrefab, targetTile.tileObject.transform.position, Quaternion.identity);
                Lamb lamb = new Lamb(lambInstance, targetTile, lambStep);

                // 將小羊和小羊所在的格子加入清單
                sheeps.Add(lamb);
                tilesFromSheep.Add(lamb.currentTile);
                lambCount++;
            }
        }
    }

    // 當小羊吃到指定數量的草後被呼叫
    public void LambGrowUp(Sheep _sheep, Tile _tile)
    {
        // 傳入類型如果是小羊，創建一隻羊在相同位置
        if (_sheep.GetType() == typeof(Lamb))
        {
            GameObject sheepInstance = Instantiate(sheepPrefab, _tile.tileObject.transform.position, Quaternion.identity);
            Sheep sheep = new Sheep(sheepInstance, _tile, sheepStep);

            int index = sheeps.FindIndex(x => x == _sheep);
            sheeps[index] = sheep;
        }
    }

    // 創建羊
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
                // 取得地圖上隨機的空地創建羊
                Tile targetTile = MyUtility.GetRandomTile(_tiles);
                GameObject sheepInstance = Instantiate(sheepPrefab, targetTile.tileObject.transform.position, Quaternion.identity);
                Sheep sheep = new Sheep(sheepInstance, targetTile, sheepStep);

                // 將羊和羊所在的格子加入清單
                sheeps.Add(sheep);
                tilesFromSheep.Add(sheep.currentTile);
                sheepCount++;
            }
        }
    }

    // 創建狗
    public void SpawnDog(Tile[,] _tiles)
    {
        // 取得地圖上隨機的空地創建狗
        Tile targetTile = MyUtility.GetRandomTile(_tiles);
        GameObject dogInstance = Instantiate(dogPrefab, targetTile.tileObject.transform.position, Quaternion.identity);
        Dog dog = new Dog(dogInstance, targetTile, dogStep);

        // 將狗加入清單
        dogs.Add(dog);
    }

    // 創建狼
    public IEnumerator SpawnWolf(Tile[,] _tiles, int _roundToSpawn)
    {
        int spawnRound = round + _roundToSpawn;

        // 當達到指定回合繼續執行
        yield return new WaitUntil(() => round == spawnRound);

        // 隨機回合數，用來指定下一次狼幾回合後出現
        int randomRound = MyUtility.GetRandomNum(6, 10);

        // 地圖上沒狼創建一隻
        if (wolves.Count < wolfLimit)
        {
            Tile targetTile = MyUtility.GetRandomTile(_tiles);
            GameObject wolfInstance = Instantiate(wolfPrefab, targetTile.tileObject.transform.position, Quaternion.identity);
            Wolf wolf = new Wolf(wolfInstance, targetTile, wolfStep);
            wolves.Add(wolf);
            tilesFromWolf.Add(wolf.currentTile);

            StartCoroutine(SpawnWolf(MapManager.Instance.tiles, randomRound));
        }
        // 地圖上有狼不創建，直接呼叫下一次生成
        else
        {
            StartCoroutine(SpawnWolf(MapManager.Instance.tiles, randomRound));
        }
    }
}
