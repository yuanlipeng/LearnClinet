using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class EntityAISystem : IExecuteSystem
{
    public void Execute()
    {
        if (BattleMgr.Instance.IsAIOpen() == false)
        {
            return;
        }
        Contexts contexts = EntityMgr.Instance.GetContexts();
        IGroup<GameEntity> group = contexts.game.GetGroup(GameMatcher.EntityAiComp);
        foreach(var item in group)
        {
            //updateAI(item);
            if(searchMonster(item) == false)
            {
                updateAI(item);
            }
        }
    }

    private bool searchMonster(GameEntity monster)
    {
        EntitySetting setting = EntitySetting.Setting[monster.entityInfoComp.Id];
        Vector3 bornPos = setting.BornPos;
        Vector3 monsterPos = monster.moveComp.CurPos;
        float minX = bornPos.x - monster.entityAiComp.ScopeX;
        float maxX = bornPos.x + monster.entityAiComp.ScopeX;
        float minY = bornPos.z - monster.entityAiComp.ScopeY;
        float maxY = bornPos.z + monster.entityAiComp.ScopeY;
        Contexts contexts = EntityMgr.Instance.GetContexts();
        HashSet<GameEntity> gameEntities = contexts.game.GetEntitiesWithEntityInfoCompEntityType(EntityType.MainPlayer);
        foreach(var item in gameEntities)
        {
            Vector3 mainPlayerPos = item.moveComp.CurPos;
            if(mainPlayerPos.x >= minX && mainPlayerPos.x <= maxX && mainPlayerPos.z >= minY && mainPlayerPos.z <= maxY)
            {
                Vector3 dir = monsterPos - mainPlayerPos;
                if(dir.sqrMagnitude < 1.5f) 
                {
                    //开始攻击
                    if (BattleCommandRuner.Instance.IsPuttingSkill(monster.entityInfoComp.Id) == false)
                    {
                        //Debug.LogError(" 开始攻击 " + monster.entityInfoComp.Id);
                        BattleCommand command = new BattleCommand();
                        command.CommandType = BattleCommandType.PutSkill;
                        command.EntityId = monster.entityInfoComp.Id;
                        command.PutSkillInfo = new BattleCommand.CommandPutSkillInfo();
                        command.PutSkillInfo.SkillId = 101;
                        BattleLoop.Instance.AddCommand(command);
                    }
                }
                else
                {
                    //移动过去
                    Vector3 destPos = mainPlayerPos + Vector3.Normalize(dir);
                    //monster.moveComp.DestPos = destPos;
                    //monster.entityAiComp.IsAIEnded = false;
                    if (monster.moveComp.IsArrived == true)
                    {
                        BattleCommand command = new BattleCommand();
                        command.CommandType = BattleCommandType.Move;
                        command.EntityId = monster.entityInfoComp.Id;
                        command.MoveInfo = new BattleCommand.CommandMoveInfo();
                        command.MoveInfo.DestPos = destPos;
                        BattleLoop.Instance.AddCommand(command);
                    }
                }
                return true;
            }
        }
        return false;
    }

    private void updateAI(GameEntity entity)
    {
        if (entity.entityAiComp.IsAIEnded == true)
        {
            EntitySetting setting = EntitySetting.Setting[entity.entityInfoComp.Id];
            float scopeX = entity.entityAiComp.ScopeX;
            float scopeY = entity.entityAiComp.ScopeY;

            float randomX = Random.Range(-scopeX, scopeX);
            float randomY = Random.Range(-scopeY, scopeY);

            Vector3 destPos = new Vector3(setting.BornPos.x + randomX, setting.BornPos.y, setting.BornPos.z + randomY);
            //entity.moveComp.DestPos = destPos;
            //entity.moveComp.IsArrived = false;
            BattleCommand command = new BattleCommand();
            command.CommandType = BattleCommandType.Move;
            command.EntityId = entity.entityInfoComp.Id;
            command.MoveInfo = new BattleCommand.CommandMoveInfo();
            command.MoveInfo.DestPos = destPos;
            BattleLoop.Instance.AddCommand(command);

            entity.entityAiComp.IsAIEnded = false;
        }
    }
}
