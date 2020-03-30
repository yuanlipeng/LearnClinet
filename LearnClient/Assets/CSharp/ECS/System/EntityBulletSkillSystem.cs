using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class EntityBulletSkillSystem : IExecuteSystem 
{
    public void Execute()
    {
        List<GameEntity> removeList = new List<GameEntity>();
        Contexts contexts = EntityMgr.Instance.GetContexts();
        HashSet<GameEntity> gameEntities = contexts.game.GetEntitiesWithEntityInfoCompEntityType(EntityType.Bullet);
        foreach(var item in gameEntities)
        {
            HashSet<GameEntity> monsters = contexts.game.GetEntitiesWithEntityInfoCompEntityType(EntityType.Monster);

            foreach (var monster in monsters)
            {
                float dist = (monster.moveComp.CurPos - item.entityBulletMoveComp.CurPos).sqrMagnitude;
                if (dist <= 1.0f)
                {
                    BattleCommand command = new BattleCommand();
                    command.CommandType = BattleCommandType.Attacked;
                    command.EntityId = monster.entityInfoComp.Id;
                    BattleLoop.Instance.AddCommand(command);
                }
            }

            if (item.entityBulletMoveComp.IsArrived == true)
            {
                removeList.Insert(0, item);
            }
        }

        for(int i = 0; i < removeList.Count; i++)
        {
            EntityMgr.Instance.ReturnEntity(removeList[i]);
        }
    }
}
