/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MyDragableCamera
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

public class MyDragableCamera : MonoBehaviour
{
    public List<Transform> transformList; // 翻页Pos列表
    public List<GameObject> ListPageDown;
    public Transform PageTopLeft;
    public Transform PageBottomRight;

    public UISprite LeftSign;
    public UISprite RightSign;  

    public bool IsHorizontal = true;
    public bool IsMovePage = true;
    public bool IsDraggingMove = false;
    public bool IsAloneMove = false;
    public bool IsNeedSHowArrow;

    public float MoveFactor = 1f;

    TweenPosition m_tp;    
    Transform m_myTransform;
    private int m_currentTransform = 0;
    public int CurrentTransform
    {
        get { return m_currentTransform; }
        set
        {
            m_currentTransform = value;
        }
    }

    /// <summary>
    /// 最大页数索引(所有创建的页数)
    /// </summary>
    public int MaxPageIndex
    {
        get
        {
            if (transformList != null)
                return transformList.Count - 1;

            return 0;
        }
    }

    public Action MovePageDone = null;

    float m_fViewPortWidth;
    float m_fViewPortHeight;

    float m_fPageWidth;
    public float FPageWidth
    {
        get { return m_fPageWidth;}
        set
        {
            m_fPageWidth = value;
        }
    }

    float m_fPageHeight;
    public float FPageHeight
    {
        get { return m_fPageHeight;}
        set
        {
            m_fPageHeight = value;
        }
    }

    Vector3 m_vec3OldCamPos;
    // 为去除警告暂时屏蔽以下代码
    //bool m_bIsAloneDone = false;
    bool m_bIsCancelAloneMove = false;

    public float m_MINY = 0f;
    public float MINY
    {
        get { return m_MINY; }
        set
        {
            m_MINY = value;

            //if (!IsOverOnePage())
            //    MINY = MAXY;
        }
    }

    public float MAXX = 0f;

    public float MAXY = 0f;
    public float MINX = 0f;

    void Awake()
    {
        m_myTransform = transform;

        if (PageTopLeft == null || PageBottomRight == null)
        {
            FPageWidth = transformList[0].localPosition.x -
                transformList[1].localPosition.x;
            FPageHeight = transformList[1].localPosition.y -
               transformList[0].localPosition.y;

            m_vec3OldCamPos = transformList[0].localPosition;

            MAXY = transformList[0].localPosition.y;
            MINX = transformList[0].localPosition.x;
        }
        else
        {
            FPageWidth = PageTopLeft.localPosition.x - PageBottomRight.localPosition.x;
            FPageHeight = PageBottomRight.localPosition.y - PageTopLeft.localPosition.y;

            m_vec3OldCamPos = PageTopLeft.localPosition;
        }

        m_tp = m_myTransform.GetComponentInChildren<TweenPosition>() as TweenPosition;
        if (IsMovePage)
        {
            m_tp.from = m_myTransform.localPosition;
            m_tp.to = m_vec3OldCamPos;
        }
    }

    void Start()
    {
        // 之前放在Awake处理,但Awake之后值可能发生改变,故移到Start处理
        m_fViewPortWidth = GetComponent<Camera>().GetComponentInChildren<Camera>().pixelWidth;
        m_fViewPortHeight = GetComponent<Camera>().GetComponentInChildren<Camera>().pixelHeight;
    }

    #region 外部通知事件

    public bool IsSetCurrentPageByChangeTab = false;

    /// <summary>
    /// 滑过最后一页
    /// </summary>
    public Action OutOfBoundsMaxPage = null;
    private void OnOutOfBoundsMaxPage()
    {
        if (OutOfBoundsMaxPage != null)
            OutOfBoundsMaxPage();
    }

    /// <summary>
    /// 滑过第一页
    /// </summary>
    public Action OutOfBoundsMinPage = null;
    private void OnOutOfBoundsMinPage()
    {
        if (OutOfBoundsMinPage != null)
            OutOfBoundsMinPage();
    }

    #endregion

    public int GetCurrentPage()
    {
        return CurrentTransform;
    }

