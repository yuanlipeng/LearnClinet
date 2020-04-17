using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMgr
{
    public enum CommandType
    {
        Open,
        Close,
    }
    public class Command
    {
        public CommandType type;
        public string viewName;
    }
    public static ViewMgr Instance = new ViewMgr();

    private List<string> mViewStack = new List<string>();
    private Dictionary<string, ViewComp> mViewCompDict = new Dictionary<string, ViewComp>();

    private Dictionary<string, ViewComp> mCacheViewDict = new Dictionary<string, ViewComp>();

    private Dictionary<ViewLayer, Transform> mUIRootDict = new Dictionary<ViewLayer, Transform>();
    private GameObject mHintRootItem;

    private Command mCurCommand;
    private ViewComp mCurView;
    private Queue<Command> mWaitCommandQueue = new Queue<Command>();

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
        ViewSetting.Init();

        Transform uiRootTrans = GameObject.Find("UIRoot").transform;

        mUIRootDict[ViewLayer.Logic] = uiRootTrans.Find("Logic");
        mUIRootDict[ViewLayer.Guide] = uiRootTrans.Find("Guide");
        mUIRootDict[ViewLayer.Top] = uiRootTrans.Find("Top");

        mHintRootItem = uiRootTrans.Find("HintRootItem").gameObject;

    }

    public void Update()
    {
        
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

    public void Open(string viewName)
    {
        if(mCurView == null)
        {
            OpenImmediately(viewName);
            return;
        }

        Command command = new Command();
        command.type = CommandType.Open;
        command.viewName = viewName;

        mWaitCommandQueue.Enqueue(command);
    }

    public void Close(string viewName)
    {
        if (mCurView == null)
        {
            CloseImmediately(viewName);
            return;
        }

        Command command = new Command();
        command.type = CommandType.Open;
        command.viewName = viewName;

        mWaitCommandQueue.Enqueue(command);
    }

    private void OpenImmediately(string viewName)
    {
        ViewComp viewComp = null;
        if (mCacheViewDict.ContainsKey(viewName))
        {
            viewComp = mCacheViewDict[viewName];
            mCacheViewDict.Remove(viewName);

            viewComp.HintRootGo.transform.SetAsLastSibling();
            viewComp.Enter();
        }
        else
        {
            ViewSetting viewSetting = ViewSetting.ViewDict[viewName];

            viewComp = new ViewComp();
            GameObject hintGo = new GameObject();
            hintGo.name = viewName;
            hintGo.transform.SetParent(mUIRootDict[viewSetting.Layer]);
            hintGo.transform.localScale = new Vector3(1, 1, 1);
            hintGo.transform.localPosition = new Vector3(0, 0, 0);

            viewComp.HintRootGo = hintGo;
            viewComp.ViewName = viewName;
            viewComp.HintRootGo.transform.SetAsLastSibling();

            viewComp.CurViewState = ViewState.Loading;
            AssetManager.LoadGameObject<GameObject>(viewSetting.Key, (UnityEngine.Object obj)=>{
                GameObject insGo = GameObject.Instantiate((GameObject)obj);

                insGo.transform.SetParent(viewComp.HintRootGo.transform);
                insGo.transform.localPosition = new Vector3(0, 0, 0);
                insGo.transform.localScale = new Vector3(1, 1, 1);

                viewComp.MainGo = insGo;
                viewComp.Enter();
            }); 
        }
        this.mCurView = viewComp;
    }

    private void CloseImmediately(string viewName)
    {
        if (mViewCompDict.ContainsKey(viewName) == false)
        {
            return;
        }

        ViewComp viewComp = mViewCompDict[viewName];
        viewComp.Exit();
    }
}
