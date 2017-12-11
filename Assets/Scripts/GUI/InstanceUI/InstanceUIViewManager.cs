/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：InstanceUIViewManager
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using System;
using System.Text;
using Mogo.Game;
using Mogo.Mission;
using Mogo.GameData;

public class InstanceUIViewManager : MonoBehaviour
{
    public Action INSTANCEMAPCHESTUP;
    public Action INSTANCEMAPCHESTCONFIRMUP;

    public Action CHOOSEGRIDUIMAPUP;
    public Action CHOOSEGRIDCLOSEUP;
    public Action CHOOSELEVELUIBACKUP;
    public Action CHOOSELEVELCLOSEUP;
    public Action INSTANCEMAPCLOSEUP;
    public Action INSTANCECHOOSELEVEL0UP;
    public Action INSTANCECHOOSELEVEL1UP;
    public Action INSTANCECHOOSELEVEL2UP;
    public Action INSTANCEENTERUP;
    public Action PASSREWARDUIOKUP;

    public Action<int> INSTANCECHOOSEGRIDUP;
    public Action CHOOSELEVELUICLEANUP;
    public Action LEVEL2DIAMONDUP;
    public Action<int> INSTANCEMAPUIMAPUP;
    public Action INSTANCEFRIENDSHIPUP;

    public Action CANRESETMISSIONCONFIRM;
    public Action CANRESETMISSIONCANCEL;
    public Action CANNOTRESETMISSIONCONFIRM;


    GameObject m_tranInstanceChooseUI;
    GameObject m_tranInstanceLevelChooseUI;
    GameObject m_tranInstanceMapUI;
    GameObject m_tranInstancePassRewardUI;
    GameObject m_tranInstancePassUI;
    GameObject m_tranInstanceUIMaskBG;

    GameObject m_tranInstancePlayerListCamera;
    Camera m_dragCamera;

    UILabel m_labelInstancePassFriendShip;
    GameObject m_spriteInstancePassFriendShipHeart;
    GameObject m_labelInstancePassFriendShipGood;

    GameObject m_resetMissionWindow;
    GameObject m_canResetMissionWindow;
    GameObject m_canNotResetMissionWindow;
    UILabel m_canResetMissionWindowTextNum;
    UILabel m_canNotResetMissionWindowText;

    GameObject m_InstanceUIPassWinText;

    UISprite m_spInstanceUICtrl;

    private static InstanceUIViewManager m_instance;

    public static InstanceUIViewManager Instance
    {
        get
        {

            return InstanceUIViewManager.m_instance;
        }
    }

    private Transform m_myTransform;
    private Transform m_instanceRewardItemList;

    private int m_RewardItemNum;

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();
    //public static Dictionary<string, Action<int>> ButtonTypeToEventUp = new Dictionary<string, Action<int>>();

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    private List<UISprite> m_listGridImage = new List<UISprite>();
    private List<UILabel> m_listGridName = new List<UILabel>();
    private List<UISprite> m_listGridBG = new List<UISprite>();
    private List<UISprite> m_listGridNameBG = new List<UISprite>();

    private List<GameObject> m_listInstaceRewardItem = new List<GameObject>();

    private List<GameObject> m_listInstanceChooseBodyGrid = new List<GameObject>();

    List<GameObject> m_listInstanceMercenary = new List<GameObject>();

    List<GameObject> m_listInstancePassStar = new List<GameObject>();
    UILabel m_listInstancePassTimeNum;

    private UISprite[] m_levelStar = new UISprite[3];

    const float REWARDITEMSPACE = 0.153f;
    const float LEAVEINSTANCETIME = 7.0f;
    const float LEAVEINSTANCEWAITTIME = 3.0f;

    private bool m_bBeginCount = false;
    private bool isCountingTime = false;
    private bool isShowingPassUIOrRewardUI = false;

    private float m_fCurrentTime = 0f;
    private float m_fElapseTime = 0f;

    private GameObject m_goInstanceChooseBodyGridList;

    // to do 
    private GameObject[] m_arrInstanceLevel = new GameObject[3];
    private List<InstanceHelperGrid> helper = new List<InstanceHelperGrid>();
    private GameObject[] m_ListInstancePassSign = new GameObject[3];
    private uint timeID = uint.MaxValue;


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

    private void SetUIText(string UIName, string text)
    {
        var l = m_myTransform.Find(UIName).GetComponentsInChildren<UILabel>(true);
        if (l != null)
        {
            l[0].text = text;
            l[0].transform.localScale = new Vector3(30, 30, 30);
        }
    }

    private void SetUITexture(string UIName, string imageName)
    {
        var s = m_myTransform.Find(UIName).GetComponentsInChildren<UISlicedSprite>(true);
        if (s != null)
            s[0].spriteName = imageName;
    }

    void OnMapChestButtonUp(int i)
    {
        if (INSTANCEMAPCHESTUP != null)
            INSTANCEMAPCHESTUP();
    }

    void OnMapChestConfirmButtonUp(int i)
    {
        if (INSTANCEMAPCHESTCONFIRMUP != null)
            INSTANCEMAPCHESTCONFIRMUP();
    }

