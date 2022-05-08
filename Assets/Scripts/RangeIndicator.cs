using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    private List<FieldTile> current; // 현재 표시하고 있는 타일
    private void Awake()
    {
        current = new List<FieldTile>();
    }
    public void ShowRange(List<FieldTile> tiles, Color color)
    {
        for (int i=0; i<tiles.Count; i++)
        {
            current.Add(tiles[i]);
            tiles[i].GetComponent<SpriteRenderer>().color = color;
        }
    }
    
    public void ShowTile(FieldTile tile, Color color)
    {
        current.Add(tile);
        tile.GetComponent<SpriteRenderer>().color = color;
    }
    public void ClearRange()
    {
        if (current == null) return;
        for(int i=0; i<current.Count; i++)
        {
            current[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
        current.Clear();
    }
}
