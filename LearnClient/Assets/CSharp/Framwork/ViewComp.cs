using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ViewState
{
    Invalid,
    Loading,
    Entering,
    Entered,
    Exiting,
    Exited,
}

public class ViewComp : MonoBehaviour
{
    public GameObject HintRootGo;
    public GameObject MainGo;
    public string ViewName;

    public ViewState CurViewState = ViewState.Invalid;
    public bool IsDestroyNow = false;

    public void SetAtLastSibling()
    {
        HintRootGo.transform.SetAsLastSibling();
    }

    public void Loaded()
    {
        IsDestroyNow = false;

        this.BuildUI();
        this.AddClickListener();
    }

    public void Destory()
    {
        this.RemoveClickListener();
        this.DestroyUI();
    }

    public void Enter()
    {
        this.OnEnter();

        HintRootGo.SetActive(true);

        PlayEnterAnimation();
    }

    public void Exit()
    {
        if(MainGo == null)
        {
            return;
        }
        this.RemoveEvent();

        PlayExitAnimation();
    }

    public void OnPlayAniEnded()
    {
        AddEvent();
        OnEnterFinished();
        //CurViewStat = ViewState.EnterFinished;

        //HintRootGo.SetActive(ViewMgr.Instance.IsSelfActive(ViewName));
    }

    public void OnExitAniEnded()
    {
        HintRootGo.SetActive(false);
        this.OnExit();
        //this.CurViewStat = ViewState.ExitFinished;
    }

    protected virtual void BuildUI()
    {

    }

    protected virtual void AddClickListener()
    {

    }

    protected virtual void OnEnter()
    {

    }

    protected virtual void PlayEnterAnimation()
    {
        OnPlayAniEnded();
    }

    protected virtual void AddEvent()
    {

    }

    protected virtual void OnEnterFinished()
    {

    }

    protected virtual void RemoveEvent()
    {

    }

    protected virtual void PlayExitAnimation()
    {
        OnExitAniEnded();
    }

    protected virtual void OnExit()
    {

    }

    protected virtual void RemoveClickListener()
    {

    }

    protected virtual void DestroyUI()
    {

    }
}
