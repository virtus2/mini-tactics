using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum MODE
{
    NONE,
    SELECT,
    MOVE,
    ATTACK,
    SKILL,
    ITEM,
}
public class InputManager : MonoBehaviour
{
    public enum OBJECT
    {
        NONE,
        TILE,
        UNIT
    }
    public static InputManager Instance;
    public CommandWindow commandWindow;
    public Button sightButton;
    private OBJECT target; // 이전에 선택한 오브젝트
    public MODE mode;

    public Unit selectedUnit;
    public FieldTile selectedTile;
    void Awake()
    {
        Instance = this;
        target = OBJECT.NONE;
    }

    private void Update()
    {
        // 키보드의 z을 눌렀을때
        if(Input.GetKeyUp(KeyCode.Z))
        {
            // 플레이어의 턴일때
            if (BattleManager.Instance.PlayerTurn)
            {
                // 플레이어가 선택한 것이 유닛일때
                if (target == OBJECT.UNIT && selectedUnit.isPlayerUnit)
                {
                    // 명령창의 이동버튼을 누른것과 같은 동작
                    ExecuteEvents.Execute(commandWindow.moveButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            // 플레이어의 턴일때
            if (BattleManager.Instance.PlayerTurn)
            {
                // 플레이어가 선택한 것이 유닛일때
                if (target == OBJECT.UNIT && selectedUnit.isPlayerUnit)
                {
                    // 명령창의 공격버튼을 누른것과 같은 동작
                    ExecuteEvents.Execute(commandWindow.attackButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            ExecuteEvents.Execute(sightButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
        if (Input.GetKeyUp(KeyCode.F1))
        {
        }
    }
    public void OnUnitClick(Unit unit)
    {
        // 현재 클릭한 오브젝트가 Unit일때
        // 이전에 클릭했던 오브젝트에 따라 처리를 다르게 함
        switch (target)
        {
            case (OBJECT.NONE):
                // 이전에 클릭했던 오브젝트가 없는 상태에서 유닛을 클릭할 경우
                // 해당 유닛을 선택한다.
                SelectUnit(unit);
                break;
            case (OBJECT.TILE):
                // 이전에 클릭했던 오브젝트가 타일인 상태에서 유닛을 클릭할 경우
                // 해당 유닛을 선택한다.
                SelectUnit(unit);
                break;
            case (OBJECT.UNIT):
                // 이전에 클릭했던 오브젝트가 유닛인 상태에서 유닛을 클릭할 경우
                // 전투일수도 있고 이동일 수도 있고 스킬일수도 선택일수도 있음
                if (mode == MODE.ATTACK && unit.isPlayerUnit == false)
                {
                    BattleManager.Instance.Attack(selectedUnit, unit);
                }
                if(mode == MODE.NONE)
                {
                    SelectUnit(unit);

                }
                break;
        }
        target = OBJECT.UNIT;
        selectedUnit = unit;
    }
    public void SelectUnit(Unit unit)
    {
        if (unit.isPlayerUnit)
        {
            // 해당 유닛이 플레이어의 유닛일 경우 명령창을 띄운다.
            BattleManager.Instance.SelectUnit(unit);
            UIManager.Instance.ShowCommandWindow(unit);
            target = OBJECT.UNIT;
        }
        else
        {
            // 해당 유닛이 플레이어의 유닛이 아닐 경우 상태창만 띄운다.
            BattleManager.Instance.SelectUnit(unit);
            UIManager.Instance.HideCommandWindow();
            //UIManager.Instance.Show

        }

    }
    public void OnTileClick(FieldTile tile)
    {
        // 현재 클릭한 오브젝트가 타일일때
        // 이전에 클릭했던 오브젝트에 따라 처리를 다르게 함
        switch (target)
        {
            case (OBJECT.NONE):
                break;
            case (OBJECT.TILE):
                break;
            case (OBJECT.UNIT):
                if (mode == MODE.MOVE)
                {
                    BattleManager.Instance.MoveUnit(selectedUnit, tile);
                    OnUnitClick(selectedUnit);
                    return;
                }
                else
                {
                    BattleManager.Instance.DeselectUnit();
                    UIManager.Instance.HideCommandWindow();
                }
                break;
        }
        target = OBJECT.TILE;
        selectedTile = tile;
    }

}
