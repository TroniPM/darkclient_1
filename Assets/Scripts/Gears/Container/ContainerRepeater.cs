using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;
using System;

public class ContainerRepeater : GearParent
{
    public int checkSkillActionID;

    void Start()
    {
        gearType = "ContainerRepeater";
        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        base.SetGearEventStateTwo(stateTwoID);
        if (stateTwoID == ID)
            AllRepeat();
    }

    protected void AllRepeat()
    {
        if (MogoWorld.Entities == null)
            return;

        if (MogoWorld.Entities.Count == 0)
            return;

        bool hasFound = false;
        int hitActionID = -1;
        EntityParent theOwner = new EntityParent();
        Transform theOwnerTransform = null;
        

        foreach (var entityData in MogoWorld.Entities)
        {
            List<int> skillIDs = new List<int>();

            if (entityData.Value is EntityMonster)
                skillIDs = (entityData.Value as EntityMonster).MonsterData.skillIds;

            if (entityData.Value is EntityMercenary)
                skillIDs = (entityData.Value as EntityMercenary).MonsterData.skillIds;

            if (entityData.Value is EntityDummy)
                skillIDs = (entityData.Value as EntityDummy).MonsterData.skillIds;

            if (skillIDs.Contains(checkSkillActionID))
            {
                theOwner = entityData.Value;
                theOwnerTransform = entityData.Value.Transform;
                hitActionID = checkSkillActionID;
                hasFound = true;
                break;
            }
        }

        if (!hasFound)
            return;

        List<Transform> hitContainers = GetHitContainers(theOwnerTransform, hitActionID);

        if (hitContainers.Count > 0)
        {
            AttackContainers(theOwner, hitActionID, hitContainers);
        }
    }


    protected List<Transform> GetHitContainers(Transform theOwnerTransform, int hitActionID)
    {
        var spellData = SkillAction.dataMap[hitActionID];

        // 目标类型 0 敌人， 1 自己  2 队友  3  友方
        int targetType = spellData.targetType;
        // 攻击范围类型。  0  扇形范围 1  圆形范围， 2， 单体。 3  直线范围 4 前方范围
        int targetRangeType = spellData.targetRangeType;
        // 攻击范围参数。 针对不同类型，有不同意义。 浮点数列表
        List<float> targetRangeParam = spellData.targetRangeParam;
        // 最大攻击人数
        //int maxTargetCount = spellData.maxTargetCount;
        // 触发伤害特效帧数
        //int damageTriggerFrame = spellData.damageTriggerFrame;

        List<Transform> result = new List<Transform>();

        if (targetType == (int)TargetType.Myself)
            return result;

        if (theOwnerTransform == null)
            return result;

        TargetRangeType rangeType = (TargetRangeType)targetRangeType;
        switch (rangeType)
        {
            case TargetRangeType.CircleRange:
                if (targetRangeParam.Count >= 1)
                {
                    float radius = targetRangeParam[0] * 0.01f;
                    result = MogoUtils.GetTransformsInSector(theOwnerTransform, Container.containerRange, radius);
                }
                break;

            case TargetRangeType.SectorRange:
                if (targetRangeParam.Count >= 2)
                {
                    float radius = targetRangeParam[0] * 0.01f;
                    float angle = targetRangeParam[1];
                    result = MogoUtils.GetTransformsInSector(theOwnerTransform, Container.containerRange, radius, angle);
                }
                break;

            case TargetRangeType.SingeTarget:
                if (targetRangeParam.Count >= 1)
                {
                    float radius = targetRangeParam[0] * 0.01f;
                    float angle = 150;
                    result = MogoUtils.GetTransformsInSector(theOwnerTransform, Container.containerRange, radius, angle);

                    //entities = Utils.GetEntities(theOwner.Transform, radius, angle);
                    MogoUtils.SortByDistance(theOwnerTransform, result);
                    if (result.Count > 1)
                    {
                        for (int i = 1; i < result.Count; i++)
                            result.RemoveAt(i);
                    }
                }
                break;

            case TargetRangeType.WorldRange:
                if (targetRangeParam.Count >= 4)
                {
                    float x1 = targetRangeParam[0] * 0.01f;
                    float y1 = targetRangeParam[1] * 0.01f;
                    float x2 = targetRangeParam[2] * 0.01f;
                    float y2 = targetRangeParam[3] * 0.01f;
                    float minX = Math.Min(x1, x2);
                    float maxX = Math.Max(x1, x2);
                    float minY = Math.Min(y1, y2);
                    float maxY = Math.Max(y1, y2);
                    float radiusX = minX + (maxX - minX) * 0.5f;
                    float radiusY = minY + (maxY - minY) * 0.5f;
                    float radius = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2)) * 0.5f;
                    result = MogoUtils.GetTransformsInRange(new Vector3(radiusX, 0, radiusY), Container.containerRange, radius);
                }
                break;

            case TargetRangeType.LineRange:
            default:
                if (targetRangeParam.Count >= 2)
                {
                    float length = targetRangeParam[0] * 0.01f;
                    float width = targetRangeParam[1] * 0.01f;
                    result = MogoUtils.GetTransformsFrontLineNew(theOwnerTransform, Container.containerRange, length, theOwnerTransform.forward, width);
                }
                break;
        }
        return result;
    }


    protected void AttackContainers(EntityParent theOwner, int hitActionID, List<Transform> hitContainers)
    {
        foreach (Transform target in hitContainers)
        {
            if (!Container.containers.ContainsKey(target))
                continue;

            if (!Container.containers[target].triggleEnable)
                continue;

            if ((theOwner is EntityMyself && Container.containers[target].isMyselfAttackable)
                || (theOwner is EntityMercenary && theOwner.ID == MogoWorld.theLittleGuyID && Container.containers[target].isLittelGuyAttackable)
                || (theOwner is EntityMercenary && theOwner.ID != MogoWorld.theLittleGuyID && Container.containers[target].isMercenarayAttackable)
                || (theOwner is EntityDummy && Container.containers[target].isDummyAttackable))
                Container.containers[target].OnDeath(hitActionID);
        }
    }
}
