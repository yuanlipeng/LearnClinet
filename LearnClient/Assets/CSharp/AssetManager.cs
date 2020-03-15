using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public static class AssetManager
{
    private static Dictionary<string, AsyncOperationHandle> mAssetCacheDict = new Dictionary<string, AsyncOperationHandle>();
    private static Dictionary<string, int> mAssetRefDict = new Dictionary<string, int>();

    private static Dictionary<AsyncOperationHandle, IList<Action<UnityEngine.Object>>> mLoadedCbDict = new Dictionary<AsyncOperationHandle, IList<Action<UnityEngine.Object>>>();

    private static void onLoadCompleted(AsyncOperationHandle handler)
    {
        if(mLoadedCbDict.ContainsKey(handler) == false)
        {
            Debug.LogError("资源加载完成，但回调已经清空");
            Addressables.Release(handler);
            return;
        }

        int count = mLoadedCbDict[handler].Count;
        for(int i=count-1;i>=0;i--)
        {
            mLoadedCbDict[handler][i]((UnityEngine.Object)handler.Result);
        }

        mLoadedCbDict.Clear();
    }

    private static void LoadGameObjectAsync<T>(string key, Action<UnityEngine.Object> cb)
    {
        AsyncOperationHandle handler = new AsyncOperationHandle();
        
        if(mLoadedCbDict.ContainsKey(handler)==false)
        {
            mLoadedCbDict[handler] = new List<Action<UnityEngine.Object>>();
        }

        mAssetCacheDict[key] = handler;

        mLoadedCbDict[handler].Insert(0, cb);

        handler = Addressables.LoadAssetAsync<T>(key);
        handler.Completed += onLoadCompleted;
    }

    public static void LoadGameObject<T>(string key, Action<UnityEngine.Object> cb) where T : UnityEngine.Object
    {
        mAssetRefDict[key]++;
        
        if (mAssetCacheDict.ContainsKey(key))
        {
            if (mAssetCacheDict[key].IsDone == true)
            {
                cb((UnityEngine.Object)mAssetCacheDict[key].Result);
            }
            else
            {
                AsyncOperationHandle handler = mAssetCacheDict[key];
                if (mLoadedCbDict.ContainsKey(handler) == false)
                {
                    mLoadedCbDict[handler] = new List<Action<UnityEngine.Object>>();
                }
                mLoadedCbDict[handler].Insert(0, cb);
            }
            return;
        }

        LoadGameObjectAsync<T>(key, cb);
    }

    public static void Release(string key)
    {
        mAssetRefDict[key]--;
        if (mAssetRefDict[key] < 0)
        {
            mAssetRefDict[key] = 0;
        }

        if (mAssetRefDict[key] == 0)
        {
            if (mAssetCacheDict.ContainsKey(key))
            {
                Addressables.Release(mAssetCacheDict[key]);
            }
        }
    }
}