    public void SetCurrentPage(int page)
    {
        if (page != CurrentTransform)
        {
            Mogo.Util.LoggerHelper.Debug("SetCurrentPage" + page);


            m_tp.Reset();
            m_tp.enabled = false;
            transform.localPosition = transformList[page].localPosition;
            m_vec3OldCamPos = transformList[page].localPosition;
            for (int i = 0; i < ListPageDown.Count; ++i)
            {
                ListPageDown[i].SetActive(false);
            }

            if (ListPageDown.Count > 0)
            {
                ListPageDown[page].SetActive(true);
            }

            CurrentTransform = page;
        }

        SetArrow();
        SetDOTPage();
    }

    public void DraggingMove(float length)
    {
        //Mogo.Util.LoggerHelper.Debug(length);
        if (IsDraggingMove)
        {
            //if (Math.Abs(length) > 30)
            //{
            //    m_bIsCancelAloneMove = true;
            //}
            if (IsMovePage)
            {
                if (CurrentTransform == transformList.Count - 1 || CurrentTransform == 0)
                {
                    //m_tp.Reset();
                    m_tp.enabled = false;

                    if (IsHorizontal)
                    {
                        m_myTransform.localPosition = m_myTransform.localPosition + new Vector3(length * 0.5f / m_fViewPortWidth * FPageWidth, 0, 0);
                    }
                    else
                    {
                        m_myTransform.localPosition = m_myTransform.localPosition + new Vector3(0, length * 0.5f / m_fViewPortHeight * FPageHeight, 0);
                    }
                }
                else
                {
                    //m_tp.Reset();
                    m_tp.enabled = false;

                    if (IsHorizontal)
                    {
                        m_myTransform.localPosition = m_myTransform.localPosition + new Vector3(length / m_fViewPortWidth * FPageWidth, 0, 0);
                    }
                    else
                    {
                        m_myTransform.localPosition = m_myTransform.localPosition + new Vector3(0, length / m_fViewPortHeight * FPageHeight, 0);
                    }
                }
            }
            else
            {
                //m_tp.Reset();
                m_tp.enabled = false;

                if (IsHorizontal)
                {
                    m_myTransform.localPosition = m_myTransform.localPosition + new Vector3(length / m_fViewPortWidth * FPageWidth, 0, 0);
                }
                else
                {
                    m_myTransform.localPosition = m_myTransform.localPosition + new Vector3(0, length / m_fViewPortHeight * FPageHeight, 0);
                    //Debug.LogError(length);
                    //Debug.LogError(FPageHeight);
                    //Debug.LogError(m_fViewPortHeight);
                    //Debug.LogError(camera.name);
                }

                //if (m_myTransform.localPosition.y > transformList[0].localPosition.y)
                //{
                //    m_myTransform.localPosition = transformList[0].localPosition;
                //}
                //else if (m_myTransform.localPosition.y < MINY)
                //{
                //    m_myTransform.localPosition = new Vector3(m_myTransform.localPosition.x, MINY, m_myTransform.localPosition.z);
                //}
            }
        }
    }

    void OnAloneMoveFallBack()
    {
        IsMoving(false);
    }

    void OnAloneMoveDone()
    {
        LoggerHelper.Debug("MoveDone.....");
        IsMoving(false);

        if (IsHorizontal)
        {
            if (m_myTransform.localPosition.x > MAXX)
            {
                m_tp.from = m_myTransform.localPosition;
                m_tp.to = new Vector3(MAXX, transformList[0].localPosition.y, transformList[0].localPosition.z);

                m_tp.Reset();
                m_tp.enabled = true;
                m_tp.Play(true);
                m_tp.callWhenFinished = "OnAloneMoveFallBack";
                m_tp.eventReceiver = gameObject;

                m_vec3OldCamPos = m_tp.to;
                IsMoving(true);
            }
            else if (m_myTransform.localPosition.x < MINX)
            {
                m_tp.from = m_myTransform.localPosition;
                m_tp.to = new Vector3(MINX,transformList[0].localPosition.y,transformList[0].localPosition.z);

                m_tp.Reset();
                m_tp.enabled = true;
                m_tp.Play(true);
                m_tp.callWhenFinished = "OnAloneMoveFallBack";
                m_tp.eventReceiver = gameObject;

                m_vec3OldCamPos = m_tp.to;
                IsMoving(true);
            }
        }
        else
        {
            if (m_myTransform.localPosition.y > MAXY)
            {
                m_tp.from = m_myTransform.localPosition;
                m_tp.to = new Vector3(transformList[0].localPosition.x,MAXY,transformList[0].localPosition.z);

                m_tp.Reset();
                m_tp.enabled = true;
                m_tp.Play(true);
                m_tp.callWhenFinished = "OnAloneMoveFallBack";
                m_tp.eventReceiver = gameObject;

                m_vec3OldCamPos = m_tp.to;
                IsMoving(true);
            }
            else if (m_myTransform.localPosition.y < MINY)
            {
                m_tp.from = m_myTransform.localPosition;
                m_tp.to = new Vector3(transformList[0].localPosition.x, MINY, transformList[0].localPosition.z);

                m_tp.Reset();
                m_tp.enabled = true;
                m_tp.Play(true);
                m_tp.callWhenFinished = "OnAloneMoveFallBack";
                m_tp.eventReceiver = gameObject;

                m_vec3OldCamPos = m_tp.to;
                IsMoving(true);
            }
        }
    }

