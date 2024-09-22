using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    #region Singleton
    private static MapManager instance;
    public static MapManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MapManager>();
            return instance;
        }
    }
    #endregion

    [Header("Map Size")]
    public Tile[,] tiles;
    [SerializeField] private int xSize = 20;
    public int XSize
    {
        get { return xSize; }
    }
    [SerializeField] private int ySize = 20;
    public int YSize
    {
        get { return ySize; }
    }

    [Header("Tile Prefab")]
    [SerializeField] private GameObject tilePrefab;

    private const int INITIAL_GRASS_COUNT = 5;
    [Header("Tile Sprite")]
    [SerializeField] private Sprite grassTile;
    [SerializeField] private Sprite groundTile;

    [Header("Grass List")]
    public List<Tile> grassTiles;
    [SerializeField] private List<Tile> groundTiles;

    void Awake()
    {
        InitializeMap();
    }

    // 初始化地圖
    // 10羊 1犬 5草
    // 6回合後 出1狼
    private void InitializeMap()
    {
        CreateTile();
        SpawnGrass(INITIAL_GRASS_COUNT);

        int spawnLambCount = MyUtility.GetRandomNum(0, 10);
        GameManager.Instance.SpawnLamb(tiles, spawnLambCount);

        int initialSheepCount = GameManager.Instance.InitialSheepCount;
        GameManager.Instance.SpawnSheep(tiles, initialSheepCount - spawnLambCount);

        GameManager.Instance.SpawnDog(tiles);

        int initialSpawnWolfRound = GameManager.Instance.InitialSpawnWolfRound;
        StartCoroutine(GameManager.Instance.SpawnWolf(tiles, initialSpawnWolfRound));
    }

    // 依據 MapSize 創建地圖格
    // 大小 = xSize * ySize
    private void CreateTile()
    {
        tiles = new Tile[xSize, ySize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                // 創建格子物件
                GameObject tileInstance = Instantiate(tilePrefab, new Vector3(x * 1.5f, y * 1.5f, 0), Quaternion.identity);
                // 根據座標賦予格子名稱
                // Ex: tile(0, 0)
                tileInstance.name = $"{tilePrefab.name} ({x}, {y})";
                // 替換sprite
                SpriteRenderer tileSpriteRenderer = tileInstance.GetComponent<SpriteRenderer>();
                tileSpriteRenderer.sprite = groundTile;
                // 創建Tile類別並加入List，方便後續使用
                Tile _groundTile = tiles[x, y] = new Tile(true, tileInstance.transform.position, x, y, tileInstance);
                groundTiles.Add(_groundTile);
            }
        }
    }

    // 將 普通地板 隨機替換成 草地
    public void SpawnGrass(int _count)
    {
        if (grassTiles.Count >= GameManager.Instance.GrassLimit)
        {
            return;
        }
        else
        {
            if (_count > GameManager.Instance.GrassLimit - grassTiles.Count)
                _count = GameManager.Instance.GrassLimit - grassTiles.Count;

            int grassCount = 0;

            while (grassCount < _count)
            {
                // 取得隨機普通地板
                Tile targetTile = MyUtility.GetRandomTile(tiles);

                // 替換sprite
                SpriteRenderer tileSpriteRenderer = targetTile.tileObject.transform.GetComponent<SpriteRenderer>();
                tileSpriteRenderer.sprite = grassTile;

                // 更新 普通地板的清單 及 草地的清單
                grassTiles.Add(targetTile);
                groundTiles.Remove(targetTile);
                grassCount++;
            }
        }
    }

    // 將 草地 轉變為 普通地板 => 代表草被吃掉
    public void ChangeGrassToGround(Tile _targetTile)
    {
        // 更換sprite
        SpriteRenderer spriteRenderer = _targetTile.tileObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = groundTile;

        // 讓該格變成可行走
        _targetTile.walkable = true;

        // 更新 普通地板的清單 及 草地的清單
        groundTiles.Add(_targetTile);
        grassTiles.Remove(_targetTile);
    }

    // 回傳 傳入的位置是哪一格 Tile
    //public Tile TileFromWorldPoint(Vector3 _worldPosition)
    //{
    //    var query = from x in Enumerable.Range(0, tiles.GetLength(0))
    //                from y in Enumerable.Range(0, tiles.GetLength(1))
    //                where tiles[x, y].worldPosition == _worldPosition
    //                select new { Row = x, Column = y };

    //    return tiles[query.FirstOrDefault().Row, query.FirstOrDefault().Column];
    //}

    //public List<Tile> path;
    //void OnDrawGizmos()
    //{
    //    if (tiles != null)
    //    {
    //        foreach (Tile n in tiles)
    //        {
    //            SpriteRenderer _spriteRenderer = n.tileObject.GetComponent<SpriteRenderer>();

    //            if (path != null)
    //                if (path.Contains(n))
    //                    _spriteRenderer.color = Color.red;
    //        }
    //    }
    //}
}
