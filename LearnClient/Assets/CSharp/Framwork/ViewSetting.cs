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

public class ViewNames
{
    public static string Login = "Login";
    public static string Main = "Main";
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
        
        view = new ViewSetting("Assets/Src/Prefab/LoginPanel.prefab", (mainGo)=> { return mainGo.AddComponent<LoginView>(); }, false, ViewLayer.Logic, ViewType.Full);
        ViewDict[ViewNames.Login] = view;

        view = new ViewSetting("Assets/Src/Prefab/MainPanel.prefab", (mainGo) => { return mainGo.AddComponent<MainView>(); }, false, ViewLayer.Logic, ViewType.Window);
        ViewDict[ViewNames.Main] = view;
    }
}
