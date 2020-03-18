using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMgrMono : MonoBehaviour
{

    public List<string> ViewStack = new List<string>();
    public Dictionary<string, ViewComp> CacheViewDict = new Dictionary<string, ViewComp>();

    void Update()
    {
        ViewStack = ViewMgr.Instance.ViewStack;
        CacheViewDict = ViewMgr.Instance.CacheViewDict;
    }
}
