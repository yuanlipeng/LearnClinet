using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class BattleLoop
{
    public static BattleLoop Instance = new BattleLoop();

    private int mFrameRate = 25;
    private string mTimerStr = "BattleLoop";

    private List<BattleCommand> mBattleCommandList = new List<BattleCommand>();
    private List<BattleCommand> mCacheCommandList = new List<BattleCommand>();
    private List<BattleRenderCommand> mBattleRenderCommandList = new List<BattleRenderCommand>();

    private Systems mBattleSystem;
    public void Init()
    {
        Timer.Instance.AddTimer(mTimerStr, loop, 1.0f / mFrameRate, true);

        mBattleSystem = new BattleSystems();
        mBattleSystem.Initialize();
    }

    public void Reset()
    {
        Timer.Instance.RemoveTimer(mTimerStr);
        GameObject.Destroy(GameObject.Find("Battle Systems"));
        mBattleSystem = null;
    }

    private void loop()
    {
        for(int i = 0; i < mBattleCommandList.Count; i++)
        {
            BattleCommandRuner.Instance.DoCommand(mBattleCommandList[i]);
        }

        mBattleCommandList.Clear();

        mBattleSystem.Execute();
        mBattleSystem.Cleanup();

        BattleRenderMgr.Instance.Render();

        for(int i = 0; i < mCacheCommandList.Count; i++)
        {
            mBattleCommandList.Insert(0, mCacheCommandList[i]);
        }
        mCacheCommandList.Clear();
    }

    public List<BattleCommand> GetBattleCommands()
    {
        return mBattleCommandList;
    }

    public List<BattleRenderCommand> GetBattleRenderCommands()
    {
        return mBattleRenderCommandList;
    }

    public void AddCommand(BattleCommand command)
    {
        mCacheCommandList.Insert(0, command);
    }

    public void AddRenderCommand(BattleRenderCommand command)
    {
        mBattleRenderCommandList.Insert(0, command);
    }
}
