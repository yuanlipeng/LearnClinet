using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainView : ViewComp
{
    private Button mAIOpenBtn;
    private Button mAICloseBtn;
    private Button mSkillABtn;
    private Button mSkillBBtn;
    private Button mSkillCBtn;
    private Button mExitBattleBtn;

    protected override void BuildUI()
    {
        mAIOpenBtn = MainGo.transform.Find("GameObject/Button (1)").GetComponent<Button>();
        mAICloseBtn = MainGo.transform.Find("GameObject/Button (2)").GetComponent<Button>();

        mSkillABtn = MainGo.transform.Find("GameObject (1)/Button (1)").GetComponent<Button>();
        mSkillBBtn = MainGo.transform.Find("GameObject (1)/Button (2)").GetComponent<Button>();
        mSkillCBtn = MainGo.transform.Find("GameObject (1)/Button (3)").GetComponent<Button>();

        mExitBattleBtn = MainGo.transform.Find("Button").GetComponent<Button>();
    }

    protected override void AddClickListener()
    {
        mAIOpenBtn.onClick.AddListener(OnClickAIOpenBtn);
        mAICloseBtn.onClick.AddListener(OnClickAICloseBtn);
        mSkillABtn.onClick.AddListener(OnClickSkillABtn);
        mSkillBBtn.onClick.AddListener(OnClickSkillBBtn);
        mSkillCBtn.onClick.AddListener(OnClickSkillCBtn);
        mExitBattleBtn.onClick.AddListener(OnClickExitBattleBtn);
    }

    protected override void RemoveClickListener()
    {
        mAIOpenBtn.onClick.RemoveAllListeners();
        mAICloseBtn.onClick.RemoveAllListeners();
        mSkillABtn.onClick.RemoveAllListeners();
        mSkillBBtn.onClick.RemoveAllListeners();
        mSkillCBtn.onClick.RemoveAllListeners();
        mExitBattleBtn.onClick.RemoveAllListeners();
    }

    public void OnClickExitBattleBtn()
    {
        BattleMgr.Instance.Close();
    }

    public void OnClickAIOpenBtn()
    {
        BattleMgr.Instance.SetAIOpenState(true);
    }

    public void OnClickAICloseBtn()
    {
        BattleMgr.Instance.SetAIOpenState(false);
    }

    public void OnClickSkillABtn()
    {
        BattleCommand command = new BattleCommand();
        command.EntityId = 1;
        command.CommandType = BattleCommandType.PutSkill;
        command.PutSkillInfo = new BattleCommand.CommandPutSkillInfo();
        command.PutSkillInfo.SkillId = 101;
        BattleLoop.Instance.AddCommand(command);
    }

    public void OnClickSkillBBtn()
    {
        BattleCommand command = new BattleCommand();
        command.EntityId = 1;
        command.CommandType = BattleCommandType.PutSkill;
        command.PutSkillInfo = new BattleCommand.CommandPutSkillInfo();
        command.PutSkillInfo.SkillId = 102;
        BattleLoop.Instance.AddCommand(command);
    }

    public void OnClickSkillCBtn()
    {
        BattleCommand command = new BattleCommand();
        command.EntityId = 1;
        command.CommandType = BattleCommandType.PutSkill;
        command.PutSkillInfo = new BattleCommand.CommandPutSkillInfo();
        command.PutSkillInfo.SkillId = 103;
        BattleLoop.Instance.AddCommand(command);
    }
}
