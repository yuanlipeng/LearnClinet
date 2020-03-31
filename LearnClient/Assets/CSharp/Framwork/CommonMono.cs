using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CommonMono : MonoBehaviour
{
    private List<Action> mUpdateEvent = new List<Action>();
    void Start()
    {
        this.AddUpdateEvent(ViewMgr.Instance.Update);
        this.AddUpdateEvent(AssetManager.Update);
        this.AddUpdateEvent(Timer.Instance.Update);
    }

    void Update()
    {
        for(int i = 0; i < mUpdateEvent.Count; i++)
        {
            mUpdateEvent[i]();
        }
    }

    public void AddUpdateEvent(Action cb)
    {
        mUpdateEvent.Add(cb);
    }
}
