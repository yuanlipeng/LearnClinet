﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class MainCameraFollow : MonoBehaviour
{
    public Vector3 diff = new Vector3(0.0f, 3.0f, -5.1f);
    public Vector3 DestPos;
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdatePos()
    {
        GameEntity entity = EntityMgr.Instance.GetGameEntity(1);
        if(entity != null)
        {
            transform.position = diff + entity.moveComp.CurPos;
        }
    }
}
