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
using System.Collections.Generic;
using Mogo.Util;
using System;
using System.Text;
using Mogo.Game;
using Mogo.Mission;

public enum TreasureChestUITab
{
    ShowItemTab = 1,
    GetItemTab = 2,
}

public class InstanceTreasureChestUIViewManager : MogoUIBehaviour 
{
    private static InstanceTreasureChestUIViewManager m_instance;
    public static InstanceTreasureChestUIViewManager Instance { get { return InstanceTreasureChestUIViewManager.m_instance; } }
  
    private List<GameObject> m_listInstaceRewardItem = new List<GameObject>();

    private UILabel m_lblInstanceTreasureChestUITitle;
    private UILabel m_lblInstanceTreasureChestUIDesc;

    private GameObject m_goInstanceTreasureChestUIBtnOK;
    private GameObject m_goInstanceTreasureChestUIBtnGet;

    private GameObject m_goGOInstanceTreasureChestUIRewardList;
    private GameObject m_goInstanceTreasureChestUIRewardPos1;
    private GameObject m_goInstanceTreasureChestUIRewardPos2;
    private GameObject m_goInstanceTreasureChestUIRewardPos3;

    public int CurrentID { get; set; }

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_lblInstanceTreasureChestUITitle = m_myTransform.Find(m_widgetToFullName["InstanceTreasureChestUITitle"]).GetComponent<UILabel>();
        m_lblInstanceTreasureChestUIDesc = m_myTransform.Find(m_widgetToFullName["InstanceTreasureChestUIDesc"]).GetComponent<UILabel>();

        m_goInstanceTreasureChestUIBtnOK = m_myTransform.Find(m_widgetToFullName["InstanceTreasureChestUIBtnOK"]).gameObject;
        m_goInstanceTreasureChestUIBtnGet = m_myTransform.Find(m_widgetToFullName["InstanceTreasureChestUIBtnGet"]).gameObject;

        m_listInstaceRewardItem.Add(m_myTransform.Find(m_widgetToFullName["InstanceTreasureChestUIRewardItem0"]).gameObject);
        m_listInstaceRewardItem.Add(m_myTransform.Find(m_widgetToFullName["InstanceTreasureChestUIRewardItem1"]).gameObject);
        m_listInstaceRewardItem.Add(m_myTransform.Find(m_widgetToFullName["InstanceTreasureChestUIRewardItem2"]).gameObject);
        for (int i = 0; i < m_listInstaceRewardItem.Count; i++)
        {
            m_listInstaceRewardItem[i].AddComponent<InventoryGrid>();
        }

        m_goGOInstanceTreasureChestUIRewardList = m_myTransform.Find(m_widgetToFullName["GOInstanceTreasureChestUIRewardList"]).gameObject;
        m_goInstanceTreasureChestUIRewardPos1 = m_myTransform.Find(m_widgetToFullName["InstanceTreasureChestUIRewardPos1"]).gameObject;
        m_goInstanceTreasureChestUIRewardPos2 = m_myTransform.Find(m_widgetToFullName["InstanceTreasureChestUIRewardPos2"]).gameObject;
        m_goInstanceTreasureChestUIRewardPos3 = m_myTransform.Find(m_widgetToFullName["InstanceTreasureChestUIRewardPos3"]).gameObject;

