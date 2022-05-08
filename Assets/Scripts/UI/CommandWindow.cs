using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CommandWindow : MonoBehaviour
{
    [SerializeField]
    Text unitNameText;
    Unit selectedUnit;

    public Button moveButton;
    public Button attackButton;
    private void Start()
    {
        moveButton.onClick.AddListener(OnMoveButtonClicked);
    }
    /// <summary>
    /// 해당 유닛의 명령창을 보여주거나 숨긴다.
    /// </summary>
    /// <param name="show"></param>
    /// <param name="unit"></param>
    public void Show(Unit unit=null)
    {
        gameObject.SetActive(true);
        if(unit != null)
        {
            selectedUnit = unit;
            unitNameText.text = selectedUnit.unitName;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        InputManager.Instance.mode = MODE.NONE;
    }
    public void OnMoveButtonClicked()
    {
        if (InputManager.Instance.mode != MODE.MOVE && selectedUnit.isPlayerUnit)
        {
            // 이동버튼을 처음 눌렀을 경우
            // 플레이어 턴이 아닐 경우 이동버튼 눌러도 아무 효과가 없음
            if (BattleManager.Instance.PlayerTurn == false)
            {
                UIManager.Instance.ShowMessageText("플레이어의 턴이 아닙니다", 1f);
                return;
            }
            if (selectedUnit.movementPoint <= 0)
            {
                UIManager.Instance.ShowMessageText("유닛의 행동력이 없습니다", 1f);
                return;
            }
            if (selectedUnit.turnEnded)
            {
                UIManager.Instance.ShowMessageText("유닛의 턴이 종료되었습니다", 1f);
                return;
            }
            BattleManager.Instance.MoveUnitReady();
            //UIManager.Instance.ShowTileArrow();
            InputManager.Instance.mode = MODE.MOVE;


        }
        else
        {
            // 이동 버튼을 누른 상태에서 한번 더 누를경우
            BattleManager.Instance.MoveUnitCancel();
            InputManager.Instance.mode = MODE.NONE;
        }
        
    }

    public void OnMoveButtonCancel()
    {

    }

    public void OnAttackButtonClicked()
    {
        // 공격 버튼을 처음 눌렀을 경우
        if (InputManager.Instance.mode != MODE.ATTACK)
        {
            // 플레이어 턴이 아닐 경우 공격버튼 눌러도 아무 효과가 없음
            if (BattleManager.Instance.PlayerTurn == false)
            {
                UIManager.Instance.ShowMessageText("플레이어의 턴이 아닙니다", 1f);
                return;
            }
            if(selectedUnit.turnEnded)
            {
                UIManager.Instance.ShowMessageText("유닛의 턴이 종료되었습니다", 1f);
                return;
            }
            /*
            if (selectedUnit.movementPoint <= 0)
            {
                UIManager.Instance.ShowMessageText("유닛의 행동력이 없습니다", 1f);
                return;
            }
            */
            BattleManager.Instance.AttackUnitReady();
            //UIManager.Instance.ShowTileArrow();
            InputManager.Instance.mode = MODE.ATTACK;
        }
        // 공격 버튼을 누른 상태에서 한번 더 누를경우
        else
        {
            BattleManager.Instance.AttackUnitCancel();
            InputManager.Instance.mode = MODE.NONE;
        }
    }

    public void OnSkillButtonClicked()
    {

    }

    public void OnItemButtonClicked()
    {

    }
}
