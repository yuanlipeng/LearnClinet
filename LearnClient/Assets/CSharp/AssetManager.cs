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

    private static List<string> mWaitReleaseList = new List<string>();

    private static Dictionary<string, IList<Action<UnityEngine.Object>>> mLoadedCbDict = new Dictionary<string, IList<Action<UnityEngine.Object>>>();

    private static int maxReleaseCount = 10;

    public static void Update()
    {
        int count = 0;
        List<string> releaseList = new List<string>();
        for(int i = 0; i < mWaitReleaseList.Count; i++)
        {
            if (count < maxReleaseCount && mAssetCacheDict.ContainsKey(mWaitReleaseList[i])==true)
            {
                Addressables.Release(mAssetCacheDict[mWaitReleaseList[i]]);
                mAssetCacheDict.Remove(mWaitReleaseList[i]);
                count++;
                releaseList.Insert(0, mWaitReleaseList[i]);
            }
        }

        for(int i = 0; i < releaseList.Count; i++)
        {
            mWaitReleaseList.Remove(releaseList[i]);
        }
    }

    private static void LoadGameObjectAsync<T>(string key, Action<UnityEngine.Object> cb)
    {
        AsyncOperationHandle handler = new AsyncOperationHandle();
        
        if(mLoadedCbDict.ContainsKey(key)==false)
        {
            mLoadedCbDict[key] = new List<Action<UnityEngine.Object>>();
        }

        mLoadedCbDict[key].Insert(0, cb);

        handler = Addressables.LoadAssetAsync<T>(key);
        handler.Completed += (AsyncOperationHandle loadedHandler) =>
        {
            if (loadedHandler.Status == AsyncOperationStatus.Succeeded)
            {
                mAssetCacheDict[key] = loadedHandler;
                if (mLoadedCbDict.ContainsKey(key) == false)
                {
                    Debug.LogError("资源加载完成，但回调已经清空");
                    Addressables.Release(handler);
                    return;
                }

                int count = mLoadedCbDict[key].Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    mLoadedCbDict[key][i](loadedHandler.Result as UnityEngine.Object);
                }

                mLoadedCbDict.Clear();
            }
        };
    }

    public static void LoadGameObject<T>(string key, Action<UnityEngine.Object> cb) where T : UnityEngine.Object
    {
        if (mAssetRefDict.ContainsKey(key) == false)
        {
            mAssetRefDict[key] = 0;
        }
        mAssetRefDict[key]++;
        
        if (mAssetCacheDict.ContainsKey(key))
        {
            if (mAssetCacheDict[key].IsDone == true)
            {
                cb((UnityEngine.Object)mAssetCacheDict[key].Result);
            }
            return;
            
        }
        
        if (mLoadedCbDict.ContainsKey(key) == true)
        {
            mLoadedCbDict[key].Insert(0, cb);
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
            mWaitReleaseList.Insert(0, key);
        }
    }
}