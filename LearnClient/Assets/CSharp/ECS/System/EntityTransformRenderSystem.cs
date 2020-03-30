using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class EntityTransformRenderSystem : IExecuteSystem
{
    public void Execute()
    {
        Contexts contexts = EntityMgr.Instance.GetContexts();
        IGroup<GameEntity> group = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.MoveComp, GameMatcher.EntityRenderComp));
        foreach(var item in group)
        {
            item.entityRenderComp.MainGo.transform.position = item.moveComp.CurPos;
            item.entityRenderComp.MainGo.transform.LookAt(item.moveComp.DestPos);
        }

        IGroup<GameEntity> groups = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.EntityBulletMoveComp, GameMatcher.EntityRenderComp));
        foreach (var item in groups)
        {
            item.entityRenderComp.MainGo.transform.position = item.entityBulletMoveComp.CurPos;
            GameEntity entity = EntityMgr.Instance.GetGameEntity(item.entityBulletMoveComp.DestEntityId);
            if (entity != null)
            {
                item.entityRenderComp.MainGo.transform.LookAt(entity.entityRenderComp.MainGo.transform.position);
            }
        }
    }
}
