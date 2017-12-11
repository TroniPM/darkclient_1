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
using Mogo.GameData;

public struct UpgradePowerSystemData
{
    public int xmlID;
    public int xmlLevel;
    public string xmlText;
    public string xmlFloatText;
    public string systemName;
    public string iconName;
    public float modulus;
    public int starNum;
    public float scoreProgress;
    public bool hasOpened; // 系统是否开启
}

public class UpgradePowerUIViewManager : MogoUIBehaviour 
{
    private static UpgradePowerUIViewManager m_instance;
    public static UpgradePowerUIViewManager Instance { get { return UpgradePowerUIViewManager.m_instance; } }

    private static int INSTANCE_COUNT = 0; // 异步加载资源数量
    
    private Transform m_tranUpgradePowerUISystemList;
    private Camera m_camUpgradePowerUISystemListCamera;

    private GameObject m_goUpgradePowerUITipEnableBtnBGDown;
    private UILabel m_lblUpgradePowerUITipEnableBtnText;
    private UILabel m_lblUpgradePowerUICurrentPowerNum;

    // 精灵提示
    private GameObject m_goGOUpgradePowerUITipDialog;
    private UILabel m_lblUpgradePowerUITipDialogText;

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_tranUpgradePowerUISystemList = m_myTransform.Find(m_widgetToFullName["UpgradePowerUISystemList"]);
        m_camUpgradePowerUISystemListCamera = m_myTransform.Find(m_widgetToFullName["UpgradePowerUISystemListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camUpgradePowerUISystemListCamera.GetComponent<UIViewport>().sourceCamera = GameObject.Find("Camera").GetComponent<Camera>();

        m_goUpgradePowerUITipEnableBtnBGDown = m_myTransform.Find(m_widgetToFullName["UpgradePowerUITipEnableBtnBGDown"]).gameObject;
        m_lblUpgradePowerUITipEnableBtnText = m_myTransform.Find(m_widgetToFullName["UpgradePowerUITipEnableBtnText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblUpgradePowerUICurrentPowerNum = m_myTransform.Find(m_widgetToFullName["UpgradePowerUICurrentPowerNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_myTransform.Find(m_widgetToFullName["UpgradePowerUIBtnClose"]).gameObject.AddComponent<UpgradePowerUIButton>();
        m_myTransform.Find(m_widgetToFullName["UpgradePowerUITipEnableBtn"]).gameObject.AddComponent<UpgradePowerUIButton>();

        m_goGOUpgradePowerUITipDialog = FindTransform("GOUpgradePowerUITipDialog").gameObject;
        m_lblUpgradePowerUITipDialogText = FindTransform("UpgradePowerUITipDialogText").GetComponentsInChildren<UILabel>(true)[0];

        IsShowUpgradePowerTipDialog = SystemConfig.Instance.IsShowUpgradePowerTipDialog;

        SetUpgradePowerGridList(() =>
            {
                SetUpgradePowerGridListDetail(MogoWorld.thePlayer.level);
            });

        Initialize();
    }

    #region 事件

    public Action UPGRADEPOWERUICLOSEUP;
    public Action UPGRADEPOWERUITIPENABLEUP;
    

    public void Initialize()
    {
        UpgradePowerUIDict.ButtonTypeToEventUp.Add("UpgradePowerUIBtnClose", OnCloseUp);
        UpgradePowerUIDict.ButtonTypeToEventUp.Add("UpgradePowerUITipEnableBtn", OnTipEnableUp);

        UpgradePowerUILogicManager.Instance.Initialize();
        m_uiLoginManager = UpgradePowerUILogicManager.Instance;
    }

    public void Release()
    {
        UpgradePowerUILogicManager.Instance.Release();
        UpgradePowerUIDict.ButtonTypeToEventUp.Clear();
    }

    void OnCloseUp(int i)
    {
        if (UPGRADEPOWERUICLOSEUP != null)
            UPGRADEPOWERUICLOSEUP();
    }

    void OnTipEnableUp(int i)
    {
        if (UPGRADEPOWERUITIPENABLEUP != null)
            UPGRADEPOWERUITIPENABLEUP();
    }

    #endregion

    #region 创建提升系统Gird

    readonly static float GRIDSPACEHORIZON = 370;
    readonly static float GRIDSPACEVERTICAL = -150;
    readonly static int OffsetX = -205;
    readonly static int GRID_COUNT_ONE_PAGE = 6;// 每页显示的Grid数量
    readonly static int GRID_COUNT_ONE_LINE = 2;// 每行显示的Grid数量
    private List<UpgradePowerUISystemGrid> m_listUpgradePowerGridGO = new List<UpgradePowerUISystemGrid>();

    /// <summary>
    /// 设置提升系统Grid
    /// </summary>
    /// <param name="idList"></param>
    public void SetUpgradePowerGridList(Action action)
    {
        if (m_listUpgradePowerGridGO.Count == UpgradePowerData.dataMap.Count)
        {
            if (action != null)
                action();

            return;
        }

        LoadUpgradePowerGridList(UpgradePowerData.dataMap.Count, () =>
        {
            if (action != null)
                action();
        });
    }

    /// <summary>
    /// 创建提升系统Grid
    /// </summary>
    /// <param name="num"></param>
    /// <param name="act"></param>
    private bool IsLoadUpgradePowerGridList = false;
    private void LoadUpgradePowerGridList(int num, Action act = null)
    {
        if (IsLoadUpgradePowerGridList)
            return;

        IsLoadUpgradePowerGridList = true;
        for (int i = 0; i < num; ++i)
        {
            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);

            int index = i;            
            AssetCacheMgr.GetUIInstance("UpgradePowerUISystemGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_tranUpgradePowerUISystemList;

                if (index % GRID_COUNT_ONE_LINE == 0)
                    obj.transform.localPosition = new Vector3(0, GRIDSPACEVERTICAL * (index / GRID_COUNT_ONE_LINE) + OffsetX, 0);
                else
                    obj.transform.localPosition = new Vector3(GRIDSPACEHORIZON, GRIDSPACEVERTICAL * (index / GRID_COUNT_ONE_LINE) + OffsetX, 0);

                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                UpgradePowerUISystemGrid grid = obj.AddComponent<UpgradePowerUISystemGrid>();
                m_listUpgradePowerGridGO.Add(grid);

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);


                // 数量大于一页时,创建拖动效果
                if (num > GRID_COUNT_ONE_PAGE)
                {
                    MyDragCamera myDragCamera = obj.AddComponent<MyDragCamera>();
                    myDragCamera.RelatedCamera = m_camUpgradePowerUISystemListCamera;
                    m_camUpgradePowerUISystemListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].MAXX =
                        OffsetX + (m_listUpgradePowerGridGO.Count - GRID_COUNT_ONE_PAGE) / GRID_COUNT_ONE_LINE * GRIDSPACEVERTICAL;
             
                    // 创建翻页位置
                    if (index % GRID_COUNT_ONE_PAGE == 0)
                    {
                        GameObject trans = new GameObject();
                        trans.transform.parent = m_camUpgradePowerUISystemListCamera.transform;
                        trans.transform.localPosition = new Vector3(index / GRID_COUNT_ONE_LINE * GRIDSPACEVERTICAL, 0, 0);
                        trans.transform.localEulerAngles = Vector3.zero;
                        trans.transform.localScale = new Vector3(1, 1, 1);
                        trans.name = "DragListPosHorizon" + index / GRID_COUNT_ONE_PAGE;
                        m_camUpgradePowerUISystemListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList.Add(trans.transform);
                    }
                }

                if (index == num - 1)
                {
                    if (act != null)
                    {
                        act();
                    }
                }
            });
        }
    }
   
    /// <summary>
    /// 设置提升系统详细信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="imgName"></param>
    /// <param name="itemName"></param>
    public void SetUpgradePowerGridListDetail(byte playerLevel)
    {
        if (UpgradePowerData.GetUpgradePowerIDList().Count != m_listUpgradePowerGridGO.Count)
            return;

        CalUpgradePowerGridList(playerLevel);
        int recommendSystemID = CalRecommendSystemID();

        // 显示精灵提示
        UpgradePowerData xmlData = UpgradePowerData.GetUpgradePowerDataByID(recommendSystemID);
        SetSpriteDialogInfo(LanguageData.GetContent(xmlData.name));
        ShowSpriteDialog = true;

        for (int index = 0; index < m_listUpgradePowerSystemData.Count; index++)
        {
            UpgradePowerUISystemGrid upgradePowerUISystemGrid = m_listUpgradePowerGridGO[index];    
            UpgradePowerSystemData gridData = m_listUpgradePowerSystemData[index];
            upgradePowerUISystemGrid.XMLID = gridData.xmlID;
            upgradePowerUISystemGrid.SystemName = gridData.systemName;
            upgradePowerUISystemGrid.SystemIconName = gridData.iconName;                 
          
            if (gridData.hasOpened)
                upgradePowerUISystemGrid.SetSystemHasOpen(gridData.starNum, gridData.scoreProgress);
            else
                upgradePowerUISystemGrid.SetSystemNoOpen(gridData.xmlText, gridData.xmlFloatText);

            // 是否为推荐系统
            if (gridData.xmlID == recommendSystemID)
                upgradePowerUISystemGrid.SetSystemIsRecommend(true);
            else
                upgradePowerUISystemGrid.SetSystemIsRecommend(false);           
        }

        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
    } 

    #endregion   

    #region 数据缓存

    private List<UpgradePowerSystemData> m_listUpgradePowerSystemData = new List<UpgradePowerSystemData>();

    /// <summary>
    /// 计算提升系统数据
    /// </summary>
    /// <param name="playerLevel"></param>
    private void CalUpgradePowerGridList(byte playerLevel)
    {
        m_listUpgradePowerSystemData.Clear();

        List<int> upgradePowerIDList = UpgradePowerData.GetUpgradePowerIDList();
        for (int index = 0; index < upgradePowerIDList.Count; index++)
        {
            UpgradePowerData xmlData = UpgradePowerData.GetUpgradePowerDataByID(upgradePowerIDList[index]);
            if (xmlData != null)
            {
                UpgradePowerSystemData upgradePowerSystemData;

                // 基本信息
                upgradePowerSystemData.systemName = LanguageData.GetContent(xmlData.name);
                upgradePowerSystemData.iconName = IconData.GetIconPath(xmlData.icon);
                upgradePowerSystemData.xmlID = xmlData.id;
                upgradePowerSystemData.xmlLevel = xmlData.level;
                upgradePowerSystemData.xmlText = LanguageData.GetContent(xmlData.text);
                upgradePowerSystemData.xmlFloatText = LanguageData.GetContent(xmlData.floatText);

                // 评分
                upgradePowerSystemData.modulus = 0;
                upgradePowerSystemData.starNum = 0;
                upgradePowerSystemData.scoreProgress = 0;
                CalStarNumAndScoreProgress(xmlData.id, 
                    ref upgradePowerSystemData.starNum, 
                    ref upgradePowerSystemData.scoreProgress, 
                    ref upgradePowerSystemData.modulus);

                // <=200表示开启等级
                // >900表示特殊条件; 901表示开启公会后开启
                if (xmlData.level <= 200)
                {
                    if (MogoWorld.thePlayer.level >= xmlData.level)
                        upgradePowerSystemData.hasOpened = true;
                    else
                        upgradePowerSystemData.hasOpened = false;
                }
                else if (xmlData.level == 901)
                {
                    if (MogoUIManager.ISTONGUIOPENED)
                        upgradePowerSystemData.hasOpened = true;
                    else
                        upgradePowerSystemData.hasOpened = false;
                }
                else
                {
                    upgradePowerSystemData.hasOpened = false;
                }

                m_listUpgradePowerSystemData.Add(upgradePowerSystemData);
            }
        }       
    }

    /// <summary>
    /// 计算出推荐的系统ID
    /// </summary>
    /// <returns></returns>
    private int CalRecommendSystemID()
    {
        int recommendIndex = 0;
        for (int i = 0; i < m_listUpgradePowerSystemData.Count; i++)
        {
            if(m_listUpgradePowerSystemData[recommendIndex].modulus > m_listUpgradePowerSystemData[i].modulus
                && m_listUpgradePowerSystemData[i].hasOpened)
                recommendIndex = i;
        }

        if (recommendIndex < m_listUpgradePowerSystemData.Count && m_listUpgradePowerSystemData[recommendIndex].hasOpened)
            return m_listUpgradePowerSystemData[recommendIndex].xmlID;
        return 0;
    }

    #endregion

    #region 精灵提示

    private void SetSpriteDialogInfo(string info)
    {
        m_lblUpgradePowerUITipDialogText.text = string.Format(LanguageData.GetContent(3108), info);
    }

    private bool m_showSpriteDialog = false;
    private bool ShowSpriteDialog
    {
        get{return m_showSpriteDialog;}
        set
        {
            m_showSpriteDialog = value;
            m_goGOUpgradePowerUITipDialog.SetActive(m_showSpriteDialog);

            if (m_showSpriteDialog)
                m_fCurrentTime = 0;
        }
    }

    const float MAXSHOWTIME = 7.0f;
    private float m_fCurrentTime = 0f;
    void Update()
    {
        if (ShowSpriteDialog)
        {
            m_fCurrentTime += Time.deltaTime;
            if (m_fCurrentTime >= MAXSHOWTIME)
            {
                ShowSpriteDialog = false;
                m_fCurrentTime = 0.0f;
            }
        }      
    }

    #endregion

    #region CheckTip

    /// <summary>
    /// 是否Check
    /// </summary>
    bool m_IsShowUpgradePowerTipDialog;
    public bool IsShowUpgradePowerTipDialog
    {
        get
        {
            return m_IsShowUpgradePowerTipDialog;
        }
        set
        {
            m_IsShowUpgradePowerTipDialog = value;
            if (!m_IsShowUpgradePowerTipDialog)
            {
                m_IsShowUpgradePowerTipDialog = value;

                // 判断是否需要重置(每天第一次登陆重置为true)  
                if (MogoTime.Instance.GetCurrentDateTime().Day != Mogo.Util.SystemConfig.Instance.UpgradePowerTipDialogDisableDay)
                {
                    m_IsShowUpgradePowerTipDialog = true;
                    SaveIsShowUpgradePowerTipDialog(true);
                }
            }

            OnUpgradePowerTipClick();
        }
    }

    /// <summary>
    /// 计算是否弹出界面
    /// </summary>
    /// <returns></returns>
    static public bool CalIsShowUpgradePowerTipDialog()
    {
        if (MogoTime.Instance.GetCurrentDateTime().Day != Mogo.Util.SystemConfig.Instance.UpgradePowerTipDialogDisableDay)
        {
            SaveIsShowUpgradePowerTipDialog(true);
        }

        return Mogo.Util.SystemConfig.Instance.IsShowGoldMetallurgyTipDialog;
    }

    static public void SaveIsShowUpgradePowerTipDialog(bool isShow)
    {
        Mogo.Util.SystemConfig.Instance.IsShowGoldMetallurgyTipDialog = isShow;
        Mogo.Util.SystemConfig.Instance.UpgradePowerTipDialogDisableDay = MogoTime.Instance.GetCurrentDateTime().Day;
        Mogo.Util.SystemConfig.SaveConfig();
    }

    /// <summary>
    /// 设置是否以后不提示
    /// </summary>
    void OnUpgradePowerTipClick()
    {
        if (IsShowUpgradePowerTipDialog)
            m_goUpgradePowerUITipEnableBtnBGDown.SetActive(false);
        else
            m_goUpgradePowerUITipEnableBtnBGDown.SetActive(true);
    }

    #endregion

    #region 界面信息

    public void SetPlayerCurrentPower(uint power)
    {
        if (m_lblUpgradePowerUICurrentPowerNum != null)
            m_lblUpgradePowerUICurrentPowerNum.text = power.ToString();
    }

    #endregion

    #region 成长系统得分计算方法

    /// <summary>
    /// 计算星数和装备得分系数
    /// </summary>
    /// <param name="starNum"></param>
    /// <param name="scoreProgress"></param>
    private void CalStarNumAndScoreProgress(int UpgradePowerXMLID, ref int starNum, ref float scoreProgress, ref float modulus)
    {
        int currentScore = 0;
        int topScore = 0;
        switch ((UpgradePowerSystem)UpgradePowerXMLID)
        {
            case UpgradePowerSystem.Equipment:
                currentScore = InventoryManager.Instance.CalPlayerEquipmentScore(); // 计算玩家身上装备得分
                topScore = PowerScoreData.GetScoreEquipByLevel(MogoWorld.thePlayer.level);
                break;
            case UpgradePowerSystem.JewelInset:
                currentScore = InventoryManager.Instance.CalPlayerJewelScore(); // 计算宝石得分
                topScore = PowerScoreData.GetScoreDiamondByLevel(MogoWorld.thePlayer.level);
                break;
            case UpgradePowerSystem.Rune:
                currentScore = RuneManager.Instance.CalcuScore(); // 计算符文得分
                topScore = PowerScoreData.GetScoreRuneByLevel(MogoWorld.thePlayer.level);
                break;
            case UpgradePowerSystem.BodyEnhance:
                currentScore = InventoryManager.Instance.CalBodyEnhanceScore(); // 计算装备强化得分
                topScore = PowerScoreData.GetScoreBodyEnhanceByLevel(MogoWorld.thePlayer.level);
                break;
            case UpgradePowerSystem.Tong:
                currentScore = CalTongScore();
                topScore = PowerScoreData.GetScoreTongByLevel(MogoWorld.thePlayer.level);
                break;
            default:
                currentScore = 0;
                break;
        }

        // 系数X=当前分数/满分；满分读取配置表，当前分数根据玩家身上算出
        if (topScore > 0)
            modulus = (float)currentScore / (float)topScore;

        starNum = PowerScoreStarData.CalStarNumByModulus(modulus, UpgradePowerXMLID); // 计算星数
        scoreProgress = PowerScoreStarData.CalScoreProgressByModulus(modulus, UpgradePowerXMLID); // 计算得分进度
    }

    /// <summary>
    /// 计算公会得分
    /// </summary>
    /// <returns></returns>
    private int CalTongScore()
    {
        int currentScore = 0;

        return currentScore;
    }

    #endregion

    #region 界面打开和关闭

    protected override void OnEnable()
    {
 	    base.OnEnable();
        MogoWorld.thePlayer.HasDeadBefore = false; // 是否在副本中死亡

        SetUpgradePowerGridList(() =>
            {
                SetUpgradePowerGridListDetail(MogoWorld.thePlayer.level); 
            });        
    }

    void OnDisable()
    {
        ShowSpriteDialog = false;

        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroyUpgradePowerUI();
        }
    }

    #endregion
}
