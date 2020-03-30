using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySetting
{
    public int EntityId;
    public Vector3 BornPos;
    public EntityType EntityType;
    public float MoveSpeed;
    public float BoxColliderR;
    public string ResPath;

    public EntitySetting(int entityId, Vector3 bornPos, EntityType entityType, float moveSpeed, float boxColliderR, string resPath)
    {
        EntityId = entityId;
        BornPos = bornPos;
        EntityType = entityType;
        MoveSpeed = moveSpeed;
        BoxColliderR = boxColliderR;
        ResPath = resPath;
    }

    public static Dictionary<int, EntitySetting> Setting = new Dictionary<int, EntitySetting>();
    public static void Init()
    {
        string monsterKey = "Assets/Hero Fighter/model/prefab/Fighter Devil.prefab";

        EntitySetting setting1 = new EntitySetting(1, new Vector3(0, 0, 0), EntityType.MainPlayer, 0.1f, 0.5f, "Assets/Hero Fighter/model/prefab/Fighter.prefab");
        Setting[1] = setting1;

        EntitySetting setting2 = new EntitySetting(2, new Vector3(1.0f, 0, 1.0f), EntityType.Monster, 0.1f, 0.5f, monsterKey);
        Setting[2] = setting2;

        EntitySetting setting3 = new EntitySetting(3, new Vector3(-1.0f, 0, -1.0f), EntityType.Monster, 0.1f, 0.5f, monsterKey);
        Setting[3] = setting3;

        EntitySetting setting4 = new EntitySetting(4, new Vector3(1.0f, 0, -1.0f), EntityType.Monster, 0.1f, 0.5f, monsterKey);
        Setting[4] = setting4;

        EntitySetting setting13 = new EntitySetting(13, new Vector3(1.0f, 0, 1.0f), EntityType.Bullet, 0.5f, 0.3f, "Assets/Hero Fighter/model/prefab/Cube.prefab");
        Setting[13] = setting13;
    }
}
