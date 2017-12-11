using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;

using Mogo.GameData;
using Mogo.Game;

public class ChargeRewardUILogicManager
{
    protected List<int> chargeGotID = new List<int>();

    protected int currentChargeRewardPage;
    protected bool isCharge;

    private static ChargeRewardUILogicManager m_instance;

    public static ChargeRewardUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ChargeRewardUILogicManager();
            }
            return ChargeRewardUILogicManager.m_instance;
        }
    }

    public void Initialize()
    {
        currentChargeRewardPage = 0;
        isCharge = true;

        ChargeRewardUIViewManager.Instance.CHARGEBUTTONUP += OnChargeUp;
        ChargeRewardUIViewManager.Instance.BTNVIPUP += OnBtnVIPUp;
        ChargeRewardUIViewManager.Instance.GETREWARDBUTTONUP += OnGetRewardUp;

        EventDispatcher.AddEventListener<int>("ShowChargeRewardByBtn", ShowChargeRewardByBtn);
    }

    public void Release()
    {
        currentChargeRewardPage = 0;
        isCharge = true;

        ChargeRewardUIViewManager.Instance.CHARGEBUTTONUP -= OnChargeUp;
        ChargeRewardUIViewManager.Instance.BTNVIPUP -= OnBtnVIPUp;
        ChargeRewardUIViewManager.Instance.GETREWARDBUTTONUP -= OnGetRewardUp;

        EventDispatcher.RemoveEventListener<int>("ShowChargeRewardByBtn", ShowChargeRewardByBtn);
    }


    #region 按钮事件

    void OnChargeUp()
    {
        LoggerHelper.Debug("OnChargeUp");        

        //if (isCharge)
        //    EventDispatcher.TriggerEvent(Events.OperationEvent.Charge);
        //else
        //    EventDispatcher.TriggerEvent(Events.OperationEvent.ChargeGetReward);
        if (isCharge)
        {
            //MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(25570));
            EventDispatcher.TriggerEvent(Events.OperationEvent.Charge);
        }
    }

    void OnBtnVIPUp()
    {
        LoggerHelper.Debug("OnBtnVIPUp");   
        MogoUIManager.Instance.ShowVIPInfoUI();
    }

    void OnGetRewardUp()
    {
        LoggerHelper.Debug("OnGetRewardUp");
        if (!isCharge)
            EventDispatcher.TriggerEvent(Events.OperationEvent.ChargeGetReward);
    }

    void OnShowChargeRewardUp()
    {
 
    }

    #endregion


    #region 逻辑事件

    public void UpdateRewardGot(List<int> theChargeGotID)
    {
        chargeGotID = theChargeGotID;
    }

    public void UpdateCurrentChargeRewardPage(int money)
    {
        int i = -1;
        foreach (var data in RewardRechargeData.dataMap)
        {
            i++;
            if (data.Value.money > money)
            {
                break;
            }
        }
        if (currentChargeRewardPage != i)
        {
            SetChargeProgress(currentChargeRewardPage);
        }
    }

    public void ShowChargeRewardByBtn(int offset)
    {
        //MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(25570));

        List<KeyValuePair<int, int>> spItemMessages = new List<KeyValuePair<int, int>>();
        List<int> spIDs = new List<int>();

        int count = -1;

        foreach (var data in RewardRechargeData.dataMap)
        {
            count++;
            if (count == currentChargeRewardPage + offset)
            {
                Dictionary<int, int> items;
                switch (MogoWorld.thePlayer.vocation)
                {
                    case Vocation.Warrior:
                        items = data.Value.items1;
                        break;
                    case Vocation.Assassin:
                        items = data.Value.items2;
                        break;
                    case Vocation.Archer:
                        items = data.Value.items3;
                        break;
                    case Vocation.Mage:
                        items = data.Value.items4;
                        break;
                    default:
                        items = new Dictionary<int, int>();
                        break;
                }

                foreach (var item in items)
                {
                    if (ItemParentData.GetItem(item.Key) != null)
                    {
                        spIDs.Add(item.Key);
                        spItemMessages.Add(item);
                    }
                }

                // 设置按钮
                if (data.Value.money <= MogoWorld.thePlayer.chargeSum)
                {
                    isCharge = false;
                }
                else
                {
                    isCharge = true;
                }
                ChargeRewardUIViewManager.Instance.SwitchBtn(isCharge);
                break;
            }
        }

        if (spItemMessages.Count != 0)
        {
            ChargeRewardUIViewManager.Instance.SetChargeRewardListGrid(spIDs);
            ChargeRewardUIViewManager.Instance.SetChargeRewardListIcon(spItemMessages);
        }

        ChargeRewardUIViewManager.Instance.SetBtnGroupPress(offset);
        LoggerHelper.Debug(offset);
        ChargeRewardUIViewManager.Instance.SetProgressMark(offset);
    }

    public void SetChargeProgress(int i)
    {
        int position = SetPosition(i);
        LoggerHelper.Debug("SetChargeProgress" + position);
        ChargeRewardUIViewManager.Instance.SetBtnGroupPress(position);
        ChargeRewardUIViewManager.Instance.SetProgressMark(position);
    }

    protected int SetPosition(int i)
    {
        currentChargeRewardPage = i;
        int position = 0;


        if (i + 6 > RewardRechargeData.dataMap.Count)
        {
            position = 6 - i % 6 - 1;
            currentChargeRewardPage = i - position;
        }

        SetChargeData(currentChargeRewardPage);
        return position;
    }

    private void SetChargeData(int position)
    {
        List<int> lbMoney = new List<int>();
        List<KeyValuePair<int, int>> spItemMessages = new List<KeyValuePair<int, int>>();
        List<int> spIDs = new List<int>();

        int count = -1;
        int offset = -1;
        bool beginOffset = false;

        foreach (var data in RewardRechargeData.dataMap)
        {
            count++;
            if (count == position)
            {
                beginOffset = true;

                var items = data.Value.items1;

                ChargeRewardUIViewManager.Instance.SetChargeRewardListEnable(items.Count);

                foreach (var item in items)
                {
                    spIDs.Add(item.Key);
                    spItemMessages.Add(item);
                }

                if (data.Value.money <= MogoWorld.thePlayer.chargeSum)
                {
                    isCharge = false;
                }
                else
                {
                    isCharge = true;
                }
                ChargeRewardUIViewManager.Instance.SwitchBtn(isCharge);
            }

            if (beginOffset && offset < 6)
            {
                offset++;
                lbMoney.Add(data.Value.money);
            }
        }

        ChargeRewardUIViewManager.Instance.SetBtnGroupText(lbMoney);

        ChargeRewardUIViewManager.Instance.SetChargeRewardListGrid(spIDs);
        ChargeRewardUIViewManager.Instance.SetChargeRewardListIcon(spItemMessages);
    }

    #endregion
}
