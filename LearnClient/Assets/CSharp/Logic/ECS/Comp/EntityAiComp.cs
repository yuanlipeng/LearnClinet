using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

[Game]
public class EntityAiComp : IComponent
{
    public float ScopeX;
    public float ScopeY;
    public bool IsAIEnded;
}
