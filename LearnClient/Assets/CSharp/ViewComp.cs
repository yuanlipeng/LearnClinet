using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewComp : MonoBehaviour
{
    public GameObject HintRootGo;
    public GameObject MainGo;
    public string ViewName;

    public bool IsOpen;

    public void SetAtLastSibling()
    {
        HintRootGo.transform.SetAsLastSibling();
    }

    public bool IsLoaded()
    {
        return (MainGo != null);
    }

    public void Destory()
    {
        StartCoroutine(destroyTimer());
    }

    IEnumerator destroyTimer()
    {
        ViewSetting view = ViewSetting.ViewDict[ViewName];
        yield return new WaitForSeconds(view.WaitTime);
        GameObject.Destroy(MainGo);
        AssetManager.Release(view.Key);
    }

    public void Enter()
    {
        this.OnEnter();

        MainGo.SetActive(true);

        PlayEnterAni();
    }

    public void Exit()
    {
        if(MainGo == null)
        {
            return;
        }
        this.OnExit();

        PlayExitAni();
    }

    IEnumerator onPlayAniEnded()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        OnEnterFinished();
    }

    public void OnPlayAniEnded()
    {
        StartCoroutine(onPlayAniEnded());
    }

    IEnumerator onExitAniEnded()
    {
        MainGo.SetActive(false);

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        OnExitFinished();
    }

    public void OnExitAniEnded()
    {
        StartCoroutine(onExitAniEnded());
    }

    protected virtual void OnEnter()
    {

    }

    protected virtual void PlayEnterAni()
    {
        OnPlayAniEnded();
    }

    protected virtual void OnEnterFinished()
    {

    }

    protected virtual void OnExit()
    {

    }

    protected virtual void PlayExitAni()
    {
        OnExitAniEnded();
    }

    protected virtual void OnExitFinished()
    {

    }
}
