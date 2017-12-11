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

public class InstanceHelpChooseUIViewManager : MogoUIBehaviour 
{
	private static InstanceHelpChooseUIViewManager m_instance;
	public static InstanceHelpChooseUIViewManager Instance { get {	 return InstanceHelpChooseUIViewManager.m_instance; } }	

	UILabel m_lblInstanceLevelChooseUITitle;
	private GameObject[] m_arrInstanceLevel = new GameObject[2];
	GameObject m_goInstanceLevelChooseUIBtnBack;
	Camera m_dragCamera;
	GameObject m_tranInstancePlayerListCamera;
    GameObject m_goInstanceHelpChooseUIPlayerListCamera;
    List<GameObject> m_listInstanceHelpPlayerGridSet = new List<GameObject>();
	List<GameObject> m_listInstanceMercenary = new List<GameObject>();

	void Awake()
	{
		m_instance = gameObject.GetComponent<InstanceHelpChooseUIViewManager>();
		m_myTransform = transform;
		FillFullNameData(m_myTransform);

        Camera temp = m_myTransform.Find(m_widgetToFullName["InstanceHelpChooseUICamera"]).GetComponentsInChildren<Camera>(true)[0];
		m_myTransform.Find(m_widgetToFullName["InstanceHelpChooseUIPlayerListCamera"]).GetComponent<UIViewport>().sourceCamera = temp;

		m_lblInstanceLevelChooseUITitle = m_myTransform.Find(m_widgetToFullName["InstanceHelpChooseUITitle"]).GetComponent<UILabel>();

        //for (int i = 0; i < 2; ++i)
        //{
        //    m_arrInstanceLevel[i] = m_myTransform.FindChild(m_widgetToFullName["InstanceHelpChooseLevel" + i]).gameObject;
        //    NewInstanceLevelGrid grid = m_arrInstanceLevel[i].AddComponent<NewInstanceLevelGrid>();
        //    grid.id = i;
        //    grid.parentNameFlag = "InstanceHelpChooseLevel";
        //    grid.MyStart();
        //}

		m_goInstanceLevelChooseUIBtnBack = m_myTransform.Find(m_widgetToFullName["InstanceHelpChooseUIBtnBack"]).gameObject;

		m_tranInstancePlayerListCamera = m_myTransform.Find(m_widgetToFullName["InstanceHelpChooseUIPlayerListGrid"]).gameObject;
		m_dragCamera = m_tranInstancePlayerListCamera.GetComponentsInChildren<Camera>(true)[0];
        m_goInstanceHelpChooseUIPlayerListCamera = m_myTransform.Find(m_widgetToFullName["InstanceHelpChooseUIPlayerListCamera"]).gameObject; ;
		Initialize();
	}

	#region 事件
	public Action INSTANCEHELPBACKUP;
	public Action<int> INSTANCESELECTPLAYER;

	public void Initialize()
	{
		InstanceUIDict.ButtonTypeToEventUp.Add("InstanceHelpChooseUIBtnBack", OnHelpUIBackUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceHelpChooseUIBtnOK", OnHelpUIBackUp);

		EventDispatcher.TriggerEvent("InstanceUILoadPartEnd");
	}

	public void Release()
	{

	}

	void OnHelpUIBackUp(int i)
	{
		if (INSTANCEHELPBACKUP != null)
			INSTANCEHELPBACKUP();
	}

	void OnHelpUISelectPlayerUp(int i)
	{
		if (INSTANCESELECTPLAYER != null)
		{
			INSTANCESELECTPLAYER(i);
		}
	}

	#endregion

	/// <summary>
	/// 设置副本名称
	/// </summary>
	/// <param name="name"></param>
	public void SetInstanceChooseGridTitle(string name)
	{
		m_lblInstanceLevelChooseUITitle.text = name;
	}

	/// <summary>
	/// 该难度是否开放
	/// </summary>
	/// <param name="gridId"></param>
	/// <param name="enable"></param>
	public void SetInstanceLevelEnable(int gridId, bool enable)
	{
		LoggerHelper.Debug("Setting InstanceLevelGridEnable!!!!!!!!!!!!!!!!!!!!");
        if (m_arrInstanceLevel != null)
		    m_arrInstanceLevel[gridId].GetComponentsInChildren<NewInstanceLevelGrid>(true)[0].SetEnable(enable);
	}

	/// <summary>
	///  设置难度选择
	/// </summary>
	/// <param name="id"></param>
	public void SetInstanceLevelChoose(int id)
	{
        if (m_arrInstanceLevel != null)
        {
            if (id == -1)
            {
                for (int i = 0; i < 2; i++)
                {
                    m_arrInstanceLevel[i].GetComponent<NewInstanceLevelGrid>().SetChoose(false);
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i == id)
                        m_arrInstanceLevel[i].GetComponent<NewInstanceLevelGrid>().SetChoose(true);
                    else
                        m_arrInstanceLevel[i].GetComponent<NewInstanceLevelGrid>().SetChoose(false);
                }
            }
        }
	}

