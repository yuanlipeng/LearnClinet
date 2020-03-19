using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMgr
{
    public static ViewMgr Instance = new ViewMgr();

    private List<string> mViewStack = new List<string>();
    private Dictionary<string, ViewComp> mViewCompDict = new Dictionary<string, ViewComp>();

    private Dictionary<string, ViewComp> mCacheViewDict = new Dictionary<string, ViewComp>();
    private Dictionary<string, ViewComp> mCloseViewDcit = new Dictionary<string, ViewComp>();

    private Dictionary<ViewLayer, Transform> mUIRootDict = new Dictionary<ViewLayer, Transform>();
    private GameObject mHintRootItem;

    private MonoBehaviour mono;

    private int destroyMaxNum = 3;

    public List<string> ViewStack
    {
        get
        {
            return mViewStack;
        }
    }

    public Dictionary<string, ViewComp> CacheViewDict
    {
        get
        {
            return mCacheViewDict;
        }
    }

    public void Init()
    {
        Transform uiRootTrans = GameObject.Find("UIRoot").transform;

        mUIRootDict[ViewLayer.Logic] = uiRootTrans.Find("Logic");
        mUIRootDict[ViewLayer.Guide] = uiRootTrans.Find("Guide");
        mUIRootDict[ViewLayer.Top] = uiRootTrans.Find("Top");

        mHintRootItem = uiRootTrans.Find("HintRootItem").gameObject;

    }

    public void Update()
    {
        int count = 0;
        List<string> removeList = new List<string>();
        foreach(var item in mCloseViewDcit)
        {
            if (item.Value.CurViewStat == ViewState.ExitFinished)
            {
                ViewSetting curViewSet = ViewSetting.ViewDict[item.Key];
                if (curViewSet.IsImportant == false || item.Value.IsDestroyNow == true)
                {
                    if (count < destroyMaxNum)
                    {
                        count = count + 1;
                        destroyView(item.Value);
                        removeList.Insert(0, item.Key);
                    }
                }
                else
                {
                    if(mCacheViewDict.ContainsKey(item.Key) == true)
                    {
                        destroyView(mCacheViewDict[item.Key]);
                    }

                    mCacheViewDict[item.Key] = item.Value;
                    removeList.Insert(0, item.Key);
                }
            }
        }

        for(int i=0;i< removeList.Count;i++)
        {
            mCloseViewDcit.Remove(removeList[i]);
        }

        for(int i=0;i<mViewStack.Count;i++)
        {
            if(mCacheViewDict.ContainsKey(mViewStack[i]) == true)
            {
                destroyView(mCacheViewDict[mViewStack[i]]);
                mCacheViewDict.Remove(mViewStack[i]);
            }
        }
    }

    private void destroyView(ViewComp comp)
    {
        if (ViewSetting.ViewDict.ContainsKey(comp.ViewName) == true)
        {
            ViewSetting curViewSet = ViewSetting.ViewDict[comp.ViewName];
            AssetManager.Release(curViewSet.Key);
        }

        comp.Destory();
        GameObject.Destroy(comp.MainGo);
        GameObject.Destroy(comp.HintRootGo);
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
        //for(int i=0;i<mViewStack.Count;i++)
        //{
        //    Debug.LogError(" ViewStack " + mViewStack[i]);
        //}
        if (mViewStack.Count == 0)
        {
            return;
        }

        judgeLayerState(ViewLayer.Logic);
        judgeLayerState(ViewLayer.Guide);
        judgeLayerState(ViewLayer.Top);
    }

    private void judgeLayerState(ViewLayer layer)
    {
        List<string> viewStack = new List<string>();
        for (int i = mViewStack.Count - 1; i >= 0; i--)
        {
            ViewSetting curViewSet = ViewSetting.ViewDict[mViewStack[i]];
            if(curViewSet.Layer == layer)
            {
                viewStack.Insert(0, mViewStack[i]);
            }
        }
        
        if (viewStack.Count == 0)
        {
            return;
        }

        int showIndex = viewStack.Count - 1;
        string viewName = viewStack[showIndex];
        ViewSetting viewSet = ViewSetting.ViewDict[viewName];
        ViewComp topViewComp = mViewCompDict[viewName];

        topViewComp.SetAtLastSibling();
        if (topViewComp.CurViewStat == ViewState.Load)
        {
            topViewComp.Enter();
        }
        else
        {
            topViewComp.HintRootGo.SetActive(true);
        }

        if (viewSet.ViewType == ViewType.Window)
        {
            for (int i = showIndex - 1; i >= 0; i--)
            {
                ViewSetting curViewSet = ViewSetting.ViewDict[viewStack[i]];
                mViewCompDict[mViewStack[i]].HintRootGo.SetActive(true);

                if (curViewSet.ViewType == ViewType.Full)
                {
                    return;
                }
            }
        }
        else if (viewSet.ViewType == ViewType.Full)
        {
            for (int i = showIndex - 1; i >= 0; i--)
            {
                ViewSetting curViewSet = ViewSetting.ViewDict[viewStack[i]];

                if (mViewCompDict[viewStack[i]].CurViewStat == ViewState.EnterFinished)
                {
                    mViewCompDict[viewStack[i]].HintRootGo.SetActive(false);
                }

                if (curViewSet.ViewType == ViewType.Full)
                {
                    return;
                }
            }
        }
    }

    public bool IsSelfActive(string viewName)
    {
        int index = getIndex(viewName);

        if (index == -1)
        {
            return false;
        }

        if(index == mViewStack.Count - 1)
        {
            return true;
        }

        for(int i = mViewStack.Count - 1; i > index; i--)
        {
            ViewSetting curViewSet = ViewSetting.ViewDict[mViewStack[i]];
            if(curViewSet.ViewType == ViewType.Full)
            {
                return false;
            }
        }

        return true;
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
            if(index == mViewStack.Count - 1)
            {
                return;
            }

            if(index >= 0)
            {
                mViewStack.RemoveAt(index);
                mViewStack.Insert(mViewStack.Count, viewName);
            }

            makeViewEnable();
            return;
        }

        if (mCacheViewDict.ContainsKey(viewName) == true)
        {
            mViewStack.Insert(mViewStack.Count, viewName);
            mViewCompDict[viewName] = mCacheViewDict[viewName];
            mCacheViewDict.Remove(viewName);
            mViewCompDict[viewName].CurViewStat = ViewState.Load;
            makeViewEnable();
            return;
        }

        GameObject hintRootGo = GameObject.Instantiate(mHintRootItem);
        hintRootGo.name = viewName;
        hintRootGo.transform.SetParent(mUIRootDict[view.Layer]);
        hintRootGo.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        hintRootGo.transform.localPosition = mHintRootItem.transform.localPosition;
        hintRootGo.GetComponent<RectTransform>().sizeDelta = new Vector2(0.0f, 0.0f);
        hintRootGo.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 0.0f, 0.0f);

        ViewComp viewComp = view.AddCompCb(hintRootGo);
        viewComp.HintRootGo = hintRootGo;
        viewComp.ViewName = viewName;
        mViewStack.Insert(mViewStack.Count, viewName);
        mViewCompDict[viewName] = viewComp;

        AssetManager.LoadGameObject<GameObject>(view.Key, (mainGo) =>
        {
            if (mViewCompDict.ContainsKey(viewName) == false)
            {
                AssetManager.Release(view.Key);
                return;
            }
            
            GameObject rootGo = GameObject.Instantiate((GameObject)mainGo);
            ViewComp rootViewComp = mViewCompDict[viewName];
            Transform parentTrans = ((GameObject)mainGo).transform;
            RectTransform parentRect = parentTrans.GetComponent<RectTransform>();
            rootViewComp.MainGo = rootGo;
            rootViewComp.MainGo.transform.SetParent(rootViewComp.HintRootGo.transform);
            rootViewComp.MainGo.transform.localScale = parentTrans.localScale;
            rootViewComp.MainGo.transform.localPosition = parentTrans.localPosition;
            rootViewComp.MainGo.GetComponent<RectTransform>().sizeDelta = parentRect.sizeDelta;
            rootViewComp.MainGo.GetComponent<RectTransform>().anchoredPosition = parentRect.anchoredPosition;

            rootViewComp.Loaded();
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

        if (mViewCompDict.ContainsKey(viewName) == false)
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

        mCloseViewDcit[viewName] = comp;

        makeViewEnable();
    }

    public void Destroy(string viewName)
    {
        if (ViewSetting.ViewDict.ContainsKey(viewName) == false)
        {
            Debug.LogError(" 界面配置不存在 ");
            return;
        }

        if (mViewCompDict.ContainsKey(viewName) == false)
        {
            return;
        }

        mViewCompDict[viewName].IsDestroyNow = true;
        Close(viewName);
    }
}
