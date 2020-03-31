using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCommand
{
    public int EntityId;
    public BattleCommandType CommandType;

    public CommandMoveInfo MoveInfo;
    public CommandPutSkillInfo PutSkillInfo;
    public CommandPlayAniInfo PlayAniInfo;

    public class CommandMoveInfo
    {
        public Vector3 DestPos;
    }

    public class CommandPutSkillInfo
    {
        public int SkillId;
    }

    public class CommandPlayAniInfo
    {
        public string AniName;
    }
}

public class BattleRenderCommand
{
    public int EntityId;
    public string AniName;
}
