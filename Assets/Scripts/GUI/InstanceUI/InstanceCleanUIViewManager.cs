#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：副本扫荡界面
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using System;
using Mogo.GameData;
using System.Linq;
using Mogo.Mission;

public class InstanceCleanUIViewManager : MogoParentUI
{
    private static InstanceCleanUIViewManager m_instance;
    public static InstanceCleanUIViewManager Instance { get { return InstanceCleanUIViewManager.m_instance; } }

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

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

    UILabel m_lblInstanceCleanUITitle;
    UILabel m_lblInstanceCleanUIEnemyText;
    UILabel m_lblInstanceCleanUICleaningText;
    UILabel m_lblInstanceCleanUIBtnCleanText;

    GameObject m_goInstanceCleanUIBtnClose;
    GameObject m_goInstanceCleanUIBtnClean;
    GameObject m_goInstanceCleanUIBtnReward;

    private Transform m_tranReportList;
    private SweepMissionRepostData m_reportData;

    void Awake()
    {
        m_instance = gameObject.GetComponent<InstanceCleanUIViewManager>();
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_lblInstanceCleanUITitle = m_myTransform.Find(m_widgetToFullName["InstanceCleanUITitle"]).GetComponent<UILabel>();
        m_lblInstanceCleanUIEnemyText = m_myTransform.Find(m_widgetToFullName["InstanceCleanUIEnemyText"]).GetComponent<UILabel>();
        m_lblInstanceCleanUICleaningText = m_myTransform.Find(m_widgetToFullName["InstanceCleanUICleaningText"]).GetComponent<UILabel>();
        m_lblInstanceCleanUIBtnCleanText = m_myTransform.Find(m_widgetToFullName["InstanceCleanUIBtnCleanText"]).GetComponent<UILabel>();

        m_goInstanceCleanUIBtnClose = m_myTransform.Find(m_widgetToFullName["InstanceCleanUIBtnClose"]).gameObject;
        m_goInstanceCleanUIBtnClean = m_myTransform.Find(m_widgetToFullName["InstanceCleanUIBtnClean"]).gameObject;
        m_goInstanceCleanUIBtnReward = m_myTransform.Find(m_widgetToFullName["InstanceCleanUIBtnReward"]).gameObject;

        m_tranReportList = m_myTransform.Find(m_widgetToFullName["InstanceCleanUIReportList"]);

        // 设置SourceCamera
        Camera sourceCamera = m_myTransform.Find(m_widgetToFullName["InstanceCleanUICamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_tranReportList.GetComponentsInChildren<MogoListImproved>(true)[0].SourceCamera = sourceCamera;

        Initialize();
    }

    #region 事件
    public Action INSTANCECLEANCLEANUP;
    public Action INSTANCECLEANCLEANREALUP;
    public Action INSTANCECLEANCLOSEUP;
    public Action INSTANCECLEANREWARDUP;

    public void Initialize()
    {
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceCleanUIBtnClean", OnCleanUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceCleanUIBtnClose", OnCloseUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceCleanUIBtnReward", OnRewadUp);

        EventDispatcher.TriggerEvent("InstanceUILoadPartEnd");
    }

    public void Release()
    {

    }

    void OnCleanUp(int i)
    {
        if (INSTANCECLEANCLEANUP != null)
            INSTANCECLEANCLEANUP();
    }

    void OnCleanRealUp()
    {
        if (INSTANCECLEANCLEANREALUP != null)
            INSTANCECLEANCLEANREALUP();
    }

    void OnCloseUp(int i)
    {
        if (INSTANCECLEANCLOSEUP != null)
            INSTANCECLEANCLOSEUP();
    }

    void OnRewadUp(int i)
    {
        if (INSTANCECLEANREWARDUP != null)
            INSTANCECLEANREWARDUP();
    }

    #endregion

    /// <summary>
    /// 展示界面信息
    /// </summary>
    /// <param name="IsShow"></param>
    public void Show(bool IsShow)
    {
        if (IsShow)
        {
            IsCleaning = false;
            SetCleanUICleaningText(true);
            m_goInstanceCleanUIBtnReward.SetActive(false);
            ShowCleanBtn(false);
            ClearBattleReport();
            m_lblInstanceCleanUIEnemyText.text = "";

            ShowSmallTitle(true, false);
        }
    }

    /// <summary>
    /// 设置标题 = 关卡名称 + 难度
    /// </summary>
    /// <param name="name"></param>
    /// <param name="level"></param>
    public void SetTitle(string name, int level)
    {
        string levelText = "";
        switch ((LevelType)level)
        {
            case LevelType.Simple:
                {
                    levelText = LanguageData.GetContent(46950);// "普通模式"
                } break;
            case LevelType.Difficult:
                {
                    levelText = LanguageData.GetContent(46951);// "精英模式"
                } break;
        }

        m_lblInstanceCleanUITitle.text = name + "  " + levelText;
    }

    /// <summary>
    /// 设置是否扫荡中
    /// </summary>
    /// <param name="isCleaning"></param>
    bool m_isCleaning = false;
    public bool IsCleaning
    {
        get
        {
            return m_isCleaning;
        }
        set
        {
            m_isCleaning = value;

            m_lblInstanceCleanUICleaningText.gameObject.SetActive(m_isCleaning);
            if (m_isCleaning)
            {
                m_lblInstanceCleanUIBtnCleanText.text = LanguageData.GetContent(46952); //"取消扫荡"
                SetCleanUICleaningText(true);
            }
            else
            {
                m_lblInstanceCleanUIBtnCleanText.text = LanguageData.GetContent(46953);//"扫荡"
            }
        }
    }

    void SetCleanUICleaningText(bool isCleaning)
    {
        if (isCleaning)
            m_lblInstanceCleanUICleaningText.text = LanguageData.GetContent(46954); //"扫荡进行中..."
        else
            m_lblInstanceCleanUICleaningText.text = LanguageData.GetContent(46955); //"扫荡完成"
    }

    /// <summary>
    /// 是否显示扫荡按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowCleanBtn(bool isShow)
    {
        m_goInstanceCleanUIBtnClean.SetActive(isShow);
    }

    /// <summary>
    /// 设置怪物小标题
    /// </summary>
    /// <param name="show"></param>
    public void ShowSmallTitle(bool isMonster, bool isShow)
    {
        if (isMonster)
        {
            m_lblInstanceCleanUIEnemyText.text = LanguageData.GetContent(46956); //"副本怪物"
        }
        else
        {
            m_lblInstanceCleanUIEnemyText.text = LanguageData.GetContent(46957); //"获得奖励"
        }
        m_lblInstanceCleanUIEnemyText.gameObject.SetActive(isShow);
    }

    /// <summary>
    /// 清除扫荡信息
    /// </summary>
    public void ClearBattleReport()
    {
        m_tranReportList.gameObject.SetActive(false);
    }

    /// <summary>
    /// 创建怪物扫荡信息
    /// </summary>
    /// <param name="data"></param>
    public void GenerateMonsterReport(SweepMissionRepostData data)
    {
        ShowSmallTitle(true, true);
        ShowCleanBtn(true);

        m_reportData = data;

        if (m_tranReportList == null)
        {
            LoggerHelper.Error("m_tranReportList is null");
        }
        if (m_tranReportList.GetComponentsInChildren<MogoListImproved>(true).Length == 0)
        {
            LoggerHelper.Error("MogoListImproved is null");
        }
        if (m_reportData == null || m_reportData.Enemys == null)
        {
            LoggerHelper.Error("m_reportData or m_reportData.Enemys is null");
        }

        m_tranReportList.GetComponentsInChildren<MogoListImproved>(true)[0].SetGridLayout<InstanceCleanReportGrid>((int)Math.Ceiling((double)m_reportData.Enemys.Count) /*+ 3*/, m_tranReportList.transform, MonsterResourceLoaded);
    }

    // 创建怪物扫荡信息并播放
    void MonsterResourceLoaded()
    {
        var m_reportDataList = m_tranReportList.GetComponentsInChildren<MogoListImproved>(true)[0].DataList;

        int enemyStartIndex = 0;
        int enemyEndIndex = enemyStartIndex + (int)Math.Ceiling((double)m_reportData.Enemys.Count) - 1;

        for (int i = 0; i < m_reportDataList.Count; i++)
        {
            InstanceCleanReportGrid unit = (InstanceCleanReportGrid)m_reportDataList[i];
            if (i >= enemyStartIndex && i <= enemyEndIndex)
            {
                string[] temp = (from x in m_reportData.Enemys select GetMonsterReportText(x.Key, x.Value)).ToArray();
                unit.Head = false;
                if (i == 1 + (int)Math.Ceiling((double)m_reportData.Enemys.Count))
                {
                    string content = String.Join(",", temp, (i - enemyStartIndex), 1);
                    unit.SetContent(content, new Color32(255, 255, 255, 255));
                }
                else
                {
                    string content = String.Join(",", temp, (i - enemyStartIndex), 1);
                    unit.SetContent(content, new Color32(255, 255, 255, 255));
                }
            }
            else
            {
                unit.SetContent(String.Empty, new Color32(255, 255, 255, 255));
                unit.Head = false;
            }
        }
        m_tranReportList.gameObject.SetActive(true);

        if (m_myTransform.gameObject.activeSelf == true)
            StartCoroutine("PlayerAnimation");
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    IEnumerator PlayerAnimation()
    {
        string[] temp = (from x in m_reportData.Enemys select GetMonsterReportText(x.Key, x.Value, true)).ToArray();
        var m_reportDataList = m_tranReportList.GetComponentsInChildren<MogoListImproved>(true)[0].DataList;
        for (int i = 0; i < m_reportData.Enemys.Count; i++)
        {
            if (i < m_reportDataList.Count)
            {
                yield return new WaitForSeconds(1.0f);
                InstanceCleanReportGrid unit = (InstanceCleanReportGrid)m_reportDataList[i];
                string content = temp[i];
                unit.SetContent(content, new Color32(172, 227, 5, 255));
            }
        }

        yield return new WaitForSeconds(1.5f);

        // 真正请求扫荡
        OnCleanRealUp();

        // 显示奖励信息
        GenerateRewardReport();
    }

    /// <summary>
    /// 取消扫荡，停止播放动画
    /// </summary>
    public void StopPlayAnimationOutSide()
    {
        StopCoroutine("PlayerAnimation");
        Show(true);
        ShowCleanBtn(true);
    }

    /// <summary>
    /// 创建获取奖励信息
    /// </summary>
    public void GenerateRewardReport()
    {
        m_goInstanceCleanUIBtnClean.SetActive(false);
        SetCleanUICleaningText(false);

        ShowSmallTitle(false, true);
        m_goInstanceCleanUIBtnReward.SetActive(true);

        int iExtra = 0;
        if (m_reportData.exp > 0)
            iExtra++;
        if (m_reportData.gold > 0)
            iExtra++;
        m_tranReportList.GetComponentsInChildren<MogoListImproved>(true)[0].SetGridLayout<InstanceCleanReportGrid>((int)Math.Ceiling((double)m_reportData.Items.Count) + iExtra, m_tranReportList.transform, RewardResourceLoaded);
    }

    // 创建获取奖励信息
    void RewardResourceLoaded()
    {
        var m_reportDataList = m_tranReportList.GetComponentsInChildren<MogoListImproved>(true)[0].DataList;

        int itemStartIndex = 0;
        int itemEndIndex = itemStartIndex + (int)Math.Ceiling((double)m_reportData.Items.Count) - 1;

        for (int i = 0; i < m_reportDataList.Count; i++)
        {
            InstanceCleanReportGrid unit = (InstanceCleanReportGrid)m_reportDataList[i];
            if (i >= itemStartIndex && i <= itemEndIndex)
            {
                string[] temp = (from x in m_reportData.Items select GetRewardItemReportText(x.Key, x.Value, false)).ToArray();
                unit.Head = false;
                if (i == 1 + (int)Math.Ceiling((double)m_reportData.Items.Count))
                {
                    string content = String.Join(",", temp, (i - itemStartIndex), 1);
                    unit.SetContent(content, new Color32(172, 227, 5, 255));
                }
                else
                {
                    string content = String.Join(",", temp, (i - itemStartIndex), 1);
                    unit.SetContent(content, new Color32(172, 227, 5, 255));
                }
            }
            else if (i == itemEndIndex + 1)
            {
                if (m_reportData.exp > 0)
                {
                    // 经验
                    string content = string.Concat(LanguageData.GetContent(262), "x", m_reportData.exp/*, LanguageData.GetContent(46959)*/);
                    unit.SetContent(content, new Color32(172, 227, 5, 255));
                }
                else if (m_reportData.gold > 0)
                {
                    // 金币
                    string content = string.Concat(LanguageData.GetContent(263), "x", m_reportData.gold/*, LanguageData.GetContent(46959)*/);
                    unit.SetContent(content, new Color32(172, 227, 5, 255));
                }
            }
            else if (i == itemEndIndex + 2)
            {
                // 金币
                string content = string.Concat(LanguageData.GetContent(263), "x", m_reportData.gold, LanguageData.GetContent(46959));
                unit.SetContent(content, new Color32(172, 227, 5, 255));
            }
            else
            {
                unit.SetContent(String.Empty, new Color32(255, 255, 255, 255));
                unit.Head = false;
            }
        }

        m_tranReportList.gameObject.SetActive(true);
    }

    // 获得怪物信息
    string GetMonsterReportText(int enemyId, int count, bool isDead = false)
    {
        string reportText = "";

        if (MonsterData.dataMap.ContainsKey(enemyId))
        {
            int nameCode = MonsterData.GetData(enemyId).nameCode;
            if (LanguageData.dataMap.ContainsKey(nameCode))
                reportText = LanguageData.dataMap[nameCode].content;
            else
                reportText += enemyId.ToString();
        }
        else
        {
            reportText += enemyId.ToString();
        }
        reportText += "x" + count;

        if (isDead)
            reportText += LanguageData.GetContent(46958);//"    (已击杀)";

        return reportText;
    }

    // 获得物品信息
    string GetRewardItemReportText(int itemId, int count, bool isObtain = false)
    {
        string reportText = "";
        if (!string.IsNullOrEmpty((ArenaRewardUIViewManager.GetNameByItemID(itemId))))
        {
            reportText += (ArenaRewardUIViewManager.GetNameByItemID(itemId));
        }
        else
        {
            reportText += itemId.ToString();
        }

        reportText += "x" + count;
        if (isObtain)
            reportText += LanguageData.GetContent(46959);//"    (已获得)";	

        return reportText;
    }
}
