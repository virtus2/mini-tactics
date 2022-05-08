using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AI : MonoBehaviour
{
    public List<Unit> enemyUnits;
    public void EnemyTurn(List<Unit> enemies)
    {
        StartCoroutine(Test(enemies));
    } 

    IEnumerator Test(List<Unit> enemies)
    {
        // 적 유닛 순차적으로 행동(원래는 행동번호 순으로 해야하지만 그냥 BattleManager의 enemyUnits리스트에 등록된 순으로 실행)
        for (int i = 0; i < enemies.Count; i++)
        {
            Unit enemy = enemies[i];
            enemy.FindUnits();
            Unit target = null;
            Debug.Log("AI: " + enemy.unitName + "행동 중...");
            // 타겟 설정
            if (enemy.profile.changeTarget == true || target == null)
            {
                target = SetTarget(enemy);
            }
            if (target != null)
            {
                Debug.Log(target.unitName + "을 목표 유닛으로 설정");
                // 도망
                // 포착 유닛들의 다음 공격으로 사망하는지 계산
                int totalDamage = 0;
                Debug.Log(enemy.foundUnits.Count);
                for (int j = 0; j < enemy.foundUnits.Count; j++)
                {
                    int dist = Pathfind.Instance.GetDistance(enemy, enemy.foundUnits[j]);
                    if (enemy.foundUnits[j].attackDistance >= dist)
                    {
                        //totalDamage += found[j].attack + (int)found[j].str - enemy.defense;
                        // ???: 지금은 단순히 공격력으로만 계산
                        totalDamage += enemy.foundUnits[j].attack;
                    }
                }
                Debug.Log(totalDamage);
                if (enemy.hp <= totalDamage)
                {
                    // 우선도가 제일 높은 유닛의 반대방향으로 남은 이동력만큼 이동
                    EnemyFlee(enemy, target);
                    continue;
                }
                // 공격
                // TODO: 애니메이션이나 연출 재생을 위해서 일반공격인지 스킬공격인지 기록해야함
                int distance = Pathfind.Instance.GetDistance(enemy, target);
                if (distance <= enemy.attackDistance)
                {
                    EnemyAttack(enemy, target);
                }
                else
                {
                    EnemyMoveForAttack(enemy, target);
                }
            }
            else
            {
                Debug.Log("포착된 유닛이 없음");
            }
            yield return new WaitForSeconds(1.0f);
        }
        // 1.5초뒤에 턴을 넘김
        // 바로 넘기면 너무 없어보임
        BattleManager.Instance.EnemyEndTurn();
    }

    public Unit SetTarget(Unit enemy)
    {
        Unit target = null;
        List<Unit> targets = enemy.foundUnits;
        float max = float.MinValue;
        for(int i=0; i<targets.Count; i++)
        {
            FieldTile targetTile = Pathfind.Instance.GetTile(targets[i].x, targets[i].y);
            FieldTile enemyTile = Pathfind.Instance.GetTile(enemy.x, enemy.y);
            int distance = Pathfind.Instance.GetDistance(targetTile, enemyTile);

            float closest = (1/distance) * enemy.profile.closest; // 가까울수록 값 높음
            float lowestHP = (1/targets[i].hp) * enemy.profile.lowHP; // 체력낮을수록 값 높음
            float highestLV = targets[i].level * enemy.profile.highestLevel; // 레벨 높을수록 값 높음
            float HighestAttack = targets[i].attack * enemy.profile.highestAttack; // 공격력 높을수록 값 높음
            float weight = closest + lowestHP + highestLV + HighestAttack;
            if(weight >= max)
            {
                target = targets[i];
            }
        }
        return target;
    }


    // TODO: 유닛의 실질적인 좌표계산이나 체력 계산 등의 수치 계산적인 부분은 BattleManage의 함수와 합칠 수 있을것
    // 연출도 구현되었을때를 고려해보면 분리해놓는게 나을수도 있음
    // 나중에 생각해보기
    // EnemyMoveTo -> MoveUnit
    // EnemyAttack -> Attack
    public void EnemyMoveTo(Unit enemy, FieldTile tile)
    {
        if (enemy.CanMoveTo(tile))
        {
            // TODO: 명령(Command) 추가해야함
            FieldTile original = Pathfind.Instance.GetTile(enemy.x, enemy.y);
            Command move = Command.MoveCommand(enemy, original, tile);
            // BattleManager.Instance.commands.Add(move);
            // 
            Pathfind.Instance.GetPath(original, tile, enemy.movementPoint, false, false);
            List<Pathfind.Node> path = Pathfind.Instance.GetPath(original, tile, enemy.movementPoint, false, false);
            StartCoroutine(MoveAnimation(enemy, path, 3f));
            original.unit = null;
            tile.unit = enemy;
            enemy.pivot.transform.localPosition = tile.transform.localPosition;
            enemy.x = tile.x;
            enemy.y = tile.y;
            //enemy.movementPoint -= tile.cost;
            //이동하고 난 후의 이동가능 타일 다시 계산
            enemy.GetMovableTiles();
        }
    }
    IEnumerator MoveAnimation(Unit unit, List<Pathfind.Node> path, float speed)
    {
        if (path.Count == 0)
            yield break;
        Vector3 start = path[path.Count - 1].tile.transform.position;
        for (int i = path.Count - 1; i >= 0; i--)
        {
            Vector3 end = path[i].tile.transform.position;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                unit.pivot.transform.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }
            start = end;
        }
        Debug.Log("이동 애니메이션 끝");
        path[0].tile.unit = unit;
        unit.pivot.transform.localPosition = path[0].tile.transform.localPosition;
        unit.x = path[0].tile.x;
        unit.y = path[0].tile.y;
        unit.movementPoint -= path[0].cost;
        //이동하고 난 후의 이동가능 타일 다시 계산
        unit.GetMovableTiles();
        unit.GetAttackTiles();
        unit.GetFindTiles();
    }
    public void EnemyMoveForAttack(Unit enemy, Unit target)
    {
        FieldTile tile = null;
        List<FieldTile> tiles = enemy.movableTiles;
        int min = int.MaxValue;
        for(int i=0; i< tiles.Count; i++)
        {
            int distance = Pathfind.Instance.GetDistance(target, tiles[i]);
            if (distance == enemy.attackDistance)
            {
                tile = tiles[i];
                break;
            }
        }
        EnemyMoveTo(enemy, tile);
    }

    public void EnemyFlee(Unit enemy, Unit target)
    {
        Debug.LogWarning("AI: "+enemy.unitName + "이(가) " + target.unitName + "로부터 도망");
        List<FieldTile> tiles = enemy.movableTiles;
        // 해당 유닛으로부터 제일 먼 타일을 찾음
        // TODO: 거리계산이 제대로 되는지 확인해봐야함
        int t = int.MinValue;
        FieldTile tile = null;
        for(int i=0; i<tiles.Count; i++)
        {
            int distance = Pathfind.Instance.GetDistance(target, tiles[i]);
            if(t <= distance)
            {
                t = distance;
                tile = tiles[i];
            }
        }
        // 제일 먼 타일로 이동
        EnemyMoveTo(enemy, tile);
    }

    public void EnemyAttack(Unit enemy, Unit target)
    {
        Debug.Log("AI: " + enemy.unitName +"이(가) " + target.unitName + "을 공격!");
        
        // 공격식 계산
        int damage = enemy.attack;
        // 공격식 타겟 HP에 적용
        target.TakeDamage(damage);
        // 죽음 판정
        if (target.hp <= 0)
        {
            Debug.LogWarning("AI: " + "죽음 판정 구현 필요");

        }
        else // 죽지 않았다면 반격식 계산
        {
            Debug.LogWarning("AI: " + "반격 구현 필요");
            int riptoste = (int)(target.attack * 0.7f);
            enemy.hp -= riptoste;
            // 죽음 판정
            if (enemy.hp <= 0)
            {

            }
        }
    }

}
