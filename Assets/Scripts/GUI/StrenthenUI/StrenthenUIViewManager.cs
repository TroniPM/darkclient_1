using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.GameData;

public class StrenthenUIViewManager : MogoUIBehaviour
{
    private static StrenthenUIViewManager m_instance;
    public static StrenthenUIViewManager Instance { get{ return StrenthenUIViewManager.m_instance; }}

    //private static int INSTANCE_COUNT = 0; // �첽������Դ����

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();

    //private GameObject m_goStrenthenDialogStarRewardStarLevel;
    //private GameObject m_goStarLevelListPosBegin;
    //private StrenthenStarLevelInfo m_StrenthenStarLevelInfo;

    private List<GameObject> m_listEquipmentGrid = new List<GameObject>();
    private UILabel m_lblBaseAttribute;
    private UILabel m_lblAfterAttribute;
    private UILabel m_lblEquipmentLevel;
    private UISlicedSprite m_ssEquipmentImage;

    // ǿ��ǰ��ֵ
    private UILabel m_lblBaseReward0Title;
    private UILabel m_lblBaseReward1Title;
    private UILabel m_lblBaseReward2Title;
    private UILabel m_lblBaseReward0Num;
    private UILabel m_lblBaseReward1Num;
    private UILabel m_lblBaseReward2Num;
    private UILabel m_lblBaseReward0TextAnimation;
    private UILabel m_lblBaseReward1TextAnimation;
    private UILabel m_lblBaseReward2TextAnimation;

    // ǿ������ֵ
    private GameObject m_goLevelRewardAfter;
    private UILabel m_lblAfterReward0;
    private UILabel m_lblAfterReward1;
    private UILabel m_lblAfterReward2;

    // ǿ����ť
    private GameObject m_goStrenthenDialogStrenth;
    private UISprite m_spStrenthenDialogStrenthBGUp;

    private UILabel m_lblNeedGold;    
    private UILabel m_lblBaseEquipType;
    private UILabel m_lblStarLevel;
    private UILabel m_lblCurrentGold;
    private UILabel m_lblNeedLevel;  

    // ǿ����Ҫ�Ĳ���
    private UILabel m_lblNeedMaterial1;
    private UILabel m_lblNeedMaterial2;
    private UISprite m_spNeedMaterialIcon1;
    private UISprite m_spNeedMaterialIcon2;
    private GameObject m_goNeedMaterial2;

    // ǿ��������ʾ��ť
    private GameObject m_goGOMaterialObtainTip;
    private GameObject m_goStrenthenDialogMaterialTip;
    private UISprite m_spStrenthenDialogMaterialTipBGUp;
    private UISprite m_spStrenthenDialogMaterialTipBGDown;

    private BoxCollider m_bcStrenthUIBoxCollider;
    private Transform m_transStrenthenDialogIconList;
    private Camera m_dragCamera;
    private MyDragableCamera m_dragableCamera;

    private GameObject[] m_arrStrenthLevelBG = new GameObject[10];
    private UIFilledSprite[] m_fsEquipmentExpList = new UIFilledSprite[10];

    private const int ICONGRIDNUM = 10; // �ܹ�10������
    private const int ICON_GRID_ONE_PAGE = 6; // һҳ6������
    private const float ICONGRIDSPACE = -0.074f;
    private const float ICON_OFFSET_Y = -0.185f;
    private UILabel m_lblCost;

    #region ��ʼ��

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        Initialize();

        m_transStrenthenDialogIconList = m_myTransform.Find(m_widgetToFullName["StrenthenDialogIconList"]);

