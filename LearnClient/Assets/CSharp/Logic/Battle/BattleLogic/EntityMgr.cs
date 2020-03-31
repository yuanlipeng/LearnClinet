using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;


public class EntityMgr
{
    public static EntityMgr Instance = new EntityMgr();

    private Contexts mContextsInstance;
    private GameObject mHintRoot;

    private Dictionary<int, GameEntity> mEntityCacheDict = new Dictionary<int, GameEntity>();

    public GameObject MainCamera;

    private int mBulletIndex = 13;

    public void Init()
    {
        mContextsInstance = Contexts.sharedInstance;
        mHintRoot = GameObject.Find("3DRoot").gameObject;
        MainCamera = GameObject.Find("MainCamera").gameObject;
    }

    public void Reset()
    {
        List<int> removeList = new List<int>();
        foreach(var item in mEntityCacheDict)
        {
            if (item.Value.hasEntityInfoComp == true)
            {
                EntitySetting entitySetting = EntitySetting.Setting[item.Value.entityInfoComp.ConfigId];
                AssetManager.Release(entitySetting.ResPath);
                if (item.Value.hasEntityRenderComp == true)
                {
                    GameObject.Destroy(item.Value.entityRenderComp.MainGo);
                }
            }
            removeList.Insert(0, item.Key);
        }

        for(int i = 0; i < removeList.Count; i++)
        {
            GameEntity entity = mEntityCacheDict[removeList[i]];
            mEntityCacheDict.Remove(removeList[i]);
            entity.Release(EntityMgr.Instance);
        }

        removeList.Clear();
        mEntityCacheDict.Clear();

        mContextsInstance.game.DestroyAllEntities();
    }

    public Contexts GetContexts()
    {
        return mContextsInstance;
    }

    public void CreateMainPlayer()
    {
        CreateEntity(1);
    }

    public GameEntity CreateMonster(int entityId)
    {
        GameEntity entity = CreateEntity(entityId);
        entity.AddEntityAiComp(2.5f, 2.5f, true);
        return entity;
    }

    public GameEntity CreateEntity(int entityId)
    {
        GameEntity entity = mContextsInstance.game.CreateEntity();
        EntitySetting setting = EntitySetting.Setting[entityId];

        entity.AddEntityInfoComp(setting.EntityId, setting.EntityType, entityId);
        entity.AddMoveComp(setting.BornPos, setting.BornPos, setting.MoveSpeed, true, new Vector3(0,0,1), false, false);
        entity.AddBoxColliderComp(setting.BoxColliderR);

        mEntityCacheDict[entityId] = entity;
        entity.Retain(EntityMgr.Instance);

        AssetManager.LoadGameObject<GameObject>(setting.ResPath, (UnityEngine.Object obj) =>
        {
            GameObject model = GameObject.Instantiate<GameObject>((GameObject)obj);
            model.transform.SetParent(mHintRoot.transform);
            entity.AddEntityRenderComp((GameObject)model);
        });

        return entity;
    }

    public GameEntity CreateBullet()
    {
        GameEntity entity = mContextsInstance.game.CreateEntity();
        EntitySetting setting = EntitySetting.Setting[13];

        entity.AddEntityInfoComp(mBulletIndex, setting.EntityType, 13);
        entity.AddEntityBulletMoveComp(setting.BornPos, 0, setting.MoveSpeed, false);
        entity.AddBoxColliderComp(setting.BoxColliderR);

        mEntityCacheDict[mBulletIndex] = entity;
        entity.Retain(EntityMgr.Instance);

        AssetManager.LoadGameObject<GameObject>(setting.ResPath, (UnityEngine.Object obj) =>
        {
            if (entity.retainCount > 0)
            {
                GameObject model = GameObject.Instantiate<GameObject>((GameObject)obj);
                model.transform.SetParent(mHintRoot.transform);
                entity.AddEntityRenderComp((GameObject)model);
            }
        });

        mBulletIndex++;

        return entity;
    }

    public GameEntity GetGameEntity(int entityId)
    {
        return mEntityCacheDict[entityId];
    }

    public void ReturnEntity(GameEntity entity)
    {
        EntitySetting entitySetting = EntitySetting.Setting[entity.entityInfoComp.ConfigId];
        mEntityCacheDict.Remove(entity.entityInfoComp.Id);
        if (entity.hasEntityRenderComp == true)
        {
            GameObject.Destroy(entity.entityRenderComp.MainGo);
        }
        AssetManager.Release(entitySetting.ResPath);
        entity.Release(EntityMgr.Instance);
        entity.Destroy();
    }
}