    void OnInstanceLevelChooseUICloseUp(int i)
    {
        if (CHOOSELEVELCLOSEUP != null)
            CHOOSELEVELCLOSEUP();

        //ShowInstanceChooseUI(true);
        //ShowInstanceLevelChooseUI(false);
        //MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    void OnChooseGridCloseUp(int i)
    {
        if (CHOOSEGRIDCLOSEUP != null)
            CHOOSEGRIDCLOSEUP();
    }

    void OnChooseLevelUIBackUp(int i)
    {
        if (CHOOSELEVELUIBACKUP != null)
            CHOOSELEVELUIBACKUP();
        //m_tranInstanceLevelChooseUI.SetActive(false);
        //m_tranInstanceChooseUI.SetActive(true);
        //m_tranInstanceUIMaskBG.SetActive(true);
    }

    void OnChooseLevelCloseUp(int i)
    {
        if (CHOOSELEVELCLOSEUP != null)
            CHOOSELEVELCLOSEUP();
        //m_tranInstanceChooseUI.SetActive(true);
        //m_tranInstanceLevelChooseUI.SetActive(false);
        //m_tranInstanceUIMaskBG.SetActive(true);
    }

    void OnChooseGridUIMapUp(int i)
    {
        if (CHOOSEGRIDUIMAPUP != null)
            CHOOSEGRIDUIMAPUP();

        //m_tranInstanceMapUI.SetActive(true);
        //m_tranInstanceChooseUI.SetActive(false);
        //m_tranInstanceUIMaskBG.SetActive(true);
    }

    void OnInstanceMapCloseUp(int i)
    {
        if (INSTANCEMAPCLOSEUP != null)
            INSTANCEMAPCLOSEUP();
        //m_tranInstanceMapUI.SetActive(false);
        //m_tranInstanceChooseUI.SetActive(true);
        //m_tranInstanceUIMaskBG.SetActive(true);
    }

    void OnPassRewardUIOKUp(int i)
    {
        if (PASSREWARDUIOKUP != null)
            PASSREWARDUIOKUP();
        //m_tranInstancePassRewardUI.SetActive(false);
        //m_tranInstancePassUI.SetActive(true);
        //m_tranInstanceUIMaskBG.SetActive(false);

        //BeginCountDown();

    }



    void OnInstanceChooseLevel0Up(int i)
    {
        if (INSTANCECHOOSELEVEL0UP != null)
            INSTANCECHOOSELEVEL0UP();
    }

    void OnInstanceChooseLevel1Up(int i)
    {
        if (INSTANCECHOOSELEVEL1UP != null)
            INSTANCECHOOSELEVEL1UP();
    }

    void OnInstanceChooseLevel2Up(int i)
    {
        if (INSTANCECHOOSELEVEL2UP != null)
            INSTANCECHOOSELEVEL2UP();
    }

    void OnInstanceEnterUp(int i)
    {
        if (INSTANCEENTERUP != null)
            INSTANCEENTERUP();
    }

    void OnChooseLevelUICleanUp(int i)
    {
        if (CHOOSELEVELUICLEANUP != null)
            CHOOSELEVELUICLEANUP();
    }

    void OnInstanceLevel2DiamondUp(int i)
    {
        if (LEVEL2DIAMONDUP != null)
            LEVEL2DIAMONDUP();
    }

    void OnInstantceMapUIMapUp(int id)
    {
        if (INSTANCEMAPUIMAPUP != null)
            INSTANCEMAPUIMAPUP(id);
    }

    void OnRewardUIFriendShipGoodUp(int i)
    {
        if (INSTANCEFRIENDSHIPUP != null)
            INSTANCEFRIENDSHIPUP();
    }

    void OnCanResetMissionConfirmUp(int i)
    {
        if (CANRESETMISSIONCONFIRM != null)
            CANRESETMISSIONCONFIRM();
    }

    void OnCanResetMissionCancelUp(int i)
    {
        if (CANRESETMISSIONCANCEL != null)
            CANRESETMISSIONCANCEL();
    }

    void OnCanNotResetMissionConfirmUp(int i)
    {
        if (CANNOTRESETMISSIONCONFIRM != null)
            CANNOTRESETMISSIONCONFIRM();
    }

    public void ShowInstanceChooseUI(bool show)
    {
        m_tranInstanceChooseUI.SetActive(show);
        // InstanceUILogicManager.Instance.UpdateGridName();
    }


    public void ShowInstanceLevelChooseUI(bool show)
    {
        //Mogo.Util.LoggerHelper.Debug("damn hererrerereeer");
        m_spInstanceUICtrl.ShowAsWhiteBlack(true,true);
        m_tranInstanceLevelChooseUI.SetActive(show);
    }

    public void ShowInstancePlayerList(bool show)
    {
        m_tranInstancePlayerListCamera.SetActive(show);
    }

    public void ShowInstancePassRewardUI(bool show)
    {
        // m_tranInstancePassRewardUI.SetActive(show);
        InstanceUIViewManager.Instance.ShowInstanceUIMaskBG(show);

        //MogoUIManager.Instance.LoadMogoInstancePassRewardUI(true);
        //InstancePassRewardUIViewManager.Instance.PlayerRewardAnimationOutSide();

        //TimerHeap.AddTimer(3000, 0, () =>
        //{
        //    MogoUIManager.Instance.LoadMogoInstancePassUI(true);
        //    InstancePassUIViewManager.Instance.PlayResultOutside();
        //});

        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.PassCountDownUI);
    }

    public void ShowInstancePassUI(bool show)
    {
        if (!isShowingPassUIOrRewardUI && timeID == uint.MaxValue)
        {
            MainUIViewManager.Instance.LockOut = true;
            timeID = TimerHeap.AddTimer(3000, 0, ShowInstancePassUIEnable);
        }
        else if (!show)
        {
            m_tranInstancePassUI.SetActive(false);
            MogoUIManager.Instance.ShowMogoBattleMainUIWithInstancePassUI(false);
            timeID = uint.MaxValue;
        }

        //MogoMainCamera.instance.SetActive(true);
        //MogoUIManager.Instance.ShowMogoInstanceUI();
        //m_tranInstanceChooseUI.SetActive(false);
        //m_tranInstancePassUI.SetActive(show);
    }

    public void ShowInstancePassUIEnable()
    {
        MainUIViewManager.Instance.LockOut = false;
        isShowingPassUIOrRewardUI = true;
        BeginCountDown();
        MogoUIManager.Instance.ShowMogoInstanceUI();
        MogoUIManager.Instance.m_InstanceUI.SetActive(true);
        m_tranInstanceChooseUI.SetActive(false);
        m_tranInstancePassUI.SetActive(true);
        MogoUIManager.Instance.ShowMogoBattleMainUIWithInstancePassUI(true);
    }

    public void ShowInstanceUIMaskBG(bool show)
    {
        m_tranInstanceUIMaskBG.SetActive(show);
    }

    public void SetInstanceGridImage(string imageName, int gridID)
    {
        m_listGridImage[gridID].spriteName = imageName;
    }

    public void SetInstanceGridStars(int gridID, int starNum)
    {
        m_listGridImage[gridID].transform.parent.GetComponentsInChildren<InstanceGrid>(true)[0].ShowStars(starNum);
    }

    public void SetInstanceLevelStars(int levelID, int starNum)
    {
        //Mogo.Util.LoggerHelper.Debug("levelID            " + levelID + "starNum             " + starNum);
        m_arrInstanceLevel[levelID].GetComponent<InstanceLevelGrid>().ShowStars(starNum);
    }


    public void SetInstanceGridEnable(int gridID, bool enable)
    {
        m_listGridImage[gridID].transform.parent.GetComponentsInChildren<InstanceGrid>(true)[0].SetEnable(enable);
    }

