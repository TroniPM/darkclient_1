#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using Mogo.GameData;
using System.Linq;
/// <summary>
/// 领悟技能
/// </summary>
public struct SpriteLearnSkillData
{
    public string name; // 技能名字
    public string icon; // 图标
    public bool hasLearned; // 是否领悟
    public bool isEquipped; // 是否装备
}

/// <summary>
/// 技能
/// </summary>
public struct SpriteSkillData
{
    public int ID;
    public string name; // 技能名字
    public int unlockLevel; // 解锁等级
    public bool hasFilled; // 是否填满
}

/// <summary>
/// 技能属性球
/// </summary>
public struct SpriteSkillPieceData
{
    public string icon;
    public string name;
    public bool isAwake;
}

/// <summary>
/// 技能觉醒数据
/// </summary>
public class SpriteSkillDetailData
{
    public SpriteSkillData spriteSkillData;
    public List<SpriteSkillPieceData> listPieceData = new List<SpriteSkillPieceData>();
    public string skillName;
}

public class SpriteUILogicManager : UILogicManager
{
    private static SpriteUILogicManager m_instance;
    public static SpriteUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SpriteUILogicManager();
            }

            return SpriteUILogicManager.m_instance;
        }
    }

    #region 事件

    public void Initialize()
    {
        SpriteUIViewManager.Instance.SPRITEUICLOSEUP += OnCloseUp;
        SpriteUIViewManager.Instance.SPRITEUISWITCHTODETAILEND += OnPlaySpriteSkillUISwitchToSpriteDetailUIEnd;
        SpriteUIViewManager.Instance.SPRITEUISKILLUP += OnSpriteSkillUISkillUp;

        SpriteUIViewManager.Instance.SPRITEUILEARNSKILLUP += OnSpriteLearnSkillUILearnSkillUp;
        SpriteUIViewManager.Instance.SPRITEUILEARNSKILLUIBACKUP += OnSpriteLearnSkillUIBackUp;
        SpriteUIViewManager.Instance.SPRITEUILEARNSKILLUIRESETUP += OnLearnSkillUIResetUp;
        SpriteUIViewManager.Instance.SPRITEUILEARNSKILLUIUPGRADEUP += OnLearnSkillUIUpgradeUp;
        SpriteUIViewManager.Instance.SPRITEUILEARNSKILLUIEQUIPUP += OnLearnSkillUIEquipUp;
        SpriteUIViewManager.Instance.SPRITEUILEARNSKILLCHOOSEUP += OnLearnSkillUISkillChoose;

        EventDispatcher.AddEventListener<int>(SpriteUIDict.SpriteUIEvent.OnSpriteSkillUp, OnSpriteSkillUp);
        EventDispatcher.AddEventListener(SpriteUIDict.SpriteUIEvent.OnBackUp, OnSpriteSkillDetaiUIBackUp);
        EventDispatcher.AddEventListener<bool>(SpriteUIDict.SpriteUIEvent.OnAwakePress, OnAwakePress);

        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource
    }

    public override void Release()
    {
        base.Release();
        SpriteUIViewManager.Instance.SPRITEUICLOSEUP -= OnCloseUp;
        SpriteUIViewManager.Instance.SPRITEUISWITCHTODETAILEND -= OnPlaySpriteSkillUISwitchToSpriteDetailUIEnd;
        SpriteUIViewManager.Instance.SPRITEUISKILLUP -= OnSpriteSkillUISkillUp;

        SpriteUIViewManager.Instance.SPRITEUILEARNSKILLUP -= OnSpriteLearnSkillUILearnSkillUp;
        SpriteUIViewManager.Instance.SPRITEUILEARNSKILLUIBACKUP -= OnSpriteLearnSkillUIBackUp;
        SpriteUIViewManager.Instance.SPRITEUILEARNSKILLUIRESETUP -= OnLearnSkillUIResetUp;
        SpriteUIViewManager.Instance.SPRITEUILEARNSKILLUIUPGRADEUP -= OnLearnSkillUIUpgradeUp;
        SpriteUIViewManager.Instance.SPRITEUILEARNSKILLUIEQUIPUP -= OnLearnSkillUIEquipUp;
        SpriteUIViewManager.Instance.SPRITEUILEARNSKILLCHOOSEUP -= OnLearnSkillUISkillChoose;

        EventDispatcher.RemoveEventListener<int>(SpriteUIDict.SpriteUIEvent.OnSpriteSkillUp, OnSpriteSkillUp);
        EventDispatcher.RemoveEventListener(SpriteUIDict.SpriteUIEvent.OnBackUp, OnSpriteSkillDetaiUIBackUp);
        EventDispatcher.RemoveEventListener<bool>(SpriteUIDict.SpriteUIEvent.OnAwakePress, OnAwakePress);
    }

    /// <summary>
    /// 点击关闭按钮
    /// </summary>
    void OnCloseUp()
    {
        LoggerHelper.Debug("OnSpriteUICloseUp");
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    /// <summary>
    /// SkillUI点击技能
    /// </summary>
    /// <param name="index"></param>
    void OnSpriteSkillUp(int index)
    {
        if (!SpriteUIViewManager.Instance.IsSpriteSkillClickEnable)
            return;

        if (SpriteUIViewManager.Instance != null && SpriteUIViewManager.Instance.CurrentSpriteUI == SpriteUIEnum.SpriteSkillUI)
        {
            Debug.LogError(index);
            ElfSystem.Instance.CurAreaId = index + 1;
            ElfSystem.Instance.CurTearCount = ElfSystemData.dataMap[ElfSystem.Instance.CurAreaId];
            ElfSystem.Instance.RefreshTearUIBySync();
            SpriteUIViewManager.Instance.IsSpriteSkillClickEnable = false;
            SpriteUIViewManager.Instance.SpriteNPCActionChange(SpriteAnimatorController.ActionStatus.ActionHand);
            TimerHeap.AddTimer(500, 0, () =>
            {
                SpriteUIViewManager.Instance.PlaySwitchToSpriteSkillDetailUI(index);
            });
        }
    }

    /// <summary>
    /// 从技能UI到觉醒UI播放动画结束
    /// </summary>
    void OnPlaySpriteSkillUISwitchToSpriteDetailUIEnd()
    {
        SpriteUIViewManager.Instance.IsSpriteSkillClickEnable = true;
        TimerHeap.AddTimer(100, 0, () =>
            {
                SpriteUIViewManager.Instance.SwitchSpriteSkillUIToSpriteSkillDetailUI();
            });
    }

    /// <summary>
    /// 觉醒界面点击返回
    /// </summary>
    void OnSpriteSkillDetaiUIBackUp()
    {
        SpriteUIViewManager.Instance.SwitchSpriteSkillDetailUIToSpriteSkillUI();
    }

    /// <summary>
    /// 点击觉醒
    /// </summary>
    /// <param name="isPress"></param>
    void OnAwakePress(bool isPress)
    {
        if (ElfSystem.Instance.MaxTearCount != 0)
        {
            if (isPress)
            {
                ElfSystem.Instance.CurTearCount++;
                ElfSystem.Instance.RefreshTearUIByPress();
            }
            else
            {
                ElfSystem.Instance.OnReleaseAwakeBtn();
            }
        }
        else
        {
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(29601));
        }

    }

    /// <summary>
    /// 点击精灵技能按钮
    /// </summary>
    void OnSpriteSkillUISkillUp()
    {
        SpriteUIViewManager.Instance.SwitchSpriteSkillUIToSpriteLearnSkillUI();
        ElfSystem.Instance.ElfSkillInfoReq();
        
    }

    /// <summary>
    /// 点击领悟UI领悟按钮
    /// </summary>
    void OnSpriteLearnSkillUILearnSkillUp()
    {
        ElfSystem.Instance.LearnElfSkillReq();
    }

    /// <summary>
    /// 点击领悟UI返回按钮
    /// </summary>
    void OnSpriteLearnSkillUIBackUp()
    {
        SpriteUIViewManager.Instance.SwitchSpriteLearnSkillUIToSpriteSkillUI();
    }

    /// <summary>
    /// 点击领悟UI装备按钮
    /// </summary>
    void OnLearnSkillUIEquipUp()
    {
        ElfSystem.Instance.ElfEquipSkillReq(ElfSystem.Instance.GetCurSkillIndex());
    }

    /// <summary>
    /// 点击领悟UI重置按钮
    /// </summary>
    void OnLearnSkillUIResetUp()
    {
        MogoMessageBox.Confirm(LanguageData.GetContent(29602, PriceListData.GetPrice(22, 0)),
            (flag) =>
            {
                if (flag)
                    ElfSystem.Instance.ResetElfSkillReq();
            });

    }

    /// <summary>
    /// 点击领悟UI升级按钮
    /// </summary>
    void OnLearnSkillUIUpgradeUp()
    {
        ElfSystem.Instance.ElfSkillUpgradeReq(ElfSystem.Instance.GetCurSkillIndex());
    }

    /// <summary>
    /// 领悟UI选择精灵技能
    /// </summary>
    /// <param name="index"></param>
    public void OnLearnSkillUISkillChoose(int index)
    {
        Debug.LogError(index);
        ElfSystem.Instance.CurrentSkill = index;
        RefreshCurSkillDesc(ElfSystem.Instance.GetCurSkillIndex());
    }

    void RefreshCurSkillDesc(int skillId)
    {
        
        SpriteUIViewManager.Instance.SetDetailCurrentSkill(
LanguageData.GetContent(SkillData.dataMap.Get(skillId).name),
                    IconData.dataMap.Get(SkillData.dataMap.Get(skillId).icon).path,
                    SkillData.dataMap.Get(skillId).level
            );
        SpriteUIViewManager.Instance.SetDetailCurrentSkillDesc(LanguageData.GetContent(SkillData.dataMap.Get(skillId).desc));

        var nextSkillId = ElfSkillUpgradeData.dataMap.FirstOrDefault(x => x.Value.preSkillId == skillId);
        if (nextSkillId.Value!=null)
        {
            SpriteUIViewManager.Instance.SetDetailNextSkillDesc(LanguageData.GetContent(SkillData.dataMap.Get(nextSkillId.Key).desc));
            var pair = ElfSkillUpgradeData.dataMap.Get(nextSkillId.Key).consume.ToList()[0];
            SpriteUIViewManager.Instance.SetDetailNextSkillRequest(LanguageData.GetContent(29600, pair.Value, ItemParentData.GetItem(pair.Key).Name));
            SpriteUIViewManager.Instance.SetDetailNextSkillProgress(MogoWorld.thePlayer.inventoryManager.GetItemNumById(pair.Key) / (float)pair.Value);
            SpriteUIViewManager.Instance.ShowDetailNextSkillInfo(true);
        }
        else
        {
            SpriteUIViewManager.Instance.ShowDetailNextSkillInfo(false);
        }
    }
    #endregion
}
