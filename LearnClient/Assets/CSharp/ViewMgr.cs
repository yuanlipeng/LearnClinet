using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMgr
{
    public static ViewMgr Instance = new ViewMgr();

    private List<string> mViewStack = new List<string>();
    private Dictionary<string, ViewComp> mViewCompDict = new Dictionary<string, ViewComp>();
    private Dictionary<string, ViewComp> mCacheViewDict = new Dictionary<string, ViewComp>();
    private Dictionary<ViewLayer, Transform> mUIRootDict = new Dictionary<ViewLayer, Transform>();

    public void Init()
    {
        Transform uiRootTrans = GameObject.Find("UIRoot").transform;

        mUIRootDict[ViewLayer.Bottom] = uiRootTrans.Find("Bottom");
        mUIRootDict[ViewLayer.Mid] = uiRootTrans.Find("Mid");
        mUIRootDict[ViewLayer.Top] = uiRootTrans.Find("Top");
    }

    private int getIndex(string viewName)
    {
        int index = -1;
        for (int i = 0; i < mViewStack.Count; i++)
        {
            if (mViewStack[i].Equals(viewName))
            {
                index = i;
            }
        }
        return index;
    }

    private void makeViewEnable()
    {
        int showIndex = mViewStack.Count - 1;
        string viewName = mViewStack[showIndex];
        ViewSetting viewSet = ViewSetting.ViewDict[viewName];
        ViewComp topViewComp = mViewCompDict[viewName];

        topViewComp.SetAtLastSibling();
        topViewComp.Enter();

        if(viewSet.ViewType == ViewType.Window)
        {
            for (int i = showIndex - 1; i >= 0; i--)
            {
                ViewSetting curViewSet = ViewSetting.ViewDict[mViewStack[i]];

                mViewCompDict[viewName].MainGo.SetActive(true);

                if (curViewSet.ViewType == ViewType.Full)
                {
                    return;
                }
            }
        }
        else if(viewSet.ViewType == ViewType.Full)
        {
            for (int i = showIndex - 1; i >= 0; i--)
            {
                ViewSetting curViewSet = ViewSetting.ViewDict[mViewStack[i]];

                mViewCompDict[viewName].MainGo.SetActive(false);

                if (curViewSet.ViewType == ViewType.Full)
                {
                    return;
                }
            }
        }
    }

    public void Open(string viewName)
    {
        if (ViewSetting.ViewDict.ContainsKey(viewName) == false)
        {
            Debug.LogError(" 界面配置不存在 ");
            return;
        }

        ViewSetting view = ViewSetting.ViewDict[viewName];

        if (mViewCompDict.ContainsKey(viewName) == true)
        {
            int index = getIndex(viewName);
            if(index >= 0)
            {
                mViewStack.RemoveAt(index);
                mViewStack.Insert(mViewStack.Count - 1, viewName);
            }

            makeViewEnable();
            return;
        }

        if (mCacheViewDict.ContainsKey(viewName) == true)
        {
            mViewStack.Insert(mViewStack.Count - 1, viewName);
            mViewCompDict[viewName] = mCacheViewDict[viewName];
            makeViewEnable();
            return;
        }

        GameObject hintRootGo = new GameObject();
        hintRootGo.transform.SetParent(mUIRootDict[view.Layer]);

        ViewComp viewComp = view.AddCompCb(hintRootGo);
        viewComp.HintRootGo = hintRootGo;
        viewComp.ViewName = viewName;
        mViewStack.Insert(mViewStack.Count - 1, viewName);
        mViewCompDict[viewName] = viewComp;

        AssetManager.LoadGameObject<GameObject>(view.Key, (mainGo) =>
        {
            if (mViewCompDict.ContainsKey(view.Key) == false)
            {
                AssetManager.Release(view.Key);
                return;
            }

            GameObject rootGo = GameObject.Instantiate((GameObject)mainGo);
            ViewComp rootViewComp = mViewCompDict[view.Key];
            rootViewComp.MainGo = rootGo;
            makeViewEnable();
        });
    }

    public void Close(string viewName)
    {
        if (ViewSetting.ViewDict.ContainsKey(viewName) == false)
        {
            Debug.LogError(" 界面配置不存在 ");
            return;
        }

        if (mViewCompDict[viewName] == false)
        {
            return;
        }

        ViewComp comp = mViewCompDict[viewName];
        comp.Exit();

        mViewCompDict.Remove(viewName);
        int index = getIndex(viewName);
        if (index != -1)
        {
            mViewStack.RemoveAt(index);
        }

        ViewSetting viewSet = ViewSetting.ViewDict[viewName];
        if (viewSet.IsImportant == true)
        {
            mCacheViewDict[viewName] = comp;
        }
        else
        {
            comp.Destory();
        }

        makeViewEnable();
    }

    public void Destroy(string viewName)
    {
        if (ViewSetting.ViewDict.ContainsKey(viewName) == false)
        {
            Debug.LogError(" 界面配置不存在 ");
            return;
        }

        if (mViewCompDict[viewName] == false)
        {
            return;
        }

        Close(viewName);

        ViewSetting viewSet = ViewSetting.ViewDict[viewName];
        if (viewSet.IsImportant == true)
        {
            mCacheViewDict[viewName].Destory();
            mCacheViewDict.Remove(viewName);
        }
    }
}