    #region  显示助阵玩家

    readonly static int MercenaryPageCount = 3;// 每页显示助阵玩家个数

    /// <summary>
    /// 更新助阵玩家信息(一页N行，上下拖动)
    /// </summary>
    /// <param name="mercenaryInfo"></param>
    /// <returns></returns>
    //public int UpdateMercenaryList(Dictionary<int, MercenaryInfo> mercenaryInfo)
    //{
    //    Mogo.Util.LoggerHelper.Debug("friendList.Count" + mercenaryInfo.Count);

    //    ClearMercenaryList();

    //    // 重置Camera位置
    //    m_goInstanceHelpChooseUIPlayerListCamera.transform.localPosition = new Vector3(0, 0, 0);

    //    // 删除翻页位置
    //    if (m_goInstanceHelpChooseUIPlayerListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList != null)
    //    {
    //        for (int i = 0; i < m_goInstanceHelpChooseUIPlayerListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList.Count; ++i)
    //        {
    //            Destroy(m_goInstanceHelpChooseUIPlayerListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList[i].gameObject);
    //        }

    //        m_goInstanceHelpChooseUIPlayerListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList.Clear();
    //    }

    //    int count = -1;
    //    int ret = 0;
    //    foreach (var item in mercenaryInfo)
    //    {
    //        int index = ++count;
    //        int id = item.Key;
    //        var info = item.Value;
    //        AssetCacheMgr.GetUIInstance("InstanceLevelChooseUIPlayerGrid.prefab", (prefab, guid, go) =>
    //        {
    //            if (index == 0)
    //                ret = id;

    //            GameObject temp = (GameObject)go;
    //            temp.transform.parent = m_tranInstancePlayerListCamera.transform;
    //            temp.transform.localPosition = new Vector3(0.25f, -index * 0.08f + 0.268f, 100);
    //            temp.transform.localScale = new Vector3(0.00078f, 0.00078f, 1);
    //            InstanceHelperGrid theGrid = temp.AddComponent<InstanceHelperGrid>();

    //            if (theGrid != null)
    //            {
    //                theGrid.id = id;
    //                theGrid.SetHelper((Vocation)(info.vocation), info.name, info.level.ToString(), info.fight.ToString(), 0); // to do
    //            }
    //            temp.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragCamera;

    //            m_listInstanceMercenary.Add(temp);
    //            InstanceUILogicManager.Instance.CountMercenaryGridCreate();

    //            // 创建翻页位置
    //            if (index % MercenaryPageCount == 0)
    //            {
    //                GameObject trans = new GameObject();
    //                trans.transform.parent = m_goInstanceHelpChooseUIPlayerListCamera.transform;
    //                trans.transform.localPosition = new Vector3(0, index * (-0.08f), 0);
    //                trans.transform.localEulerAngles = Vector3.zero;
    //                trans.transform.localScale = new Vector3(1, 1, 1);
    //                trans.name = "MercenaryPagePos" + index / MercenaryPageCount;
    //                m_goInstanceHelpChooseUIPlayerListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList.Add(trans.transform);
    //            }
    //        });
    //    }

    //    m_dragCamera.gameObject.GetComponent<MyDragableCamera>().MAXX = (mercenaryInfo.Count - 3) * 0.25f;
    //    return ret;
    //}

