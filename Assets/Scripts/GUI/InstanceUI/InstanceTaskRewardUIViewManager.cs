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
using System;
using Mogo.GameData;

public class InstanceTaskRewardUIViewManager : MonoBehaviour
{
    private static InstanceTaskRewardUIViewManager m_instance;

    public static InstanceTaskRewardUIViewManager Instance
    {
        get
        {
            return InstanceTaskRewardUIViewManager.m_instance;
        }
    }

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    const float REWARDITEMSPACE = 0.153f;

    public void AddWigetToFullNameData(string widgetName, string fullName)
    {
        if (m_widgetToFullName.ContainsKey(widgetName))
            LoggerHelper.Debug(widgetName);
        m_widgetToFullName.Add(widgetName, fullName);
    }

    private string GetFullName(Transform currentTransform)
    {
        string fullName = "";

        while (currentTransform != m_myTransform)
        {
            fullName = currentTransform.name + fullName;

            if (currentTransform.parent != m_myTransform)
            {
                fullName = "/" + fullName;
            }

            currentTransform = currentTransform.parent;
        }

        return fullName;
    }

    private void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }

    private Transform m_myTransform;

    UILabel m_lblInstanceTaskRewardExpNum;
    UILabel m_lblInstanceTaskRewardGoldNum;
    UILabel m_lblInstanceTaskRewardItemNum;
    GameObject m_InstanceTaskRewardItem;
    private List<GameObject> m_listInstaceRewardItem = new List<GameObject>();

    // task reward pos
    GameObject m_goInstanceTaskRewardCloseBtnPos1;
    GameObject m_goInstanceTaskRewardCloseBtnPos2;
    GameObject m_goInstanceTaskRewardInfoPos1;
    GameObject m_goInstanceTaskRewardInfoPos2;
    GameObject m_goInstanceTaskRewardTitlePos1;
    GameObject m_goInstanceTaskRewardTitlePos2;

    GameObject m_goGOInstanceTaskRewardBG1;
    GameObject m_goGOInstanceTaskRewardBG2;
    GameObject m_goInstanceTaskRewardCloseBtn;
    GameObject m_goGOInstanceTaskRewardInfo;
    GameObject m_goInstanceTaskRewardTitle;

    void Awake()
    {
        m_instance = gameObject.GetComponent<InstanceTaskRewardUIViewManager>();
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_lblInstanceTaskRewardExpNum = m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardExpNum"]).GetComponent<UILabel>();
        m_lblInstanceTaskRewardGoldNum = m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardGoldNum"]).GetComponent<UILabel>();
        m_lblInstanceTaskRewardItemNum = m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardItemNum"]).GetComponent<UILabel>();
        m_InstanceTaskRewardItem = m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardItem"]).gameObject;

        m_listInstaceRewardItem.Add(m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardItem0"]).gameObject);
        m_listInstaceRewardItem.Add(m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardItem1"]).gameObject);
        m_listInstaceRewardItem.Add(m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardItem2"]).gameObject);
        for (int i = 0; i < m_listInstaceRewardItem.Count; i++)
        {
            m_listInstaceRewardItem[i].AddComponent<InventoryGrid>();
        }

        m_goInstanceTaskRewardCloseBtnPos1 = m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardCloseBtnPos1"]).gameObject;
        m_goInstanceTaskRewardCloseBtnPos2 = m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardCloseBtnPos2"]).gameObject;
        m_goInstanceTaskRewardInfoPos1 = m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardInfoPos1"]).gameObject;
        m_goInstanceTaskRewardInfoPos2 = m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardInfoPos2"]).gameObject;
        m_goInstanceTaskRewardTitlePos1 = m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardTitlePos1"]).gameObject;
        m_goInstanceTaskRewardTitlePos2 = m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardTitlePos2"]).gameObject;

        m_goGOInstanceTaskRewardBG1 = m_myTransform.Find(m_widgetToFullName["GOInstanceTaskRewardBG1"]).gameObject;
        m_goGOInstanceTaskRewardBG2 = m_myTransform.Find(m_widgetToFullName["GOInstanceTaskRewardBG2"]).gameObject;
        m_goInstanceTaskRewardCloseBtn = m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardCloseBtn"]).gameObject;
        m_goGOInstanceTaskRewardInfo = m_myTransform.Find(m_widgetToFullName["GOInstanceTaskRewardInfo"]).gameObject;
        m_goInstanceTaskRewardTitle = m_myTransform.Find(m_widgetToFullName["InstanceTaskRewardTitle"]).gameObject;

        Initialize();
    }

    #region 事件
    public Action INSTANCETASKCLOSEUP;

    public void Initialize()
    {
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceTaskRewardCloseBtn", OnTaskUIClose);

        EventDispatcher.TriggerEvent("InstanceUILoadPartEnd");
    }

    public void Release()
    {

    }

    void OnTaskUIClose(int i)
    {
        if (INSTANCETASKCLOSEUP != null)
            INSTANCETASKCLOSEUP();
    }  

    #endregion

    /// <summary>
    /// 设置经验奖励
    /// </summary>
    /// <param name="i"></param>
    public void SetExpNum(int i)
    {
        m_lblInstanceTaskRewardExpNum.text = "x" + i.ToString();
    }

    /// <summary>
    /// 设置金钱奖励
    /// </summary>
    /// <param name="i"></param>
    public void SetGoldNum(int i)
    {
        m_lblInstanceTaskRewardGoldNum.text = "x" + i.ToString();
    }

    #region 物品奖励

    /// <summary>
    /// 设置物品奖励
    /// </summary>
    /// <param name="idList"></param>
    public void SetRewardItemID(List<int> idList, List<int> countList)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (i < idList.Count && i < countList.Count)
            {
                UISprite itemSprite = m_listInstaceRewardItem[i].transform.Find("InstanceTaskRewardItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0];
                UISprite bgSprite = m_listInstaceRewardItem[i].transform.Find("InstanceTaskRewardItem" + i + "BG").GetComponentsInChildren<UISprite>(true)[0];
                UILabel countLabel = m_listInstaceRewardItem[i].transform.Find("InstanceTaskRewardItem" + i + "Count").GetComponentsInChildren<UILabel>(true)[0];
                m_listInstaceRewardItem[i].GetComponentsInChildren<InventoryGrid>(true)[0].iconID = idList[i];
                InventoryManager.SetIcon(idList[i], itemSprite, countList[i], countLabel, bgSprite);

                if (countList[i] > 1 == false)
                    countLabel.text = "";

                m_listInstaceRewardItem[i].SetActive(true);
            }
            else
            {
                m_listInstaceRewardItem[i].SetActive(false);
            }
        }

        if (idList.Count == 0)
            ShowPanelDiversity(false);
            
        else
            ShowPanelDiversity(true);
    }

    /// <summary>
    /// 设置奖励物品数量
    /// </summary>
    /// <param name="i"></param>
    public void SetRewardItemNum(int i)
    {
        m_lblInstanceTaskRewardItemNum.text = i.ToString();
    }

    /// <summary>
    /// 设置物品奖励
    /// </summary>
    /// <param name="imageNameList"></param>
    public void SetInstanceLevelRewardItemImage(List<string> imageNameList)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (i < imageNameList.Count)
            {
                m_listInstaceRewardItem[i].transform.Find("InstanceTaskRewardItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetAtlasByIconName(imageNameList[i]);
                m_listInstaceRewardItem[i].transform.Find("InstanceTaskRewardItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0].spriteName = imageNameList[i];
                m_listInstaceRewardItem[i].SetActive(true);
            }
            else
            {
                m_listInstaceRewardItem[i].SetActive(false);
            }
        }
    }

    public void SetInstanceLevelRewardItemCount(List<int> countList)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (i < countList.Count)
            {              
                m_listInstaceRewardItem[i].transform.Find("InstanceTaskRewardItem" + i + "Count").GetComponentsInChildren<UILabel>(true)[0].text = countList[i].ToString();
            }
            else
            {
            }
        }
    }

    void ShowTaskRewardItem(bool isShow)
    {
        m_InstanceTaskRewardItem.SetActive(isShow);
    }

    void ShowPanelDiversity(bool isContainItem)
    {
        if (isContainItem)
        {
            ShowTaskRewardItem(true);

            m_goGOInstanceTaskRewardBG1.SetActive(true);
            m_goGOInstanceTaskRewardBG2.SetActive(false);

            m_goInstanceTaskRewardCloseBtn.transform.localPosition = new Vector3(
                m_goInstanceTaskRewardCloseBtn.transform.localPosition.x,
                m_goInstanceTaskRewardCloseBtnPos1.transform.localPosition.y,
                m_goInstanceTaskRewardCloseBtn.transform.localPosition.z);

            m_goGOInstanceTaskRewardInfo.transform.localPosition = new Vector3(
                m_goGOInstanceTaskRewardInfo.transform.localPosition.x,
                m_goInstanceTaskRewardInfoPos1.transform.localPosition.y,
                m_goGOInstanceTaskRewardInfo.transform.localPosition.z);

            m_goInstanceTaskRewardTitle.transform.localPosition = new Vector3(
                m_goInstanceTaskRewardTitle.transform.localPosition.x,
                m_goInstanceTaskRewardTitlePos1.transform.localPosition.y,
                m_goInstanceTaskRewardTitle.transform.localPosition.z);
        }
        else
        {
            ShowTaskRewardItem(false);

            m_goGOInstanceTaskRewardBG2.SetActive(true);
            m_goGOInstanceTaskRewardBG1.SetActive(false);

            m_goInstanceTaskRewardCloseBtn.transform.localPosition = new Vector3(
                m_goInstanceTaskRewardCloseBtn.transform.localPosition.x,
                m_goInstanceTaskRewardCloseBtnPos2.transform.localPosition.y,
                m_goInstanceTaskRewardCloseBtn.transform.localPosition.z);

            m_goGOInstanceTaskRewardInfo.transform.localPosition = new Vector3(
               m_goGOInstanceTaskRewardInfo.transform.localPosition.x,
               m_goInstanceTaskRewardInfoPos2.transform.localPosition.y,
               m_goGOInstanceTaskRewardInfo.transform.localPosition.z);

            m_goInstanceTaskRewardTitle.transform.localPosition = new Vector3(
                m_goInstanceTaskRewardTitle.transform.localPosition.x,
                m_goInstanceTaskRewardTitlePos2.transform.localPosition.y,
                m_goInstanceTaskRewardTitle.transform.localPosition.z);
        }
    }

    /// <summary>
    /// Test
    /// </summary>   
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    List<int> idList = new List<int>();
        //    idList.Add(1);
        //    List<int> countList = new List<int>();
        //    countList.Add(1);
        //    SetRewardItemID(idList, countList);
        //}
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    List<int> idList = new List<int>();
        //    List<int> countList = new List<int>();
        //    SetRewardItemID(idList, countList);
        //}
    }   

    #endregion
}
