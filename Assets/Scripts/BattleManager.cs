using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class BattleManager : MonoBehaviour
{
    // 턴제 전투를 관리하는 클래스
    public static BattleManager Instance; // 싱글톤
    public bool PlayerTurn { get; private set; }// 플레이어 차례면 True 그외 False

    private List<Command> commands; // 번복시스템을 위해 플레이어와 AI의 행동을 저장할 리스트, 
    // 행동을 어떻게 저장할 건지는 아직 안정했음
    [SerializeField]
    private List<Unit> playerUnits; // 플레이어가 소유중인 유닛
    [SerializeField]
    private List<Unit> enemyUnits; // 적이 소유중인 유닛
    public RangeIndicator indicator; // 범위 표시기
    public AI Ai;
    [Header("이동가능한 타일 색")]
    public Color movableColor;
    [Header("공격 가능한 타일 색")]
    public Color attackableColor;
    [Header("공격 범위 내 적의 타일 색")]
    public Color attackTargetColor;
    [Header("포착 범위 타일 색")]
    public Color findColor;

    [Header("타일 이동 속도")]
    public float MoveSpeed = 3f;
    [Header("타일 이동 대기 시간")]
    public float MoveWaitTime = 0.2f;
    private WaitForSeconds moveWaitTime;
    [Header("전투 연출 오브젝트")]
    public GameObject battleScene;
    public GameObject leftCharacter;
    public GameObject rightCharacter;
    public GameObject leftTile;
    public GameObject rightTile;

    private Unit currentUnit;
    private void Awake()
    {
        Instance = this;
        Ai = GetComponent<AI>();
        moveWaitTime = new WaitForSeconds(MoveWaitTime);
    }
    private void Start()
    {
        commands = new List<Command>();
    }

    public void Test()
    {
        PlayerTurn = true;
        UIManager.Instance.ShowTurnText(PlayerTurn);
        for(int i=0; i<playerUnits.Count; i++)
        {
            Pathfind.Instance.SetUnitToTile(playerUnits[i], Pathfind.Instance.GetTile(playerUnits[i].x, playerUnits[i].y));

        }
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            Pathfind.Instance.SetUnitToTile(enemyUnits[i], Pathfind.Instance.GetTile(enemyUnits[i].x, enemyUnits[i].y));
        }
        // 플레이어 유닛 설정
        
        for (int i = 0; i < playerUnits.Count; i++)
        {
            //playerUnits[i].GetMovableTiles();
           // playerUnits[i].GetAttackTiles();
            playerUnits[i].prefab = playerUnits[i].GetComponent<SPUM_Prefabs>();
        }
        // 적 유닛 설정
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].GetFindTiles();
            enemyUnits[i].GetMovableTiles();
            enemyUnits[i].GetAttackTiles();
        }
    }
    public void PlayerEndTurn()
    {
        PlayerTurn = false;
        InputManager.Instance.mode = MODE.NONE;
        // 적의 모든 유닛 행동력 회복
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].movementPoint = enemyUnits[i].maxMovementPoint;
            enemyUnits[i].GetAttackTiles();
            enemyUnits[i].GetMovableTiles();
        }

        UIManager.Instance.ShowTurnText(PlayerTurn);
        DeselectUnit();

        Ai.EnemyTurn(enemyUnits);
    }
    public void EnemyEndTurn()
    {
        PlayerTurn = true;
        InputManager.Instance.mode = MODE.NONE;
        // 플레이어의 모든 유닛 행동력 회복
        for(int i=0; i<playerUnits.Count; i++)
        {
            playerUnits[i].movementPoint = playerUnits[i].maxMovementPoint;
            playerUnits[i].GetAttackTiles();
            playerUnits[i].GetMovableTiles();
        }

        UIManager.Instance.ShowTurnText(PlayerTurn);
    }
    public void SelectUnit(Unit unit)
    {
        currentUnit = unit;
        indicator.ClearRange();
        if (unit.isPlayerUnit == true)
        {
            if (unit.movementPoint > 0 && PlayerTurn)
            {

            }
        }
        else
        {
            indicator.ShowRange(unit.findTiles, findColor);
        }
        /*
        UIManager.Instance.ShowSelectCircle();
        UIManager.Instance.MoveSelectCircle(unit);
        */
    }

    public void DeselectUnit()
    {
        currentUnit = null;
        UIManager.Instance.HideCommandWindow();
        indicator.ClearRange();
    }
    #region 유닛 이동
    public bool IsMovableTile(FieldTile selectedTile)
    {
        return currentUnit.CanMoveTo(selectedTile);
    }
    public void MoveUnitReady()
    {
        indicator.ClearRange();
        currentUnit.GetMovableTiles();
        indicator.ShowRange(currentUnit.movableTiles, movableColor);
        InputManager.Instance.mode = MODE.MOVE;
    }

    public void MoveUnitCancel()
    {
        indicator.ClearRange();
        InputManager.Instance.mode = MODE.NONE;
    }

    public void MoveUnit(Unit unit, FieldTile selectedTile)
    {
        // currentUnit이 현재 이동하려는 유닛이고
        // selectedTile은 이동하고자 하는 목적지 타일
        if(unit.CanMoveTo(selectedTile))
        {
            FieldTile original = Pathfind.Instance.GetTile(unit.x, unit.y);

            // TODO: 명령(Command) 추가해야함
            Command move = Command.MoveCommand(unit, original, selectedTile);
            commands.Add(move);
            Debug.Log("명령: " + move.type);

            original.unit = null;
            indicator.ClearRange();
            List<Pathfind.Node> path = Pathfind.Instance.GetPath(original, selectedTile, unit.movementPoint, false, false);
            StartCoroutine(MoveAnimation(unit, path, MoveSpeed));
        }
        else
        {
            // 이동할 수 없을때
            UIManager.Instance.ShowMessageText("행동력이 부족합니다", 1f);
            indicator.ClearRange();
        }
        
        InputManager.Instance.mode = MODE.NONE;
        //UIManager.Instance.HideCommandWindow();
        
    } 
    IEnumerator MoveAnimation(Unit unit, List<Pathfind.Node> path, float speed)
    {
        if (path.Count == 0)
            yield break;
        Vector3 start = path[path.Count - 1].tile.transform.position;
        for (int i = path.Count - 1; i >= 0; i--)
        {
            unit.prefab.PlayAnimation(1);
            Vector3 end = path[i].tile.transform.position;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                unit.pivot.transform.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }
            unit.prefab.PlayAnimation(0);
            yield return moveWaitTime;
            start = end;
        }

        Debug.Log("이동 애니메이션 끝");

        path[0].tile.unit = currentUnit;
        unit.pivot.transform.localPosition = path[0].tile.transform.localPosition;
        unit.x = path[0].tile.x;
        unit.y = path[0].tile.y;
        unit.movementPoint -= path[0].cost;
        //이동하고 난 후의 이동가능 타일 다시 계산
        unit.GetMovableTiles();
        unit.GetAttackTiles();
        unit.prefab.PlayAnimation(0);
    }

    public void MoveFinished()
    {

    }
    #endregion
    #region 유닛 공격
    public void AttackUnitReady()
    {
        List<FieldTile> attackTiles = currentUnit.GetAttackTiles();

        indicator.ClearRange();
        indicator.ShowRange(attackTiles, attackableColor);
        for (int i = 0; i < attackTiles.Count; i++)
        {
            if (attackTiles[i].unit != null && attackTiles[i].unit.isPlayerUnit == false)
            {
                indicator.ShowTile(attackTiles[i], attackTargetColor);
            }
        }
        InputManager.Instance.mode = MODE.ATTACK;

    }

    public void AttackUnitCancel()
    {
        indicator.ClearRange();
        InputManager.Instance.mode = MODE.NONE;
    }

    public void Attack(Unit unit, Unit target)
    {
        if (unit.CanAttack(target))
        {
            StartCoroutine(AttackAnimation(unit, target));
            /*
            // 공격식 계산
            int damage = unit.attack;
            // 공격식 타겟 HP에 적용
            target.TakeDamage(damage);
            // 죽음 판정
            if (target.hp <= 0)
            {
                Debug.LogWarning("죽음 판정 구현 필요");

            }
            else // 죽지 않았다면 반격식 계산
            {
                Debug.LogWarning("반격 구현 필요");
                int riptoste = (int)(target.attack * 0.7f);
                unit.TakeDamage(riptoste);
                // 죽음 판정
                if (unit.hp <= 0)
                {

                }
            }
            */
        }
        else
        {
            // 이동할 수 없을때
            UIManager.Instance.ShowMessageText("공격할 수 없습니다.", 1f);
            indicator.ClearRange();
        }
    }

    IEnumerator AttackAnimation(Unit unit, Unit target)
    {
        FieldTile unitTile = Pathfind.Instance.GetTile(unit.x, unit.y);
        FieldTile targetTile = Pathfind.Instance.GetTile(target.x, target.y);
        // 전투 연출 씬 타일의 스프라이트 변경
        leftTile.GetComponent<SpriteRenderer>().sprite = unitTile.GetComponent<SpriteRenderer>().sprite;
        rightTile.GetComponent<SpriteRenderer>().sprite = targetTile.GetComponent<SpriteRenderer>().sprite;
        battleScene.SetActive(true);
        // 왼쪽 유닛 설정
        Unit leftUnit = Instantiate(unit, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), leftCharacter.transform);
        Destroy(leftUnit.slider.gameObject);
        leftUnit.pivot.transform.localPosition = new Vector3(0, 0, 0);
        leftUnit.gameObject.GetComponentInChildren<SortingGroup>().sortingLayerName = "Foreground Object";
        // 오른쪽 유닛 설정
        Unit rightUnit = Instantiate(target, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), rightCharacter.transform);
        Destroy(rightUnit.slider.gameObject);
        rightUnit.pivot.transform.localPosition = new Vector3(0, 0, 0);
        rightUnit.gameObject.GetComponentInChildren<SortingGroup>().sortingLayerName = "Foreground Object";
        // 공격 애니메이션 재생
        // TODO: 무기에 따라 다른 애니메이션과 소리 재생
        yield return new WaitForSeconds(0.5f);
        leftUnit.prefab.PlayAnimation(4);
        yield return new WaitForSeconds(0.3f);
        rightUnit.prefab.PlayAnimation(3);
        yield return new WaitForSeconds(0.5f);
        rightUnit.prefab.PlayAnimation(0);
        yield return new WaitForSeconds(0.5f);
        // 공격 데미지 계산
        int damage = unit.attack;
        // 공격식 타겟 HP에 적용
        target.TakeDamage(damage);
        // 죽음 판정
        if (target.hp <= 0)
        {
            OnUnitDead(target);
        }
        else // 죽지 않았다면 반격식 계산
        {
            Debug.LogWarning("반격 구현 필요");
            if(target.CanAttack(unit))
            {
                // 반격 데미지 계산
                int riptoste = (int)(target.attack * 0.7f);
                unit.TakeDamage(riptoste);
                // 반격 애니메이션 재생
                // 죽음 판정
                if (unit.hp <= 0)
                {
                    OnUnitDead(unit);
                }
            }
        }



        // 연출 종료
        Destroy(leftUnit.gameObject);
        Destroy(rightUnit.gameObject);
        battleScene.SetActive(false);
        // 후처리
        indicator.ClearRange();
        InputManager.Instance.mode = MODE.NONE;
        UIManager.Instance.HideCommandWindow();

        unit.turnEnded = true;

        yield return null;
    }
    #endregion

    private void OnUnitDead(Unit unit)
    {
        Debug.LogWarning("죽음 판정 구현 필요");
    }

    public void ShowFindTiles(bool show)
    {
        InputManager.Instance.mode = MODE.NONE;
        indicator.ClearRange();
        if (show==true)
        {
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                indicator.ShowRange(enemyUnits[i].findTiles, findColor);
            }
        }
        else
        {
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                indicator.ShowRange(enemyUnits[i].findTiles, Color.white);
            }
        }
    }

    public void Reverse()
    {
        Command recent = commands[commands.Count - 1];
        commands.RemoveAt(commands.Count - 1);
        Debug.Log(recent.type);
        switch (recent.type)
        {
            case Command.COMMAND.MOVE:
                // 이동 번복
                // targetTile에 있던 unit을 다시 tile로 이동시킴
                int distance = Pathfind.Instance.GetDistance(recent.targetTile, recent.tile);
                recent.unit.movementPoint += distance;
                recent.tile.unit = recent.unit;
                recent.targetTile.unit = null;
                recent.unit.pivot.transform.localPosition = recent.tile.transform.localPosition;
                recent.unit.x = recent.tile.x;
                recent.unit.y = recent.tile.y;
                recent.unit.GetMovableTiles();
                recent.unit.GetAttackTiles();
                break;
        }
    }
}
