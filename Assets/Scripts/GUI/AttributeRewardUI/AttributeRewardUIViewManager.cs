using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class AttributeRewardGridData
{
    public string SignFGName;
    public string SignText;
    public string Title;
    public string Quest;
    public int ProcessStatus;
    public bool IsFinshed;
    public bool IsShare;
    public string ProcessText;
    public bool IsRunning;
}

public class AttributeRewardUIViewManager : MogoUIBehaviour
{
    private static AttributeRewardUIViewManager m_instance;
    public static AttributeRewardUIViewManager Instance { get { return AttributeRewardUIViewManager.m_instance; }}
   
    public static Dictionary<string, Action<int>> ButtonTypeToEventUp = new Dictionary<string, Action<int>>();

    private Camera m_camAttributeGridList;
    private GameObject m_goAttributeGridList;
    private List<GameObject> m_listAttributeGridGameObject = new List<GameObject>();
    private List<AttributeRewardGrid> m_listAttributeGrid = new List<AttributeRewardGrid>();

    const int ATTRIBUTEGRIDHEIGHT = 145;

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_camAttributeGridList = m_myTransform.Find(m_widgetToFullName["AttributeRewardGridListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camAttributeGridList.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_goAttributeGridList = m_myTransform.Find(m_widgetToFullName["AttributeRewardGridList"]).gameObject;

        Initialize();

        EventDispatcher.TriggerEvent(Events.OperationEvent.GetAchievementMessage);
    }

    #region �¼�

    void Initialize()
    {
        AttributeRewardLogicManager.Instance.Initialize();
        EventDispatcher.AddEventListener<int>(AttributeRewardLogicManager.AttributeRewardUIEvent.AttributeGridShareBtnUp, OnShareBtnUp);
    }

    public void Release()
    {
        ButtonTypeToEventUp.Clear();

        EventDispatcher.RemoveEventListener<int>(AttributeRewardLogicManager.AttributeRewardUIEvent.AttributeGridShareBtnUp, OnShareBtnUp);

        EmptyAttributeGridList();

        AttributeRewardLogicManager.Instance.Release();

        //for (int i = 0; i < m_listAttributeGrid.Count; ++i)
        //{
        //    AssetCacheMgr.ReleaseInstance(m_listAttributeGrid[i]);
        //    m_listAttributeGrid[i] = null;
        //}
        m_listAttributeGrid.Clear();

        for (int i = 0; i < m_listAttributeGridGameObject.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listAttributeGridGameObject[i]);
            m_listAttributeGridGameObject[i] = null;
        }
        m_listAttributeGridGameObject.Clear();
    }

    void OnShareBtnUp(int id)
    {
        Mogo.Util.LoggerHelper.Debug(id);
        EventDispatcher.TriggerEvent(Events.OperationEvent.AchievementShareToGetDiamond, id);
    }

    #endregion

    public void EmptyAttributeGridList()
    {
        for (int i = 0; i < m_listAttributeGridGameObject.Count; ++i)
        {
            int index = i;

            m_listAttributeGridGameObject[i].GetComponentsInChildren<AttributeRewardGrid>(true)[0].Release();

            AssetCacheMgr.ReleaseInstance(m_listAttributeGridGameObject[index]);
        }

        m_listAttributeGridGameObject.Clear();
    }

    public void AddAttributeGrid(AttributeRewardGridData ad, int gridID = -1)
    {        
        AssetCacheMgr.GetUIInstance("AttributeRewardGrid.prefab", (prefab, id, go) =>
        {
            //for (int i = 0; i < m_listAttributeGrid.Count; ++i)
            //{
            //    if (m_listAttributeGrid[i].Id == gridID)
            //    {
            //        AssetCacheMgr.ReleaseInstance((GameObject)go);
            //        return;
            //    }
            //}
            GameObject obj = (GameObject)go;
            //AttributeRewardGrid ag = obj.GetComponentsInChildren<AttributeRewardGrid>(true)[0];
            AttributeRewardGrid ag = obj.AddComponent<AttributeRewardGrid>();

            obj.name = "AttributeRewardGrid" + m_listAttributeGridGameObject.Count;
            obj.transform.Find("AttributeRewardShareBtn").name = "AttributeRewardShareBtn" + m_listAttributeGridGameObject.Count;
            string btnName = "AttributeRewardShareBtn" + m_listAttributeGridGameObject.Count;

            Mogo.Util.LoggerHelper.Debug("AddAttributeGrid Process == " + ad.ProcessStatus);

            if (gridID == -1)
                ag.Id = m_listAttributeGridGameObject.Count;
            else
                ag.Id = gridID;

            obj.transform.parent = m_goAttributeGridList.transform;
            obj.transform.localPosition = new Vector3(0, m_listAttributeGridGameObject.Count * -ATTRIBUTEGRIDHEIGHT, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            m_listAttributeGrid.Add(ag);
            m_listAttributeGridGameObject.Add(obj);
 
            obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camAttributeGridList;

            m_camAttributeGridList.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -220 - ATTRIBUTEGRIDHEIGHT * (m_listAttributeGridGameObject.Count - 4);

            EventDispatcher.TriggerEvent<int>("LoadAttributeRewardGridDone", ag.Id);

            ag.SignFGImg = ad.SignFGName;
            ag.SignText = ad.SignText;
            ag.Title = ad.Title;
            ag.Quest = ad.Quest;
            ag.ProcessStatus = ad.ProcessStatus;
            ag.IsFinished = ad.IsFinshed;
            ag.IsShare = ad.IsShare;
            ag.ProcessText = ad.ProcessText;
            ag.IsRunning = ad.IsRunning;

            if (obj.name == MogoUIManager.Instance.WaitingWidgetName || btnName == MogoUIManager.Instance.WaitingWidgetName)
            {
                TimerHeap.AddTimer(500, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });
            }
        });
    }   

    public void EmptyLoginRewardGridList()
    {
        for (int i = 0; i < m_listAttributeGridGameObject.Count; ++i)
        {
            int index = i;
            AssetCacheMgr.ReleaseInstance(m_listAttributeGridGameObject[index]);
        }

        m_listAttributeGrid.Clear();
        m_listAttributeGridGameObject.Clear();
    }

    public void SetGridIsShare(int id, bool isShare)
    {
        foreach (var item in m_listAttributeGrid)
        {
            if (item.Id == id)
            {
                m_listAttributeGrid[id].IsShare = isShare;
                break;
            }
        }
    }

    public void SetGridIsFinshed(int id, bool isFinished)
    {
        foreach (var item in m_listAttributeGrid)
        {
            if (item.Id == id)
            {
                m_listAttributeGrid[id].IsFinished = isFinished;
                break;
            }
        }
    }

    public void SetGridIsRunning(int id, bool isRunning)
    {
        foreach (var item in m_listAttributeGrid)
        {
            if (item.Id == id)
            {
                m_listAttributeGrid[id].IsRunning = isRunning;
                break;
            }
        }
    }

    #region ����򿪺͹ر�

    void ReleasePreLoadResources()
    {
        AssetCacheMgr.ReleaseResourceImmediate("AttributeRewardGrid.prefab");
    }

    public void DestroyUIAndResources()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            ReleasePreLoadResources();
            MogoUIManager.Instance.DestroyAttributeRewardUI();
        }
    }
    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            DestroyUIAndResources();
        }
    }

    #endregion
}
