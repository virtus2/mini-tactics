using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TILE
{
    DEFAULT, // 움직일 수 있는 기본 타일, 0
    OBSTACLE, // 이동 불가능, 1
    MOVE_ENERGY, // 이동 버프, 2
    EVADE, // 회피 버프, 3
    HEAL, // 회복, 4
    DEF, // 방어 버프, 5
    // 새로 추가할 땐 여기에
    COUNT // enum 개수 세는 용, 6
}
public class FieldTile : MonoBehaviour
{
    [Header("타일 타입")]
    public TILE type;

    public bool canMove = true;
    public int moveEnergy;
    public float evade;
    public int heal;
    public int def;
    public int x;
    public int y;
    public Unit unit; // 해당 타일에 있는 유닛
    // 길찾기 알고리즘용
    // 그래픽용
    private SpriteRenderer spriteRenderer;
    private Color previous;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnMouseUp()
    {
        InputManager.Instance.OnTileClick(this);
    }

    private void OnMouseEnter()
    {
        if(InputManager.Instance.mode == MODE.MOVE&& BattleManager.Instance.IsMovableTile(this))
        {
            previous = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            //UIManager.Instance.MoveTileArrow(this);
        }
    }
    private void OnMouseExit()
    {
        if (InputManager.Instance.mode == MODE.MOVE && BattleManager.Instance.IsMovableTile(this))
        {
            spriteRenderer.color = previous;
            //UIManager.Instance.MoveTileArrow(this);
        }

    }
}
