using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfind : MonoBehaviour
{
    public static Pathfind Instance;
    [SerializeField]
    private FieldTileSpawner fieldTileSpawner;
    private int[] dx = { -1, 0, 0, 1 }; // 좌, 상, 하, 우
    private int[] dy = { 0, 1, -1, 0 };
    private void Awake()
    {
        Instance = this;
    }
    public FieldTile GetTile(int x, int y)
    {
        return fieldTileSpawner.GetTile(x, y);
    }
    
    public void SetUnitToTile(Unit unit, int x, int y)
    {
        GetTile(x, y).unit = unit;
    }
    public void SetUnitToTile(Unit unit, FieldTile tile)
    {
        tile.unit = unit;
    }
    public int GetDistance(FieldTile src, FieldTile dst)
    {
        int distance = Mathf.Abs(src.x - dst.x) + Mathf.Abs(src.y - dst.y);
        return distance;
    }
    public int GetDistance(Unit src, FieldTile dst)
    {
        int distance = Mathf.Abs(src.x - dst.x) + Mathf.Abs(src.y - dst.y);
        return distance;
    }
    public int GetDistance(Unit src, Unit dst)
    {
        int distance = Mathf.Abs(src.x - dst.x) + Mathf.Abs(src.y - dst.y);
        return distance;
    }
    public class Node
    {
        public FieldTile tile;
        public int cost;
        public Node parent;
        public Node(FieldTile tile, int cost, Node parent)
        {
            this.tile = tile;
            this.cost = cost;
            this.parent = parent;
        }
    }
    /// <summary>
    /// 반경 내의 타일을 계산함
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="distance"></param>
    /// <param name="tiles"></param>
    /// <param name="containUnitTile"></param>
    /// <param name="passObstacle"></param>
    public void GetTilesWithin(Unit unit, int distance, List<FieldTile> tiles, bool containUnitTile, bool passObstacle)
    {
        // 탐색하지 않은 타일 리스트
        List<Node> open = new List<Node>();
        // 탐색을 완료한 타일 리스트
        List<Node> closed = new List<Node>();
        // 탐색을 처음 시작하는 타일(현재 유닛이 있는 타일)
        Node start = new Node(fieldTileSpawner.GetTile(unit.x, unit.y), 0, null);
        start.cost = 0;
        open.Add(start);
        int c = 0;
        while (open.Count > 0)
        {
            c++;
            Node current = open[0];
            open.Remove(current);
            if (Visited(closed, current)) continue;
            closed.Add(current);
            if (current.cost > distance)
            {
                continue;
            }
            if (containUnitTile)
            {
                // 유닛이 있는 타일을 포함
                tiles.Add(current.tile);
            }
            else
            {// 유닛이 없는 타일만 포함
                if (current.tile.unit == null)
                {
                    tiles.Add(current.tile);
                }
            }
            // 이웃 노드 탐색
            for (int i = 0; i < dx.Length; i++)
            {
                int newX = current.tile.x + dx[i];
                int newY = current.tile.y + dy[i];
                if (newX > fieldTileSpawner.sizeX || newX < 0) continue;
                if (newY > fieldTileSpawner.sizeY || newY < 0) continue;
                if (Visited(closed, newX, newY)) continue;
                Node newNode = new Node(fieldTileSpawner.GetTile(newX, newY), 0, current);
                // 장애물이 상관없을경우(포착범위 등)
                if (passObstacle == true)
                {
                    newNode.cost = current.cost + 1;
                    open.Add(newNode);
                }
                else
                {
                    // 장애물이 상관있을경우(이동가능 타일 등)
                    // 장애물 타일이나 이미 탐색한 타일이 아닐 경우에 해당 노드를 open list에 추가
                    if (newNode.tile.type != TILE.OBSTACLE)
                    {
                        newNode.cost = current.cost + 1;
                        open.Add(newNode);
                    }
                }

            }
        }
        Debug.Log("범위찾기:  " + c + "번 실행됨.");
    }
    public List<Node> GetPath(FieldTile original, FieldTile target, int distance, bool containUnitTile, bool passObstacle)
    {
        List<Node> path = new List<Node>();
        // 탐색하지 않은 타일 리스트
        List<Node> open = new List<Node>();
        // 탐색을 완료한 타일 리스트
        List<Node> closed = new List<Node>();
        // 탐색을 처음 시작하는 타일(현재 유닛이 있는 타일)
        Node start = new Node(fieldTileSpawner.GetTile(original.x, original.y), 0, null);
        start.cost = 0;
        open.Add(start);
        int c = 0;
        while (open.Count > 0)
        {
            c++;
            Node current = open[0];
            open.Remove(current);
            if (Visited(closed, current)) continue;
            closed.Add(current);
            if (current.cost > distance)
            {
                continue;
            }
            if(current.tile == target)
            {
                Node root = current;
                path.Add(root);
                while(current.parent != null)
                {
                    current = current.parent;
                    path.Add(current);
                }
                //Debug.Log("경로찾기:  " + c + "번 실행됨.");
                return path;
            }
            // 이웃 노드 탐색
            for (int i = 0; i < dx.Length; i++)
            {
                int newX = current.tile.x + dx[i];
                int newY = current.tile.y + dy[i];
                if (newX > fieldTileSpawner.sizeX || newX < 0) continue;
                if (newY > fieldTileSpawner.sizeY || newY < 0) continue;
                if (Visited(closed, newX, newY)) continue;
                Node newNode = new Node(fieldTileSpawner.GetTile(newX, newY), 0, current);
                // 장애물이 상관없을경우(포착범위 등)
                if (passObstacle == true)
                {
                    newNode.cost = current.cost + 1;
                    open.Add(newNode);
                }
                else
                {
                    // 장애물이 상관있을경우(이동가능 타일 등)
                    // 장애물 타일이나 이미 탐색한 타일이 아닐 경우에 해당 노드를 open list에 추가
                    if (newNode.tile.type != TILE.OBSTACLE)
                    {
                        newNode.cost = current.cost + 1;
                        open.Add(newNode);
                    }
                }
            }
        }
        return null;
    }

    public bool Visited(List<Node> closed, Node node)
    {
        for(int i=0; i<closed.Count; i++)
        {
            if (closed[i].tile.x == node.tile.x && closed[i].tile.y == node.tile.y)
            {
                return true;
            }
        }
        return false;
    }

    public bool Visited(List<Node> closed, int x, int y)
    {
        for (int i = 0; i < closed.Count; i++)
        {
            if (closed[i].tile.x == x && closed[i].tile.y == y)
            {
                return true;
            }
        }
        return false;
    }
    public bool CanMove(Unit unit, FieldTile tile)
    {
        return unit.movableTiles.Contains(tile);
    }
}