        Initialize();
    }

    #region 事件
    public Action TREASURECHESTOKUP;
    public Action<int> TREASURECHESTGETUP;

    void Initialize()
    {
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceTreasureChestUIBtnOK", OnTreasureChestUIOK);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceTreasureChestUIBtnGet", OnTreasureChestUIGet);

        EventDispatcher.TriggerEvent("InstanceUILoadPartEnd");
    }

    public void Release()
    {
      
    }

    void OnTreasureChestUIOK(int i)
    {
        if (TREASURECHESTOKUP != null)
            TREASURECHESTOKUP();
    }

    void OnTreasureChestUIGet(int i)
    {
        if (TREASURECHESTGETUP != null)
            TREASURECHESTGETUP(CurrentID);
    }

    #endregion


    #region 物品奖励

    /// <summary>
    /// 设置物品奖励Id
    /// </summary>
    /// <param name="idList"></param>
    public void SetRewardItemID(List<int> idList)
    {
        SetRewardItemPos(idList.Count);

        for (int i = 0; i < 3; ++i)
        {
            if (i < idList.Count)
            {
                UISprite itemSprite = m_listInstaceRewardItem[i].transform.Find("InstanceTreasureChestUIRewardItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0];
                UISprite bgSprite = m_listInstaceRewardItem[i].transform.Find("InstanceTreasureChestUIRewardItem" + i + "BG").GetComponentsInChildren<UISprite>(true)[0];                
                m_listInstaceRewardItem[i].GetComponentsInChildren<InventoryGrid>(true)[0].iconID = idList[i];
                InventoryManager.SetIcon(idList[i], itemSprite, 0, null, bgSprite);

                m_listInstaceRewardItem[i].SetActive(true);
            }
            else
            {
                m_listInstaceRewardItem[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 设置宝箱物品奖励Image,Name
    /// </summary>
    /// <param name="imageNameList"></param>
    public void SetInstanceLevelRewardItemImage(List<string> imageNameList, List<string> itemNameList)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (i < imageNameList.Count)
            {
                UISprite itemSprite = m_listInstaceRewardItem[i].transform.Find("InstanceTreasureChestUIRewardItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0];
                MogoUIManager.Instance.TryingSetSpriteName(imageNameList[i], itemSprite);

                m_listInstaceRewardItem[i].transform.Find("InstanceTreasureChestUIRewardItem" + i + "Text").GetComponentsInChildren<UILabel>(true)[0].text = itemNameList[i];
            }
            else
            {
                m_listInstaceRewardItem[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 设置物品数量
    /// </summary>
    /// <param name="countList"></param>
    public void SetInstanceLevelRewardItemCount(List<int> countList)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (i < countList.Count)
            {
                if (countList[i] > 1)
                    m_listInstaceRewardItem[i].transform.Find("InstanceTreasureChestUIRewardItem" + i + "Count").GetComponentsInChildren<UILabel>(true)[0].text = countList[i].ToString();
                else
                    m_listInstaceRewardItem[i].transform.Find("InstanceTreasureChestUIRewardItem" + i + "Count").GetComponentsInChildren<UILabel>(true)[0].text = "";
            }          
        }
    }

    /// <summary>
    /// 调整物品位置
    /// </summary>
    /// <param name="count"></param>
    private void SetRewardItemPos(int count)
    {
        switch (count)
        {
            case 1:
                m_goGOInstanceTreasureChestUIRewardList.transform.localPosition = m_goInstanceTreasureChestUIRewardPos1.transform.localPosition;
                break;
            case 2:
                m_goGOInstanceTreasureChestUIRewardList.transform.localPosition = m_goInstanceTreasureChestUIRewardPos2.transform.localPosition;
                break;
            case 3:
                m_goGOInstanceTreasureChestUIRewardList.transform.localPosition = m_goInstanceTreasureChestUIRewardPos3.transform.localPosition;
                break;
            default:
                m_goGOInstanceTreasureChestUIRewardList.transform.localPosition = m_goInstanceTreasureChestUIRewardPos3.transform.localPosition;
                break;
        }     
    }

    #endregion

    /// <summary>
    /// 设置宝箱名称
    /// </summary>
    /// <param name="name"></param>
    public void SetTitle(string name)
    {
        m_lblInstanceTreasureChestUITitle.text = name;
    }

    /// <summary>
    /// 设置描述信息
    /// </summary>
    /// <param name="desc"></param>
    public void SetDesc(string desc)
    {
        m_lblInstanceTreasureChestUIDesc.text = desc;
    }

    public void ChangeTab(int tab)
    {
        if (tab == (int)TreasureChestUITab.ShowItemTab)
        {
            m_goInstanceTreasureChestUIBtnOK.SetActive(true);
            m_goInstanceTreasureChestUIBtnGet.SetActive(false);
        }
        else if (tab == (int)TreasureChestUITab.GetItemTab)
        {
            m_goInstanceTreasureChestUIBtnOK.SetActive(false);
            m_goInstanceTreasureChestUIBtnGet.SetActive(true);
        }
    }
}
