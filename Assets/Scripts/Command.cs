using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Command
{// 플레이어 및 AI의 명령
    public enum COMMAND
    {
        MOVE,
        ATTACK,
        SKILL,
    }
    public Unit unit; // 행동 주체
    public Unit targetUnit; // 행동 타겟 유닛
    public FieldTile tile; // 타일 주체
    public FieldTile targetTile; // 행동 타겟 타일
    public int srcX, srcY; // 원래 위치
    public int dstX, dstY; // 대상 위치
    public int movementPoint;
    // public SKill skill; // 스킬
    public COMMAND type;
    private Command() { }
    public static Command MoveCommand(Unit movingUnit, FieldTile original, FieldTile target)
    {
        Command c = new Command();
        c.unit = movingUnit;
        c.tile = original;
        c.targetTile = target;
        c.type = COMMAND.MOVE;
        return c;
    }
    public static Command AttackCommand(Unit unit, Unit target)
    {
        Command c = new Command();
        c.unit = unit;
        c.targetUnit = target;
        c.type = COMMAND.ATTACK;
        return c;
    }
}

