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
    }

    public static BattleCommandRuner Instance = new BattleCommandRuner();

    private List<PutedSkillInfo> mPutedSkillInfos = new List<PutedSkillInfo>();

    public void DoCommand(BattleCommand command)
    {
        int entityId = command.EntityId;
        if(command.CommandType == BattleCommandType.Move)
        {
            GameEntity gameEntity = EntityMgr.Instance.GetGameEntity(entityId);
            MoveComp moveComp = gameEntity.GetComponent(GameComponentsLookup.MoveComp) as MoveComp;
            moveComp.DestPos = command.MoveInfo.DestPos;
            moveComp.IsArrived = false;
            moveComp.Speed = 0.1f;
            moveComp.IsAniMove = false;
            moveComp.Forward = Vector3.Normalize(moveComp.DestPos - moveComp.CurPos);

            BattleRenderCommand renderCommand = new BattleRenderCommand();
            renderCommand.EntityId = entityId;
            renderCommand.AniName = "walk";
            BattleRenderMgr.Instance.AddCommand(renderCommand);
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
                //UnitBullet bullet = EntirtyMgr.Instance.CreateBullet();
                //MoveComp bulletMoveComp = bullet.GetComp(CompType.Move) as MoveComp;
                //bulletMoveComp.SetCurPos(entity.HintRootGo.transform.position);
                //bulletMoveComp.SetSpeed(0.5f);
                //bulletMoveComp.SetDestPos(new Vector3(0, 0, 0));

                ////moveComp.SetIsStop(true);
                moveComp.IsBlock = true;
                Timer.Instance.AddTimer("Move", () =>
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
                    BattleRenderCommand newRenderCommand = new BattleRenderCommand();
                    newRenderCommand.EntityId = entityId;
                    newRenderCommand.AniName = "walk";
                    BattleRenderMgr.Instance.AddCommand(newRenderCommand);

                }, 0.6f, false);
            }
            else
            {
                mPutedSkillInfos.Insert(0, skillInfo);
            }

            BattleRenderCommand renderCommand = new BattleRenderCommand();
            renderCommand.EntityId = entityId;
            renderCommand.AniName = setting.AniName;
            BattleRenderMgr.Instance.AddCommand(renderCommand);
        }
        else if(command.CommandType == BattleCommandType.Attacked)
        {
            BattleRenderCommand renderCommand = new BattleRenderCommand();
            renderCommand.EntityId = entityId;
            renderCommand.AniName = "attacked";
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
}
