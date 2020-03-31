using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class EntityBulletMoveSystem : IExecuteSystem
{
    public void Execute()
    {
        Contexts contexts = EntityMgr.Instance.GetContexts();
        IGroup<GameEntity> group = contexts.game.GetGroup(GameMatcher.EntityBulletMoveComp);
        foreach (var item in group)
        {
            updateEntityPos(item);
        }
    }

    private void updateEntityPos(GameEntity entity)
    {
        int followEntityId = entity.entityBulletMoveComp.DestEntityId;
        GameEntity gameEntity = EntityMgr.Instance.GetGameEntity(followEntityId);
        Vector3 destPos = gameEntity.moveComp.CurPos;
        Vector3 dir = destPos - entity.entityBulletMoveComp.CurPos;
        Vector3 diff = Vector3.Normalize(dir) * entity.entityBulletMoveComp.Speed;
        Vector3 newDestPos = entity.entityBulletMoveComp.CurPos + diff;

        if (diff.sqrMagnitude >= dir.sqrMagnitude)
        {
            entity.entityBulletMoveComp.CurPos = destPos;
            entity.entityBulletMoveComp.IsArrived = true;
        }
        else
        {
            entity.entityBulletMoveComp.CurPos = newDestPos;
        }
    }
}
