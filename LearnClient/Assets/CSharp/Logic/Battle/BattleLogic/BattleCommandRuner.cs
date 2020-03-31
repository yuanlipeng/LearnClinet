using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCommandRuner
{
    public class PutedSkillInfo
    {
        public int EntityId;
        public float PutSkillTime;
        public int SkillId;

        public bool IsTimeOver()
        {
            SkillSetting skillSetting = SkillSetting.SkillSettingDict[SkillId];
            if (PutSkillTime + skillSetting.SkillAttackTime <= Time.time)
            {
                return true;
            }
            return false;
        }
    }

    public static BattleCommandRuner Instance = new BattleCommandRuner();

    private List<PutedSkillInfo> mPutedSkillInfos = new List<PutedSkillInfo>();

    public void DoCommand(BattleCommand command)
    {
        int entityId = command.EntityId;
        if(command.CommandType == BattleCommandType.Move)
        {
            EntitySetting setting = EntitySetting.Setting[entityId];
            GameEntity gameEntity = EntityMgr.Instance.GetGameEntity(entityId);
            MoveComp moveComp = gameEntity.GetComponent(GameComponentsLookup.MoveComp) as MoveComp;
            moveComp.DestPos = command.MoveInfo.DestPos;
            moveComp.IsArrived = false;
            moveComp.Speed = setting.MoveSpeed;
            moveComp.IsAniMove = false;
            moveComp.Forward = Vector3.Normalize(moveComp.DestPos - moveComp.CurPos);

            BattleCommand command1 = new BattleCommand();
            command1.CommandType = BattleCommandType.PlayAni;
            command1.EntityId = entityId;
            command1.PlayAniInfo = new BattleCommand.CommandPlayAniInfo();
            command1.PlayAniInfo.AniName = "walk";
            BattleLoop.Instance.AddCommand(command1);
        }
        else if(command.CommandType == BattleCommandType.PutSkill)
        {
            SkillSetting setting = SkillSetting.SkillSettingDict[command.PutSkillInfo.SkillId];
            GameEntity gameEntity = EntityMgr.Instance.GetGameEntity(entityId);
            MoveComp moveComp = gameEntity.GetComponent(GameComponentsLookup.MoveComp) as MoveComp;
            PutedSkillInfo skillInfo = new PutedSkillInfo();

            skillInfo.EntityId = entityId;
            skillInfo.PutSkillTime = Time.time;
            skillInfo.SkillId = command.PutSkillInfo.SkillId;

            moveComp.Speed = 0.1f;
            moveComp.IsAniMove = false;
            moveComp.IsBlock = false;

            if (setting.IsBlockWalk == true)
            {
                moveComp.DestPos = moveComp.CurPos;
                moveComp.IsArrived = true;
            }

            if (setting.IsNeedAniMove == true)
            {
                moveComp.Speed = 0.5f;
                moveComp.IsArrived = false;
                moveComp.IsAniMove = true;
                moveComp.DestPos = moveComp.CurPos + moveComp.Forward * 3.0f;
            }

            if(setting.IsBulletShoot == true)
            {
                moveComp.IsBlock = true;
                Timer.Instance.AddTimer(BattleTimerName.BulletMove, () =>
                {
                    Contexts contexts = EntityMgr.Instance.GetContexts();
                    HashSet<GameEntity> gameEntities = contexts.game.GetEntitiesWithEntityInfoCompEntityType(EntityType.Monster);
                    int monsterId = 1;
                    float dist = 10000.0f;
                    foreach (var item in gameEntities)
                    {
                        float diff = (item.moveComp.CurPos - gameEntity.moveComp.CurPos).sqrMagnitude;
                        if (diff < dist)
                        {
                            dist = diff;
                            monsterId = item.entityInfoComp.Id;
                        }
                    }

                    if (monsterId != 1)
                    {
                        GameEntity bullet = EntityMgr.Instance.CreateBullet();
                        bullet.entityBulletMoveComp.CurPos = gameEntity.moveComp.CurPos + new Vector3(0, 2.0f, 0);
                        bullet.entityBulletMoveComp.DestEntityId = monsterId;
                        mPutedSkillInfos.Insert(0, skillInfo);
                    }

                    moveComp.IsBlock = false;

                    if(gameEntity.moveComp.IsArrived == false)
                    {
                        BattleCommand command2 = new BattleCommand();
                        command2.CommandType = BattleCommandType.PlayAni;
                        command2.EntityId = entityId;
                        command2.PlayAniInfo = new BattleCommand.CommandPlayAniInfo();
                        command2.PlayAniInfo.AniName = "walk";
                        BattleLoop.Instance.AddCommand(command2);
                    }
                }, 0.6f, false);
            }
            else
            {
                mPutedSkillInfos.Insert(0, skillInfo);
            }

            BattleCommand command1 = new BattleCommand();
            command1.CommandType = BattleCommandType.PlayAni;
            command1.EntityId = entityId;
            command1.PlayAniInfo = new BattleCommand.CommandPlayAniInfo();
            command1.PlayAniInfo.AniName = setting.AniName;
            BattleLoop.Instance.AddCommand(command1);
        }
        else if(command.CommandType == BattleCommandType.PlayAni)
        {
            if(command.PlayAniInfo.AniName.Equals("attacked") == true)
            {
                GameEntity gameEntity = EntityMgr.Instance.GetGameEntity(entityId);
                gameEntity.moveComp.IsArrived = true;
                Timer.Instance.AddTimer(BattleTimerName.AttackedTimer, () =>
                {
                    if(gameEntity.hasEntityAiComp == true)
                    {
                        gameEntity.entityAiComp.IsAIEnded = true;
                    }
                }, 1.0f, false);
            }

            BattleRenderCommand renderCommand = new BattleRenderCommand();
            renderCommand.EntityId = entityId;
            renderCommand.AniName = command.PlayAniInfo.AniName;
            BattleRenderMgr.Instance.AddCommand(renderCommand);
        }
    }

    public List<PutedSkillInfo> GetPutedSkillInfos()
    {
        return mPutedSkillInfos;
    }

    public void RemovePutedSkillInfo(PutedSkillInfo info)
    {
        mPutedSkillInfos.Remove(info);
    }

    public bool IsPuttingSkill(int entityId)
    {
        for(int i = 0; i < mPutedSkillInfos.Count; i++)
        {
            if(mPutedSkillInfos[i].EntityId == entityId )
            {
                return true;
            }
        }
        return false;
    }
}
