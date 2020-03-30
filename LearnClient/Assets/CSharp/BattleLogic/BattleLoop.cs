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
    private List<BattleRenderCommand> mBattleRenderCommandList = new List<BattleRenderCommand>();

    private Systems mBattleSystem;
    public void Init()
    {
        Timer.Instance.AddTimer(mTimerStr, loop, 1.0f / mFrameRate, true);

        mBattleSystem = new BattleSystems();
    }

    private void loop()
    {
        for(int i = 0; i < mBattleCommandList.Count; i++)
        {
            BattleCommandRuner.Instance.DoCommand(mBattleCommandList[i]);
        }

        mBattleCommandList.Clear();

        mBattleSystem.Execute();

        BattleRenderMgr.Instance.Render();
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
        mBattleCommandList.Insert(0, command);
    }

    public void AddRenderCommand(BattleRenderCommand command)
    {
        mBattleRenderCommandList.Insert(0, command);
    }
}
