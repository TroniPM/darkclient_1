#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：一个大关，包含最多9个小关，用于拖曳
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;

public class InstanceMapWithMissionGrid : MogoUIBehaviour 
{
    private GameObject m_goLevelGridList; // 关卡选择
    private GameObject m_goLevelPathList; // 关卡路径
    private const int PathPointNum = 9; // 一个路径有9个路径点构成

    private const int MaxNormalMissionIndex = 8; // [普通关卡][9](index:0~8)
    public const int FoggyAbyssMissionIndex = MaxNormalMissionIndex + 1; // [特殊关卡-迷雾深渊][1](index:9)
    
    private const float MapWidth = 45f;
    private const float MapHeight = 26f;

    private Dictionary<int, GameObject> m_maplistChooseLevelGrid = new Dictionary<int, GameObject>(); // 关卡列表
    private List<GameObject> m_listPath = new List<GameObject>(); // 自动生成路径列表
    private List<GameObject> m_listChooseLevelGridPos = new List<GameObject>(); // 路径点(配表)
    private List<List<GameObject>> m_listChooseLevelGridPath = new List<List<GameObject>>();
    private List<int> m_listPathDir = new List<int>();
    private List<string> m_listGridPosData = new List<string>();
    private UITexture m_texInstanceMap = null;

    private UILabel m_lblTitleName;
    private GameObject m_goInstanceMap;

    private GameObject m_goNewInstanceBossChestButton;
    private GameObject m_goNewInstanceMapChestButton;

