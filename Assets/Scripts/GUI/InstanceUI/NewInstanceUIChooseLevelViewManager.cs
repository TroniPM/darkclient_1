using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;

public class NewInstanceUIChooseLevelGridData
{
    public string GridIcon;
    public int GridNormalLevel;
    public int GridHardLevel;
}

public class NewInstanceUIChooseLevelViewManager : MonoBehaviour
{
    Transform m_Transform;

    GameObject m_goLevelGridList;
    GameObject m_goLevelPathList;

    GameObject m_goGridTip;

    List<GameObject> m_listChooseLevelGrid = new List<GameObject>();
    List<List<GameObject>> m_listChooseLevelGridPath = new List<List<GameObject>>();
    List<GameObject> m_listChooseLevelGridPathPoint = new List<GameObject>();
    List<string> m_listGridPosData = new List<string>();

    List<GameObject> m_listPath = new List<GameObject>();

    UILabel m_lblTitleName;

    UILabel m_lblSweepNum;
    UILabel m_lblResetNum;

    GameObject m_goInstanceMap;

    List<GameObject> m_listChooseLevelGridPos = new List<GameObject>();
    List<int> m_listPathDir = new List<int>();

    UITexture m_texInstanceMap;

    const float MapWidth = 45f;
    const float MapHeight = 26f;

    const int PathPointNum = 9;

    private static NewInstanceUIChooseLevelViewManager m_instance;

    public static NewInstanceUIChooseLevelViewManager Instance
    {
        get
        {
            return NewInstanceUIChooseLevelViewManager.m_instance;
        }
    }

    #region �ؼ�����ȡ

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    public void AddWigetToFullNameData(string widgetName, string fullName)
    {
        m_widgetToFullName.Add(widgetName, fullName);
    }

