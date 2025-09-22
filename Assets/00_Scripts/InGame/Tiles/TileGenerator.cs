using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{ 
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject[] tilePrefabs; // 0: wall, 1: normal, 2: heal, 3: trap
    [SerializeField] private Vector2Int size;
    [SerializeField] public TileBehavior[,] tileMap;

    public TileType[,] MapTiles;
    public int mapSeed;


    [Header("Tile Percent")]
    [SerializeField, Range(0, 100)] private int wallPercent = 20;
    [SerializeField, Range(0, 100)] private int normalPercent = 50;
    [SerializeField, Range(0, 100)] private int healPercent = 15;
    [SerializeField, Range(0, 100)] private int trapPercent = 15;


    [Header("CellulerAutomata")]
    public int cellularRepeatCnt;
    public int cellularRule;

    private void Start()
    {
        //mapSeed = PhotonManager.Instance.ReturnSeed();
        mapSeed = DateTime.Now.GetHashCode();
        tileMap = new TileBehavior[size.x, size.y];
        InitializeMap();
        ApplyCellularAutomata();
        ApplyMSTandCarveCorridor();
        SpecialTileGenerate();
        SpawnPointRegist();
        MakeMap();
    }

    private void InitializeMap()
    {
        MapTiles = new TileType[size.x, size.y];
        UnityEngine.Random.InitState(mapSeed);
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (x == 0 || y == 0 || x == size.x - 1 || y == size.y - 1)
                    MapTiles[x, y] = TileType.Wall;
                else
                    MapTiles[x, y] = (UnityEngine.Random.Range(0, 100) < wallPercent) ? TileType.Wall : TileType.Normal;
            }
        }
    }
    #region CellularAutomata
    /// <summary>
    /// 셀룰러 오토마타(Cellular Automata) 알고리즘 적용
    /// 맵의 각 셀을 검사하여, 주변 8방향의 벽 개수가 cellularRule 이상이면 벽으로,
    /// 그렇지 않으면 일반 타일로 설정하여 맵을 점차 다듬는다.
    /// </summary>
    private void ApplyCellularAutomata()
    {
        for (int i = 0; i < cellularRepeatCnt; i++)
        {
            TileType[,] newMap = (TileType[,])MapTiles.Clone();
            for (int x = 1; x < size.x - 1; x++)
            {
                for (int y = 1; y < size.y - 1; y++)
                {
                    int wallCnt = CountWallNeighbors(x, y);
                    if (wallCnt >= cellularRule)
                        newMap[x, y] = TileType.Wall;
                    else
                        newMap[x, y] = TileType.Normal;
                }
            }
            MapTiles = newMap;
        }
    }


    /// <summary>
    /// (x,y)좌표를 기준으로 3 * 3만큼의 벽 갯수 반환
    /// 자기자신은 제외
    /// </summary>
    /// <returns></returns>
    private int CountWallNeighbors(int x, int y)
    {
        int cnt = 0;
        for (int nx = x - 1; nx <= x + 1; nx++)
        {
            for (int ny = y - 1; ny <= y + 1; ny++)
            {
                if (nx < 0 || ny < 0 || nx >= size.x || ny >= size.y) continue;
                if (nx == x && ny == y) continue;
                if (MapTiles[nx, ny] == 0) cnt++;
            }
        }
        return cnt;
    }
    #endregion
    #region RFS
    private class Region
    {
        public List<Vector2Int> tiles = new();
        public Vector2 centroid;
        public int index;
        public Vector2Int GetCentroidCell() => new Vector2Int(Mathf.RoundToInt(centroid.x), Mathf.RoundToInt(centroid.y));
    }
    private bool isWalkable(int x, int y) => MapTiles[x, y] != TileType.Wall;

    /// <summary>
    /// BFS 기반으로 맵 전체에서 연결된 Walkable 영역(Region)을 찾아낸다.
    /// 각 영역은 고유 index, 타일 목록, 중심점(centroid)을 가진다.
    /// </summary>
    private List<Region> FindRegions()
    {
        int idx = 0;
        bool[,] visited = new bool[size.x, size.y];
        List<Region> regions = new();

        for (int x = 1; x < size.x - 1; x++)
        {
            for (int y = 1; y < size.y - 1; y++)
            {
                if (visited[x, y])
                    continue;
                if (!isWalkable(x, y))
                {
                    visited[x, y] = true;
                    continue;
                }
                Region region = new();
                Queue<Vector2Int> queue = new();
                queue.Enqueue(new Vector2Int(x, y));
                visited[x, y] = true;
                while (queue.Count > 0)
                {
                    Vector2Int currentCell = queue.Dequeue();
                    region.tiles.Add(currentCell);

                    foreach (var dir in Defines.directions)
                    {
                        int neighborX = currentCell.x + dir.x;
                        int neighborY = currentCell.y + dir.y;

                        if (neighborX < 1 || neighborY < 1 || neighborX >= size.x - 1 || neighborY >= size.y - 1)
                            continue;
                        if (visited[neighborX, neighborY])
                            continue;
                        if (!isWalkable(neighborX, neighborY))
                        {
                            visited[neighborX, neighborY] = true;
                            continue;
                        }

                        visited[neighborX, neighborY] = true;
                        queue.Enqueue(new Vector2Int(neighborX, neighborY));
                    }
                }
                ComputeRegionCentroid(region);
                regions.Add(region);
                regions[idx].index = idx++;
            }
        }
        return regions;
    }
    /// <summary>
    /// 주어진 Region의 모든 타일 좌표 평균을 내어 중심 좌표(centroid)를 계산한다.
    /// </summary>
    private void ComputeRegionCentroid(Region region)
    {
        if (region.tiles.Count == 0)
        {
            region.centroid = Vector2.zero;
            return;
        }

        float sumX = 0f, sumY = 0f;
        foreach (var cell in region.tiles)
        {
            sumX += cell.x;
            sumY += cell.y;
        }
        region.centroid = new Vector2(sumX / region.tiles.Count, sumY / region.tiles.Count);
    }

    #endregion
    #region MST
    private class Edge
    {
        public Region regionA, regionB;
        public float distance;
        public Edge(Region a, Region b, float dist)
        {
            regionA = a;
            regionB = b;
            distance = dist;
        }
    }
    private class UnionFind
    {
        private int[] parent;
        public UnionFind(int cnt)
        {
            parent = new int[cnt];
            for (int i = 0; i < cnt; i++)
                parent[i] = i;
        }
        public int Find(int x)
        {
            if (parent[x] != x)
                parent[x] = Find(parent[x]);
            return parent[x];
        }
        public bool SameSet(Region a, Region b)
        {
            return Find(a.index) == Find(b.index);
        }
        public void Union(Region a, Region b)
        {
            parent[Find(a.index)] = Find(b.index);
        }
    }
    /// <summary>
    /// 맵에서 발견된 Region들을 MST로 연결하고,
    /// 각 MST 간선을 따라 복도를 깎아 최종적으로 연결된 맵을 만든다.
    /// </summary>
    private List<Edge> BuildMST()
    {
        List<Region> regions = FindRegions();
        List<Edge> candidateEdges = new();

        for (int i = 0; i < regions.Count; i++)
        {
            for (int j = i + 1; j < regions.Count; j++)
            {
                float distance = Vector2.Distance(regions[i].centroid, regions[j].centroid);
                candidateEdges.Add(new Edge(regions[i], regions[j], distance));
            }
        }

        candidateEdges.Sort((a, b) => a.distance.CompareTo(b.distance));

        UnionFind regionSets = new(regions.Count);
        List<Edge> mstEdges = new();
        foreach (var edge in candidateEdges)
        {
            if (!regionSets.SameSet(edge.regionA, edge.regionB))
            {
                regionSets.Union(edge.regionA, edge.regionB);
                mstEdges.Add(edge);
            }
        }

        return mstEdges;
    }

    private void CarveCorridor(Vector2Int startCell, Vector2Int endCell)
    {
        Vector2Int currentCell = startCell;

        while (currentCell.x != endCell.x)
        {
            MapTiles[currentCell.x, currentCell.y] = TileType.Normal;
            currentCell.x += (endCell.x > currentCell.x) ? 1 : -1;
        }

        while (currentCell.y != endCell.y)
        {
            MapTiles[currentCell.x, currentCell.y] = TileType.Normal;
            currentCell.y += (endCell.y > currentCell.y) ? 1 : -1;
        }
    }

    private void ApplyMSTandCarveCorridor()
    {
        var mstEdges = BuildMST();
        foreach (var edge in mstEdges)
        {
            CarveCorridor(edge.regionA.GetCentroidCell(), edge.regionB.GetCentroidCell());
        }
    }
    #endregion
    #region SpecialTileGenerate

    private void SpecialTileGenerate()
    {
        UnityEngine.Random.InitState(mapSeed);
        var normalTiles = FindNormalTiles();
        foreach (var normalTile in normalTiles)
        {
            int randVal = UnityEngine.Random.Range(0, 100);
            if (randVal <= healPercent)
                MapTiles[normalTile.x, normalTile.y] = TileType.Heal;
            else if (randVal <= healPercent + trapPercent)
                MapTiles[normalTile.x, normalTile.y] = TileType.Trap;
        }
    }
    private List<Vector2Int> FindNormalTiles()
    {
        List<Vector2Int> normalTiles = new();
        for (int x = 1; x < size.x - 1; x++)
            for (int y = 1; y < size.y - 1; y++)
            {
                if (MapTiles[x, y] == TileType.Normal)
                    normalTiles.Add(new Vector2Int(x, y));
            }
        return normalTiles;
    }

    #endregion
    #region CharactorSpawnPoint
    private void SpawnPointRegist() 
    {
        UnityEngine.Random.InitState(mapSeed);
        var normalTiles = FindNormalTiles();
        var quad = normalTiles.Count / 4;
        var player1Spawn = UnityEngine.Random.Range(0, quad);
        var player2Spawn = UnityEngine.Random.Range(normalTiles.Count - quad , normalTiles.Count);
        MapTiles[normalTiles[player1Spawn].x, normalTiles[player1Spawn].y] = TileType.Spawn;
        MapTiles[normalTiles[player2Spawn].x, normalTiles[player2Spawn].y] = TileType.Spawn;
    }

    #endregion

    private void MakeMap()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 pos = grid.CellToWorld(new Vector3Int(x, y, 0));
                tileMap[x, y] = Instantiate(tilePrefabs[(int)MapTiles[x, y]], pos, Quaternion.identity, transform).GetComponent<TileBehavior>();
            }
        }
    }
}
