using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

[Game]
public class EntityBulletMoveComp : IComponent
{
    public Vector3 CurPos;
    public int DestEntityId;
    public float Speed = 0.5f;
    public bool IsArrived;
}
