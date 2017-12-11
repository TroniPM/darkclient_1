/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：BuffManager
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013-7-10
// 模块描述：buff管理
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.GameData;
using Mogo.Util;

public class BuffManager
{
    private uint timer = 0;
    private Dictionary<int, ClientBuff> clientBuffs;

    private EntityParent m_theOwner;
    public BuffManager(EntityParent theOwner)
    {
        m_theOwner = theOwner;
        clientBuffs = new Dictionary<int, ClientBuff>();
        EventDispatcher.AddEventListener(Events.OtherEvent.SecondPast, CountDownSkillBuffTime);
        timer = TimerHeap.AddTimer(500, 100, UpdateBuff);
    }

    private bool m_hasGotLoginBuff = false;
    public bool HasGotLoginBuff
    {
        get
        {
            return m_hasGotLoginBuff;
        }
        set
        {
            m_hasGotLoginBuff = value;
        }
    }

    struct BuffData
    {
        public int buffId;
        public UInt32 lastTime;
    }

    List<BuffData> m_skillBuffDataList = new List<BuffData>();

    public void Clean()
    {
        TimerHeap.DelTimer(timer);
        EventDispatcher.RemoveEventListener(Events.OtherEvent.SecondPast, CountDownSkillBuffTime);
        clientBuffs.Clear();
        m_skillBuffDataList.Clear();
    }

    private void CountDownSkillBuffTime()
    {
        for (int i = 0; i < m_skillBuffDataList.Count; i++)
        {
            if (m_skillBuffDataList[i].lastTime > 1000)
            {
                BuffData buffData = m_skillBuffDataList[i];
                buffData.lastTime = buffData.lastTime - 1000;
                m_skillBuffDataList[i] = buffData;
            }
            else
            {
                BuffData buffData = m_skillBuffDataList[i];
                buffData.lastTime = 0;
                m_skillBuffDataList[i] = buffData;
            }
        }     
    }
    
    private void AddSkillBuff(int buffId, UInt32 time)
    {
        BuffData buffData;
        buffData.buffId = buffId;
        buffData.lastTime = time;

        RemoveSkillBuff(buffId);
        m_skillBuffDataList.Add(buffData);     
    }

