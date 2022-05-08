using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FieldTileSpawner : MonoBehaviour
{
    public int[][] map = new int[30][];
    public FieldTile[][] tiles = new FieldTile[30][];
    [Header("타일의 transform parent")]
    public GameObject parent;
    [Header("타일 프리팹(enum순서에 맞게 등록)")]
    public List<GameObject> prefabs = new List<GameObject>((int)TILE.COUNT);
    [Header("필드 전체의 크기 X,Y")]
    public int sizeX;
    public int sizeY;

    private void Start()
    {
        Test();
    }
    private void Test()
    {
        int x = 11, y = 11;
        sizeX = x;
        sizeY = y;
        map[11] = new int[]{ 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1 };
        map[10] = new int[]{ 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
        map[9] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1 };
        map[8] = new int[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
        map[7] = new int[] { 0, 0, 1, 0, 0, 0, 3, 0, 1, 0, 0, 0 };
        map[6] = new int[] { 0, 0, 0, 0, 0, 3, 0, 0, 0, 1, 0, 0 };
        map[5] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 };
        map[4] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 };
        map[3] = new int[] { 0, 0, 1, 1, 1, 0, 0, 1, 3, 1, 0, 0 };
        map[2] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        map[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        map[0] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        for(int i=0; i<=x; i++)
        {
            tiles[i] = new FieldTile[30];
        }

        for(int i=0; i<=y; i++)
        {
            for (int j=0; j<=x; j++)
            {
                FieldTile go = Instantiate(prefabs[map[i][j]], parent.transform, false).GetComponent<FieldTile>();
                tiles[i][j] = go;
                go.x = j;
                go.y = i;
                go.transform.localPosition = new Vector3(j, i , 0) ;
            }
        }
        // TODO: xy값에 맞게 타일전체가 화면에 들어오도록 카메라 이동 및 축소해야함
        // 배열을 따로 파일이나 시트에서 가져오면 좋을 것 같음
    }


    public FieldTile GetTile(int x, int y)
    {
        return tiles[y][x];
    }
    
}
