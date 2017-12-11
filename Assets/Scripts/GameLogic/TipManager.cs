/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：TipManagre
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013-10-12
// 模块描述：提示助手管理
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;
using System;
using Mogo.Game;
using TDBID = System.UInt64;

public class TipManager
{
    private static TipManager m_instance;

    const int LEVEL_NEED = 1;
    const uint TIME_LENGTH = 20000;

    public const int TIP_TYPE_MAIL_HAS_JEWEL = 0;
    public const int TIP_TYPE_EQUIP = 1;
    public const int TIP_TYPE_GIFT = 2;
    public const int TIP_TYPE_ENHANCE = 4;
    public const int TIP_TYPE_SKILL = 3;
    public const int TIP_TYPE_ENERGY_EVENT = 5;
    public const int TIP_TYPE_ARENA_REWARD = 6;
    public const int TIP_TYPE_DRAGON_MATCH = 7;
    public const int TIP_TYPE_JEWEL_LAYOUT = 8;
    public const int TIP_TYPE_ELF = 9;
    Queue<ItemEquipment> m_equipQueue = new Queue<ItemEquipment>();
    bool m_isLevelUp = false;

    public static TipManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new TipManager();
            }
            return m_instance;
        }
    }

    private TipManager()
    {
        AddListener();

    }

    static public void Init()
    {
        m_instance = new TipManager();
    }

    ~TipManager()
    {
        RemoveListener();
    }

    public void RemoveListener()
    {
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, InstanceLoaded);
    }

    private void InstanceLoaded(int copyId, bool isInCopy)
    {
        if (!isInCopy)
        {
            ShowTip();
        }
    }

    private void ShowTip()
    {
        while (m_equipQueue.Count > 0)
        {
            OnGetEquipment(m_equipQueue.Dequeue());
        }
        if (m_isLevelUp)
        {
            OnLevelUp(MogoWorld.thePlayer.level);
            m_isLevelUp = false;
        }
    }

    private void AddListener()
    {
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, InstanceLoaded);
    }

    public void OnGetEquipment(ItemEquipment equip)
    {
        if (MogoWorld.thePlayer.level <= 1) return;
        if (!MogoWorld.inCity)
        {
            m_equipQueue.Enqueue(equip);
            return;
        }

        if (InventoryManager.Instance.IsEquipmentBetter(equip.templateId))
        {
            //uint timerId = TimerHeap.AddTimer(TIME_LENGTH, 0, () => { TipUIManager.Instance.Hide(TIP_TYPE_EQUIP); });
            TipViewData viewData = GetTipViewDataByEquip(equip);
            TipUIManager.Instance.AddTipViewData(viewData);
        }
        else if (InventoryManager.Instance.EquipmentInBagList.Count >= 10
            && InventoryManager.Instance.IsRubbish((equip.templateId), 4))
        {
            //uint timerId = TimerHeap.AddTimer(TIME_LENGTH, 0, () => { TipUIManager.Instance.Hide(TIP_TYPE_EQUIP); });
            TipViewData viewData = GetTipViewDataByEquipToDecompose(equip);
            TipUIManager.Instance.AddTipViewData(viewData);
        }
    }

    private TipViewData GetTipViewDataByEquipToDecompose(ItemEquipment equip)
    {
        TipViewData viewData = new TipViewData()
        {
            priority = TIP_TYPE_ENHANCE,
            itemId = equip.templateId,
            tipText = LanguageData.GetContent(3028) + "\n" + LanguageData.GetContent(3029),
            btnName = LanguageData.GetContent(3030),
            btnAction = () =>
            {
                //TimerHeap.DelTimer(timerId);
                TipUIManager.Instance.HideAll(TIP_TYPE_ENHANCE);
                ItemParent temp = InventoryManager.Instance.GetItemByItemID(equip.templateId);
                if (temp == null)
                {
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(472));
                }
                else
                {
                    MogoWorld.thePlayer.StopMove();
                    MogoUIManager.Instance.SwitchDecomposeUI();
                }
            }
        };
        return viewData;
    }

    private TipViewData GetTipViewDataByEquip(ItemEquipment equip)
    {
        TipViewData viewData = new TipViewData()
        {
            priority = TIP_TYPE_EQUIP,
            itemId = equip.templateId,
            tipText = LanguageData.GetContent(3020) + "\n" + equip.name,
            btnName = LanguageData.GetContent(3021),
            btnAction = () =>
            {
                //TimerHeap.DelTimer(timerId);
                TipUIManager.Instance.Hide(TIP_TYPE_EQUIP);
                ItemParent temp = InventoryManager.Instance.GetItemByItemID(equip.templateId);
                if (temp == null)
                {
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(472));
                }
                else
                {
                    MogoWorld.thePlayer.RpcCall("ExchangeEquipment", temp.id, (byte)temp.gridIndex);
                }

            }
        };
        return viewData;
    }

    public void OnLevelUp(int level)
    {
        if (level <= LEVEL_NEED) return;
        if (!MogoWorld.inCity)
        {
            m_isLevelUp = true;
            return;
        }

        //AddGiftTip(level);

        NewAddGiftTip(level);

        AddSkillTip(level);
        if (level <= 3) return;
        CheckBodyEnhance(level);

    }

    private void AddSkillTip(int level)
    {
        //得到skill
        int skillId = (MogoWorld.thePlayer.skillManager as PlayerSkillManager).HadStudySkill();
        if (skillId == -1) return;

        SkillData skillData = SkillData.dataMap.Get(skillId);
        //设置viewData
        //uint timerID = TimerHeap.AddTimer(TIME_LENGTH, 0, () =>
        //{
        //    TipUIManager.Instance.Hide(TIP_TYPE_SKILL);
        //});
        TipViewData viewData = new TipViewData()
        {
            priority = TIP_TYPE_SKILL,
            btnName = LanguageData.GetContent(3032),
            icon = IconData.dataMap.Get(skillData.icon).path,
            tipText = LanguageData.GetContent(3031),
            btnAction = () =>
            {
                MogoWorld.thePlayer.StopMove();
                //TimerHeap.DelTimer(timerID);
                //弹到技能界面界面
                (MogoWorld.thePlayer.skillManager as PlayerSkillManager).OpenSkillUI(skillId, () =>
                {
                    TipUIManager.Instance.HideAll(TIP_TYPE_SKILL);
                });

            }
        };
        //加入viewData
        TipUIManager.Instance.AddTipViewData(viewData);
        GuideSystem.Instance.TriggerEvent<int>(GlobalEvents.SkillAvailable, skillId);
    }

    private void CheckBodyEnhance(int level)
    {
        int canEnhanceSlot = BodyEnhanceManager.Instance.GetCanEnhanceSlot(level);
        if (canEnhanceSlot <= 0) return;

        //uint timerId = TimerHeap.AddTimer(TIME_LENGTH, 0, () => { TipUIManager.Instance.Hide(TIP_TYPE_ENHANCE); });
        TipViewData viewData = new TipViewData()
        {
            priority = TIP_TYPE_ENHANCE,
            icon = BodyIcon.icons[canEnhanceSlot - 1],
            btnName = LanguageData.GetContent(3027),
            tipText = LanguageData.GetContent(3025) + "\n" + LanguageData.GetContent(3026),
            btnAction = () =>
            {
                InventoryManager.Instance.CurrentView = InventoryManager.View.BodyEnhanceView;
                //Debug.LogError("canEnhanceSlot:" + canEnhanceSlot);
                MogoUIManager.Instance.SwitchStrenthUI(() => { OnSwitchStrenthUIDone(canEnhanceSlot); });//
            }
        };
        TipUIManager.Instance.AddTipViewData(viewData);
    }

    private void OnSwitchStrenthUIDone(int slot)
    {
        //Debug.LogError("slot:" + slot);
        MogoWorld.thePlayer.StopMove();
        TipUIManager.Instance.Hide(TIP_TYPE_ENHANCE);
        BodyEnhanceManager.Instance.CurrentSlot = slot;
        BodyEnhanceManager.Instance.RefreshUI();
        //EventDispatcher.TriggerEvent<int>(BodyEnhanceManager.ON_SELECT_SLOT, slot);
    }

    private void NewAddGiftTip(int level)
    {
        int gap = 5;
        if (level % gap != 0) return;

        TipViewData viewData = new TipViewData()
        {
            priority = TIP_TYPE_GIFT,
            itemId = 1100101,
            btnName = LanguageData.GetContent(3024),
            tipText = LanguageData.GetContent(3022) + "\n" + LanguageData.GetContent(3023, level),
            btnAction = () =>
            {
                //TimerHeap.DelTimer(timerId);
                TipUIManager.Instance.HideAll(TIP_TYPE_GIFT);
                MogoUIManager.Instance.SwitchToMarket(MarketUITab.ItemTab);
            }
        };
        TipUIManager.Instance.AddTipViewData(viewData);
    }

    private void AddGiftTip(int level)
    {
        int gap = 10;
        if (level % gap != 0) return;
        int index = level / gap;
        int itemId = 1100100;
        ItemParent item = null;
        int count = 1;
        while (item == null && count <= index)
        {
            itemId++;
            item = InventoryManager.Instance.GetItemByItemID(itemId);
            count++;
        }
        if (item == null)
        {
            LoggerHelper.Warning("can not find item:" + itemId);
            return;
        }
        //uint timerId = TimerHeap.AddTimer(TIME_LENGTH, 0, () => { TipUIManager.Instance.Hide(TIP_TYPE_GIFT); });
        TipViewData viewData = new TipViewData()
        {
            priority = TIP_TYPE_GIFT,
            itemId = item.templateId,
            btnName = LanguageData.GetContent(3024),
            tipText = LanguageData.GetContent(3022) + "\n" + LanguageData.GetContent(3023, (count - 1) * gap),
            btnAction = () =>
            {
                //TimerHeap.DelTimer(timerId);
                TipUIManager.Instance.HideAll(TIP_TYPE_GIFT);

                if (InventoryManager.Instance.GetItemByItemID(itemId) != null)
                {
                    MogoWorld.thePlayer.RpcCall("UseItemReq", item.id, item.gridIndex, TIP_TYPE_GIFT);
                    //防止使用未回调就弹出新的tip
                    TimerHeap.AddTimer(1000, 0, () => { AddGiftTip(MogoWorld.thePlayer.level); });
                }
                else
                {
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(2115));
                    AddGiftTip(MogoWorld.thePlayer.level);
                }


            }
        };
        TipUIManager.Instance.AddTipViewData(viewData);
    }


    public void AddMailHasJewelTip(TDBID mailGridId)
    {
        TipViewData viewData = new TipViewData()
        {
            priority = TIP_TYPE_MAIL_HAS_JEWEL,
            btnName = LanguageData.GetContent(3044),
            icon = IconData.dataMap.Get(118).path,
            tipText = LanguageData.GetContent(3043),
            existTime = 50000,
            btnAction = () =>
            {
                MogoWorld.thePlayer.StopMove();

                MogoUIManager.Instance.SwitchSocialUI(() =>
                {
                    TipUIManager.Instance.HideAll(TIP_TYPE_MAIL_HAS_JEWEL);
                    if (SocietyUIViewManager.Instance)
                    {
                        SocietyUIViewManager.Instance.ChangeToMailTab();
                        EventDispatcher.TriggerEvent<TDBID>(SocietyUILogicManager.SocietyUIEvent.MAILGRIDUP, mailGridId);
                    }
                }); 
            }
        };
        //加入viewData
        TipUIManager.Instance.AddTipViewData(viewData);
    }
}

