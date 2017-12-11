using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;

public class FriendGridData
{
    public int Id;
    public string name;
    public string headImg;
    public string bossName;
    public string bossLevel;
    public string bossHP;
    public string battleInfo;
    public bool isKill = false; //��true = ʤ�����Լ�����ɱ��Boss��Boss���Լ�ɱ��
    public bool isHelp = false; //��true = ʤ�����Լ�����ɱ��Boss��Boss�������Լ�ɱ��
    public bool isWin = false; //��true = ʤ����
    public bool isLose = false; //��true = ʧ�ܡ�
    public int hour;
    public int minus;
}

public class DoorOfBuryUIViewManager : MogoUIBehaviour
{
    private static DoorOfBuryUIViewManager m_instance;
    public static DoorOfBuryUIViewManager Instance { get { return DoorOfBuryUIViewManager.m_instance; } }   

    private const float GRIDHEIGHT = 124;
    public static Dictionary<string, Action<int>> ButtonTypeToEventUp = new Dictionary<string, Action<int>>();
    private List<DoorOfBuryUIFriendGrid> m_listFriendGrid = new List<DoorOfBuryUIFriendGrid>();
    private Transform m_trans0;
    private Transform m_trans1;
    private Transform m_friendGridList;
    private MyDragableCamera m_friendGridDCam;

    private UILabel m_lblCountDownTime;
    private int m_iCountDownTime = 60;
    private uint m_uiTimerId;
    
    private UILabel m_lblTitle;

    private GameObject m_goGODoorOfBuryChooseHasGrid;
    private GameObject m_goGODoorOfBuryChooseNoGrid;