    void OnMovePageDone()
    {
        IsMoving(false);
        if (MovePageDone != null)
        {
            MovePageDone();
        }

        SetArrow();
        SetDOTPage();
    }

    public void OnDraggingMoveDone()
    {
        if (IsHorizontal)
        {
            if(m_myTransform.localPosition.x < MINX)
            {                
                m_tp.from = m_myTransform.localPosition;

                if (transformList != null && transformList.Count > 0)
                {
                    m_tp.to = new Vector3(MINX, transformList[0].localPosition.y, transformList[0].localPosition.z);
                }
                else
                {
                    LoggerHelper.Error("transformList is out of bounds");
                    return;
                }

                m_tp.enabled = true;
                m_tp.Reset();
                m_tp.Play(true);

                m_vec3OldCamPos = m_tp.to;

                OnOutOfBoundsMinPage();
            }
            else if (m_myTransform.localPosition.x > MAXX)
            {
                m_tp.from = m_myTransform.localPosition;
                m_tp.to = new Vector3(MAXX,m_myTransform.localPosition.y,m_myTransform.localPosition.z);

                m_tp.enabled = true;
                m_tp.Reset();
                m_tp.Play(true);

                m_vec3OldCamPos = m_tp.to;
                OnOutOfBoundsMaxPage();
            }
            else
            {
                m_vec3OldCamPos = m_myTransform.localPosition;
            }
        }
        else
        {
            if (m_myTransform.localPosition.y > MAXY)
            {
                m_tp.from = m_myTransform.localPosition;

                if (transformList != null && transformList.Count > 0)
                {
                    m_tp.to = new Vector3(transformList[0].localPosition.x,MAXY,transformList[0].localPosition.z);
                }
                else
                {
                    LoggerHelper.Error("transformList is out of bounds");
                    return;
                }                

                m_tp.enabled = true;
                m_tp.Reset();
                m_tp.Play(true);

                m_vec3OldCamPos = m_tp.to;
                OnOutOfBoundsMinPage();
            }
            else if (m_myTransform.localPosition.y < MINY)
            {
                m_tp.from = m_myTransform.localPosition;
                m_tp.to = new Vector3(m_myTransform.localPosition.x,MINY, m_myTransform.localPosition.z);

                m_tp.enabled = true;
                m_tp.Reset();
                m_tp.Play(true);

                m_vec3OldCamPos = m_tp.to;
                OnOutOfBoundsMaxPage();
            }
            else
            {
                m_vec3OldCamPos = m_myTransform.localPosition;
            }
        }

        SetArrow();
        SetDOTPage();
    }

