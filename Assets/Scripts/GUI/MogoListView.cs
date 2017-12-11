/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoListView
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：列表控件
 * 用法：
 * 1.先调用 m_listView = new MogoListView(m_dragableCamera, listRoot, dragCameraBegin, GRID_PREFAB_NAME,
            true, GRID_GAP, GRID_NUM_PER_PAGE);
 * 2.需要动态添加perfab时
 *    m_listView.AddList(null,
            list.Count,
            (index, go) =>
            {
               //加载回调
            });
 * 参考：MogoListView
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;
using System;

public class MogoListView
{
    private Transform m_tranDragableCamera;
    private Transform m_listRoot;
    private Vector3 m_dragCameraBeginPos;
    private string m_prefabName;
    private bool m_isHorizontal;
    private int m_gap;
    private int m_numPerPage;

    private List<GameObject> m_objList = new List<GameObject>();

    public MogoListView
        (Transform dragableCamera, Transform listRoot, Transform dragCameraBegin,
        string prefabName, bool isHorizontal, int gap, int numPerPage, List<GameObject> objList)
    {
        m_tranDragableCamera = dragableCamera;
        m_listRoot = listRoot;
        m_dragCameraBeginPos = dragCameraBegin.transform.localPosition;
        m_prefabName = prefabName;
        m_isHorizontal = isHorizontal;
        m_gap = gap;
        m_numPerPage = numPerPage;
        m_objList = objList;    
    }

    public void ClearOldObjList()
    {
        foreach (GameObject go in m_objList)
        {
            AssetCacheMgr.ReleaseInstance(go);
        }

        m_objList.Clear();
    }

    public void AddList(MogoSingleButtonList btnList, int count, Action<int, GameObject> onLoadObj)
    {
        ClearOldObjList();        

        if (btnList != null)
        {
            btnList.SingleButtonList = new List<MogoSingleButton>();
        }

        for (int i = 0; i < count; i++)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance(m_prefabName, (str, id, obj) =>
            {
                GameObject go = obj as GameObject;
                Utils.MountToSomeObjWithoutPosChange(go.transform, m_listRoot);
                if (m_isHorizontal)
                {
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x + index * m_gap, go.transform.localPosition.y, go.transform.localPosition.z);
                }
                else
                {
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y - index * m_gap, go.transform.localPosition.z);
                }

                m_objList.Add(go);
                go.GetComponent<MyDragCamera>().RelatedCamera = m_tranDragableCamera.GetComponent<Camera>();
                onLoadObj(index, go);
            });
        }

        //重置摄像机
        ResetListCamera(count);
    }

    public void ResetListCamera(int gridNum)
    {
        MyDragableCamera dragableCamera = m_tranDragableCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        dragableCamera.DestroyMovePagePosList(); // 删除翻页点

        int temp = (gridNum - 1);
        if (temp < 0) temp = 0;
        int pageNum = (int)(temp / m_numPerPage) + 1;

        m_tranDragableCamera.GetComponent<TweenPosition>().to = m_dragCameraBeginPos;
        m_tranDragableCamera.localPosition = m_dragCameraBeginPos;

        dragableCamera.transformList = new List<Transform>();
        for (int i = 0; i < pageNum; i++)
        {
            GameObject go = new GameObject();
            go.name = string.Concat("CameraMovePagePos", i);

            Utils.MountToSomeObjWithoutPosChange(go.transform, m_listRoot);

            if (m_isHorizontal)
            {
                go.transform.localPosition = new Vector3(m_dragCameraBeginPos.x + m_gap * m_numPerPage * i, m_dragCameraBeginPos.y, m_dragCameraBeginPos.z);
            }
            else
            {
                go.transform.localPosition = new Vector3(m_dragCameraBeginPos.x, m_dragCameraBeginPos.y - m_gap * m_numPerPage * i, m_dragCameraBeginPos.z);
            }

            dragableCamera.transformList.Add(go.transform);
            dragableCamera.SetArrow();
        }     
    }
}