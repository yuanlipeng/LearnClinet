using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class EntityAISystem : IExecuteSystem
{
    public void Execute()
    {
        Contexts contexts = EntityMgr.Instance.GetContexts();
        IGroup<GameEntity> group = contexts.game.GetGroup(GameMatcher.EntityAiComp);
        foreach(var item in group)
        {
            updateAI(item);
        }
    }

    private void updateAI(GameEntity entity)
    {
        Vector3 curPos = entity.moveComp.CurPos;
    }
}