        m_dragCamera = m_myTransform.Find(m_widgetToFullName["StrenthenDialogIconListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_dragCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_dragCamera.GetComponentsInChildren<UIViewport>(true)[0].topLeft = GameObject.Find("EquipmentUIIconListBGTopLeft").transform;
        m_dragCamera.GetComponentsInChildren<UIViewport>(true)[0].bottomRight = GameObject.Find("EquipmentUIIconListBGBottomRight").transform;

        m_dragableCamera = m_dragCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_dragableCamera.LeftArrow = FindTransform("StrenthenDialogIconListArrowU").gameObject;
        m_dragableCamera.RightArrow = FindTransform("StrenthenDialogIconListArrowD").gameObject;

        m_bcStrenthUIBoxCollider = FindTransform("StrenthUIBoxCollider").GetComponentsInChildren<BoxCollider>(true)[0];

        //m_goStrenthenDialogStarRewardStarLevel = m_myTransform.FindChild(m_widgetToFullName["StrenthenDialogStarRewardStarLevel"]).gameObject;
        //m_goStarLevelListPosBegin = m_myTransform.FindChild(m_widgetToFullName["StarLevelListPosBegin"]).gameObject;
        //m_StrenthenStarLevelInfo = m_goStrenthenDialogStarRewardStarLevel.AddComponent<StrenthenStarLevelInfo>();
        //if (m_StrenthenStarLevelInfo != null)
        //    m_StrenthenStarLevelInfo.CreateStarLevelInfo(m_goStrenthenDialogStarRewardStarLevel.transform, m_goStarLevelListPosBegin.transform.localPosition);

        m_lblBaseEquipType = m_myTransform.Find(m_widgetToFullName["BaseAttributeEquipTypeText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblStarLevel = m_myTransform.Find(m_widgetToFullName["StarLevelText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblBaseAttribute = m_myTransform.Find(m_widgetToFullName["BaseAttributeText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblAfterAttribute = m_myTransform.Find(m_widgetToFullName["AfterAttributeText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblEquipmentLevel = m_myTransform.Find(m_widgetToFullName["StrenthenDialogLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];

        for (int i = 0; i < 10; i++)
        {
            m_arrStrenthLevelBG[i] = m_myTransform.Find(m_widgetToFullName["StrenthenDialogLevelBG" + i]).gameObject;
            m_arrStrenthLevelBG[i].transform.localPosition = new Vector3(
                m_arrStrenthLevelBG[0].transform.localPosition.x + 53 * i,
                m_arrStrenthLevelBG[i].transform.localPosition.y,
                m_arrStrenthLevelBG[i].transform.localPosition.z);

            UIFilledSprite fs = m_myTransform.Find(m_widgetToFullName["StrenthenDialogLevelFG" + i]).GetComponentsInChildren<UIFilledSprite>(true)[0];
            m_fsEquipmentExpList[i] = fs;
            m_fsEquipmentExpList[i].transform.localPosition = new Vector3(
               m_fsEquipmentExpList[0].transform.localPosition.x + 53 * i,
               m_fsEquipmentExpList[i].transform.localPosition.y,
               m_fsEquipmentExpList[i].transform.localPosition.z);
        }
        m_ssEquipmentImage = m_myTransform.Find(m_widgetToFullName["StrenthenDialogImgFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];

        m_lblBaseReward0Title = FindTransform("LevelReward0BaseTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_lblBaseReward1Title = FindTransform("LevelReward1BaseTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_lblBaseReward2Title = FindTransform("LevelReward2BaseTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_lblBaseReward0Num = FindTransform("LevelReward0BaseNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblBaseReward1Num = FindTransform("LevelReward1BaseNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblBaseReward2Num = FindTransform("LevelReward2BaseNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblBaseReward0TextAnimation = FindTransform("LevelReward0BaseTextAnimation").GetComponentsInChildren<UILabel>(true)[0];
        m_lblBaseReward1TextAnimation = FindTransform("LevelReward1BaseTextAnimation").GetComponentsInChildren<UILabel>(true)[0];
        m_lblBaseReward2TextAnimation = FindTransform("LevelReward2BaseTextAnimation").GetComponentsInChildren<UILabel>(true)[0];

        m_goLevelRewardAfter = FindTransform("GOLevelRewardAfter").gameObject;
        m_lblAfterReward0 = m_myTransform.Find(m_widgetToFullName["LevelReward0AfterText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblAfterReward1 = m_myTransform.Find(m_widgetToFullName["LevelReward1AfterText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblAfterReward2 = m_myTransform.Find(m_widgetToFullName["LevelReward2AfterText"]).GetComponentsInChildren<UILabel>(true)[0];

          // ǿ����ť
        m_goStrenthenDialogStrenth = FindTransform("StrenthenDialogStrenth").gameObject;
        m_spStrenthenDialogStrenthBGUp = FindTransform("StrenthenDialogStrenthBGUp").GetComponentsInChildren<UISprite>(true)[0];

        m_lblNeedGold = m_myTransform.Find(m_widgetToFullName["StrenthenDialogCostGold"]).GetComponentsInChildren<UILabel>(true)[0];       
        m_lblCurrentGold = m_myTransform.Find(m_widgetToFullName["StrenthUICrrentGoldNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblNeedLevel = m_myTransform.Find(m_widgetToFullName["StrenthenDialogNeedLevelText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblCost = m_myTransform.Find(m_widgetToFullName["StrenthenDialogCostText"]).GetComponentsInChildren<UILabel>(true)[0];

        // ǿ����Ҫ�Ĳ���
        m_lblNeedMaterial1 = FindTransform("StrenthenDialogCostMaterial1").GetComponentsInChildren<UILabel>(true)[0];
        m_lblNeedMaterial2 = FindTransform("StrenthenDialogCostMaterial2").GetComponentsInChildren<UILabel>(true)[0];
        m_spNeedMaterialIcon1 = FindTransform("StrenthenDialogCostMaterial1Icon").GetComponentsInChildren<UISprite>(true)[0];
        m_spNeedMaterialIcon2 = FindTransform("StrenthenDialogCostMaterial2Icon").GetComponentsInChildren<UISprite>(true)[0];
        m_goNeedMaterial2 = FindTransform("GOStrenthenDialogCostMaterial2").gameObject;

        // ǿ����Ҫ�Ĳ��ϲ���,��ʾ��ָ����ť
        m_goGOMaterialObtainTip = m_myTransform.Find(m_widgetToFullName["GOMaterialObtainTip"]).gameObject;
        m_goStrenthenDialogMaterialTip = m_myTransform.Find(m_widgetToFullName["StrenthenDialogMaterialTip"]).gameObject;
        m_spStrenthenDialogMaterialTipBGUp = m_myTransform.Find(m_widgetToFullName["StrenthenDialogMaterialTipBGUp"]).GetComponentsInChildren<UISprite>(true)[0];
        m_spStrenthenDialogMaterialTipBGDown = m_myTransform.Find(m_widgetToFullName["StrenthenDialogMaterialTipBGDown"]).GetComponentsInChildren<UISprite>(true)[0];

        for (int i = 0; i < ICONGRIDNUM; ++i)
        {
            //obj = (GameObject)Instantiate(Resources.Load("GUI/StrenthenDialogIconGrid"));
            //obj.transform.parent = m_transStrenthenDialogIconList ;
            //obj.transform.localPosition = new Vector3(0,-ICONGRIDSPACE * i, 0);
            //obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 0.0008f);
            //obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragCamera;
            //obj.name = "EquipmentGrid"+i.ToString();
            //var s = m_transStrenthenDialogIconList.GetComponentsInChildren<MogoSingleButtonList>(true)[0] as MogoSingleButtonList;
            //s.SingleButtonList.Add(obj.GetComponentsInChildren<MogoSingleButton>(true)[0]);
            //obj.AddComponent<StrenthenEquipmentGrid>().id = i;
            //m_listEquipmentGrid.Add(obj);

            int index = i;
            AssetCacheMgr.GetUIInstance("StrenthenDialogIconGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_transStrenthenDialogIconList;
                obj.transform.localPosition = new Vector3(0, ICONGRIDSPACE * index, 0);
                obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragCamera;
                obj.name = "EquipmentGrid" + index.ToString();
                var s = m_transStrenthenDialogIconList.GetComponentsInChildren<MogoSingleButtonList>(true)[0] as MogoSingleButtonList;
                s.SingleButtonList.Add(obj.GetComponentsInChildren<MogoSingleButton>(true)[0]);
                obj.AddComponent<StrenthenEquipmentGrid>().id = index;

                Transform tranStarLevel = obj.transform.Find("StrenthenDialogIconGridStarLevelList");
                StrenthenStarLevelInfo strenthenStarLevelInfo = tranStarLevel.gameObject.AddComponent<StrenthenStarLevelInfo>();
                if (strenthenStarLevelInfo != null)
                    strenthenStarLevelInfo.CreateStarLevelInfo(tranStarLevel, new Vector3(0, 0, 0));

                m_listEquipmentGrid.Add(obj);

                ShowEquipmentUpSign(index, false);

                if (m_listEquipmentGrid.Count == ICONGRIDNUM)
                {
                    EquipmentUIViewManager.Instance.IsCanClick = true;
                    EventDispatcher.TriggerEvent(BodyEnhanceManager.ON_SHOW);

                    if (MogoUIManager.Instance.StrenthUILoaded != null)
                    {
                        MogoUIManager.Instance.StrenthUILoaded();
                    }

                    // ������ʽ��Ҫ����(��ҳ����Ҫ����)
                    if (!m_dragableCamera.IsMovePage)
                    {
                        m_dragableCamera.FPageHeight = ICONGRIDSPACE * ICON_GRID_ONE_PAGE;
                        m_dragableCamera.MAXY = ICON_OFFSET_Y;
                        if (m_listEquipmentGrid.Count > ICON_GRID_ONE_PAGE)
                            m_dragableCamera.MINY = (m_listEquipmentGrid.Count - ICON_GRID_ONE_PAGE) * ICONGRIDSPACE + ICON_OFFSET_Y;
                        else
                            m_dragableCamera.MINY = m_dragableCamera.MAXY;
                    }                 

                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                }

                if (index == 0)
                    StrenthTabDown(index);
                else
                    StrenthTabUp(index);
            });
        }
    }

    #endregion    

    #region �¼�

    void Initialize()
    {
        StrenthenUILogicManager.Instance.Initialize();
        StrenthUIDict.ButtonTypeToEventUp.Add("StrenthenDialogStrenth", OnStrenthenUp);
        StrenthUIDict.ButtonTypeToEventUp.Add("StrenthenDialogMaterialTip", OnMaterialTipUp);
    }

    public void Release()
    {
        StrenthenUILogicManager.Instance.Release();
        StrenthUIDict.ButtonTypeToEventUp.Clear();

        for (int i = 0; i < m_listEquipmentGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listEquipmentGrid[i]);
            m_listEquipmentGrid[i] = null;
        }
    }

    /// <summary>
    /// ���ǿ����ť
    /// </summary>
    private void OnStrenthenUp()
    {
        if (IsStrenthenButtonCanClick)
        {
            IsStrenthenButtonCanClick = false;
            if (StrenthUIDict.STRENTHENUP != null)
            {
                StrenthUIDict.STRENTHENUP();
            }
        }
    }

    /// <summary>
    /// ���������ʾ��ť
    /// </summary>
    private void OnMaterialTipUp()
    {
        if (StrenthUIDict.MATERIALTIPUP != null)
        {
            StrenthUIDict.MATERIALTIPUP();
        }
    }

    private bool IsStrenthenButtonCanClick = true;
    private const float WAITINGTIME = 0.5f; // ǿ����ť����������������500ms
    private float m_fCurrentTime = 0f;
    void Update()
    {
        if (!IsStrenthenButtonCanClick)
        {
            m_fCurrentTime += Time.deltaTime;
            if (m_fCurrentTime >= WAITINGTIME)
            {
                IsStrenthenButtonCanClick = true;
                m_fCurrentTime = 0.0f;
            }
        }      
    }

    #endregion   

    #region ǿ������

    /// <summary>
    /// ǿ�����Խ���ǿ���ȼ�
    /// </summary>
    /// <param name="strenthenStep">����</param>
    /// <param name="progress">��������</param>
    public void SetEquipmentStarLevel(int strenthenStep, int progress)
    {
        int strenthenLevel = progress / 10;
        m_lblStarLevel.text = string.Format(LanguageData.GetContent(48007), strenthenStep, strenthenLevel);
    }

    /// <summary>
    /// �������� + 5%(UI���ڲ���ʾ)
    /// </summary>
    /// <param name="attribute"></param>
    public void SetEquipmentBaseAttribute(string attribute)
    {
        m_lblBaseAttribute.text = attribute;
    }

    /// <summary>
    /// ���Ǻ� + X%(UI���ڲ���ʾ)
    /// </summary>
    /// <param name="attribute"></param>
    public void SetEquipmentAfterAttribute(string attribute)
    {
        m_lblAfterAttribute.text = attribute;
    }

    #endregion

    #region ������Ϣ

    public void SetEquipmentLevel(int level)
    {
        m_lblEquipmentLevel.text = level.ToString() + "%";
    }

    public void SetEquipmentExp(int progress)
    {
        int count = progress / 10;
        for (int i = 0; i < 10; i++)
        {
            if (i < count)
                m_fsEquipmentExpList[i].gameObject.SetActive(true);
            else
                m_fsEquipmentExpList[i].gameObject.SetActive(false);
        }
    }

    public void SetEquipmentImage(string imageName)
    {
        m_ssEquipmentImage.spriteName = imageName;
    }

    /// <summary>
    /// ��Ҫ����
    /// </summary>
    /// <param name="?"></param>
    public void SetCostText(bool isEnough = true)
    {
        if (isEnough)
        {
            m_lblCost.text = LanguageData.GetContent(175);
            m_lblCost.effectStyle = UILabel.Effect.None;
        }
        else
        {
            m_lblCost.text = MogoUtils.GetRedString(LanguageData.GetContent(175));
            m_lblCost.effectStyle = UILabel.Effect.Outline;
            m_lblCost.effectColor = new Color32(50, 39, 9, 255);
        }
    }

    public void SetEquipmentName(int gridId, string name)
    {
        m_listEquipmentGrid[gridId].GetComponentsInChildren<UILabel>(true)[0].text = name;
    }

    public void SetEquipmentIcon(int gridId, string imgName,int color = 0)
    {
        UISprite sp = m_listEquipmentGrid[gridId].transform.Find("StrenthenDialogIconGridImg").GetComponentsInChildren<UISprite>(true)[0];
        sp.spriteName = imgName;
        MogoUtils.SetImageColor(sp, color);
        //if (color == 10)
        //{
        //    m_listEquipmentGrid[gridId].transform.GetComponent<StrenthenEquipmentGrid>().enabled = false;
        //}
        //else
        //{
        //    m_listEquipmentGrid[gridId].transform.GetComponent<StrenthenEquipmentGrid>().enabled = true;
        //}
    }

    public void SetEquipmentTextImg(int gridId, string imgName)
    {
        m_listEquipmentGrid[gridId].transform.Find("StrenthenDialogIconGridTextImg").GetComponentsInChildren<UISprite>(true)[0].spriteName = imgName;
    }

    public void ShowEquipmentUpSign(int gridId, bool show = true)
    {
        m_listEquipmentGrid[gridId].transform.Find("StrenthenDialogIconGridUp").gameObject.SetActive(show);
    }

    /// <summary>
    /// ǿ�����Icon��������ǿ���ȼ�
    /// </summary>
    /// <param name="gridId"></param>
    /// <param name="level"></param>
    public void SetEquipmentIconStarLevel(int gridId, int level)
    {
        StrenthenStarLevelInfo strenthenStarLevelInfo = m_listEquipmentGrid[gridId].GetComponentInChildren<StrenthenStarLevelInfo>();
        if (strenthenStarLevelInfo != null)
            strenthenStarLevelInfo.SetStarLevel(level);
    }

    public void SetBaseEquipType(string type)
    {
        m_lblBaseEquipType.text = type;
    }

    public void SetCurrentDownGrid(int gridId)
    {
        m_transStrenthenDialogIconList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(gridId);

        if (gridId >= 5)
        {
            m_dragCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].SetCurrentPage(1);
        }
        else
        {
            m_dragCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].SetCurrentPage(0);
        }
    }

    public void SetCurrentGold(string gold)
    {
        m_lblCurrentGold.text = gold;
    }

    public void SetNeedLevel(int needLevel)
    {
        string needLeveStr = LanguageData.GetContent(911, needLevel);
        if (needLevel > MogoWorld.thePlayer.level)
        {
            needLeveStr = MogoUtils.GetRedString(needLeveStr);
            m_lblNeedLevel.effectStyle = UILabel.Effect.Outline;
            m_lblNeedLevel.effectColor = new Color32(50, 39, 9, 255);
        }
        else
        {
            m_lblNeedLevel.effectStyle = UILabel.Effect.None;
        }

        m_lblNeedLevel.text = needLeveStr;
    }

    #endregion

    #region ǿ��������ֵ

    /// <summary>
    /// ǿ��ǰ����1
    /// </summary>
    /// <param name="baseReward"></param>
    public void SetEquipmentBaseLevel0Reward(string title, string num)
    {
        m_lblBaseReward0Title.text = title;
        m_lblBaseReward0Num.text = num;
        m_lblBaseReward0TextAnimation.text = num;
    }

    /// <summary>
    /// ǿ��ǰ����2
    /// </summary>
    /// <param name="baseReward"></param>
    public void SetEquipmentBaseLevel1Reward(string title, string num)
    {
        m_lblBaseReward1Title.text = title;
        m_lblBaseReward1Num.text = num;
        m_lblBaseReward1TextAnimation.text = num;
    }

    /// <summary>
    /// ǿ��ǰ����3
    /// </summary>
    /// <param name="baseReward"></param>
    public void SetEquipmentBaseLevel2Reward(string title, string num)
    {
        m_lblBaseReward2Title.text = title;
        m_lblBaseReward2Num.text = num;
        m_lblBaseReward2TextAnimation.text = num;
    }
 
    /// <summary>
    /// ǿ��������1
    /// </summary>
    /// <param name="baseReward"></param>
    public void SetEquipmentAfterLevel0Reward(string baseReward)
    {
        m_lblAfterReward0.text = baseReward;
    }

    /// <summary>
    /// ǿ��������2
    /// </summary>
    /// <param name="baseReward"></param>
    public void SetEquipmentAfterLevel1Reward(string baseReward)
    {
        m_lblAfterReward1.text = baseReward;
    }

    /// <summary>
    /// ǿ��������3
    /// </summary>
    /// <param name="baseReward"></param>
    public void SetEquipmentAfterLevel2Reward(string baseReward)
    {
        m_lblAfterReward2.text = baseReward;
    }

    #endregion

    #region ǿ���ɹ���������

    readonly static float STAR_ANIMATION_DURATION = 1.5f;
    readonly static float TEXT_ANIMATION_DURATION = 0.3f;
    readonly static Vector3 TEXT_ANIMATION_TO = new Vector3(120, 120, 1);
    private bool m_IsPlayingSuccessAnimation = false;
    private bool IsPlayingSuccessAnimation
    {
        get { return m_IsPlayingSuccessAnimation; }
        set
        {
            m_IsPlayingSuccessAnimation = value;
            EnableStrenthUIBoxCollider(m_IsPlayingSuccessAnimation);

            if (!m_IsPlayingSuccessAnimation)
                ResetStrenthenUI();
        }
    }

    /// <summary>
    /// ǿ���ɹ�����׼��
    /// </summary>
    public void PreparePlaySuccessAnimation()
    {
        IsPlayingSuccessAnimation = true;       

        m_goLevelRewardAfter.SetActive(false);

        m_lblBaseReward0Num.gameObject.SetActive(false);
        m_lblBaseReward1Num.gameObject.SetActive(false);
        m_lblBaseReward2Num.gameObject.SetActive(false);

        m_lblBaseReward0TextAnimation.gameObject.SetActive(false);
        m_lblBaseReward1TextAnimation.gameObject.SetActive(false);
        m_lblBaseReward2TextAnimation.gameObject.SetActive(false);       
        m_lblBaseReward0TextAnimation.pivot = UIWidget.Pivot.Left;
        m_lblBaseReward1TextAnimation.pivot = UIWidget.Pivot.Left;
        m_lblBaseReward2TextAnimation.pivot = UIWidget.Pivot.Left;
        m_lblBaseReward0TextAnimation.transform.localPosition = m_lblBaseReward0Num.transform.localPosition;
        m_lblBaseReward1TextAnimation.transform.localPosition = m_lblBaseReward1Num.transform.localPosition;
        m_lblBaseReward2TextAnimation.transform.localPosition = m_lblBaseReward2Num.transform.localPosition;

        SetStrenthenDialogStrenth(false);
    }

    /// <summary>
    /// ����Ϊ��������ǰ��UI״̬
    /// </summary>
    private void ResetStrenthenUI()
    {
        m_goLevelRewardAfter.SetActive(true);

        m_lblBaseReward0Num.gameObject.SetActive(true);
        m_lblBaseReward1Num.gameObject.SetActive(true);
        m_lblBaseReward2Num.gameObject.SetActive(true);
  
        m_lblBaseReward0TextAnimation.gameObject.SetActive(false);
        m_lblBaseReward1TextAnimation.gameObject.SetActive(false);
        m_lblBaseReward2TextAnimation.gameObject.SetActive(false);

        SetStrenthenDialogStrenth(true);
    }

    /// <summary>
    /// ǿ���ɹ���������
    /// </summary>
    public void PlaySuccessAnimation(int level)
    {
        m_lblBaseReward0TextAnimation.pivot = UIWidget.Pivot.Center;
        m_lblBaseReward1TextAnimation.pivot = UIWidget.Pivot.Center;
        m_lblBaseReward2TextAnimation.pivot = UIWidget.Pivot.Center;

        ShowStrenthUIFX(level);
        TimerHeap.AddTimer((uint)(STAR_ANIMATION_DURATION * 1000), 0, () => { PlayBaseReward0TextAnimation();});        
    }

    /// <summary>
    /// ���Ż���ǿ������1����
    /// </summary>
    private void PlayBaseReward0TextAnimation()
    {
        if (!IsPlayingSuccessAnimation)
            return;

        m_lblBaseReward0Num.gameObject.SetActive(true);
        m_lblBaseReward0TextAnimation.gameObject.SetActive(true);

        TweenScale ts = m_lblBaseReward0TextAnimation.GetComponentsInChildren<TweenScale>(true)[0];
        ts.Reset();
        ts.duration = TEXT_ANIMATION_DURATION;
        ts.to = TEXT_ANIMATION_TO;
        ts.callWhenFinished = "OnBaseReward0TextAnimationEnd";
        ts.eventReceiver = this.gameObject;
        ts.enabled = true;
        ts.Play(true);

        TweenAlpha ta = m_lblBaseReward0TextAnimation.GetComponentsInChildren<TweenAlpha>(true)[0];
        ta.Reset();
        ta.duration = TEXT_ANIMATION_DURATION;
        ta.enabled = true;
        ta.Play(true);
    }

    /// <summary>
    /// ����ǿ������1�����������
    /// </summary>
    private void OnBaseReward0TextAnimationEnd()
    {
        m_lblBaseReward0TextAnimation.gameObject.SetActive(false);
        PlayBaseReward1TextAnimation();
    }

    /// <summary>
    /// ���Ż���ǿ������2����
    /// </summary>
    private void PlayBaseReward1TextAnimation()
    {
        if (!IsPlayingSuccessAnimation)
            return;

        m_lblBaseReward1Num.gameObject.SetActive(true);
        m_lblBaseReward1TextAnimation.gameObject.SetActive(true);

        TweenScale ts = m_lblBaseReward1TextAnimation.GetComponentsInChildren<TweenScale>(true)[0];
        ts.Reset();
        ts.duration = TEXT_ANIMATION_DURATION;
        ts.to = TEXT_ANIMATION_TO;
        ts.callWhenFinished = "OnBaseReward1TextAnimationEnd";
        ts.eventReceiver = this.gameObject;
        ts.enabled = true;
        ts.Play(true);

        TweenAlpha ta = m_lblBaseReward1TextAnimation.GetComponentsInChildren<TweenAlpha>(true)[0];
        ta.Reset();
        ta.duration = TEXT_ANIMATION_DURATION;
        ta.enabled = true;
        ta.Play(true);
    }

    /// <summary>
    /// ����ǿ������2�����������
    /// </summary>
    private void OnBaseReward1TextAnimationEnd()
    {
        m_lblBaseReward1TextAnimation.gameObject.SetActive(false);
        PlayBaseReward2TextAnimation();
    }

    /// <summary>
    /// ���Ż���ǿ������3����
    /// </summary>
    private void PlayBaseReward2TextAnimation()
    {
        if (!IsPlayingSuccessAnimation)
            return;

        m_lblBaseReward2Num.gameObject.SetActive(true);
        m_lblBaseReward2TextAnimation.gameObject.SetActive(true);

        TweenScale ts = m_lblBaseReward2TextAnimation.GetComponentsInChildren<TweenScale>(true)[0];
        ts.Reset();
        ts.duration = TEXT_ANIMATION_DURATION;
        ts.to = TEXT_ANIMATION_TO;
        ts.callWhenFinished = "OnBaseReward2TextAnimationEnd";
        ts.eventReceiver = this.gameObject;
        ts.enabled = true;
        ts.Play(true);

        TweenAlpha ta = m_lblBaseReward2TextAnimation.GetComponentsInChildren<TweenAlpha>(true)[0];
        ta.Reset();
        ta.duration = TEXT_ANIMATION_DURATION;
        ta.enabled = true;
        ta.Play(true);
    }

    /// <summary>
    /// ����ǿ������3�����������
    /// </summary>
    private void OnBaseReward2TextAnimationEnd()
    {
        m_lblBaseReward2TextAnimation.gameObject.SetActive(false);

        m_lblBaseReward0TextAnimation.GetComponentsInChildren<TweenScale>(true)[0].Reset(); // ����Position
        m_lblBaseReward1TextAnimation.GetComponentsInChildren<TweenScale>(true)[0].Reset(); // ����Position
        m_lblBaseReward2TextAnimation.GetComponentsInChildren<TweenScale>(true)[0].Reset(); // ����Position

        TimerHeap.AddTimer(200, 0, () =>
        {
            m_goLevelRewardAfter.SetActive(true);
            SetStrenthenDialogStrenth(true);
            IsPlayingSuccessAnimation = false;
        });
    }

    /// <summary>
    /// ����ǿ����ť
    /// </summary>
    /// <param name="isEnable">�Ƿ���</param>
    private void SetStrenthenDialogStrenth(bool isEnable)
    {
        if(isEnable)
        {
            m_goStrenthenDialogStrenth.GetComponentsInChildren<BoxCollider>(true)[0].enabled = true;
            m_spStrenthenDialogStrenthBGUp.spriteName = "btn_03up";
        }
        else
        {
            m_goStrenthenDialogStrenth.GetComponentsInChildren<BoxCollider>(true)[0].enabled = false;
            m_spStrenthenDialogStrenthBGUp.spriteName = "btn_03grey";
        }        
    }

    /// <summary>
    /// �Ƿ�ʹ���ɰ���ײ��
    /// </summary>
    /// <param name="isEnable"></param>
    private void EnableStrenthUIBoxCollider(bool isEnable)
    {
        m_bcStrenthUIBoxCollider.enabled = isEnable;
        EventDispatcher.TriggerEvent(Events.EquipmentEvent.SetEquipmentUICloseValueZ, isEnable);
    }

    #endregion

    #region ǿ����Ҫ�Ĳ���

    /// <summary>
    /// ����ǿ����Ҫ�Ĳ���1ͼ��
    /// </summary>
    /// <param name="id"></param>
    public void SetStrenthenNeedMaterialIcon1(int id)
    {
        InventoryManager.SetIcon(id, m_spNeedMaterialIcon1);
    }

    /// <summary>
    /// ǿ����Ҫ�Ľ��
    /// </summary>
    /// <param name="gold"></param>
    public void SetStrenthenNeedGold(string gold)
    {
        m_lblNeedGold.text = gold;
    }

    /// <summary>
    /// ǿ����Ҫ�Ĳ���1
    /// </summary>
    /// <param name="materialName">��������</param>
    public void SetStrenthenNeedMaterial1(string materialName)
    {
        m_lblNeedMaterial1.text = materialName;
    }

    /// <summary>
    /// �Ƿ���ʾǿ����Ҫ�Ĳ���2
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowStrenthenNeedMaterial2(bool isShow)
    {
        m_goNeedMaterial2.SetActive(isShow);
    }

    /// <summary>
    /// ����ǿ����Ҫ�Ĳ���2ͼ��
    /// </summary>
    /// <param name="id"></param>
    public void SetStrenthenNeedMaterialIcon2(int id)
    {
        InventoryManager.SetIcon(id, m_spNeedMaterialIcon2);
    }

    /// <summary>
    /// ǿ����Ҫ�Ĳ���2
    /// </summary>
    /// <param name="materialName">��������</param>
    public void SetStrenthenNeedMaterial2(string materialName)
    {
        m_lblNeedMaterial2.text = materialName;
    }

    #endregion  

    #region ���ϲ�������

    MaterialObtainTip m_materialObtainTip;

    bool m_isChangeMaterialTipBG = true;
    public bool IsChangeMaterialTipBG
    {
        get { return m_isChangeMaterialTipBG; }
        set
        {
            m_isChangeMaterialTipBG = value;

            if (m_isChangeMaterialTipBG == false)
                ChangeMaterialTipBG();
        }
    }

    public void ShowMaterialTip(bool isShow, bool isChange = true)
    {
        m_goStrenthenDialogMaterialTip.SetActive(isShow);

        if (isShow)
        {
            IsChangeMaterialTipBG = isChange;
            ChangeMaterialTipBG();
        }
        else
        {
            IsChangeMaterialTipBG = false;
            ChangeMaterialTipBG();
        }
    }

    public void ChangeMaterialTipBG(bool isShowUp = true)
    {
        ShowMaterialTipBG(isShowUp);

        if (IsChangeMaterialTipBG && false)
        {
            Mogo.Util.TimerHeap.AddTimer<bool>(100, 0, ChangeMaterialTipBG, !isShowUp);
        }
        else
            ShowMaterialTipBG(true);
    }

    private void ShowMaterialTipBG(bool isShowUp)
    {
        if (isShowUp)
        {
            m_spStrenthenDialogMaterialTipBGUp.gameObject.SetActive(true);
            m_spStrenthenDialogMaterialTipBGDown.gameObject.SetActive(false);
        }
        else
        {
            m_spStrenthenDialogMaterialTipBGUp.gameObject.SetActive(false);
            m_spStrenthenDialogMaterialTipBGDown.gameObject.SetActive(true);
        }
    }

    bool IsMaterialObtainTipLoaded = false;
    public void ShowMaterialObtainTip(bool isShow, bool refresh = false, int itemId1 = 0, int itemId2 = 0)
    {
        if (m_materialObtainTip == null)
        {
            if (!IsMaterialObtainTipLoaded)
            {
                IsMaterialObtainTipLoaded = true;

                AssetCacheMgr.GetUIInstance("MaterialObtainTip.prefab", (prefab, guid, go) =>
                {
                    GameObject obj = (GameObject)go;
                    obj.transform.parent = m_goGOMaterialObtainTip.transform;
                    obj.transform.localPosition = new Vector3(5000, 0, 0);
                    obj.transform.localScale = new Vector3(1, 1, 1);
                    m_materialObtainTip = obj.AddComponent<MaterialObtainTip>();
                    m_materialObtainTip.LoadResourceInsteadOfAwake();
                    m_materialObtainTip.ShowMaterial(refresh, itemId1, itemId2);
                    m_materialObtainTip.gameObject.SetActive(isShow);
                });
            }
        }
        else
        {
            m_materialObtainTip.ShowMaterial(refresh, itemId1, itemId2);
            m_materialObtainTip.gameObject.SetActive(isShow);
        }
    }

    #endregion    

    #region Tab Change

    public void HandleStrenthTabChange(int fromTab, int toTab)
    {
        StrenthTabUp(fromTab);
        StrenthTabDown(toTab);
    }

    private void StrenthTabUp(int tab)
    {
        UILabel lblFromTab = GetStrenthenEquipmentGridText(tab);
        if (lblFromTab != null)
        {
            lblFromTab.color = new Color32(37, 29, 6, 255);
            lblFromTab.effectStyle = UILabel.Effect.None;
        }
    }

    private void StrenthTabDown(int tab)
    {
        UILabel lblToTab = GetStrenthenEquipmentGridText(tab);
        if (lblToTab != null)
        {
            lblToTab.color = new Color32(255, 255, 255, 255);
            lblToTab.effectStyle = UILabel.Effect.Outline;
            lblToTab.effectColor = new Color32(53, 22, 2, 255);
        }
    }

    private UILabel GetStrenthenEquipmentGridText(int tab)
    {
        UILabel lblGridText = null;

        if (tab < 0 || tab >= m_listEquipmentGrid.Count)
            tab = 0;

        if (tab >= 0 && tab < m_listEquipmentGrid.Count)
        {
            GameObject goEquipmentGrid = m_listEquipmentGrid[tab];
            if (goEquipmentGrid != null)
            {
                Transform tranGridText = goEquipmentGrid.transform.Find("StrenthenDialogIconGridText");
                if (tranGridText != null)
                    lblGridText = tranGridText.GetComponent<UILabel>();
            }
        }

        return lblGridText;
    }

    #endregion

    #region ǿ����Ч

    private string m_fx1StrengthSuccess = "StrengthSuccessFx";

    private void ShowStrenthUIFX(int level)
    {
        if (level == 0)
        {
            //MogoFXManager.Instance.AttachUIFX(3, MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, m_arrStrenthLevelBG[9]);
            //MogoFXManager.Instance.AttachUIFX(4, MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, null);

            TimerHeap.AddTimer((uint)(2000), 0, () => { ReleaseStrenthSuccessAnimation(); });
            MogoFXManager.Instance.AttachParticleAnim("fx_ui_strength_succeed.prefab", m_fx1StrengthSuccess, m_arrStrenthLevelBG[9].transform.position,
                MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, () =>
                {

                });
        }
        else
        {
            //MogoFXManager.Instance.AttachUIFX(3, MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, m_arrStrenthLevelBG[level - 1]);

            TimerHeap.AddTimer((uint)(2000), 0, () => { ReleaseStrenthSuccessAnimation(); });
            MogoFXManager.Instance.AttachParticleAnim("fx_ui_strength_succeed.prefab", m_fx1StrengthSuccess, m_arrStrenthLevelBG[level - 1].transform.position,
                MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, () =>
                {

                });
        }
    }

    /// <summary>
    /// �ͷų�ֵ��ť��Ч
    /// </summary>
    private void ReleaseStrenthSuccessAnimation()
    {
        MogoFXManager.Instance.ReleaseParticleAnim(m_fx1StrengthSuccess);
    }

    #endregion

    #region ����򿪺͹ر�

    protected override void OnEnable()
    {
        base.OnEnable();
        IsPlayingSuccessAnimation = false;
    }

    public void ReleaseUIAndResources()
    {
        ReleasePreLoadResources();
    }

    void ReleasePreLoadResources()
    {
        AssetCacheMgr.ReleaseResourceImmediate("StrenthenDialogIconGrid.prefab");
        MogoUIManager.Instance.DestroyStrenthUI();
    }

    void OnDisable()
    {
        IsPlayingSuccessAnimation = false;

        if (SystemSwitch.DestroyAllUI)
        {            
            ReleaseUIAndResources();
        }      
    }

    #endregion
}
