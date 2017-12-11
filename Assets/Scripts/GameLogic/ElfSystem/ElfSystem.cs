using UnityEngine;
using System.Collections;
using Mogo.Game;
using Mogo.RPC;
using Mogo.GameData;
using System.Collections.Generic;
using System.Linq;
using System;
using Mogo.Util;

public class ElfSystem : IEventManager
{
    private EntityMyself m_myself;

    public ElfSystem()
    {
        m_myself = MogoWorld.thePlayer;
        AddListeners();
    }
    private static ElfSystem m_instance;
    public static ElfSystem Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ElfSystem();
            }
            return m_instance;
        }
    }
    private int m_currentSkill = 0;

    public int CurrentSkill
    {
        get { return m_currentSkill; }
        set { m_currentSkill = value; }
    }

    private int m_curAreaId = 0;

    public int CurAreaId
    {
        get { return m_curAreaId; }
        set
        {
            m_curAreaId = value;
            start = String.Concat(CurAreaId, "01");
        }
    }
    private int m_skillPoint;

    public int SkillPoint
    {
        get { return m_skillPoint; }
        set
        {
            m_skillPoint = value;
        }
    }
    public string start;
    public float m_progress;

    private int m_curTearCount;
    private int m_maxTearCount;

    public int MaxTearCount
    {
        get { return m_maxTearCount; }
        set { m_maxTearCount = value; }
    }

    public int CurTearCount
    {
        get { return m_curTearCount; }
        set { m_curTearCount = value; }
    }

    public void RefreshTearUIBySync()
    {
        RefreshTearUI(m_curTearCount, false);
    }
    public void RefreshTearUIByPress()
    {
        if (m_maxTearCount == 0) return;
        if (m_curTearCount - ElfSystemData.dataMap[CurAreaId] > m_maxTearCount) return;
        int sum = ElfNodeData.dataMap.Where(x => x.Key.ToString()
            .StartsWith(String.Concat(CurAreaId, "01")))
            .Sum(x => x.Value.consume);
        if (m_curTearCount > sum) return;

        if (m_curTearCount - ElfSystemData.dataMap[CurAreaId] == m_maxTearCount)
        {
            if (SpriteUIViewManager.Instance.IsInSpriteDetailUI())
            {
                SpriteUIViewManager.Instance.CancelAwakePress();
                OnReleaseAwakeBtn();
            }
        }
        else
        {
            RefreshTearUI(m_curTearCount, true);
        }

    }
    public void SetElfTear(int maxCount)
    {
        m_maxTearCount = maxCount;
        SpriteUIViewManager.Instance.SetMaterialCurrentNum(m_maxTearCount);
        if (CurAreaId != 0)
        {
            //说明选了领域
            ElfSystem.Instance.CurTearCount = ElfSystemData.dataMap[CurAreaId];
            ElfSystem.Instance.RefreshTearUIBySync();
        }

        RefreshElfNodeFillStatus();

    }
    private void RefreshElfNodeFillStatus()
    {
        //没选领域时
        List<int> fullArea = new List<int>();
        foreach (var item in ElfSystemData.dataMap.OrderByKey())
        {
            int sum = ElfNodeData.dataMap.Where(x => x.Key.ToString()
                .StartsWith(String.Concat(item.Key, "01")))
                .Sum(x => x.Value.consume);


            if (item.Value == sum)
            {
                //满的
                fullArea.Add(item.Key);
            }
        }
        OnOpenUI(fullArea);
    }
    private int GetConsumeFromDataMap(int index)
    {
        return ElfNodeData.dataMap.Get(Int32.Parse(start) * 100 + index + 1).consume;
    }
    /// <summary>
    /// 刷新女神之泪为自变量的界面
    /// </summary>
    /// <param name="curCount">女神之泪的数量</param>
    public void RefreshTearUI(int curCount, bool isUsingTear = false)
    {
        int index = 0;
        int count = curCount;
        bool isFinalOne = false;
        while (true)
        {
            if (count - GetConsumeFromDataMap(index) >= 0)
            {
                count -= GetConsumeFromDataMap(index++);
                if (count == 0)
                {
                    //刚好在节点位置
                    if (isUsingTear)
                    {
                        Debug.LogError("214141");
                        SpriteUIViewManager.Instance.CancelAwakePress();
                        OnReleaseAwakeBtn();
                    }
                }

                if (!ElfNodeData.dataMap.ContainsKey(Int32.Parse(start) * 100 + index + 1))
                {
                    isFinalOne = true; //最后一个球特殊处理
                    break;
                }
            }
            else
            {
                break;
            }
        }

        if (!isFinalOne)
        {
            m_progress = count / (float)GetConsumeFromDataMap(index);
            SpriteUIViewManager.Instance.SetAwakeProgressList(index, m_progress);
            SpriteUIViewManager.Instance.ShowMaterialNextNum(true, GetConsumeFromDataMap(index) - count);
        }
        else
        {
            index--;
            SpriteUIViewManager.Instance.SetAwakeProgressList(index, 1);
            SpriteUIViewManager.Instance.ShowMaterialNextNum(true, 0);
            if (isUsingTear)
            {
                ShowSkillPointBox(index);
            }
        }
        SpriteUIViewManager.Instance.SetMaterialCurrentNum(m_maxTearCount - (m_curTearCount - ElfSystemData.dataMap[CurAreaId]));
        SetSkillBall(index);
    }
    public void ShowSkillPointBox(int index)
    {
        if (ElfNodeData.dataMap.Get(Int32.Parse(start) * 100 + index + 1).AwardSkillPoint > 0)
        {
            MogoMessageBox.Confirm(LanguageData.GetContent(29619), LanguageData.GetContent(29620),
                (flag) =>
                {
                    if (flag)
                    {
                        SpriteUIViewManager.Instance.SwitchSpriteSkillDetailUIToSpriteSkillUI();
                        SpriteUIViewManager.Instance.SwitchSpriteSkillUIToSpriteLearnSkillUI();
                        ElfSystem.Instance.ElfSkillInfoReq();
                    }

                });
        }

    }
    public void SetSkillBall(int index)
    {
        var data = new List<SpriteSkillPieceData>();
        data.Add(new SpriteSkillPieceData()
        {
            icon = "",
            name = "",
            isAwake = false
        });
        foreach (var item in ElfNodeData.dataMap.OrderByKey().Where(x => x.Key.ToString().StartsWith(start)))
        {
            if (item.Key > Int32.Parse(start) * 100 + index)
            {
                data.Add(new SpriteSkillPieceData()
                {
                    icon = IconData.dataMap.Get(item.Value.iconId).path,
                    name = LanguageData.GetContent(item.Value.nameCode),
                    isAwake = false
                });
            }
            else
            {
                data.Add(new SpriteSkillPieceData()
                {
                    icon = IconData.dataMap.Get(item.Value.iconId).path,
                    name = LanguageData.GetContent(item.Value.nameCode),
                    isAwake = true
                });
            }
        }

        SpriteUIViewManager.Instance.SetSpriteSkillDetailUIPieceData(data);
    }
    public void OnReleaseAwakeBtn()
    {
        ElfSystem.Instance.OnUseTear((byte)CurAreaId, m_curTearCount - ElfSystemData.dataMap[CurAreaId]);

    }
    public void OnFillEnergy()
    {

    }
    private List<int> skillData = new List<int>();
    /// <summary>
    /// 格子to技能ID的映射map
    /// </summary>
    public List<int> PropSkillData
    {
        get { return skillData; }
        set { skillData = value; }
    }
    public int GetCurSkillIndex()
    {
        Debug.LogError(skillData.PackList());
        return PropSkillData[CurrentSkill];
    }

    public void RefreshLearnedSkill(Dictionary<int, int> data, uint equipSkill)
    {
        MogoWorld.thePlayer.ElfEquipSkillId = equipSkill;
        var LearnData = new List<SpriteLearnSkillData>(data.Count);
        var skillIDList = new List<int>();
        foreach (var item in data)
        {

            if (item.Key != MogoWorld.thePlayer.ElfEquipSkillId)
            {
                skillIDList.Add(item.Key);
                LearnData.Add(new SpriteLearnSkillData()
                {
                    name = LanguageData.GetContent(SkillData.dataMap.Get(item.Key).name),
                    icon = IconData.dataMap.Get(SkillData.dataMap.Get(item.Key).icon).path,
                    hasLearned = Convert.ToBoolean(item.Value),
                    isEquipped = false
                });

            }
            else
            {
                skillIDList.Add(item.Key);
                LearnData.Add(new SpriteLearnSkillData()
                {
                    name = LanguageData.GetContent(SkillData.dataMap.Get(item.Key).name),
                    icon = IconData.dataMap.Get(SkillData.dataMap.Get(item.Key).icon).path,
                    hasLearned = Convert.ToBoolean(item.Value),
                    isEquipped = true
                });
            }
        }
        ElfSystem.Instance.PropSkillData = skillIDList;
        SpriteUIViewManager.Instance.SetListSpriteUILearnSkill(LearnData);
        SpriteUILogicManager.Instance.OnLearnSkillUISkillChoose(CurrentSkill);

    }
    public void OnOpenUI(List<int> FulfillAreaList)
    {
        var skillList = new List<SpriteSkillData>(ElfSkillData.dataMap.Count);
        foreach (var item in ElfAreaLimitData.dataMap.OrderByKey())
        {
            if (FulfillAreaList.Contains(item.Value.areaId))
            {
                //填满了
                skillList.Add(new SpriteSkillData()
                {
                    ID = item.Key,
                    name = LanguageData.GetContent(item.Value.nameCode),
                    unlockLevel = item.Value.limitLevel,
                    hasFilled = true
                });
            }
            else
            {
                //填满了
                skillList.Add(new SpriteSkillData()
                {
                    ID = item.Key,
                    name = LanguageData.GetContent(item.Value.nameCode),
                    unlockLevel = item.Value.limitLevel,
                    hasFilled = false
                });
            }
        }
        SpriteUIViewManager.Instance.SetSpriteSkillListData(skillList);
    }


    public void OnRequestInfo()
    {
        m_myself.RpcCall("ElfUseTearProgReq");
    }

    /// <summary>
    /// 使用女神之泪
    /// </summary>
    /// <param name="id">领域ID，1开始计数</param>
    /// <param name="count">数量</param>
    public void OnUseTear(byte id, int count)
    {

        if (count > 0)
        {
            m_myself.RpcCall("ElfUseTearReq", id, count);
        }

    }

    public void LearnElfSkillReq()
    {
        m_myself.RpcCall("LearnElfSkillReq");
    }

    public void ResetElfSkillReq()
    {
        m_myself.RpcCall("ResetElfSkillReq");
    }

    public void ElfSkillUpgradeReq(int id)
    {
        m_myself.RpcCall("ElfSkillUpgradeReq", id);
    }

    public void ElfEquipSkillReq(int id)
    {
        m_myself.RpcCall("ElfEquipSkillReq", id);
    }
    public void ElfSkillInfoReq()
    {
        m_myself.RpcCall("ElfSkillInfoReq");
    }
    public void AddListeners()
    {
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, HandleLeaveInstance);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, HandleEnterInstance);
    }

    public void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, HandleLeaveInstance);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, HandleEnterInstance);
    }
    void HandleEnterInstance(int sceneID, bool isInstance)
    {

        if (isInstance)
        {
            MainUILogicManager.Instance.ShowSpriteSkillButton();
        }
        if (sceneID == MogoWorld.globalSetting.homeScene)
        {
            Debug.LogError(ElfSystem.Instance.SkillPoint);
            if (ElfSystem.Instance.SkillPoint > 0)
            {
                TipUIManager.Instance.AddTipViewData(new TipViewData()
                {
                    priority = TipManager.TIP_TYPE_ELF,
                    icon = "zc_jinglin_up",
                    tipText = LanguageData.GetContent(3045),
                    btnName = LanguageData.GetContent(3046),
                    btnAction = () =>
                    {
                        System.Action callback = (
                            () =>
                            {
                                TipUIManager.Instance.Hide((int)TipManager.TIP_TYPE_ELF);
                                ElfSystem.Instance.OnRequestInfo();
                                ElfSystem.Instance.OnOpenUI(new List<int>());
                            });
                        MogoUIManager.Instance.ShowSpriteUI(callback);
                    },
                    atlasName = "MogoNormalMainUI"
                });
            }
            foreach (var item in ElfSystem.Instance.PropSkillData)
            {
                var nextSkillCost = ElfSkillUpgradeData.dataMap.FirstOrDefault(x => x.Value.preSkillId == item).Value.consume.ToList()[0];
                if (m_myself.inventoryManager.GetItemNumById(nextSkillCost.Key) >= nextSkillCost.Value)
                {
                    //数量足够了
                    TipUIManager.Instance.AddTipViewData(new TipViewData()
                    {
                        priority = TipManager.TIP_TYPE_ELF,
                        icon = "zc_jinglin_up",
                        tipText = LanguageData.GetContent(3047),
                        btnName = LanguageData.GetContent(3048),
                        btnAction = () =>
                        {
                            System.Action callback = (
                                () =>
                                {
                                    TipUIManager.Instance.Hide((int)TipManager.TIP_TYPE_ELF);
                                    ElfSystem.Instance.OnRequestInfo();
                                    ElfSystem.Instance.OnOpenUI(new List<int>());
                                });
                            MogoUIManager.Instance.ShowSpriteUI(callback);
                        },
                        atlasName = "MogoNormalMainUI"
                    });
                    break;
                }
            }
        }
    }
    void HandleLeaveInstance(int sceneID, bool isInstance)
    {

    }
}
