using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class EntityInfoComp : IComponent
{
    [EntityIndex]
    public int Id;

    [EntityIndex]
    public EntityType EntityType;

    public int ConfigId;
}
