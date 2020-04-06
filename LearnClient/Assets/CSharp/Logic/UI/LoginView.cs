using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : ViewComp
{
    private Button mOpenBattleBtn;
    protected override void BuildUI()
    {
        mOpenBattleBtn = MainGo.transform.Find("Button").GetComponent<Button>();
    }

    protected override void AddClickListener()
    {
        mOpenBattleBtn.onClick.AddListener(OnClickOpenBattleBtn);
    }

    protected override void RemoveClickListener()
    {
        mOpenBattleBtn.onClick.RemoveAllListeners();
    }

    private void OnClickOpenBattleBtn()
    {
        ViewMgr.Instance.Close(ViewNames.Login);
        BattleMgr.Instance.Open();
    }
}
