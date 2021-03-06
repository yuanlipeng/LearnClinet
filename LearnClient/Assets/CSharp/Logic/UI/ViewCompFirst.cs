﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewCompFirst : ViewComp
{
    private Animation viewAni;
    private Button closeBtn;

    private void OnClickCloseBtn()
    {
        ViewMgr.Instance.Close("View_1");
    }

    protected override void BuildUI()
    {
        viewAni = MainGo.GetComponent<Animation>();
        closeBtn = MainGo.transform.Find("Button").GetComponent<Button>();
    }

    protected override void AddClickListener()
    {
        closeBtn.onClick.AddListener(OnClickCloseBtn);
    }

    protected override void OnEnter()
    {
        //Debug.LogError(ViewName + " OnEnter ");
    }

    protected override void PlayEnterAnimation()
    {
        viewAni.Play("View_1_Open");
        Timer.Instance.AddTimer("View_1_EnterAniEnded", OnPlayAniEnded, 0.5f, false);
    }

    protected override void AddEvent()
    {
        //Debug.LogError(ViewName + " AddEvent ");
    }

    protected override void OnEnterFinished()
    {
        //Debug.LogError(ViewName + " OnEnterFinished ");
    }

    protected override void RemoveEvent()
    {
        //Debug.LogError(ViewName + " RemoveEvent ");
    }

    protected override void PlayExitAnimation()
    {
        //Debug.LogError(ViewName + " PlayExitAnimation ");
        //base.PlayExitAnimation();
        viewAni.Play("View_1_Close");
        //StartCoroutine(ExitAniEnded());
        Timer.Instance.AddTimer("View_1_ExitAniEnded", OnExitAniEnded, 0.5f, false);
    }

    protected override void OnExit()
    {
        Timer.Instance.RemoveTimer("View_1_EnterAniEnded");
        //Debug.LogError(ViewName + " OnExit ");
    }

    protected override void DestroyUI()
    {
        Timer.Instance.RemoveTimer("View_1_ExitAniEnded");
        Timer.Instance.RemoveTimer("View_1_EnterAniEnded");
        //Debug.LogError(ViewName + " DestroyUI ");
    }
}
