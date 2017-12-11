using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using System;
using Mogo.GameData;

public enum EquipmentUITab
{
    NoneTab = -1,
    StrenthTab = 0,
    InsetTab = 1,
    ComposeTab = 2,
    DecomposeTab = 3,
}

public class EquipmentUIViewManager : MogoUIBehaviour
{
    private static EquipmentUIViewManager m_instance;
    public static EquipmentUIViewManager Instance { get { return EquipmentUIViewManager.m_instance; } }

    private UISprite m_spEquipmentUIMogoUIRefreshCtrl; // ����ˢ��MogoUIͼ��
    private Transform m_tranEquipmentUIClose; // �رհ�ť

    void Awake()
    {        
        m_instance = this;
        m_myTransform = transform;

        Initialize();

        m_spEquipmentUIMogoUIRefreshCtrl = m_myTransform.Find("EquipmentUIMogoUIRefreshCtrl").GetComponentsInChildren<UISprite>(true)[0];
        m_tranEquipmentUIClose = m_myTransform.Find("EquipmentUIClose");

        m_tabUpLabelList.Clear();
        m_tabUpLabelList.Add(m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/StrenthenIcon/StrenthenIconTextUp").GetComponent<UILabel>());
        m_tabUpLabelList.Add(m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/InsetIcon/InsetIconTextUp").GetComponent<UILabel>());
        m_tabUpLabelList.Add(m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/ComposeIcon/ComposeIconTextUp").GetComponent<UILabel>());
        m_tabUpLabelList.Add(m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/DecomposeIcon/DecomposeIconTextUp").GetComponent<UILabel>());

        m_tabDownLabelList.Clear();
        m_tabDownLabelList.Add(m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/StrenthenIcon/StrenthenIconTextDown").GetComponent<UILabel>());
        m_tabDownLabelList.Add(m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/InsetIcon/InsetIconTextDown").GetComponent<UILabel>());
        m_tabDownLabelList.Add(m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/ComposeIcon/ComposeIconTextDown").GetComponent<UILabel>());
        m_tabDownLabelList.Add(m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/DecomposeIcon/DecomposeIconTextDown").GetComponent<UILabel>());

        SettingTabLabel();

        // ChineseData
        m_myTransform.Find("EquipmentUITitle").GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(46852);
        m_tabUpLabelList[0].text = LanguageData.GetContent(48010);        
        m_tabUpLabelList[1].text = LanguageData.GetContent(48011);
        m_tabUpLabelList[2].text = LanguageData.GetContent(48012);
        m_tabUpLabelList[3].text = LanguageData.GetContent(48013);
        m_tabDownLabelList[0].text = LanguageData.GetContent(48010);
        m_tabDownLabelList[1].text = LanguageData.GetContent(48011);
        m_tabDownLabelList[2].text = LanguageData.GetContent(48012);
        m_tabDownLabelList[3].text = LanguageData.GetContent(48013);
    }

    #region �¼�

    public Action INSETICONUP;
    public Action DECOMPOSEICONUP;
    public Action COMPOSEICONUP;
    public Action STRENTHICONUP;
    public Action EQUIPMENTUICLOSEUP; 

    void Initialize()
    {
        m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/InsetIcon").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnInsetIconUp;
        m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/DecomposeIcon").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnDecomposeIconUp;
        m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/ComposeIcon").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnComposeIconUp;
        m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/StrenthenIcon").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnStrenthIconUp;
        m_myTransform.Find("EquipmentUIClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnEquipmentUICloseUp;

        EquipmentUILogicManager.Instance.Initialize();  
    }

    public void Release()
    {
        EquipmentUILogicManager.Instance.Release();

        m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/InsetIcon").GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnInsetIconUp;
        m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/DecomposeIcon").GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnDecomposeIconUp;
        m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/ComposeIcon").GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnComposeIconUp;
        m_myTransform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIIconList/StrenthenIcon").GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnStrenthIconUp;
        m_myTransform.Find("EquipmentUIClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnEquipmentUICloseUp;
    }

    void OnInsetIconUp()
    {
        if (INSETICONUP != null && CurrentDownTab != (int)EquipmentUITab.InsetTab)
        {
            INSETICONUP();
        }
    }

    void OnDecomposeIconUp()
    {
        if (DECOMPOSEICONUP != null && CurrentDownTab != (int)EquipmentUITab.DecomposeTab)
        {
            DECOMPOSEICONUP();
        }
    }

    void OnComposeIconUp()
    {
        if (COMPOSEICONUP != null && CurrentDownTab != (int)EquipmentUITab.ComposeTab)
        {
            COMPOSEICONUP();
        }
    }

    void OnStrenthIconUp()
    {
        if (STRENTHICONUP != null && CurrentDownTab != (int)EquipmentUITab.StrenthTab)
        {
            STRENTHICONUP();
        }
    }

    void OnEquipmentUICloseUp()
    {
        if (EQUIPMENTUICLOSEUP != null)
        {
            EQUIPMENTUICLOSEUP();
        }

        MogoUIManager.Instance.ShowMogoNormalMainUI();
        Mogo.Util.LoggerHelper.Debug("OnEquipmentUICloseUp");
        InventoryManager.Instance.CurrentView = InventoryManager.View.None;
    }

    #endregion

    #region  Tab������ɫ

    private List<UILabel> m_tabUpLabelList = new List<UILabel>();
    private List<UILabel> m_tabDownLabelList = new List<UILabel>();
    private int m_iCurrentDownTab = (int)EquipmentUITab.NoneTab;
    public int CurrentDownTab
    {
        get { return m_iCurrentDownTab; }
        set
        {
            m_iCurrentDownTab = value;
        }
    }

    /// <summary>
    /// ����Tab������ɫ
    /// </summary>
    private void SettingTabLabel()
    {
        for (int i = 0; i < m_tabUpLabelList.Count; i++)
        {
            m_tabUpLabelList[i].color = new Color32(37, 29, 6, 255);
            m_tabUpLabelList[i].effectStyle = UILabel.Effect.None;
        }

        for (int i = 0; i < m_tabDownLabelList.Count; i++)
        {
            m_tabDownLabelList[i].color = new Color32(255, 255, 255, 255);
            m_tabDownLabelList[i].effectStyle = UILabel.Effect.Outline;
            m_tabDownLabelList[i].effectColor = new Color32(53, 22, 2, 255);
        }         
    }
   
    #endregion    

    #region ������Ϣ

    /// <summary>
    /// ���ùرհ�ť��Z����,ǿ��ʱ�����ɰ嵫���Թر�
    /// </summary>
    /// <param name="isTop"></param>
    public void SetEquipmentUICloseValueZ(bool isTop = false)
    {
        if (m_tranEquipmentUIClose != null)
        {
            if (isTop)
            {
                m_tranEquipmentUIClose.gameObject.layer = 14; // IgnoreCollision
                m_tranEquipmentUIClose.localPosition = new Vector3(m_tranEquipmentUIClose.localPosition.x, m_tranEquipmentUIClose.localPosition.y, -1);
            }
            else
            {
                m_tranEquipmentUIClose.gameObject.layer = 10; // UI
                m_tranEquipmentUIClose.localPosition = new Vector3(m_tranEquipmentUIClose.localPosition.x, m_tranEquipmentUIClose.localPosition.y, -1);
            }
        }     
    }    

    #endregion

    #region  ����򿪺͹ر�

    public bool IsCanClick = false;
    protected override void OnEnable()
    {
        base.OnEnable();

        if (m_spEquipmentUIMogoUIRefreshCtrl != null)
            m_spEquipmentUIMogoUIRefreshCtrl.RefreshAllShowAs();

        SetEquipmentUICloseValueZ(false);

        if (IsCanClick == false)
        {
            return; 
        }
    }

    #endregion
}