	/// <summary>
	/// 更新助阵玩家信息(一页N行，左右拖动)
	/// </summary>
	/// <param name="mercenaryInfo"></param>
	/// <returns></returns>
    public int UpdateMercenaryList(Dictionary<int, MercenaryInfo> mercenaryInfo)
    {
        Mogo.Util.LoggerHelper.Debug("friendList.Count" + mercenaryInfo.Count);

        ClearMercenaryList();

        // 重置Camera位置
        m_goInstanceHelpChooseUIPlayerListCamera.transform.localPosition = new Vector3(0, 0, 0);

        // 删除翻页位置
        if (m_goInstanceHelpChooseUIPlayerListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList != null)
        {
            for (int i = 0; i < m_goInstanceHelpChooseUIPlayerListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList.Count; ++i)
            {
                Destroy(m_goInstanceHelpChooseUIPlayerListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList[i].gameObject);
            }

            m_goInstanceHelpChooseUIPlayerListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList.Clear();
            m_listInstanceHelpPlayerGridSet.Clear();
        }

        int count = -1;
        int ret = 0;
		int page = (mercenaryInfo.Count - 1) / MercenaryPageCount + 1;
        foreach (var item in mercenaryInfo)
        {
            int index = ++count;
            int id = item.Key;

            if (index == 0)
                ret = id;			
			
            if (index % MercenaryPageCount == 0)
            {
                AssetCacheMgr.GetUIInstance("InstanceHelpPlayerGridSet.prefab", (prefab, guid, go) =>
                {
                    GameObject temp = (GameObject)go;
                    temp.transform.parent = m_tranInstancePlayerListCamera.transform;
                    temp.transform.localPosition = new Vector3(index / MercenaryPageCount * 0.362f + 0.255f, 0.22f, 100);
                    temp.transform.localScale = new Vector3(0.00078f, 0.00078f, 1);
                    temp.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragCamera;
                    m_listInstanceHelpPlayerGridSet.Add(temp);                    

                    // 创建翻页位置
                    if (index % MercenaryPageCount == 0)
                    {
                        GameObject trans = new GameObject();
                        trans.transform.parent = m_goInstanceHelpChooseUIPlayerListCamera.transform;
                        trans.transform.localPosition = new Vector3(index / MercenaryPageCount * 0.362f, 0, 0);
                        trans.transform.localEulerAngles = Vector3.zero;
                        trans.transform.localScale = new Vector3(1, 1, 1);
                        trans.name = "MercenaryPagePosHorizon" + index / MercenaryPageCount;
                        m_goInstanceHelpChooseUIPlayerListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList.Add(trans.transform);
                    }

                    if (m_listInstanceHelpPlayerGridSet.Count == page)
                    {
                        SetInstanceMercenary(mercenaryInfo);
                    }
                });
            }       
        }

        m_dragCamera.gameObject.GetComponent<MyDragableCamera>().MAXX = page * 0.362f + 0.255f;
        return ret;
    }

    /// <summary>
    /// 设置助阵玩家信息
    /// </summary>
    void SetInstanceMercenary(Dictionary<int, MercenaryInfo> mercenaryInfo)
    {
        int count = -1;

        foreach (var item in mercenaryInfo)
        {
            int index = ++count;
            int id = item.Key;
            var info = item.Value;

            string name = "GOInstanceHelpPlayerGrid" + (index % MercenaryPageCount).ToString();
            GameObject temp = m_listInstanceHelpPlayerGridSet[(int)index / MercenaryPageCount].transform.Find(name).gameObject;
            temp.SetActive(true);
            temp.transform.parent = m_listInstanceHelpPlayerGridSet[(int)index / MercenaryPageCount].transform;
            temp.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragCamera;
            InstanceHelperGrid theGrid = temp.AddComponent<InstanceHelperGrid>();            

            if (theGrid != null)
            {
                theGrid.id = id;
                theGrid.SetHelper((Vocation)(info.vocation), info.name, info.level.ToString(), info.fight.ToString(), 0); // to do
            }

            m_listInstanceMercenary.Add(temp);
            InstanceUILogicManager.Instance.CountMercenaryGridCreate();
        }
    }

    #endregion

    /// <summary>
	/// 清除助阵玩家信息
	/// </summary>
	public void ClearMercenaryList()
	{
		foreach (GameObject go in m_listInstanceMercenary)
		{
			Destroy(go);
		}

		m_listInstanceMercenary.Clear();
	}

	/// <summary>
	/// 选择助阵玩家
	/// </summary>
	/// <param name="id"></param>
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

	/// <summary>
	/// 设置选择助阵玩家
	/// </summary>
	/// <param name="id"></param>
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

		OnHelpUISelectPlayerUp(id);
	}

	/// <summary>
	/// Test
	/// </summary>
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Dictionary<int, MercenaryInfo> mercenaryInfo = new Dictionary<int, MercenaryInfo>();
    //        for (int i = 0; i < 17; i++)
    //        {
    //            MercenaryInfo mi = new MercenaryInfo();
    //            mercenaryInfo.Add(i, mi);
    //        }

    //        UpdateMercenaryList(mercenaryInfo);
    //    }
    //}   
}
