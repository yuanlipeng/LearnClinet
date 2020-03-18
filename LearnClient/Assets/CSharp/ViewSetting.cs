using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ViewLayer
{
    Logic,
    Guide,
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
    public ViewLayer Layer;
    public ViewType ViewType;

    public ViewSetting(string key, Func<GameObject, ViewComp> addCompCb, bool isImportant, ViewLayer layer, ViewType viewType)
    {
        Key = key;
        AddCompCb = addCompCb;
        IsImportant = isImportant;
        Layer = layer;
        ViewType = viewType;
    }

    public static Dictionary<string, ViewSetting> ViewDict = new Dictionary<string, ViewSetting>();

    public static void Init()
    {
        ViewSetting view = null;
        
        view = new ViewSetting("Assets/Src/Prefab/View_1.prefab", (mainGo)=> { return mainGo.AddComponent<ViewCompFirst>(); }, false, ViewLayer.Logic, ViewType.Full);
        ViewDict["View_1"] = view;

        view = new ViewSetting("Assets/Src/Prefab/View_2.prefab", (mainGo) => { return mainGo.AddComponent<ViewCompSecond>(); }, false, ViewLayer.Guide, ViewType.Window);
        ViewDict["View_2"] = view;

        view = new ViewSetting("Assets/Src/Prefab/View_3.prefab", (mainGo) => { return mainGo.AddComponent<ViewCompThird>(); }, true, ViewLayer.Top, ViewType.Full);
        ViewDict["View_3"] = view;
    }
}
