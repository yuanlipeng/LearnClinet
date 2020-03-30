using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRenderMgr
{
    public static BattleRenderMgr Instance = new BattleRenderMgr();

    private List<BattleRenderCommand> mRenderCommands = new List<BattleRenderCommand>();

    public void Render()
    {
        for(int i = 0; i < mRenderCommands.Count; i++)
        {
            GameEntity entity = EntityMgr.Instance.GetGameEntity(mRenderCommands[i].EntityId);
            if (entity != null)
            {
                Animator animator = entity.entityRenderComp.MainGo.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetTrigger(mRenderCommands[i].AniName);
                }
            }
        }

        mRenderCommands.Clear();
    }

    public void AddCommand(BattleRenderCommand renderCommand)
    {
        mRenderCommands.Insert(0, renderCommand);
    }
}
