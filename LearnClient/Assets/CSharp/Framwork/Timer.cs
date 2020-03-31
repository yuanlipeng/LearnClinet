using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer
{
    public static Timer Instance = new Timer();
    
    public class TimerCb
    {
        public float BeginTime;
        public bool IsReapet;
        public float Interval;
        public Action Cb;
    }
    private float curWaitTime = 0.0f;
    private Dictionary<string, TimerCb> mTimerCb = new Dictionary<string, TimerCb>();
    // Start is called before the first frame update

    public void AddTimer(string key, Action cb, float interval, bool isReapet)
    {
        TimerCb time = new TimerCb();
        time.BeginTime = curWaitTime;
        time.Interval = interval;
        time.Cb = cb;
        time.IsReapet = isReapet;

        mTimerCb[key] = time;
    }

    public void RemoveTimer(string key)
    {
        mTimerCb.Remove(key);
    }

    // Update is called once per frame
    public void Update()
    {
        if (mTimerCb.Count == 0)
        {
            return;
        }
        curWaitTime = curWaitTime + Time.deltaTime * Time.timeScale;

        List<string> needRemoveList = new List<string>();
        List<string> keys = new List<string>(mTimerCb.Keys);
        for(int i=0;i< keys.Count;i++)
        {
            if (mTimerCb.ContainsKey(keys[i]) == true)
            {
                TimerCb time = mTimerCb[keys[i]];
                if (time.BeginTime + time.Interval <= curWaitTime)
                {
                    time.Cb();

                    if (time.IsReapet == false)
                    {
                        needRemoveList.Insert(0, keys[i]);
                    }
                    else
                    {
                        time.BeginTime = curWaitTime;
                    }
                }
            }
        }

        for(int i = 0; i < needRemoveList.Count; i++)
        {
            mTimerCb.Remove(needRemoveList[i]);
        }
    }
}