    public void SetInstanceLevelChoose(int id)
    {
        if (id == -1)
        {
            for (int i = 0; i < 3; i++)
            {
                m_arrInstanceLevel[i].GetComponent<InstanceLevelGrid>().SetChoose(false);
            }

        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == id)
                    m_arrInstanceLevel[i].GetComponent<InstanceLevelGrid>().SetChoose(true);
                else
                    m_arrInstanceLevel[i].GetComponent<InstanceLevelGrid>().SetChoose(false);
            }
        }
    }

    public void SetInstanceLevelDateTimes(int level, string dayTimes, string maxDayTimes)
    {
        m_arrInstanceLevel[level].GetComponent<InstanceLevelGrid>().SetDateTimes(dayTimes, maxDayTimes);
    }

    public void SetInstanceLevelRecommendLevel(int level, string recommendLevel)
    {
        m_arrInstanceLevel[level].GetComponent<InstanceLevelGrid>().SetRecommendLevel(recommendLevel);
    }

    public void SetChooseHelper(int id)
    {
        for (int i = 0; i < m_listInstanceMercenary.Count; i++)
        {
            InstanceHelperGrid temp = m_listInstanceMercenary[i].GetComponent<InstanceHelperGrid>();
            if (temp.id == id)
                temp.SetEnable(true);
            else
                temp.SetEnable(false);
        }
    }

    public void SetChooseHelperByIndex(int id)
    {
        for (int i = 0; i < m_listInstanceMercenary.Count; i++)
        {
            if (i == id)
                m_listInstanceMercenary[i].GetComponent<InstanceHelperGrid>().SetEnable(true);
            else
                m_listInstanceMercenary[i].GetComponent<InstanceHelperGrid>().SetEnable(false);
        }
    }

    public void SetInstanceChooseGridTitle(string name)
    {
        SetUIText(m_widgetToFullName["InstanceChooseTitleName"], name);
    }

    public void SetInstanceRewardListItem(List<ItemParent> list)
    {
        m_RewardItemNum = list.Count;

        LoggerHelper.Debug(list.Count + " " + m_RewardItemNum + ".........................");
        AddRewardItem(m_RewardItemNum);
    }



    public void SetInstanceChooseGridName(int gridID, string name)
    {
        m_listGridName[gridID].text = name ;
    }

    public void SetRewardItemData(int id, string imgName, string itemName)
    {
        m_listInstaceRewardItem[id].transform.Find("InstanceRewardItemFG").GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        m_listInstaceRewardItem[id].transform.Find("InstanceRewardItemFG").GetComponentsInChildren<UISprite>(true)[0].spriteName = imgName;
        m_listInstaceRewardItem[id].transform.Find("InstanceRewardItemText").GetComponentsInChildren<UILabel>(true)[0].text = itemName;
    }


    public void ClearRewardItemList()
    {
        Mogo.Util.LoggerHelper.Debug("ClearRewardItemList " + m_listInstaceRewardItem.Count);
        for (int i = 0; i < m_listInstaceRewardItem.Count; ++i)
        {
            
            AssetCacheMgr.ReleaseInstance(m_listInstaceRewardItem[i]);
        }

        m_listInstaceRewardItem.Clear();
    }


    public void AddRewardItem(int num, Action act = null)
    {
        //GameObject obj;

        ClearRewardItemList();


        for (int i = 0; i < num; ++i)
        {
            //obj = (GameObject)Instantiate(Resources.Load("GUI/InstanceRewardItem"));
            //obj.transform.parent = m_instanceRewardItemList;
            //obj.transform.localPosition = new Vector3(REWARDITEMSPACE * i, 0, 0);
            //obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 0.0008f);
            //m_listInstaceRewardItem.Add(obj);

            int index = i;

            AssetCacheMgr.GetUIInstance("InstanceRewardItem.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_instanceRewardItemList;
                obj.transform.localPosition = new Vector3(REWARDITEMSPACE * index, 0, 0);
                obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
                m_listInstaceRewardItem.Add(obj);

                Mogo.Util.LoggerHelper.Debug("AddRewardItem !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! " + index + " " + num);

                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camRewardItemList;

                if (m_listInstaceRewardItem.Count >= 5)
                {

                    m_camRewardItemList.GetComponentsInChildren<MyDragableCamera>(true)[0].MAXX =
                        m_myTransform.Find(m_widgetToFullName["InstancePassRewardItemListTransform0"]).localPosition.x +
                        (m_listInstaceRewardItem.Count - 5) * REWARDITEMSPACE;
                }
                else
                {
                    m_camRewardItemList.GetComponentsInChildren<MyDragableCamera>(true)[0].MAXX =
                       m_myTransform.Find(m_widgetToFullName["InstancePassRewardItemListTransform0"]).localPosition.x;
                }

                //if (index == num - 1)
                //{
                //    if (m_listInstaceRewardItem.Count <= 5)
                //    {

                //        m_myTransform.FindChild(m_widgetToFullName["InstancePassRewardItemListCamera"]).localPosition =
                //            m_myTransform.FindChild(m_widgetToFullName["InstancePassReward" + m_listInstaceRewardItem.Count + "ItemStartPos"]).localPosition;
                //    }
                //    else
                //    {
                //        m_myTransform.FindChild(m_widgetToFullName["InstancePassRewardItemListCamera"]).localPosition =
                //            m_myTransform.FindChild(m_widgetToFullName["InstancePassReward5ItemStartPos"]).localPosition;
                //    }

                if (index == num - 1)
                {
                    if (act != null)
                    {
                        act();
                    }
                }
                //}
            });
        }

       
    }

    public void AddRewardItem<T, U>(int num, Action<T, U> act, T arg1, U arg2)
    {
        //GameObject obj;

        ClearRewardItemList();

        for (int i = 0; i < num; ++i)
        {
            //obj = (GameObject)Instantiate(Resources.Load("GUI/InstanceRewardItem"));
            //obj.transform.parent = m_instanceRewardItemList;
            //obj.transform.localPosition = new Vector3(REWARDITEMSPACE * i, 0, 0);
            //obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 0.0008f);
            //m_listInstaceRewardItem.Add(obj);

            int index = i;

            AssetCacheMgr.GetUIInstance("InstanceRewardItem.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_instanceRewardItemList;
                obj.transform.localPosition = new Vector3(REWARDITEMSPACE * index, 0, 0);
                obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
                m_listInstaceRewardItem.Add(obj);

                Mogo.Util.LoggerHelper.Debug("AddRewardItem<TU> !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! " + index + " " + num);

                if (obj.GetComponentsInChildren<MyDragCamera>(true).Length > 0)
                {
                    obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camRewardItemList;
                }
                else
                {
                    obj.AddComponent<MyDragCamera>().RelatedCamera = m_camRewardItemList;
                }

                if (m_listInstaceRewardItem.Count >= 5)
                {

                    m_camRewardItemList.GetComponentsInChildren<MyDragableCamera>(true)[0].MAXX =
                        m_myTransform.Find(m_widgetToFullName["InstancePassRewardItemListTransform0"]).localPosition.x +
                        (m_listInstaceRewardItem.Count - 5) * REWARDITEMSPACE;
                }
                else
                {
                    m_camRewardItemList.GetComponentsInChildren<MyDragableCamera>(true)[0].MAXX =
                       m_myTransform.Find(m_widgetToFullName["InstancePassRewardItemListTransform0"]).localPosition.x;
                }

                //if (index == num - 1)
                //{
                //    if (m_listInstaceRewardItem.Count <= 5)
                //    {

                //        m_myTransform.FindChild(m_widgetToFullName["InstancePassRewardItemListCamera"]).localPosition =
                //            m_myTransform.FindChild(m_widgetToFullName["InstancePassReward" + m_listInstaceRewardItem.Count + "ItemStartPos"]).localPosition;
                //    }
                //    else
                //    {
                //        m_myTransform.FindChild(m_widgetToFullName["InstancePassRewardItemListCamera"]).localPosition =
                //            m_myTransform.FindChild(m_widgetToFullName["InstancePassReward5ItemStartPos"]).localPosition;
                //    }

                if (index == num - 1)
                {
                    if (act != null)
                    {
                        act(arg1, arg2);
                    }
                }
                //}
            });
        }


    }

    public void SetInstanceChooseLevelCost(int cost)
    {
        SetUIText(m_widgetToFullName["InstanceLevelChooseUIBodyCostNum"], cost.ToString());
    }

    public void SetInstanceChooseLevelTitle(string title)
    {
        SetUIText(m_widgetToFullName["InstanceLevelChooseUIBodyName"], title);
    }

    public void SetInstanceChooseLevelStar(int starNum)
    {
        for (int i = 0; i < 3; ++i)
        {
            if (i < starNum)
            {
                m_levelStar[i].gameObject.SetActive(true);
            }
            else
            {
                m_levelStar[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetInstanceChooseLevelSuggestLevel(int suggestLevel, int instanceLevel)
    {
        SetUIText(m_widgetToFullName["InstanceLevel" + instanceLevel + "SuggestLevelNum"], suggestLevel.ToString());
    }

    public void SetInstanceChooseLevelChallengeTime(int times, int instanceLevel)
    {
        SetUIText(m_widgetToFullName["InstanceLevel" + instanceLevel + "ChallengeTimesNum"], times.ToString());
    }

    public void ShowInstanceLevelFinishedSign(bool show, int instanceLevel)
    {
        m_myTransform.Find(m_widgetToFullName["InstanceLevel" + instanceLevel + "FinishedSign"]).gameObject.SetActive(show);
    }

    public void SetInstanceLevelRewardItemImage(List<string> imageNameList, int instanceLevel)
    {
        for (int i = 0; i < 2; ++i)
        {
            if (i < imageNameList.Count)
            {
                m_myTransform.Find(m_widgetToFullName["InstanceLevel" + instanceLevel + "Reward" + i]).gameObject.SetActive(true);
                SetUITexture(m_widgetToFullName["InstanceLevel" + instanceLevel + "Reward" + i], imageNameList[i]);
            }
            else
            {
                m_myTransform.Find(m_widgetToFullName["InstanceLevel" + instanceLevel + "Reward" + i]).gameObject.SetActive(false);
            }
        }
    }

    private void SetInstanceLeaveTime(int time)
    {
        SetUIText(m_widgetToFullName["InstancePassUILeftTimeNum"], time.ToString());
    }

    public void ShowInstanceLevelChooseUI()
    {
        m_spInstanceUICtrl.ShowAsWhiteBlack(true,true);
        m_tranInstanceChooseUI.SetActive(false);
        m_tranInstanceLevelChooseUI.SetActive(true);
    }

    public void SetInstanceLevelEnable(int gridId, bool enable)
    {
        //m_arrInstanceLevel[gridId].GetComponentsInChildren<InstanceUIButton>(true)[0].SetEnable(enable);
        m_arrInstanceLevel[gridId].GetComponentsInChildren<InstanceLevelGrid>(true)[0].SetEnable(enable);

        Mogo.Util.LoggerHelper.Debug("Setting InstanceLevelGridEnable!!!!!!!!!!!!!!!!!!!!");
    }

    public void ShowInstanceMapUI(bool isShow)
    {
        m_tranInstanceMapUI.SetActive(isShow);
    }

    public void BeginCountDown()
    {
        m_bBeginCount = true;
    }

    public int UpdateMercenaryList(Dictionary<int, MercenaryInfo> mercenaryInfo)
    {
        Mogo.Util.LoggerHelper.Debug("friendList.Count" + mercenaryInfo.Count);

        ClearMercenaryList();
        int count = -1;
        int ret = 0;
        foreach (var item in mercenaryInfo)
        {
            int index = ++count;
            int id = item.Key;
            var info = item.Value;
            AssetCacheMgr.GetUIInstance("InstanceLevelChooseUIPlayerGrid.prefab", (prefab, guid, go) =>
                {
                    if (index == 0)
                        ret = id;

                    GameObject temp = (GameObject)go;
                    temp.transform.parent = m_tranInstancePlayerListCamera.transform;
                    temp.transform.localPosition = new Vector3(index * 0.25f, 0.148f, 100);
                    temp.transform.localScale = new Vector3(0.00078f, 0.00078f, 1);
                    //InstanceHelperGrid theGrid = temp.GetComponentInChildren<InstanceHelperGrid>();
                    InstanceHelperGrid theGrid = temp.AddComponent<InstanceHelperGrid>();

                    if (theGrid != null)
                    {
                        theGrid.id = id;
                        theGrid.SetHelper((Vocation)(info.vocation), info.name, info.level.ToString(), info.fight.ToString(), 0); // to do
                    }
                    temp.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragCamera;

                    m_listInstanceMercenary.Add(temp);

                    InstanceUILogicManager.Instance.CountMercenaryGridCreate();
                });
        }

        m_dragCamera.gameObject.GetComponent<MyDragableCamera>().MAXX = (mercenaryInfo.Count - 3) * 0.25f;



        //foreach (GameObject obj in m_listInstancePlayer)
        //{
        //    obj.GetComponent<InstanceHelperGrid>().SetEnable(false);
        //}

        //for (int i = 0; i < friendList.Count; i++)
        //{
        //    int index = stranger + i;
        //    m_listInstancePlayer[index].GetComponent<InstanceHelperGrid>().SetHelper(friendList[i].headImg, friendList[i].name, friendList[i].level, friendList[i].power, friendList[i].degreeNum);
        //}

        return ret;
    }

    public void ClearMercenaryList()
    {
        foreach (GameObject go in m_listInstanceMercenary)
        {
            Destroy(go);
        }

        m_listInstanceMercenary.Clear();
    }

    bool isAddFriend = true;

    public void UpdateFriendShip(string name1, string name2)
    {
        isAddFriend = false;
        m_labelInstancePassFriendShipGood.GetComponentInChildren<UILabel>().text = LanguageData.GetContent(46993);

        StringBuilder sb = new StringBuilder();
        sb.Append(name1);
        sb.Append(LanguageData.GetContent(46994));
        sb.Append(name2);
        sb.Append(LanguageData.GetContent(46995));
        m_labelInstancePassFriendShip.text = sb.ToString();
        UIFont uiFont = m_labelInstancePassFriendShip.font;
        Vector2 length = uiFont.CalculatePrintedSize(sb.ToString(), false, UIFont.SymbolStyle.None);

        m_labelInstancePassFriendShip.transform.localPosition = Vector3.zero;
        m_spriteInstancePassFriendShipHeart.transform.localPosition = new Vector3(-length.x * 22 / 2f - 50, 0, 0);
        m_labelInstancePassFriendShipGood.transform.localPosition = new Vector3(length.x * 22 / 2f + 125, 0, 0);
    }

    public void RequireFriendShip(string name)
    {
        isAddFriend = true;
        m_labelInstancePassFriendShipGood.GetComponentInChildren<UILabel>().text = LanguageData.GetContent(46988);

        StringBuilder sb = new StringBuilder();
        sb.Append(LanguageData.GetContent(46986));
        sb.Append(name);
        sb.Append(LanguageData.GetContent(46987));
        m_labelInstancePassFriendShip.text = sb.ToString();
        UIFont uiFont = m_labelInstancePassFriendShip.font;
        Vector2 length = uiFont.CalculatePrintedSize(sb.ToString(), false, UIFont.SymbolStyle.None);

        m_labelInstancePassFriendShip.transform.localPosition = Vector3.zero;
        m_spriteInstancePassFriendShipHeart.transform.localPosition = new Vector3(-length.x * 22 / 2f - 50, 0, 0);
        m_labelInstancePassFriendShipGood.transform.localPosition = new Vector3(length.x * 22 / 2f + 125, 0, 0);
    }

    Camera m_camRewardItemList;

    void Awake()
    {

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_instance = m_myTransform.GetComponentsInChildren<InstanceUIViewManager>(true)[0];

        //m_instanceRewardItemList = m_myTransform.FindChild(m_widgetToFullName["InstancePassRewardItemList"]);

        //m_tranInstanceChooseUI = m_myTransform.FindChild(m_widgetToFullName["InstanceChooseUI"]).gameObject;
        //m_tranInstanceLevelChooseUI = m_myTransform.FindChild(m_widgetToFullName["InstanceLevelChooseUI"]).gameObject;
        //m_tranInstanceMapUI = m_myTransform.FindChild(m_widgetToFullName["InstanceMapUI"]).gameObject;
        //m_tranInstancePassRewardUI = m_myTransform.FindChild(m_widgetToFullName["InstancePassRewardUI"]).gameObject;
        //m_tranInstancePassUI = m_myTransform.FindChild(m_widgetToFullName["InstancePassUI"]).gameObject;
        //m_tranInstanceUIMaskBG = m_myTransform.FindChild(m_widgetToFullName["InstanceUIMaskBG"]).gameObject;
        //m_tranInstancePlayerListCamera = m_myTransform.FindChild(m_widgetToFullName["InstanceLevelChooseUIPlayerListGrid"]).gameObject;
        //m_goInstanceChooseBodyGridList = m_myTransform.FindChild(m_widgetToFullName["InstanceChooseBodyGridList"]).gameObject;

        //Camera temp = GameObject.Find("Camera").GetComponentInChildren<Camera>();

        //m_myTransform.FindChild(m_widgetToFullName["InstanceChooseUITopRight"]).GetComponent<UIAnchor>().uiCamera = temp;

        //m_myTransform.FindChild(m_widgetToFullName["InstanceLevelChooseUIPlayerListCamera"]).GetComponent<UIViewport>().sourceCamera = temp;

        //m_myTransform.FindChild(m_widgetToFullName["InstancePassRewardItemListCamera"]).GetComponent<UIViewport>().sourceCamera = temp;

        //m_myTransform.FindChild(m_widgetToFullName["InstanceMapUITopRight"]).GetComponent<UIAnchor>().uiCamera = temp;

        //m_dragCamera = m_tranInstancePlayerListCamera.GetComponentsInChildren<Camera>(true)[0];

        //m_spInstanceUICtrl = m_myTransform.FindChild(m_widgetToFullName["InstanceUIRefreshCtrl"]).GetComponentsInChildren<UISprite>(true)[0];

        //m_camRewardItemList = m_myTransform.FindChild(m_widgetToFullName["InstancePassRewardItemListCamera"]).GetComponentsInChildren<Camera>(true)[0];

        //for (int i = 0; i < 3; ++i)
        //{
        //    m_arrInstanceLevel[i] = m_myTransform.FindChild(m_widgetToFullName["InstanceLevel" + i]).gameObject;
        //    m_arrInstanceLevel[i].AddComponent<InstanceLevelGrid>().id = i;


        //    Mogo.Util.LoggerHelper.Debug("Adding InstanceLevelGrid !!!!!!!!!!!!1");
        //}

        //ShowInstanceLevelChooseUI(true);
        //ShowInstanceLevelChooseUI(false);

        //ShowInstanceChooseUI(true);
        //ShowInstanceChooseUI(false);

        //m_levelStar[0] = m_myTransform.FindChild(m_widgetToFullName["InstanceLevelChooseUIBodyStar0BGDown"]).GetComponentsInChildren<UISprite>(true)[0];
        //m_levelStar[1] = m_myTransform.FindChild(m_widgetToFullName["InstanceLevelChooseUIBodyStar1BGDown"]).GetComponentsInChildren<UISprite>(true)[0];
        //m_levelStar[2] = m_myTransform.FindChild(m_widgetToFullName["InstanceLevelChooseUIBodyStar2BGDown"]).GetComponentsInChildren<UISprite>(true)[0];

        //m_labelInstancePassFriendShip = m_myTransform.FindChild(m_widgetToFullName["InstancePassRewardUIFriendShipLabel"]).GetComponentsInChildren<UILabel>(true)[0];
        //m_spriteInstancePassFriendShipHeart = m_myTransform.FindChild(m_widgetToFullName["InstancePassRewardUIFriendShipHeart"]).gameObject;
        //m_labelInstancePassFriendShipGood = m_myTransform.FindChild(m_widgetToFullName["InstancePassRewardUIFriendShipGood"]).gameObject;

        //chestWindow = m_myTransform.FindChild(m_widgetToFullName["InstanceMapChest"]).gameObject;

        //for (int i = 0; i < 6; i++)
        //{
        //    mapMark.Add(m_myTransform.FindChild(m_widgetToFullName["InstanceMapMark" + i]).gameObject);
        //    mapName.Add(m_myTransform.FindChild(m_widgetToFullName["InstanceMapPlace" + i + "BtnText"]).GetComponent<UILabel>());
        //    //mapNameMark.Add(m_myTransform.FindChild(m_widgetToFullName["InstanceMapPlace" + i + "Mark"]).gameObject);
        //    mapLock.Add(m_myTransform.FindChild(m_widgetToFullName["InstanceMapPlace" + i + "Lock"]).gameObject);
        //    mapUp.Add(m_myTransform.FindChild(m_widgetToFullName["InstanceMapPlace" + i + "BtnBGUp"]).gameObject);
        //}

        //for (int i = 0; i < 6; i++)
        //{
        //    mapMark[i].SetActive(false);
        //    //mapNameMark[i].SetActive(false);
        //    mapLock[i].SetActive(false);
        //}

        //for (int i = 0; i < 10; ++i)
        //{
        //    AssetCacheMgr.GetUIInstance("InstanceChooseBodyGridQuad.prefab", (prefab, guid, go) =>
        //    {
        //        GameObject obj = (GameObject)go;
        //        obj.transform.parent = m_goInstanceChooseBodyGridList.transform;
        //        obj.transform.localPosition = new Vector3(m_listInstanceChooseBodyGrid.Count % 5 * 204, m_listInstanceChooseBodyGrid.Count / 5 * -260, 0);
        //        obj.transform.localScale = new Vector3(1, 1, 1);

        //        m_listGridImage.Add(obj.transform.FindChild("InstanceChooseBodyGridFG").GetComponentsInChildren<UISprite>(true)[0]);
        //        m_listGridName.Add(obj.transform.FindChild("InstanceChooseBodyGridBG/InstanceChooseBodyGridBGText").GetComponentsInChildren<UILabel>(true)[0]);
        //        m_listGridBG.Add(obj.transform.FindChild("InstanceChooseBodyGridBG/InstanceChooseBodyGridBGUp").GetComponentsInChildren<UISprite>(true)[0]);

        //        // m_listGridNameBG.Add(obj.transform.FindChild("InstanceChooseBodyGridStarBG").GetComponentsInChildren<UISprite>(true)[0]);

        //        //obj.GetComponentsInChildren<InstanceGrid>(true)[0].id = m_listInstanceChooseBodyGrid.Count;
        //        obj.transform.FindChild("InstanceChooseBodyGridBG").gameObject.AddComponent<InstanceGrid>().id = m_listInstanceChooseBodyGrid.Count;

        //        m_listInstanceChooseBodyGrid.Add(obj);

        //        //if (m_listInstanceChooseBodyGrid.Count == 10)
        //        //{

        //        //    EventDispatcher.TriggerEvent(Events.InstanceEvent.UpdateEnterableMissions);
        //        //    EventDispatcher.TriggerEvent(Events.InstanceEvent.UpdateMissionTimes);
        //        //    EventDispatcher.TriggerEvent(Events.InstanceEvent.UpdateMissionStars);

        //        //    InstanceUILogicManager.Instance.UpdateGridName();
        //        //}
        //    });

        //}

        
        //    for (int i = 0; i < 3; ++i)
        //    {
        //        m_listInstancePassStar.Add(m_myTransform.FindChild(m_widgetToFullName["InstancePassUIStar" + i]).gameObject);
        //    }

        //m_listInstancePassTimeNum = m_myTransform.FindChild(m_widgetToFullName["InstancePassUIPassTimeNum"]).gameObject.GetComponent<UILabel>();

        //m_resetMissionWindow = m_myTransform.FindChild(m_widgetToFullName["InstanceLevelChooseResetMissionWindow"]).gameObject;
        //m_canResetMissionWindow = m_myTransform.FindChild(m_widgetToFullName["InstanceLevelChooseCanResetMissionWindow"]).gameObject;
        //m_canNotResetMissionWindow = m_myTransform.FindChild(m_widgetToFullName["InstanceLevelChooseCanNotResetMissionWindow"]).gameObject;

        //m_canResetMissionWindowTextNum = m_myTransform.FindChild(m_widgetToFullName["InstanceLevelChooseCanResetMissionWindowTextNum"]).gameObject.GetComponentsInChildren<UILabel>(true)[0];
        //m_canNotResetMissionWindowText = m_myTransform.FindChild(m_widgetToFullName["InstanceLevelChooseCanNotResetMissionWindowText"]).gameObject.GetComponentsInChildren<UILabel>(true)[0];

        //instance0CurrentTaskMark = m_myTransform.FindChild(m_widgetToFullName["InstanceLevel0CurrentMission"]).gameObject;

        //instance1CurrentTaskMark = m_myTransform.FindChild(m_widgetToFullName["InstanceLevel1CurrentMission"]).gameObject;

        //m_InstanceUIPassWinText = m_myTransform.FindChild(m_widgetToFullName["InstancePassUIWinText"]).gameObject;


        Initialize();

        EventDispatcher.TriggerEvent(Events.InstanceUIEvent.UpdateMapName);
    }

    void Initialize()
    {

        Mogo.Util.LoggerHelper.Debug("InstanceUIViewManager Initialize@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

        if (InstanceUIDict.ButtonTypeToEventUp.ContainsKey("InstanceMapUIClose") == true)
        {
            return;
        }
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceMapUIClose", OnInstanceMapCloseUp);

        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceChooseTitleMap", OnChooseGridUIMapUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceChooseClose", OnChooseGridCloseUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseClose", OnChooseLevelCloseUp);

        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevel0", OnInstanceChooseLevel0Up);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevel1", OnInstanceChooseLevel1Up);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevel2", OnInstanceChooseLevel2Up);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevel2Diamond", OnInstanceLevel2DiamondUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseUITailBack", OnChooseLevelUIBackUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseUITailClean", OnChooseLevelUICleanUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseUITailEnter", OnInstanceEnterUp);

        InstanceUIDict.ButtonTypeToEventUp.Add("InstancePassRewardUIOK", OnPassRewardUIOKUp);



        InstanceUIDict.ButtonTypeToEventUp.Add("InstancePassRewardUIFriendShipGood", OnRewardUIFriendShipGoodUp);

        InstanceUIDict.ButtonTypeToEventUp.Add("NewInstanceMapChestButton", OnMapChestButtonUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceMapChestConfirmButton", OnMapChestConfirmButtonUp);

        for (int i = 0; i < 3; ++i)
        {
            InstanceUIDict.ButtonTypeToEventUp.Add("InstantceMapUIMap" + i, OnInstantceMapUIMapUp);
        }

        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseCanNotResetMissionWindowConfirm", OnCanNotResetMissionConfirmUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseCanResetMissionWindowCancel", OnCanResetMissionCancelUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseCanResetMissionWindowConfirm", OnCanResetMissionConfirmUp);

        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseUIClose", OnInstanceLevelChooseUICloseUp);

        InstanceUILogicManager.Instance.Initialize();
    }

    public void Release(Action action)
    {
        Mogo.Util.LoggerHelper.Debug("InstanceUIViewManager Release@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        InstanceUILogicManager.Instance.Release();
        MogoUIManager.Instance.hasLoadedInstanceUI = false;
        InstanceUIDict.ButtonTypeToEventUp.Clear();
        action();
    }


    void Update()
    {

        if (m_bBeginCount)
        {
            m_fCurrentTime += Time.deltaTime;
            m_fElapseTime += Time.deltaTime;

            if (m_fCurrentTime >= LEAVEINSTANCETIME)
            {
                LoggerHelper.Debug("m_fCurrentTime >= LEAVEINSTANCETIME + LEAVEINSTANCEWAITTIME");
                m_bBeginCount = false;
                // isCountingTime = false;
                m_fCurrentTime = 0f;
                m_fElapseTime = 0f;

                //m_myTransform.FindChild("InstancePassUI").gameObject.SetActive(false);
                //m_myTransform.FindChild("InstanceChooseUI").gameObject.SetActive(true);

                m_labelInstancePassFriendShipGood.SetActive(true);
                ShowInstancePassUI(false);

                EventDispatcher.TriggerEvent(Events.InstanceEvent.GetCurrentReward);

                ResetInstancePassUIAnim();

                MogoMainCamera.Instance.PlayVictoryCG();

                TimerHeap.AddTimer(2000, 0, ShowInstancePassRewardUI, true);
            }

            //else if (m_fCurrentTime >= LEAVEINSTANCEWAITTIME && !isCountingTime)
            //{
            //    LoggerHelper.Debug("m_fCurrentTime >= LEAVEINSTANCEWAITTIME && !isCountingTime");
            //    isCountingTime = true;
            //}

            else
            {
                if (m_fElapseTime >= 1.0f)
                {
                    LoggerHelper.Debug("isCountingTime");
                    SetInstanceLeaveTime((int)LEAVEINSTANCETIME - (int)m_fCurrentTime);
                    m_fElapseTime = 0f;
                }
            }
        }

        if (m_bIsStartStarCount)
        {
            m_fPassStarCountDown += Time.deltaTime;

            if (m_fPassStarCountDown >= 0.2f)
            {
                Mogo.Util.LoggerHelper.Debug(m_iCurrentStar);

                m_fPassStarCountDown = 0;
                ShowPassStar(m_iCurrentStar++);

                if (m_iCurrentStar == m_iShowStarNum)
                {
                    m_bIsStartStarCount = false;
                    m_iCurrentStar = 0;
                    m_fPassStarCountDown = 0;
                }
            }
        }
    }

    public void ResetIsShowingPassUIOrRewardUI()
    {
        isShowingPassUIOrRewardUI = false;
    }

    GameObject chestWindow;
    List<GameObject> mapMark = new List<GameObject>();
    List<UILabel> mapName = new List<UILabel>();
    //List<GameObject> mapNameMark = new List<GameObject>();
    List<GameObject> mapLock = new List<GameObject>();
    List<GameObject> mapUp = new List<GameObject>();
    GameObject instance0CurrentTaskMark;
    GameObject instance1CurrentTaskMark;

    float m_fPassStarCountDown = 0;
    bool m_bIsStartStarCount = false;
    int m_iShowStarNum = 0;
    int m_iCurrentStar = 0;

    public void SetMark(int id)
    {
        return;

        //for (int i = 0; i < 6; i++)
        //{
        //    if (id == i)
        //    {
        //        mapMark[i].SetActive(true);
        //    }
        //    else
        //        mapMark[i].SetActive(false);
        //}
    }

    public void ShowChestWindow(bool isShow)
    {
        chestWindow.SetActive(isShow);
    }

    //public void SetMapMark(int id)
    //{
    //    for (int i = 0; i < 6; i++)
    //    {
    //        if (id == i)
    //            mapNameMark[i].SetActive(true);
    //        else
    //            mapNameMark[i].SetActive(false);
    //    }
    //}

    public void SetEnable(int id, bool isShow)
    {
        return;

        //mapLock[id].SetActive(!isShow);

        //if (isShow)
        //{            
        //    //mapUp[id].GetComponentsInChildren<UISprite>(true)[0].ShowAsWhiteBlack(false);
        //    mapUp[id].GetComponentsInChildren<UISprite>(true)[0].spriteName = "fb_duwzd";
        //    mapUp[id].transform.parent.GetComponentsInChildren<BoxCollider>(true)[0].enabled = true;
        //}
        //else
        //{
        //    //mapLock[id].GetComponentsInChildren<UISprite>(true)[0].ShowAsWhiteBlack(true);
        //    //mapUp[id].GetComponentsInChildren<UISprite>(true)[0].ShowAsWhiteBlack(true);
        //    mapUp[id].GetComponentsInChildren<UISprite>(true)[0].spriteName = "fb_weikaiqi";
        //    mapUp[id].transform.parent.GetComponentsInChildren<BoxCollider>(true)[0].enabled = false;
        //}        
    }

    public void SetMapName(int mapID, string name)
    {
        mapName[mapID].text = name;
    }

    public void ModifyMapName(int mapID)
    {
        if (InstanceMissionChooseUIViewManager.SHOW_MISSION_BY_DRAG)
        {
            if (MogoUIManager.Instance.m_InstanceMissionChooseUI != null)
                InstanceMissionChooseUIViewManager.Instance.SetGridTitle(mapID, LanguageData.GetContent(MapUIMappingData.dataMap[mapID].name));
        }
        else
        {
            if (MogoUIManager.Instance.m_NewInstanceChooseLevelUI != null)
                NewInstanceUIChooseLevelViewManager.Instance.SetGridTitle(LanguageData.GetContent(MapUIMappingData.dataMap[mapID].name));
        }
    }

    public void AddInstanceBodyGrid(bool isQuad,int id)
    {
        //string prefabName;

        //if (isQuad)
        //{
        //    prefabName = "InstanceChooseBodyGridQuad.prefab";
        //}
        //else
        //{
        //    prefabName = "InstanceChooseBodyGridSphere.prefab";
        //}

        //AssetCacheMgr.GetUIInstance(prefabName, (prefab, guid, go) =>
        //{
        //    GameObject obj = (GameObject)go;
        //    obj.transform.parent = m_goInstanceChooseBodyGridList.transform;
        //    obj.transform.localPosition = new Vector3(m_listInstanceChooseBodyGrid.Count % 5 * 200, m_listInstanceChooseBodyGrid.Count / 5 * -260, 0);
        //    obj.transform.localScale = new Vector3(1, 1, 1);

        //    m_listGridImage.Add(obj.transform.FindChild("InstanceChooseBodyGridBG/InstanceChooseBodyGridBGDown").GetComponentsInChildren<UISprite>(true)[0]);
        //    m_listGridName.Add(obj.transform.FindChild("InstanceChooseBodyGridBG/InstanceChooseBodyGridBGText").GetComponentsInChildren<UILabel>(true)[0]);

        //    obj.GetComponentsInChildren<InstanceGrid>(true)[0].id = m_listInstanceChooseBodyGrid.Count;

        //    m_listInstanceChooseBodyGrid.Add(obj);
        //});

        //if (isQuad)
        //{
        //    m_listGridBG[id].spriteName = "fb_xinxibeijing";
        //    //m_listGridNameBG[id].spriteName = "fb_xinxibeijing";
        //    m_listGridBG[id].transform.localScale = new Vector3(192, 256, 1);
        //    m_listGridBG[id].transform.localPosition = new Vector3(0, -34.57f, -1);
        //}
        //else
        //{
        //    m_listGridBG[id].spriteName = "fb_teshuchangjing";
        //    //m_listGridNameBG[id].spriteName = "fb_teshubeijing";
        //    m_listGridBG[id].transform.localScale = new Vector3(180, 200, 1);
        //    m_listGridBG[id].transform.localPosition = new Vector3(0, -15, -1);

        //}

        m_listInstanceChooseBodyGrid[id].SetActive(true);


    }

    public void ClearInstanceBodyGrid()
    {
        //for (int i = 0; i < m_listInstanceChooseBodyGrid.Count; ++i)
        //{
        //    int index = i;

        //    AssetCacheMgr.ReleaseInstance(m_listInstanceChooseBodyGrid[index]);
        //}

        //m_listInstanceChooseBodyGrid.Clear();

        for (int i = 0; i < m_listInstanceChooseBodyGrid.Count; ++i)
        {
            m_listInstanceChooseBodyGrid[i].SetActive(false);
        }
    }

    public void ShowResetMissionWindow(bool isShow, bool isCan)
    {
        return;

        if (isShow)
        {
            m_resetMissionWindow.SetActive(true);
            if (isCan)
            {
                m_canResetMissionWindow.SetActive(true);
                m_canNotResetMissionWindow.SetActive(false);
            }
            else
            {
                m_canResetMissionWindow.SetActive(false);
                m_canNotResetMissionWindow.SetActive(true);
            }
        }
        else
        {
            m_resetMissionWindow.SetActive(false);
            m_canResetMissionWindow.SetActive(false);
            m_canNotResetMissionWindow.SetActive(false);
        }
    }

    public void ShowCurrentMissionMark(int level)
    {
        if (level == 1)
            instance0CurrentTaskMark.SetActive(true);
        else if (level == 2)
            instance1CurrentTaskMark.SetActive(true);
    }

    public void HideCurrentTaskMark()
    {
        instance0CurrentTaskMark.SetActive(false);
        instance1CurrentTaskMark.SetActive(false);
    }

    public void SetPassMark(int num)
    {
        m_iShowStarNum = num;
        m_bIsStartStarCount = true;
    }

    private void ShowPassStar(int num)
    {
        m_listInstancePassStar[num].SetActive(true);
    }

    public void SetPassTime(int minutes, int second)
    {
        m_listInstancePassTimeNum.text = minutes.ToString("d2") + " : " + second.ToString("d2");
    }

    private void ResetPassTime()
    {
        m_listInstancePassTimeNum.text = String.Empty; 
    }

    public void ResetInstancePassUIAnim()
    {
        for (int i = 0; i < m_listInstancePassStar.Count; ++i)
        {
            m_listInstancePassStar[i].SetActive(false);
            m_listInstancePassStar[i].GetComponentsInChildren<TweenScale>(true)[0].Reset();
            m_listInstancePassStar[i].GetComponentsInChildren<TweenScale>(true)[0].enabled = true;
        }

        m_InstanceUIPassWinText.GetComponentsInChildren<TweenScale>(true)[0].Reset();
        m_InstanceUIPassWinText.GetComponentsInChildren<TweenScale>(true)[0].enabled = true;

        SetInstanceLeaveTime(7);
        ResetPassTime();
    }

    public void ShowGoodAndHideFriendShipGoodButton()
    {
        if (!isAddFriend)
        {
            MogoFXManager.Instance.FloatText("赞!", m_labelInstancePassFriendShipGood);
            HideFriendShipGoodButton();
        }
    }

    public void HideFriendShipGoodButton()
    {
        m_labelInstancePassFriendShipGood.SetActive(false);
    }

    public void HideFriendShip()
    {
        m_labelInstancePassFriendShip.gameObject.SetActive(false);
        m_spriteInstancePassFriendShipHeart.gameObject.SetActive(false);
        m_labelInstancePassFriendShipGood.gameObject.SetActive(false); 
    }

    public void ResetFriendShip()
    {
        m_labelInstancePassFriendShip.gameObject.SetActive(true);
        m_spriteInstancePassFriendShipHeart.gameObject.SetActive(true);
        m_labelInstancePassFriendShipGood.gameObject.SetActive(true); 
    }

    public void SetCanResetMissionWindowTextNum(string text)
    {
        m_canResetMissionWindowTextNum.text = text;
    }

    public void SetCanNotResetMissionWindowText(string text)
    {
        m_canNotResetMissionWindowText.text = text;
    }
}