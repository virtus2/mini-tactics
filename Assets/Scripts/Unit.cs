using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Unit : MonoBehaviour
{
    public enum Armor
    {
        Normal,
        Plate   
    }

    public Transform pivot;
    public int index;
    public string unitName;
    public int level;
    public int maxHp;
    public int hp;
    public int attack;
    public int defense;
    public int attackDistance;
    public int findDistance;
    public int movementPoint;
    public int maxMovementPoint;
    public float evadeChance;
    public float critChance;
    public float critMin;
    public float critMax;
    public float str;
    public float dex;
    public float intel;
    public float wis;
    public float upStr;
    public float upDex;
    public float upInt;
    public float upWis;
    public float growRate;
    public int aggro;
    public Armor armor;
    public bool turnEnded = false;
    public bool isPlayerUnit = false;
    public int x;
    public int y;
    // Pathfind
    public List<FieldTile> movableTiles;
    public List<FieldTile> findTiles;
    public List<FieldTile> attackTiles;
    public List<Unit> foundUnits;
    // AI
    public AIProfile profile;
    // UI
    public Slider slider;
    // SPUM
    [HideInInspector]
    public SPUM_Prefabs prefab;
    
    private void Start()
    {
        x = Mathf.FloorToInt(pivot.localPosition.x) ;
        y = Mathf.FloorToInt(pivot.localPosition.y);
        movableTiles = new List<FieldTile>();
        findTiles = new List<FieldTile>();
        attackTiles = new List<FieldTile>();
        foundUnits = new List<Unit>();

        slider.maxValue = maxHp;
        slider.value = hp;
        prefab = this.GetComponent<SPUM_Prefabs>();
        
    }
    private void OnMouseUp()
    {
        InputManager.Instance.OnUnitClick(this);
    }


    public List<FieldTile> GetMovableTiles()
    {
        movableTiles.Clear();
        Pathfind.Instance.GetTilesWithin(this, this.movementPoint, movableTiles, false, false);
        return movableTiles;
    }

    public List<FieldTile> GetAttackTiles()
    {
        attackTiles.Clear();
        Pathfind.Instance.GetTilesWithin(this, this.attackDistance, attackTiles, true, false);
        return attackTiles;
    }
    public List<FieldTile> GetFindTiles()
    {
        findTiles.Clear();
        Pathfind.Instance.GetTilesWithin(this, findDistance, findTiles, true, true);
        return findTiles;
    }
    
    public List<Unit> FindUnits()
    {
        foundUnits.Clear();
        for (int i=0; i<findTiles.Count; i++)
        {
            if(findTiles[i].unit != null && findTiles[i].unit.isPlayerUnit == true)
            {
                foundUnits.Add(findTiles[i].unit);
            }
        }
        return foundUnits;
    }
    public bool CanMoveTo(FieldTile tile)
    {
        return Pathfind.Instance.CanMove(this, tile);
    }

    public bool CanAttack(Unit target)
    {
        return Pathfind.Instance.GetDistance(this, target) <= attackDistance;
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
        slider.value = hp;
    }

}
