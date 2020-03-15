using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ViewLayer
{
    Bottom,
    Mid,
    Top,
}

public enum ViewType
{
    Full,
    Window,
}

public class ViewSetting
{
    public string Key;
    public Func<GameObject, ViewComp> AddCompCb;
    public bool IsImportant;
    public float WaitTime;
    public ViewLayer Layer;
    public ViewType ViewType;

    public ViewSetting(string key, Func<GameObject, ViewComp> addCompCb, bool isImportant, float waitTime, ViewLayer layer, ViewType viewType)
    {
        Key = key;
        AddCompCb = addCompCb;
        IsImportant = isImportant;
        WaitTime = waitTime;
        Layer = layer;
        ViewType = viewType;
    }

    public static Dictionary<string, ViewSetting> ViewDict = new Dictionary<string, ViewSetting>();

    public static void Init()
    {
        ViewSetting view = null;
        
        view = new ViewSetting("Assets/Src/Prefab/View_1.prefab", (mainGo)=> { return mainGo.AddComponent<ViewCompFirst>(); }, true, 0.0f, ViewLayer.Bottom, ViewType.Full);
        ViewDict["ViewName_1"] = view;

        view = new ViewSetting("Assets/Src/Prefab/View_2.prefab", (mainGo) => { return mainGo.AddComponent<ViewCompSecond>(); }, true, 0.0f, ViewLayer.Bottom, ViewType.Window);
        ViewDict["ViewName_2"] = view;

        view = new ViewSetting("Assets/Src/Prefab/View_3.prefab", (mainGo) => { return mainGo.AddComponent<ViewCompThird>(); }, true, 0.0f, ViewLayer.Top, ViewType.Full);
        ViewDict["ViewName_3"] = view;
    }
}