    public void AloneMove(Vector2 v)
    {  
        if (IsAloneMove)
        {
            if (m_bIsCancelAloneMove)
            {
                m_bIsCancelAloneMove = false;
                return;

            }
            if (IsHorizontal)
            {
                if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
                {
                    m_tp.from = m_myTransform.localPosition;

                    //if (m_myTransform.localPosition.x - v.x * MoveFactor > MAXX)
                    //{
                    //    m_tp.to = new Vector3(MAXX, transformList[0].localPosition.y, transformList[0].localPosition.z);
                    //}
                    //else if (m_myTransform.localPosition.x - v.x * MoveFactor < transformList[0].localPosition.x)
                    //{
                    //    m_tp.to = transformList[0].localPosition;
                    //}
                    //else
                    //{
                    //    m_tp.to = m_myTransform.localPosition - new Vector3(v.x * MoveFactor, 0, 0);
                    //    LoggerHelper.Debug("Here");
                    //}
                    m_tp.to = m_myTransform.localPosition - new Vector3(v.x * MoveFactor, 0, 0);
                    LoggerHelper.Debug("Here");

                    m_tp.enabled = true;
                    m_tp.Reset();
                    m_tp.Play(true);
                    m_tp.callWhenFinished = "OnAloneMoveDone";
                    m_tp.eventReceiver = gameObject;
                    m_tp.duration = 0.5f;
                    m_vec3OldCamPos = m_tp.to;
                    IsMoving(true);
                }
            }
            else
            {
                if (Mathf.Abs(v.y) > Mathf.Abs(v.x))
                {
                    m_tp.from = m_myTransform.localPosition;
                    //if (m_myTransform.localPosition.y - v.y * MoveFactor > transformList[0].localPosition.y)
                    //{
                    //    m_tp.to = transformList[0].localPosition;
                    //}
                    //else if (m_myTransform.localPosition.y - v.y * MoveFactor < MINY - m_fPageHeight)
                    //{
                    //    m_tp.to = new Vector3(transformList[0].localPosition.x, MINY - m_fPageHeight, transformList[0].localPosition.z);
                    //}
                    //else
                    //{
                    //    m_tp.to = m_myTransform.localPosition - new Vector3(0, v.y * MoveFactor, 0);

                    //}
                    m_tp.to = m_myTransform.localPosition - new Vector3(0, v.y * MoveFactor, 0);
                    m_tp.enabled = true;
                    m_tp.Reset();
                    m_tp.Play(true);
                    m_tp.callWhenFinished = "OnAloneMoveDone";
                    m_tp.eventReceiver = gameObject;
                    m_tp.duration = 0.5f;
                    m_vec3OldCamPos = m_tp.to;
                    IsMoving(true);
                }
            }
             
            //m_tp.duration = 0.5f;
            //m_bIsAloneDone = true;
        }
    }

    public void MovePage(Vector2 v)
    {
        if (IsSetCurrentPageByChangeTab)
        {
            IsSetCurrentPageByChangeTab = false;
            return;
        }

        if (IsMovePage)
        {
            m_tp.callWhenFinished = "OnMovePageDone";
            m_tp.eventReceiver = gameObject;

            if (ListPageDown.Count > 0)
            {
                ListPageDown[CurrentTransform].SetActive(false);
            }

            if (IsHorizontal)
            {
                if (v.x < 0)
                {
                    ++CurrentTransform;
                }
                else if (v.x > 0)
                {
                    --CurrentTransform;
                }
            }
            else
            {
                if (v.y > 0)
                {
                    ++CurrentTransform;
                }
                else if (v.y < 0)
                {
                    --CurrentTransform;
                }
            }

            if (CurrentTransform > transformList.Count - 1)
            {
                CurrentTransform = transformList.Count - 1;
            }
            else if (CurrentTransform < 0)
            {
                CurrentTransform = 0;
            }
            else if (0 <= CurrentTransform && CurrentTransform <= transformList.Count - 1)
            {
                //MoveTo(CurrentTransform);
            }

            MoveTo(CurrentTransform);

            if (ListPageDown.Count > 0)
            {
                ListPageDown[CurrentTransform].SetActive(true);
            }

            m_vec3OldCamPos = transformList[CurrentTransform].localPosition;
        }
        //else
        //{
        //    m_vec3OldCamPos = m_myTransform.localPosition;
        //}         
    }

    public void MoveTo(int pageId)
    {
        if (pageId >= transformList.Count)
        {
            LoggerHelper.Debug("transformList : pageId is out of bounds");
            return;
        }

        CurrentTransform = pageId;

        m_tp.from = m_myTransform.localPosition;
        m_tp.to = transformList[pageId].localPosition;

        m_tp.Reset();
        m_tp.Play(true);
        IsMoving(true);

        if (pageId < ListPageDown.Count)
        {
            for (int i = 0; i < ListPageDown.Count; ++i)
            {
                ListPageDown[i].SetActive(false);
            }

            ListPageDown[pageId].SetActive(true);
        }     
    }