    private void RemoveSkillBuff(int buffId)
    {
        for (int i = 0; i < m_skillBuffDataList.Count; i++)
        {
            if (m_skillBuffDataList[i].buffId == buffId)
            {
                m_skillBuffDataList.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// 获取需要显示的VIPBuff的ID
    /// </summary>
    /// <returns></returns>
    private int GetShowVIPBuffId()
    {
        int buffId = 0;
        for (int i = 0; i < m_skillBuffDataList.Count; i++)
        {
            SkillBuffData buff = SkillBuffData.dataMap[m_skillBuffDataList[i].buffId];
            if (buff.notifyEvent == 1)
            {
                if (buffId == 0)
                    buffId = buff.id;
                else if (buff.vipLevel > SkillBuffData.dataMap[buffId].vipLevel)
                    buffId = buff.id;
            }
        }

        return buffId;
    }

    /// <summary>
    /// 获取buff剩余时间
    /// </summary>
    /// <param name="buffId"></param>
    /// <returns></returns>
    private UInt32 GetBuffLastTime(int buffId)
    {
        UInt32 lastTime = 0;
        for (int i = 0; i < m_skillBuffDataList.Count; i++)
        {
            if (m_skillBuffDataList[i].buffId == buffId)
            {
                lastTime = m_skillBuffDataList[i].lastTime;
                break;
            }
        }

        return lastTime;
    }

    public void HandleBuff(ushort buffId, byte isAdd, uint time)
    {
        Mogo.Util.LoggerHelper.Debug("buffID:" + buffId);
        if (!SkillBuffData.dataMap.ContainsKey(buffId))
        {
            Mogo.Util.LoggerHelper.Debug("can find the buffId:" + buffId);
            LoggerHelper.Error("can find the buffId:" + buffId);
            return;
        }
        SkillBuffData buff = SkillBuffData.dataMap[buffId];

        if (isAdd == 1) // 添加Buff
        {
            Mogo.Util.LoggerHelper.Debug("add buff fx:" + buff.sfx + ",time:" + time);

            // VIPBuff
            if (buff.notifyEvent == 1)
            {
                // 玩家
                if (m_theOwner == MogoWorld.thePlayer)
                {
                    AddSkillBuff(buff.id, time);
                    int showBuffId = GetShowVIPBuffId();
                    if (showBuffId > 0)
                    {
                        NormalMainUIViewManager.Instance.ShowVIPBuffBtn(true, showBuffId, GetBuffLastTime(buffId));
                    }
                }
            }
            else
            {
                if (m_theOwner.sfxHandler)
                {
                    m_theOwner.sfxHandler.HandleFx(buff.sfx);
                    TimerHeap.AddTimer(time, 0,
                        (owner)=>
                        {
                            if (owner != null && owner.sfxHandler != null)
                            {
                                owner.sfxHandler.RemoveFXs(buff.sfx);
                            }
                        },
                        m_theOwner);
                }

                if (m_theOwner == MogoWorld.thePlayer && buff.tips != 0)
                {
                    MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(buff.tips));
                }
            }
        }
        else // 删除Buff
        {
            // VIPBuff
            if (buff.notifyEvent == 1)
            {
                // 玩家
                if (m_theOwner == MogoWorld.thePlayer)
                {
                    LoggerHelper.Debug("Remove Buff : id =" + buff.id);
                    RemoveSkillBuff(buff.id);

                    int showBuffId = GetShowVIPBuffId();
                    if (showBuffId > 0)
                    {
                        NormalMainUIViewManager.Instance.ShowVIPBuffBtn(true, showBuffId, GetBuffLastTime(showBuffId));
                    }
                    else
                    {
                        NormalMainUIViewManager.Instance.ShowVIPBuffBtn(false);
                    }
                }
            }
            else
            {
                Mogo.Util.LoggerHelper.Debug("delete buff fx:" + buff.sfx);
                if (m_theOwner.sfxHandler)
                {
                    m_theOwner.sfxHandler.RemoveFXs(buff.sfx);
                }
            }
        }
        //UpdateState();
    }

    private void UpdateState()
    {
        ulong state = m_theOwner.stateFlag;
        foreach (var item in clientBuffs)
        {
            SkillBuffData buffData = null;
            if (!SkillBuffData.dataMap.TryGetValue(item.Value.id, out buffData))
            {
                continue;
            }
            if (buffData.appendState == null)
            {
                continue;
            }
            foreach (var st in buffData.appendState)
            {
                state = Mogo.Util.Utils.BitSet(state, st);
            }
        }
        m_theOwner.stateFlag = state;
    }

    public void ClientAddBuff(int buffId)
    {
        if (!SkillBuffData.dataMap.ContainsKey(buffId))
        {
            return;
        }
        SkillBuffData sbd = SkillBuffData.dataMap[buffId];
        if (clientBuffs.ContainsKey(buffId))
        {//累加时间
            clientBuffs[buffId].totalTime += sbd.totalTime;
            return;
        }
        if (sbd.excludeBuff != null)
        {
            for (int i = 0; i < sbd.excludeBuff.Count; i++)
            {
                if (clientBuffs.ContainsKey(sbd.excludeBuff[i]))
                {
                    return;
                }
            }
        }
        if (sbd.replaceBuff != null)
        {
            for (int j = 0; j < sbd.replaceBuff.Count; j++)
            {
                ClientDelBuff(sbd.replaceBuff[j]);
            }
        }
        if (sbd.activeSkill != null && sbd.activeSkill.Count > 0)
        {
            foreach (var item in sbd.activeSkill)
            {
                TimerHeap.AddTimer<int>((uint)item.Key, 0, (id) => { m_theOwner.skillManager.BuffUseSkill(id); }, item.Value);
            }
        }
        ClientBuff b = new ClientBuff(sbd);
        clientBuffs.Add(buffId, b);
        UpdateState();
    }

    public void ClientDelBuff(int id)
    {
        if (clientBuffs.ContainsKey(id))
        {
            clientBuffs.Remove(id);
        }
        SkillBuffData sdb = SkillBuffData.dataMap[id];
        if (sdb.appendState != null)
        {
            ulong st = m_theOwner.stateFlag;
            foreach (var p in sdb.appendState)
            {
                st = Mogo.Util.Utils.BitReset(st, p);
            }
            m_theOwner.stateFlag = st;
        }
    }

    List<int> m_l = new List<int>();
    private void UpdateBuff()
    {
        m_l.Clear();
        int currTime = (int)(Time.realtimeSinceStartup * 1000);

        foreach (var item in clientBuffs)
        {
            if ((currTime - item.Value.startTime) >= item.Value.totalTime)
            {
                m_l.Add(item.Value.id);
            }
        }
        foreach (int id in m_l)
        {
            ClientDelBuff(id);
        }
    }

    public class ClientBuff
    {
        public int id;
        public int totalTime;
        public int startTime;

        public ClientBuff(SkillBuffData cfg)
        {
            id = cfg.id;
            totalTime = cfg.totalTime;
            startTime = (int)(Time.realtimeSinceStartup * 1000);
        }
    }
}
