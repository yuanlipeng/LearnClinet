using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystems : Feature
{
    public BattleSystems()
    {
        Add(new EntityMoveSystem());
        Add(new EntityBoxColliderSystem());
        Add(new EntityTransformRenderSystem());
        Add(new EntitySkillSystem());
        Add(new EntityBulletMoveSystem());
        Add(new EntityBulletSkillSystem());
    }
}
