using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

[Game]
public class MoveComp : IComponent
{
    public Vector3 CurPos;
    public Vector3 DestPos;
    public float Speed;
    public bool IsArrived;
    public Vector3 Forward;
    public bool IsAniMove;
    public bool IsBlock;
}
