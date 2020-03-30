using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMgr
{
    public static BattleMgr Instance = new BattleMgr();

    public void Open()
    {
        EntitySetting.Init();
        BattleLoop.Instance.Init();
        EntityMgr.Instance.Init();
        SkillSetting.Init();

        EntityMgr.Instance.CreateMainPlayer();
        EntityMgr.Instance.CreateEntity(2);
        EntityMgr.Instance.CreateEntity(3);
        EntityMgr.Instance.CreateEntity(4);
    }
}