    private string GetFullName(Transform currentTransform)
    {
        string fullName = "";

        while (currentTransform != m_Transform)
        {
            fullName = currentTransform.name + fullName;

            if (currentTransform.parent != m_Transform)
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

    #endregion

    void Awake()
    {
        m_Transform = transform;
        FillFullNameData(m_Transform);

        m_instance = m_Transform.GetComponentsInChildren<NewInstanceUIChooseLevelViewManager>(true)[0];

        m_goLevelGridList = m_Transform.Find(m_widgetToFullName["NewInstanceUIChooseLevelGridList"]).gameObject;
        m_goLevelPathList = m_Transform.Find(m_widgetToFullName["NewInstanceUIChooseLevelPathList"]).gameObject;

        m_lblTitleName = m_Transform.Find(m_widgetToFullName["NewInstanceUIChooseLevelTitleText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_lblSweepNum = m_Transform.Find(m_widgetToFullName["InstanceLeftChallengeNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblResetNum = m_Transform.Find(m_widgetToFullName["InstanceLeftResetNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_lblResetNum = m_Transform.Find(m_widgetToFullName["InstanceLeftResetNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_goInstanceMap = m_Transform.Find(m_widgetToFullName["NewInstanceUIChooseLevelMapBG"]).gameObject;
        m_texInstanceMap = m_goInstanceMap.GetComponentsInChildren<UITexture>(true)[0];

        for (int i = 0; i < 10; ++i)
        {
            m_listChooseLevelGridPos.Add(m_goLevelGridList.transform.Find(string.Concat("NewInstanceUIChooseLevelGridPos", i)).gameObject);

            m_listPath.Add(m_goLevelPathList.transform.Find(string.Concat("NewInstanceUIChooseLevelPath",i)).gameObject);
        }

        ResetLevelGridPos(1);

        FillNewInstanceUIChooseLevelGridData(10);
    }       

    
    #region �����߼�

    public void ResetLevelGridPos(int level)
    {
        m_listGridPosData.Clear();
        m_listPathDir.Clear();

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

        Vector3 MapOriPos = m_goInstanceMap.transform.localPosition;
        string[] listPosData;

        for (int i = 0; i < m_listGridPosData.Count; ++i)
        {
            if (m_listGridPosData[i] == "fuck")
            {
                break;
            }

            listPosData = m_listGridPosData[i].Split(',');
            //Mogo.Util.LoggerHelper.Debug(listPosData[0] + " " + listPosData[1]);
            //Mogo.Util.LoggerHelper.Debug(float.Parse(listPosData[0]) / MapWidth * 1280 + " " +  float.Parse(listPosData[1]) / MapHeight * 720);
            m_listChooseLevelGridPos[i].transform.localPosition = new Vector3(MapOriPos.x + float.Parse(listPosData[0]) / MapWidth * 1280,
                MapOriPos.y - float.Parse(listPosData[1]) / MapHeight * 720, m_listChooseLevelGridPos[i].transform.localPosition.z);

            m_listPathDir.Add(int.Parse(listPosData[2]));
        }

        ResetLevelGridPath();
    }

    private void ResetLevelGridPath()
    {
        //for (int i = 0; i < m_listChooseLevelGridPath.Count; ++i)
        //{
        //    for (int j = 0; j < m_listChooseLevelGridPath[i].Count; ++j)
        //    {
        //        AssetCacheMgr.ReleaseInstance(m_listChooseLevelGridPath[i][j]);
        //    }

        //    m_listChooseLevelGridPath[i].Clear();
        //}

        //m_listChooseLevelGridPath.Clear();

        //FillNewInstanceUIChooseLevelPath();


        for (int i = 0; i < m_listPath.Count; ++i)
        {
            ShowChooseLevelPath(i, false);
        }

        ResetLevelPath();
    }

    public void FillNewInstanceUIChooseLevelGridData(int num)
    {
        for (int i = 0; i < num; ++i)
        {
            int id = i;

            AssetCacheMgr.GetUIInstance("NewInstanceUIChooseLevelGrid.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;

                go.transform.parent = m_listChooseLevelGridPos[id].transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.name = string.Concat("NewInstanceUIChooseLevelGrid", id);

                NewInstanceGrid grid = go.transform.Find("NewInstanceUIChooseLevelGridIcon").gameObject.AddComponent<NewInstanceGrid>();
                grid.gameObject.SetActive(true);
                grid.id = id;

                m_listChooseLevelGrid.Add(go);

                if (id == num - 1)
                {
                    FillNewInstanceUIChooseLevelPath();
                }
            });
        }
    }

    public void ClearChooseLevelGridList()
    {
        //for (int i = 0; i < m_listChooseLevelGrid.Count; ++i)
        //{
        //    AssetCacheMgr.ReleaseInstance(m_listChooseLevelGrid[i]);
        //}

        //m_listChooseLevelGrid.Clear();

        for (int i = 0; i < m_listChooseLevelGrid.Count; ++i)
        {
            m_listChooseLevelGrid[i].SetActive(false);
        }
    }

    private List<Vector3> CalculatePathPoint(int StartPointID, int EndPointID)
    {
        float offset = 500;

        float dir = m_listPathDir[StartPointID];

        Vector3 startPoint = m_listChooseLevelGrid[StartPointID].transform.position;
        Vector3 endPoint = m_listChooseLevelGrid[EndPointID].transform.position;

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

    private void ResetLevelPath()
    {
        for (int i = 0; i < m_listChooseLevelGrid.Count - 1; ++i)
        {
            List<Vector3> listPoint = CalculatePathPoint(i, i + 1);
            int gridId = i;

            List<GameObject> listPathPoint = m_listChooseLevelGridPath[i];

            for (int j = 0; j < m_listChooseLevelGridPath[i].Count; ++j)
            {
                m_listChooseLevelGridPath[i][j].transform.position = listPoint[j];
            }
            //for (int j = 0; j < listPoint.Count; ++j)
            //{
            //    int pointId = j;

            //    AssetCacheMgr.GetUIInstance("NewInstanceUIChooseLevelPathPoint.prefab", (prefab, guid, gameObject) =>
            //    {
            //        GameObject go = (GameObject)gameObject;

            //        go.transform.parent = m_goLevelPathList.transform.FindChild(string.Concat("NewInstanceUIChooseLevelPath", gridId)).transform;
            //        go.transform.position = listPoint[pointId];
            //        go.transform.localScale = Vector3.one;
            //        go.name = string.Concat("NewInstanceUIChooseLevelPathPoint", gridId * PathPointNum + pointId);

            //        listPathPoint.Add(go);


            //        if (pointId == listPoint.Count - 1)
            //        {
            //            m_listChooseLevelGridPath.Add(listPathPoint);
            //        }
            //    });
            //}

            ShowChooseLevelPath(gridId, false);
        }
    }   

    private void FillNewInstanceUIChooseLevelPath()
    {
        for (int i = 0; i < m_listChooseLevelGrid.Count - 1; ++i)
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

    public void PlayChooseLevelPathAnim(int id)
    {
        if (id < 0 || id > m_listChooseLevelGrid.Count - 2)
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

    public void ShowChooseLevelPath(int id, bool isShow = true)
    {
        //m_goLevelPathList.transform.FindChild(string.Concat("NewInstanceUIChooseLevelPath", id)).gameObject.SetActive(isShow);
        m_listPath[id].SetActive(isShow);
    }

    public void ShowMarks(int gridId, int complexData)
    {
        m_listChooseLevelGrid[gridId].GetComponentsInChildren<NewInstanceGrid>(true)[0].ShowMarksByComplexData(complexData);
    }

    public void ShowMarks(int gridId, int normalLevel, int hardLevel)
    {
        m_listChooseLevelGrid[gridId].GetComponentsInChildren<NewInstanceGrid>(true)[0].ShowMarks(normalLevel, hardLevel);
    }

    public void SetGridEnable(int gridId, bool isEnable)
    {
        m_listChooseLevelGrid[gridId].GetComponentsInChildren<NewInstanceGrid>(true)[0].SetEnable(isEnable);
    }

    public void SetGridIcon(int gridId, string icon)
    {
        m_listChooseLevelGrid[gridId].GetComponentsInChildren<NewInstanceGrid>(true)[0].SetIcon(icon);
    }
    
    public void SetGridTitle(string title)
    {
        m_lblTitleName.text = title;
    }
    
    public void ShowGridTip(int gridId, bool isShow = true)
    {
        m_listChooseLevelGrid[gridId].GetComponentsInChildren<NewInstanceGrid>(true)[0].ShowGridTip(isShow);
    }
    
    public void HideGridTip()
    {
        foreach (var item in m_listChooseLevelGrid)
            item.GetComponentsInChildren<NewInstanceGrid>(true)[0].ShowGridTip(false);
    }
  
    #endregion

    /// <summary>
    /// ����ʣ��ɨ������
    /// </summary>
    /// <param name="num"></param>
    public void SetSweepNum(int num)
    {
        m_lblSweepNum.text = LanguageData.GetContent(46966) + num;
    }

    /// <summary>
    /// ����ʣ����ս���ô���
    /// </summary>
    /// <param name="num"></param>
    public void SetResetNum(string num)
    {
        m_lblResetNum.text = num;
    }

    void OnEnable()
    {
        if (!SystemSwitch.DestroyResource)
            return;
        
          if (m_texInstanceMap.mainTexture == null)
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

        m_texInstanceMap.mainTexture = null;
        AssetCacheMgr.ReleaseResourceImmediate("fb_dt.png");
    }

    //void Update()
    //{

    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        PlayChooseLevelPathAnim(index++);
    //    }
    //}
}