    private void IsMoving(bool isMoving)
    {
        if (LeftSign && RightSign)
        {
            if (isMoving)
            {
                LeftSign.color = new Color(LeftSign.color.r, LeftSign.color.g, LeftSign.color.b, 1f);
                RightSign.color = LeftSign.color;
            }
            else
            {
                LeftSign.color = new Color(LeftSign.color.r, LeftSign.color.g, LeftSign.color.b, 0.3f);
                RightSign.color = LeftSign.color;
            }
        }        
    }

    #region 提示箭头和页点控制

    // 提示箭头
    public GameObject LeftArrow;
    public GameObject RightArrow;  

    /// <summary>
    /// 设置提示箭头
    /// </summary>
    public void SetArrow()
    {
        if (IsMovePage)
            SetArrowIsMovePageTrue(); // 目前翻页的情况有些MIN和MAX范围的值没有设准确,故不使用SetArrowIsMovePageFalse
        else
            SetArrowIsMovePageFalse();
    }

    /// <summary>
    /// 只适用于翻页
    /// </summary>
    private void SetArrowIsMovePageTrue()
    {
        if (MaxPageIndex > 0)
        {
            if (GetCurrentPage() == 0)
            {
                ShowLeftArrow(false);
                ShowRightArrow(true);
            }
            else if (GetCurrentPage() == MaxPageIndex)
            {
                ShowLeftArrow(true);
                ShowRightArrow(false);
            }
            else
            {
                ShowLeftArrow(true);
                ShowRightArrow(true);
            }
        }
        else
        {
            ShowLeftArrow(false);
            ShowRightArrow(false);
        }           
    }

    /// <summary>
    /// 适用于翻页和滑动
    /// </summary>
    private void SetArrowIsMovePageFalse()
    {
        if (!IsOverOnePage())
        {
            ShowLeftArrow(false);
            ShowRightArrow(false);
            return;
        }

        if (IsHorizontal)
        {
            if (m_myTransform.localPosition.x <= MINX)
            {
                ShowLeftArrow(false);
                ShowRightArrow(true);
            }
            else if (m_myTransform.localPosition.x >= MAXX)
            {
                ShowLeftArrow(true);
                ShowRightArrow(false);
            }
            else
            {
                ShowLeftArrow(true);
                ShowRightArrow(true);
            }
        }
        else
        {
            if (m_myTransform.localPosition.y >= MAXY)
            {
                ShowLeftArrow(false);
                ShowRightArrow(true);
            }
            else if (m_myTransform.localPosition.y <= MINY)
            {
                ShowLeftArrow(true);
                ShowRightArrow(false);
            }
            else
            {
                ShowLeftArrow(true);
                ShowRightArrow(true);
            }
        }
    }

    private void ShowLeftArrow(bool isShow)
    {
        if (LeftArrow != null)
            LeftArrow.SetActive(isShow);
    }

    private void ShowRightArrow(bool isShow)
    {
        if (RightArrow != null)
            RightArrow.SetActive(isShow);
    }

    private bool IsOverOnePage()
    {
        if (IsHorizontal)
        {
            return Math.Abs(MAXX - MINX) > 0;// Math.Abs(FPageWidth);
        }
        else
        {
            return Math.Abs(MAXY - MINY) > 0;// Math.Abs(FPageHeight);
        }
    }

    /// <summary>
    /// 设置页点显示
    /// </summary>
    public GameObject GODOTPageList = null; // 页点的父节点
    public void SetDOTPage()
    {
        if (ListPageDown != null && GODOTPageList != null)
        {
            if (MaxPageIndex > 0)
                GODOTPageList.SetActive(true);
            else
                GODOTPageList.SetActive(false);
        }
    }

    #endregion

    #region 翻页位置和页点

    /// <summary>
    /// 删除翻页位置列表
    /// </summary>
    /// <param name="myDragableCamera"></param>
    public void DestroyMovePagePosList()
    {
        if (transformList != null)
        {
            for (int i = 0; i < transformList.Count; ++i)
                Destroy(transformList[i].gameObject);

            transformList.Clear();
        }
    }

    /// <summary>
    /// 删除页点列表
    /// </summary>
    /// <param name="myDragableCamera"></param>
    public void DestroyDOTPageList()
    {
        if (ListPageDown != null)
        {
            for (int i = 0; i < ListPageDown.Count; ++i)
                Destroy(ListPageDown[i].gameObject);

            ListPageDown.Clear();
        }
    }

    #endregion

}
