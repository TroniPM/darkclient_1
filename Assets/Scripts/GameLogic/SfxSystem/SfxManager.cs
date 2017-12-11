/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SfxManager
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Game;
using Mogo.GameData;
using Mogo.Util;

public class SfxManager
{
    private EntityParent theOwner;

    public Dictionary<int, List<uint>> sfxTimerIDDic { get; protected set; }

    public SfxManager(EntityParent owner)
    {
        theOwner = owner;
        sfxTimerIDDic = new Dictionary<int, List<uint>>();
    }

    public void PlaySfx(int actionID)
    {
        if (!SkillAction.dataMap.ContainsKey(actionID))
        {
            LoggerHelper.Error("not exist spell data:" + actionID);
            return;
        }

        // 从技能表中获取 sfx 配置
        // 逐个， 按序， 按时触发特效

        Dictionary<int, float> sfx = SkillAction.dataMap[actionID].sfx;
        SfxHandler cueHandler = theOwner.sfxHandler;

        if (sfx != null && sfx.Count > 0)
        {
            if (!sfxTimerIDDic.ContainsKey(actionID))
                sfxTimerIDDic.Add(actionID, new List<uint>());
            foreach (var pair in sfx)
            {
                if (pair.Key < 1000)
                {
                    sfxTimerIDDic[actionID].Add(FrameTimerHeap.AddTimer((uint)(1000 * pair.Value), 0, PlayUIFx, pair.Key));
                }
                else
                {
                    sfxTimerIDDic[actionID].Add(FrameTimerHeap.AddTimer((uint)(1000 * pair.Value), 0, TriggerCues, cueHandler, pair.Key));
                }
            }
        }
        return;
    }

    public void ClearAllSfx()
    {
        foreach (var item in sfxTimerIDDic)
        {
            RemoveSfx(item.Key);
        }
        sfxTimerIDDic.Clear();
    }

    public void PlayUIFx(int id)
    {
        MogoFXManager.Instance.HandleUIFX(id);
    }

    public void StopUIFx(int id)
    {
        MogoFXManager.Instance.DetachUIFX(id);
    }

    public void RemoveSfx(int actionID)
    {
        var sfxs = sfxTimerIDDic.GetValueOrDefault(actionID, new List<uint>());
        foreach (var item in sfxs)
        {
            FrameTimerHeap.DelTimer(item);
        }
        Dictionary<int, float> sfx = SkillAction.dataMap[actionID].sfx;
        if (sfx == null)
        {
            return;
        }
        SfxHandler cueHandler = theOwner.sfxHandler;
        foreach (var item in sfx)
        {
            if (item.Key < 1000)
            {
                StopUIFx(item.Key);
            }
            else
            {
                if (cueHandler)
                    cueHandler.RemoveFXs(item.Key);
            }
        }
        sfxs.Clear();
    }

    private void TriggerCues(SfxHandler cueHandler, int cuesID)
    {
        if (cueHandler)
            cueHandler.HandleFx(cuesID);
    }
}