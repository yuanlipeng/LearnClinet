using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class EntityMoveSystem : IExecuteSystem
{
    private Contexts mContext;
    public EntityMoveSystem(Contexts content)
    {
        mContext = content;
    }
    public void Execute()
    {
        IGroup<GameEntity> group = mContext.game.GetGroup(GameMatcher.MoveComp);
        foreach(var item in group)
        {
            updateEntityPos(item);
        }
    }

    private void updateEntityPos(GameEntity entity)
    {
        Vector3 dir = entity.moveComp.DestPos - entity.moveComp.CurPos;
        Vector3 destPos = entity.moveComp.CurPos + Vector3.Normalize(dir) * entity.moveComp.Speed;

        if (destPos.sqrMagnitude >= dir.sqrMagnitude)
        {
            entity.moveComp.CurPos = entity.moveComp.DestPos;
        }
        else
        {
            entity.moveComp.CurPos = destPos;
        }
    }
}
