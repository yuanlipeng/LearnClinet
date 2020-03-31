using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRenderMgr
{
    public static BattleRenderMgr Instance = new BattleRenderMgr();

    private List<BattleRenderCommand> mRenderCommands = new List<BattleRenderCommand>();

    public void Render()
    {
        List<BattleRenderCommand> removeList = new List<BattleRenderCommand>();
        for(int i = 0; i < mRenderCommands.Count; i++)
        {
            GameEntity entity = EntityMgr.Instance.GetGameEntity(mRenderCommands[i].EntityId);
            if (entity != null && entity.hasEntityRenderComp == true)
            {
                Animator animator = entity.entityRenderComp.MainGo.GetComponent<Animator>();
                if (animator != null)
                {
                    //if(animator.GetCurrentAnimatorStateInfo(0).IsName(mRenderCommands[i].AniName) == false)
                    //{
                        animator.SetTrigger(mRenderCommands[i].AniName);
                    //}
                }

                removeList.Insert(0, mRenderCommands[i]);
            }
        }

        for(int i = 0; i < removeList.Count; i++)
        {
            mRenderCommands.Remove(removeList[i]);
        }
    }

    public void AddCommand(BattleRenderCommand renderCommand)
    {
        mRenderCommands.Insert(0, renderCommand);
    }
}
