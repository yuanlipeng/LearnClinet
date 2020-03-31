using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class EntitySkillSystem : IExecuteSystem
{
    public void Execute()
    {
        List<BattleCommandRuner.PutedSkillInfo> removeList = new List<BattleCommandRuner.PutedSkillInfo>();
        List<BattleCommandRuner.PutedSkillInfo> putedSkillInfos = BattleCommandRuner.Instance.GetPutedSkillInfos();
        Contexts contexts = EntityMgr.Instance.GetContexts();
        for (int i = 0; i < putedSkillInfos.Count; i++)
        {
            SkillSetting skillSetting = SkillSetting.SkillSettingDict[putedSkillInfos[i].SkillId];
            if (putedSkillInfos[i].PutSkillTime + skillSetting.SkillAttackTime <= Time.time)
            {
                HashSet<GameEntity> gameEntities = null;
                GameEntity entity = EntityMgr.Instance.GetGameEntity(putedSkillInfos[i].EntityId);
                if(entity.entityInfoComp.EntityType == EntityType.MainPlayer)
                {
                    gameEntities = contexts.game.GetEntitiesWithEntityInfoCompEntityType(EntityType.Monster);
                }
                else
                {
                    gameEntities = contexts.game.GetEntitiesWithEntityInfoCompEntityType(EntityType.MainPlayer);
                }
                foreach (var item in gameEntities)
                {
                    Vector3 forward = entity.moveComp.Forward;
                    Vector3 entityPos = item.moveComp.CurPos;
                    Vector3 skillPutEntityPos = entity.moveComp.CurPos;

                    //Debug.LogError(" 释放者 " + skillPutEntityPos.x + " " + skillPutEntityPos.z);
                    //Debug.LogError(" 被攻击者 " + entityPos.x + " " + entityPos.z);
                    //Debug.LogError(" forward " + forward.x + " " + forward.z);

                    bool isAttacked = false;

                    if (skillSetting.IsAttackForward == true)
                    {
                        if (Mathf.Abs(entityPos.x - skillPutEntityPos.x) <= skillSetting.AttackInfoCo.x &&
                            Mathf.Abs(entityPos.z - skillPutEntityPos.z) <= skillSetting.AttackInfoCo.y)
                        {
                            Vector3 vec1 = new Vector3(entityPos.x - skillPutEntityPos.x, entityPos.y - skillPutEntityPos.y, entityPos.z - skillPutEntityPos.z);
                            vec1 = Vector3.Normalize(vec1);
                            forward = Vector3.Normalize(forward);

                            float cos = Vector3.Dot(vec1, forward);

                            isAttacked = true;
                        }
                    }
                    else
                    {
                        Vector3 diff = entityPos - skillPutEntityPos;
                        if(diff.sqrMagnitude <= skillSetting.AttackInfoCo.r)
                        {
                            isAttacked = true;
                        }
                    }

                    if(isAttacked == true)
                    {
                        BattleCommand command1 = new BattleCommand();
                        command1.CommandType = BattleCommandType.PlayAni;
                        command1.EntityId = item.entityInfoComp.Id;
                        command1.PlayAniInfo = new BattleCommand.CommandPlayAniInfo();
                        command1.PlayAniInfo.AniName = "attacked";
                        BattleLoop.Instance.AddCommand(command1);
                    }
                }
                removeList.Insert(0, putedSkillInfos[i]);
            }
        }

        for(int i = 0; i < removeList.Count; i++)
        {
            BattleCommandRuner.Instance.RemovePutedSkillInfo(removeList[i]);
        }
        
    }
}
