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

    // ��l�Ʀa��
    // 10�� 1�� 5��
    // 6�^�X�� �X1�T
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

    // �̾� MapSize �Ыئa�Ϯ�
    // �j�p = xSize * ySize
    private void CreateTile()
    {
        tiles = new Tile[xSize, ySize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                // �Ыخ�l����
                GameObject tileInstance = Instantiate(tilePrefab, new Vector3(x * 1.5f, y * 1.5f, 0), Quaternion.identity);
                // �ھڮy�нᤩ��l�W��
                // Ex: tile(0, 0)
                tileInstance.name = $"{tilePrefab.name} ({x}, {y})";
                // ����sprite
                SpriteRenderer tileSpriteRenderer = tileInstance.GetComponent<SpriteRenderer>();
                tileSpriteRenderer.sprite = groundTile;
                // �Ы�Tile���O�å[�JList�A��K����ϥ�
                Tile _groundTile = tiles[x, y] = new Tile(true, tileInstance.transform.position, x, y, tileInstance);
                groundTiles.Add(_groundTile);
            }
        }
    }

    // �N ���q�a�O �H�������� ��a
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
                // ���o�H�����q�a�O
                Tile targetTile = MyUtility.GetRandomTile(tiles);

                // ����sprite
                SpriteRenderer tileSpriteRenderer = targetTile.tileObject.transform.GetComponent<SpriteRenderer>();
                tileSpriteRenderer.sprite = grassTile;

                // ��s ���q�a�O���M�� �� ��a���M��
                grassTiles.Add(targetTile);
                groundTiles.Remove(targetTile);
                grassCount++;
            }
        }
    }

    // �N ��a ���ܬ� ���q�a�O => �N���Q�Y��
    public void ChangeGrassToGround(Tile _targetTile)
    {
        // ��sprite
        SpriteRenderer spriteRenderer = _targetTile.tileObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = groundTile;

        // ���Ӯ��ܦ��i�樫
        _targetTile.walkable = true;

        // ��s ���q�a�O���M�� �� ��a���M��
        groundTiles.Add(_targetTile);
        grassTiles.Remove(_targetTile);
    }

    // �^�� �ǤJ����m�O���@�� Tile
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