    void Awake()
    {
        m_myTransform = transform;
        m_instance = m_myTransform.GetComponentsInChildren<DoorOfBuryUIViewManager>(true)[0];
        FillFullNameData(m_myTransform);        

        m_friendGridList = FindTransform("DoorOfBuryChooseFriendList");
        m_friendGridDCam = FindTransform("DoorOfBuryChooseFriendListCamera").GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_lblCountDownTime = FindTransform("DoorOfBuryChooseCountDown").GetComponentsInChildren<UILabel>(true)[0];
        m_lblCountDownTime.color = SystemUIColorManager.RED;
        m_lblTitle = FindTransform("DoorOfBuryChooseTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_goGODoorOfBuryChooseHasGrid = FindTransform("GODoorOfBuryChooseHasGrid").gameObject;
        m_goGODoorOfBuryChooseNoGrid = FindTransform("GODoorOfBuryChooseNoGrid").gameObject;

        Camera camera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_friendGridDCam.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = camera;
        FindTransform("TopRight").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;

        // ChineseData
        FindTransform("DoorOfBuryChooseNoGridText").GetComponentsInChildren<UILabel>(true)[0].text = LanguageData.GetContent(47112);

        Initialize();
    }

    #region �¼�

    public Action<int> FRIENDGRIDUP;

    void Initialize()
    {
        DoorOfBuryUILogicManager.Instance.Initialize();

        EventDispatcher.AddEventListener<int>(DoorOfBuryUILogicManager.DoorOfBuryUIEvent.FRIENDGRIDUP, OnFriendGridUp);

        m_myTransform.Find(m_widgetToFullName["DoorOfBuryChooseEnterBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnEnterBtnUp;
        m_myTransform.Find(m_widgetToFullName["DoorOfBuryUICloseBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCloseBtnUp;
    }

    public void Release()
    {
        DoorOfBuryUILogicManager.Instance.Release();
        EventDispatcher.RemoveEventListener<int>(DoorOfBuryUILogicManager.DoorOfBuryUIEvent.FRIENDGRIDUP, OnFriendGridUp);

        m_myTransform.Find(m_widgetToFullName["DoorOfBuryChooseEnterBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnEnterBtnUp;
        m_myTransform.Find(m_widgetToFullName["DoorOfBuryUICloseBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnCloseBtnUp;
        ButtonTypeToEventUp.Clear();
        m_listFriendGrid.Clear();
    }

    void OnFriendGridUp(int id)
    {
        LoggerHelper.Debug(id);

        if (FRIENDGRIDUP != null)
            FRIENDGRIDUP(id);
    }

    void OnEnterBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("EnterBtnUp");
        EventDispatcher.TriggerEvent(DoorOfBurySystem.ON_ENTER_DOOR);
    }

    void OnCloseBtnUp()
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    #endregion   

    #region CD

    /// <summary>
    /// ��ʼCD����
    /// </summary>
    /// <param name="minus"></param>
    public void BeginCountDown(int minus)
    {
        SetDoorTitleColor(true);
        m_lblCountDownTime.text = "00 : " + minus;
        m_iCountDownTime = minus;

        m_uiTimerId = TimerHeap.AddTimer(0, 1000 * 60, Callback);
    }

    /// <summary>
    /// CD����������
    /// </summary>
    void Callback()
    {
        if (--m_iCountDownTime <= 0)
        {
            SetDoorTitleColor(false);
            TimerHeap.DelTimer(m_uiTimerId);
            m_lblCountDownTime.text = "00 : 00";
        }
        else
        {
            m_lblCountDownTime.text = "00 : " + m_iCountDownTime;
        }
    }

    /// <summary>
    /// CD����
    /// </summary>
    public void ClearDoorCD()
    {
        m_lblCountDownTime.text = "";
    }

    #endregion

    #region ������Ϣ

    public void SetDoorTitle(string title)
    {
        SetDoorTitleColor(false);
        m_lblTitle.text = title;
    }

    public void SetDoorTitleColor(bool inCD)
    {
        if(m_lblTitle != null)
        {
            if (inCD)
            {
                m_lblTitle.color = SystemUIColorManager.RED;
            }
            else
            {
                m_lblTitle.color = SystemUIColorManager.UITITLE;
            }
        }
    }

    /// <summary>
    /// ��������֮������������Ϊ0ʱ��ʾTip
    /// </summary>
    /// <param name="num"></param>
    public void SetGridNum(int num)
    {
        if (num > 0)
        {
            m_goGODoorOfBuryChooseHasGrid.SetActive(true);
            m_goGODoorOfBuryChooseNoGrid.SetActive(false);
        }
        else
        {
            m_goGODoorOfBuryChooseHasGrid.SetActive(false);
            m_goGODoorOfBuryChooseNoGrid.SetActive(true);
        }
    }

    /// <summary>
    /// �������֮�ź���Grid
    /// </summary>
    /// <param name="data"></param>
    public void AddFriendListGrid(FriendGridData data)
    {
        AssetCacheMgr.GetUIInstance("DoorOfBuryChooseFriendGrid.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;
            obj.transform.parent = m_friendGridList;
            obj.transform.localPosition = new Vector3(0, -m_listFriendGrid.Count * GRIDHEIGHT, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            DoorOfBuryUIFriendGrid fg = obj.AddComponent<DoorOfBuryUIFriendGrid>();
            fg.LoadResourceInsteadOfAwake();
            fg.SetStatus(data.isLose, data.isWin, data.isKill, data.isHelp);
            fg.FriendName = data.name;
            fg.FriendHeadImg = data.headImg;
            fg.BossName = data.bossName;
            fg.BossLevel = data.bossLevel;
            fg.BossHp = data.bossHP;
            fg.BattleInfo = data.battleInfo;
            //fg.BattleTime = data.battleTime;
            fg.Id = data.Id;
            fg.hour = data.hour;
            fg.minus = data.minus;
           
            m_listFriendGrid.Add(fg);
            obj.GetComponentsInChildren<MogoSingleButton>(true)[0].ButtonListTransform = m_friendGridList;
            m_friendGridList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Add(obj.GetComponentsInChildren<MogoSingleButton>(true)[0]);
            obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_friendGridDCam.transform.GetComponentsInChildren<Camera>(true)[0];

            if (m_listFriendGrid.Count < 4)
            {
                m_friendGridDCam.MINY = -184;
            }
            else
            {
                m_friendGridDCam.MINY = -184 - GRIDHEIGHT * (m_listFriendGrid.Count - 4);
            }

            fg.BeginCountDown();
        });
    }

    /// <summary>
    /// �������֮�ź���Grid
    /// </summary>
    public void ClearFriendList()
    {
        for (int i = 0; i < m_listFriendGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listFriendGrid[i].gameObject);
            Mogo.Util.LoggerHelper.Debug("release m_listFriendGrid[i].gameObject:" + m_listFriendGrid[i].transform.parent.gameObject.name + ",i:" + i);
        }

        m_friendGridList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Clear();
        m_listFriendGrid.Clear();
    }

    #endregion
}