    void Awake()
    {
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goLevelGridList = m_myTransform.Find(m_widgetToFullName["NewInstanceUIChooseLevelGridList"]).gameObject;
        m_goLevelPathList = m_myTransform.Find(m_widgetToFullName["NewInstanceUIChooseLevelPathList"]).gameObject;

        m_lblTitleName = m_myTransform.Find(m_widgetToFullName["NewInstanceUIChooseLevelTitleText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_goInstanceMap = m_myTransform.Find(m_widgetToFullName["NewInstanceUIChooseLevelMapBG"]).gameObject;
        m_texInstanceMap = m_goInstanceMap.GetComponentsInChildren<UITexture>(true)[0];

        m_goNewInstanceBossChestButton = FindTransform("NewInstanceBossChestButton").gameObject;
        m_goNewInstanceMapChestButton = FindTransform("NewInstanceMapChestButton").gameObject;

        for (int i = 0; i < 10; ++i)
        {
            m_listChooseLevelGridPos.Add(m_goLevelGridList.transform.Find(string.Concat("NewInstanceUIChooseLevelGridPos", i)).gameObject);
            m_listPath.Add(m_goLevelPathList.transform.Find(string.Concat("NewInstanceUIChooseLevelPath", i)).gameObject);
        }

        //ResetLevelGridPos(1);
        //FillNewInstanceUIChooseLevelGridData(10);
    }

    #region 界面信息

    /// <summary>
    /// 设置关卡地图标题
    /// </summary>
    /// <param name="title"></param>
    public void SetMissionMapTitle(string title)
    {
        m_lblTitleName.text = title;
    }

    #endregion

    #region 指向路径

    /// <summary>
    /// 重置路径
    /// </summary>
    private void ResetLevelGridPath()
    {
        for (int i = 0; i < m_listPath.Count; ++i)
        {
            ShowChooseLevelPath(i, false);
        }

        ResetLevelPath();
    }

    /// <summary>
    /// 重新计算[普通副本]路径点
    /// </summary>
    private void ResetLevelPath()
    {
        for (int i = 0; i <= MaxNormalMissionIndex; ++i)
        {
            if (m_maplistChooseLevelGrid.ContainsKey(i))
            {
                List<Vector3> listPoint = CalculatePathPoint(i, i + 1);
                int gridId = i;

                List<GameObject> listPathPoint = m_listChooseLevelGridPath[i];

                for (int j = 0; j < m_listChooseLevelGridPath[i].Count; ++j)
                {
                    m_listChooseLevelGridPath[i][j].transform.position = listPoint[j];
                }

                ShowChooseLevelPath(gridId, false);
            }          
        }
    }   

    /// <summary>
    /// 播放路径指向动画(自动生成路径指向点)
    /// 路径指向播放动画只用于[普通关卡]
    /// </summary>
    /// <param name="id"></param>
    public void PlayChooseLevelPathAnim(int id)
    {
        if (!(id >= 0 && id <= MaxNormalMissionIndex))
            return;

        ShowChooseLevelPath(id, true);

        for (int i = 0; i < m_listChooseLevelGridPath[id].Count; ++i)
        {
            m_listChooseLevelGridPath[id][i].SetActive(false);
        }

        for (int i = 0; i < PathPointNum; ++i)
        {
            int index = i;
            int idIndex = id;
            TimerHeap.AddTimer((uint)index * 200, 0, () =>
            {
                m_listChooseLevelGridPath[idIndex][index].SetActive(true);
                if (index == PathPointNum - 1)
                {
                    EventDispatcher.TriggerEvent<int>("ChooseLevelPathAnimPlayDone", idIndex);
                }
            });
        }
    }

    /// <summary>
    /// 是否显示路径指向点
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isShow"></param>
    public void ShowChooseLevelPath(int id, bool isShow = true)
    {
        m_listPath[id].SetActive(isShow);
    }

    /// <summary>
    /// 自动填充路径点
    /// </summary>
    private void FillNewInstanceUIChooseLevelPath()
    {
        for (int i = 0; i <= MaxNormalMissionIndex; ++i)
        {
            List<Vector3> listPoint = CalculatePathPoint(i, i + 1);
            int gridId = i;

            List<GameObject> listPathPoint = new List<GameObject>();
            for (int j = 0; j < listPoint.Count; ++j)
            {
                int pointId = j;

                AssetCacheMgr.GetUIInstance("NewInstanceUIChooseLevelPathPoint.prefab", (prefab, guid, gameObject) =>
                {
                    GameObject go = (GameObject)gameObject;

                    go.transform.parent = m_goLevelPathList.transform.Find(string.Concat("NewInstanceUIChooseLevelPath", gridId)).transform;
                    go.transform.position = listPoint[pointId];
                    go.transform.localScale = Vector3.one;
                    go.name = string.Concat("NewInstanceUIChooseLevelPathPoint", gridId * PathPointNum + pointId);

                    listPathPoint.Add(go);


                    if (pointId == listPoint.Count - 1)
                    {
                        m_listChooseLevelGridPath.Add(listPathPoint);
                    }
                });
            }

            ShowChooseLevelPath(gridId, false);
        }
    }

    /// <summary>
    /// 计算路径点
    /// </summary>
    /// <param name="StartPointID"></param>
    /// <param name="EndPointID"></param>
    /// <returns></returns>
    private List<Vector3> CalculatePathPoint(int StartPointID, int EndPointID)
    {
        float offset = 500 * 0.0008f * 0.6f;
        float dir = m_listPathDir[StartPointID];

        Vector3 startPoint = m_maplistChooseLevelGrid[StartPointID].transform.position;
        Vector3 endPoint = m_maplistChooseLevelGrid[EndPointID].transform.position;

        Vector3 centerPoint = (startPoint + endPoint) * 0.5f;
        Vector3 ctrlPoint = centerPoint;

        float k0 = (endPoint.y - startPoint.y) / (endPoint.x - startPoint.x);
        float k1 = -1.0f / k0;

        ctrlPoint.x = centerPoint.x + dir * Mathf.Sqrt((offset * offset) / (1 + k1 * k1));
        ctrlPoint.y = k1 * ctrlPoint.x + centerPoint.y - k1 * centerPoint.x;

        List<Vector3> listPoint = new List<Vector3>();

        for (int i = 0; i < PathPointNum; ++i)
        {
            float t = 0 + 1f / PathPointNum * i;
            listPoint.Add((1 - t) * (1 - t) * startPoint + 2 * t * (1 - t) * ctrlPoint + t * t * endPoint);
        }

        return listPoint;
    }

    #endregion

    #region 关卡信息设置

    /// <summary>
    /// 重新设置关卡位置,包括[普通关卡]和[特殊关卡-迷雾深渊]
    /// </summary>
    /// <param name="level"></param>
    public void ResetNewInstanceUIChooseLevelGridPos(int level)
    {
        m_listGridPosData.Clear();
        m_listPathDir.Clear();

        // XML位置数据
        m_listGridPosData.Add(InstanceLevelGridPosData.dataMap[level].grid0Pos);
        m_listGridPosData.Add(InstanceLevelGridPosData.dataMap[level].grid1Pos);
        m_listGridPosData.Add(InstanceLevelGridPosData.dataMap[level].grid2Pos);
        m_listGridPosData.Add(InstanceLevelGridPosData.dataMap[level].grid3Pos);
        m_listGridPosData.Add(InstanceLevelGridPosData.dataMap[level].grid4Pos);
        m_listGridPosData.Add(InstanceLevelGridPosData.dataMap[level].grid5Pos);
        m_listGridPosData.Add(InstanceLevelGridPosData.dataMap[level].grid6Pos);
        m_listGridPosData.Add(InstanceLevelGridPosData.dataMap[level].grid7Pos);
        m_listGridPosData.Add(InstanceLevelGridPosData.dataMap[level].grid8Pos);
        m_listGridPosData.Add(InstanceLevelGridPosData.dataMap[level].grid9Pos);

        Vector3 MapOriPos = new Vector3(-640, 320, 0);
        string[] listPosData;

        for (int i = 0; i < m_listGridPosData.Count; ++i)
        {
            if (m_listGridPosData[i] == "fuck")
            {
                break;
            }

            // XML位置数据解析
            listPosData = m_listGridPosData[i].Split(',');

            // 位置数据解析完毕，设置关卡位置
            m_listChooseLevelGridPos[i].transform.localPosition = new Vector3(MapOriPos.x + float.Parse(listPosData[0]) / MapWidth * 1280,
                MapOriPos.y - float.Parse(listPosData[1]) / MapHeight * 720, m_listChooseLevelGridPos[i].transform.localPosition.z);
            //Mogo.Util.LoggerHelper.Debug(listPosData[0] + " " + listPosData[1]);
            //Mogo.Util.LoggerHelper.Debug(float.Parse(listPosData[0]) / MapWidth * 1280 + " " +  float.Parse(listPosData[1]) / MapHeight * 720);

            m_listPathDir.Add(int.Parse(listPosData[2]));
        }

        // 重置路径
        ResetLevelGridPath();
    }

    /// <summary>
    /// 创建关卡,包括[普通关卡]和[特殊关卡-迷雾深渊]
    /// </summary>
    /// <param name="num"></param>
    public void FillNewInstanceUIChooseLevelGridData(int num, int MapWithMissionGridIndex)
    {
        for (int i = 0; i < num; ++i)
        {
            int index = i;
            if (index <= MaxNormalMissionIndex)
            {
                // [普通副本]
                AssetCacheMgr.GetUIInstance("NewInstanceUIChooseLevelGrid.prefab", (prefab, guid, gameObject) =>
                {
                    GameObject goMissionGrid = (GameObject)gameObject;

                    goMissionGrid.transform.parent = m_listChooseLevelGridPos[index].transform;
                    goMissionGrid.transform.localPosition = Vector3.zero;
                    goMissionGrid.transform.localScale = Vector3.one;
                    goMissionGrid.name = string.Concat("NewInstanceUIChooseLevelGrid", MapWithMissionGridIndex, index);

                    NewInstanceGrid gridUI = goMissionGrid.transform.Find("NewInstanceUIChooseLevelGridIcon").gameObject.AddComponent<NewInstanceGrid>();
                    gridUI.gameObject.SetActive(true);
                    gridUI.id = index;
                    goMissionGrid.SetActive(false);
                    m_maplistChooseLevelGrid[index] = goMissionGrid;

                    if (m_maplistChooseLevelGrid.Count == num)
                    {
                        // 自动填充路径点
                        FillNewInstanceUIChooseLevelPath();
                    }
                });
            }
            else if (index == FoggyAbyssMissionIndex)
            {
                // [特殊副本-迷雾深渊]
                AssetCacheMgr.GetUIInstance("InstanceMissionFoggyAbyssGrid.prefab", (prefab, guid, gameObject) =>
                {
                    GameObject goMissionGrid = (GameObject)gameObject;

                    goMissionGrid.transform.parent = m_listChooseLevelGridPos[index].transform;
                    goMissionGrid.transform.localPosition = Vector3.zero;
                    goMissionGrid.transform.localScale = Vector3.one;
                    goMissionGrid.name = string.Concat("NewInstanceUIChooseLevelGrid", MapWithMissionGridIndex, index);

                    InstanceMissionFoggyAbyssGrid gridUI = goMissionGrid.AddComponent<InstanceMissionFoggyAbyssGrid>();
                    goMissionGrid.SetActive(false);
                    gridUI.LoadResourceInsteadOfAwake();

                    m_maplistChooseLevelGrid[index] = goMissionGrid;

                    if (m_maplistChooseLevelGrid.Count == num)
                    {
                        // 自动填充路径点
                        FillNewInstanceUIChooseLevelPath();
                    }
                });
            }            
        }
    }

    #region 普通关卡

    /// <summary>
    /// 隐藏所有[普通关卡]
    /// </summary>
    public void HideNewInstanceUIChooseLevelGridList()
    {
        for (int i = 0; i < m_maplistChooseLevelGrid.Count && i <= MaxNormalMissionIndex; ++i)
        {
            m_maplistChooseLevelGrid[i].SetActive(false);
        }
    }

    /// <summary>
    /// 设置[普通关卡]是否开启
    /// </summary>
    /// <param name="gridId"></param>
    /// <param name="isEnable"></param>
    public void SetMissionNormalGridEnable(int gridId, bool isEnable)
    {
        if (gridId <= MaxNormalMissionIndex && m_maplistChooseLevelGrid.ContainsKey(gridId))
        {
            m_maplistChooseLevelGrid[gridId].SetActive(true);
            m_maplistChooseLevelGrid[gridId].GetComponentsInChildren<NewInstanceGrid>(true)[0].SetEnable(isEnable);
        }
    }

    /// <summary>
    /// 设置[普通关卡]图标Icon
    /// </summary>
    /// <param name="gridId"></param>
    /// <param name="icon"></param>
    public void SetMissionNormalGridIcon(int gridId, string icon)
    {
        if (gridId <= MaxNormalMissionIndex && m_maplistChooseLevelGrid.ContainsKey(gridId))
        {
            m_maplistChooseLevelGrid[gridId].GetComponentsInChildren<NewInstanceGrid>(true)[0].SetIcon(icon);
        }
    }

    /// <summary>
    /// 设置[普通关卡]星级
    /// </summary>
    /// <param name="gridId"></param>
    /// <param name="complexData"></param>
    public void ShowMissionNormalMarks(int gridId, int complexData)
    {
        if (gridId <= MaxNormalMissionIndex && m_maplistChooseLevelGrid.ContainsKey(gridId))
            m_maplistChooseLevelGrid[gridId].GetComponentsInChildren<NewInstanceGrid>(true)[0].ShowMarksByComplexData(complexData);
    }

    /// <summary>
    /// 设置[普通关卡]星级
    /// </summary>
    /// <param name="gridId"></param>
    /// <param name="normalLevel"></param>
    /// <param name="hardLevel"></param>
    public void ShowMissionNormalMarks(int gridId, int normalLevel, int hardLevel)
    {
        if (gridId <= MaxNormalMissionIndex && m_maplistChooseLevelGrid.ContainsKey(gridId))
            m_maplistChooseLevelGrid[gridId].GetComponentsInChildren<NewInstanceGrid>(true)[0].ShowMarks(normalLevel, hardLevel);
    }

    /// <summary>
    /// 显示[普通关卡]当前任务提示(当前任务,掉落副本)
    /// </summary>
    /// <param name="gridId"></param>
    /// <param name="isShow"></param>
    public void ShowMissionNormalGridTip(int gridId, bool isShow = true, int tipTextID = 0)
    {
        if (gridId <= MaxNormalMissionIndex && m_maplistChooseLevelGrid.ContainsKey(gridId))
            m_maplistChooseLevelGrid[gridId].GetComponentsInChildren<NewInstanceGrid>(true)[0].ShowGridTip(isShow, tipTextID);
    }

    /// <summary>
    /// 隐藏[普通关卡]当前任务提示(当前任务,掉落副本)
    /// </summary>
    public void HideMissionNormalGridTip()
    {
        for (int index = 0; index <= MaxNormalMissionIndex; index++)
        {
            if (m_maplistChooseLevelGrid.ContainsKey(index))
                m_maplistChooseLevelGrid[index].GetComponentsInChildren<NewInstanceGrid>(true)[0].ShowGridTip(false);
        }
    }

    #endregion

    #region 特殊关卡

    /// <summary>
    /// 是否显示特殊关卡
    /// </summary>
    /// <param name="isShow">是否显示</param>
    public void ShowFoggyAbyssMission(bool isShow, int iMark)
    {
        if (m_maplistChooseLevelGrid.ContainsKey(FoggyAbyssMissionIndex))     
        {
            m_maplistChooseLevelGrid[FoggyAbyssMissionIndex].SetActive(isShow);
            m_maplistChooseLevelGrid[FoggyAbyssMissionIndex].GetComponentsInChildren<InstanceMissionFoggyAbyssGrid>(true)[0].SetMarks(iMark);
        }
    }

    /// <summary>
    /// 显示或隐藏[特殊关卡-迷雾深渊]悬浮提示
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowMissionFoggyAbyssGridTip(bool isShow)
    {
        if (m_maplistChooseLevelGrid.ContainsKey(FoggyAbyssMissionIndex))
        {
            InstanceMissionFoggyAbyssGrid gridUI = m_maplistChooseLevelGrid[FoggyAbyssMissionIndex].GetComponentsInChildren<InstanceMissionFoggyAbyssGrid>(true)[0];
            gridUI.ShowInstanceMissionFoggyAbyssGridTip(isShow);
        }
    }   

    #endregion

    #endregion

    #region 地图宝箱和Boss宝箱动画播放

    /// <summary>
    /// 是否播放地图宝箱可领取振动动画
    /// </summary>
    /// <param name="isFinished"></param>
    public void ShowMapChestRotationAnimation(bool isFinished)
    {
        TweenRotation tr = m_goNewInstanceMapChestButton.GetComponentsInChildren<TweenRotation>(true)[0];
        tr.enabled = isFinished;

        if (!isFinished)        
        {
            m_goNewInstanceMapChestButton.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
    }

    /// <summary>
    /// 是否播放Boss宝箱可领取振动动画
    /// </summary>
    /// <param name="isFinished"></param>
    public void ShowBossChestRotationAnimation(bool isFinished)
    {
        TweenRotation tr = m_goNewInstanceBossChestButton.GetComponentsInChildren<TweenRotation>(true)[0];
        tr.enabled = isFinished;

        if (!isFinished)
        {
            m_goNewInstanceBossChestButton.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
    }

    #endregion

    #region 界面打开和关闭

    protected override void OnEnable()
    {
        base.OnEnable();

        if (!SystemSwitch.DestroyResource)
            return;

        if (m_texInstanceMap != null && m_texInstanceMap.mainTexture == null)
        {
            AssetCacheMgr.GetResourceAutoRelease("fb_dt.png", (obj) =>
            {
                m_texInstanceMap.mainTexture = (Texture)obj;
            });
        }
    }

    void OnDisable()
    {
        if (!SystemSwitch.DestroyResource)
            return;

        if (m_texInstanceMap != null)
        {
            m_texInstanceMap.mainTexture = null;
            AssetCacheMgr.ReleaseResourceImmediate("fb_dt.png");
        }
    }

    #endregion       
}
