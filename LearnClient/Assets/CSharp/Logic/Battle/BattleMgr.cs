using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMgr
{
    public static BattleMgr Instance = new BattleMgr();

    private bool mIsAIOpen = true;
    private GameObject mScenePrefab;

    public void Open()
    {
        EntitySetting.Init();
        BattleLoop.Instance.Init();
        EntityMgr.Instance.Init();
        SkillSetting.Init();

        ViewMgr.Instance.Open(ViewNames.Main);

        LoadScenePrefab();

        EntityMgr.Instance.CreateMainPlayer();
        EntityMgr.Instance.CreateMonster(2);
        EntityMgr.Instance.CreateMonster(3);
        EntityMgr.Instance.CreateMonster(4);
    }

    public void Close()
    {
        Timer.Instance.RemoveTimer(BattleTimerName.BulletMove);
        Timer.Instance.RemoveTimer(BattleTimerName.AttackedTimer);
        BattleLoop.Instance.Reset();
        EntityMgr.Instance.Reset();

        ViewMgr.Instance.Close(ViewNames.Main);
        ViewMgr.Instance.Open(ViewNames.Login);

        RemoveScenePrefab();
    }

    public void LoadScenePrefab()
    {
        AssetManager.LoadGameObject<GameObject>("Assets/Src/Prefab/ScenePrefab.prefab", (UnityEngine.Object obj) =>
        {
            mScenePrefab = GameObject.Instantiate((GameObject)obj);
            mScenePrefab.transform.SetParent(GameObject.Find("3DRoot/Scene").transform);
        });
    }

    public void RemoveScenePrefab()
    {
        GameObject.Destroy(mScenePrefab);
        AssetManager.Release("Assets/Src/Prefab/ScenePrefab.prefab");
    }

    public void SetAIOpenState(bool flag)
    {
        mIsAIOpen = flag;

        if(mIsAIOpen == false)
        {
            Contexts contexts = EntityMgr.Instance.GetContexts();
            HashSet<GameEntity> gameEntities = contexts.game.GetEntitiesWithEntityInfoCompEntityType(EntityType.Monster);
            foreach(var item in gameEntities)
            {
                BattleCommand battleCommand = new BattleCommand();
                battleCommand.CommandType = BattleCommandType.Move;
                battleCommand.EntityId = item.entityInfoComp.Id;
                battleCommand.MoveInfo = new BattleCommand.CommandMoveInfo();
                battleCommand.MoveInfo.DestPos = item.moveComp.CurPos;
                BattleLoop.Instance.AddCommand(battleCommand);
            }
        }
    }

    public bool IsAIOpen()
    {
        return mIsAIOpen;
    }
}
