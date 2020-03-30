using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSetting
{
    public int SkillId; //技能Id
    public string AniName;//技能对应的 Animator 动画名字
    public bool IsBlockWalk; //该技能是否会打断 行走
    public bool IsNeedAniMove; //该技能是否是 位移技能
    public bool IsBulletShoot; //是否是射击技能
    public AttackInfo AttackInfoCo; //如果技能是普通攻击时，需要存储的技能攻击数据。
    public float SkillAttackTime;//技能释法时间。
    public bool IsAttackForward;

    public class AttackInfo
    {
        public float x;
        public float y;

        public float r;
    }

    SkillSetting(int skillId, string aniName, bool isBlockWalk, bool isNeedAniMove, bool isBulletShoot, float x, float y, float r, float skillTime, bool isAttackForward)
    {
        SkillId = skillId;
        AniName = aniName;
        IsBlockWalk = isBlockWalk;
        IsNeedAniMove = isNeedAniMove;
        IsBulletShoot = isBulletShoot;

        AttackInfoCo = new AttackInfo();
        AttackInfoCo.x = x;
        AttackInfoCo.y = y;
        AttackInfoCo.r = r;

        SkillAttackTime = skillTime;
        IsAttackForward = isAttackForward;
    }

    public static Dictionary<int, SkillSetting> SkillSettingDict = new Dictionary<int, SkillSetting>();

    public static void Init()
    {
        SkillSetting skill101 = new SkillSetting(101, "attack101", true, false, false, 1.0f, 1.0f, 1.0f, 0.4f, true);
        SkillSettingDict[101] = skill101;

        SkillSetting skill102 = new SkillSetting(102, "attack102", true, true, false, 0.0f, 0.0f, 1.0f, 0.7f, false);
        SkillSettingDict[102] = skill102;

        SkillSetting skill103 = new SkillSetting(103, "attack103", false, false, true, 0.0f, 0.0f, 1.0f, 0.3f, true);
        SkillSettingDict[103] = skill103;
    }
}
