using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class EntityMoveSystem : IExecuteSystem
{
    //private Contexts mContext;
    //public EntityMoveSystem(Contexts content)
    //{
    //    mContext = content;
    //}
    public void Execute()
    {
        Contexts contexts = EntityMgr.Instance.GetContexts();
        IGroup<GameEntity> group = contexts.game.GetGroup(GameMatcher.MoveComp);
        foreach(var item in group)
        {
            updateEntityPos(item);
        }
    }

    private void updateEntityPos(GameEntity entity)
    {
        if(entity.moveComp.IsArrived == true)
        {
            return;
        }

        if(entity.moveComp.IsBlock)
        {
            return;
        }

        Vector3 dir = entity.moveComp.DestPos - entity.moveComp.CurPos;
        Vector3 diff = Vector3.Normalize(dir) * entity.moveComp.Speed;
        Vector3 destPos = entity.moveComp.CurPos + diff;

        if (diff.sqrMagnitude >= dir.sqrMagnitude)
        {
            entity.moveComp.CurPos = entity.moveComp.DestPos;
            entity.moveComp.IsArrived = true;

            if(entity.moveComp.IsAniMove == false) 
            {
                BattleRenderCommand command = new BattleRenderCommand();
                command.EntityId = entity.entityInfoComp.Id;
                command.AniName = "idel";
                BattleRenderMgr.Instance.AddCommand(command);
            }
        }
        else
        {
            entity.moveComp.CurPos = destPos;
        }
    }
}
