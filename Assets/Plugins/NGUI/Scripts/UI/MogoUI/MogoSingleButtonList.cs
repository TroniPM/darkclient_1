/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoSingleButtonList
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MogoSingleButtonList : MonoBehaviour
{
    public bool IsImage = true;

    public List<MogoSingleButton> SingleButtonList = new List<MogoSingleButton>();

    private List<UISprite> m_spDownImage = new List<UISprite>();
    private List<UILabel> m_lblDown = new List<UILabel>();
    public MogoSingleButtonListControl mogoSingleBtnListCtrl;

    private MogoSingleButton m_theCurrentBtn;
    public MogoSingleButton CurrentBtn
    {
        get { return m_theCurrentBtn; }
        set
        {
            m_theCurrentBtn = value;
            m_theCurrentBtn.IsDown = true;
        }
    }

    public bool IsAutoResetToFirstPage = true;

    void Awake()
    {
        if (mogoSingleBtnListCtrl != null)
        {
            foreach (var item in mogoSingleBtnListCtrl.TempSingleButtonControlList)
            {
                mogoSingleBtnListCtrl.Add(item.Key, item.Value);
            }
            mogoSingleBtnListCtrl.TempSingleButtonControlList.Clear();
            mogoSingleBtnListCtrl.SetCurrentDownButton(mogoSingleBtnListCtrl.lastSelectedId);
        }

        for (int i = 0; i < SingleButtonList.Count; ++i)
        {
            if (IsImage)
            {
                m_spDownImage.Add(SingleButtonList[i].GetComponentsInChildren<UISprite>(true)[1]);
            }
            else
            {
                if (SingleButtonList[i].GetComponentsInChildren<UILabel>(true).Length > 2)
                {
                    m_lblDown.Add(SingleButtonList[i].GetComponentsInChildren<UILabel>(true)[1]);
                }
            }
        }

        if (SingleButtonList.Count > 0)
        {
            CurrentBtn = SingleButtonList[0];			
        }

    }

    void Start()
    {
        // m_currentBtn.IsDown(true);
        if (SingleButtonList.Count > 0 && CurrentBtn == null)
        {
            CurrentBtn = SingleButtonList[0];
        }
    }

    void OnEnable()
    {

        if (IsAutoResetToFirstPage &&SingleButtonList.Count > 0)
        {
            SetCurrentDownButton(0);
        }
    }

    public void SetAllButtonUp()
    {
        if (CurrentBtn != null)
            CurrentBtn.IsDown = false;
    }

    public void SetCurrentDownButton(MogoSingleButton btn)
    {
        if (CurrentBtn == null)
            CurrentBtn = SingleButtonList[0];


        if (CurrentBtn.gameObject.activeSelf || btn.name != CurrentBtn.name)
        {
            CurrentBtn.IsDown = false;
            CurrentBtn = btn;
        }
    }

    private bool IsFirstTimeSetDown = true;
    public void SetCurrentDownButton(int gridId)
    {
        if (CurrentBtn == null)
        {
            if(SingleButtonList.Count > 0)
                CurrentBtn = SingleButtonList[0];
        }

        if (gridId < SingleButtonList.Count)
        {
            if (SingleButtonList[gridId] == CurrentBtn)
            {
                if (IsFirstTimeSetDown)
                    IsFirstTimeSetDown = false;
                else
                    return;
            }

            CurrentBtn.IsDown = false;
            CurrentBtn = SingleButtonList[gridId];
        }
    }
}


public class MogoSingleButtonListControl
{
    private MogoSingleButtonControl m_currentBtnControl;

    public Camera dragIconCamera;

    public float iconGridLocalSpace = 83;
    public float iconGridSpace
    {
        get
        {
            return iconGridLocalSpace * UIUtils.UI_SCALE;
        }
    }
    public MogoSingleButtonList mogoSingleBtnList;

    public List<MogoSingleButtonControl> SingleButtonControlList = new List<MogoSingleButtonControl>();
    public Dictionary<int, MogoSingleButtonControl> TempSingleButtonControlList = new Dictionary<int, MogoSingleButtonControl>();
    public int lastSelectedId;

    public Action<MogoSingleButtonControl, int> ItemClicked;

    public void Add(int id, MogoSingleButtonControl control)
    {
        if (mogoSingleBtnList)
        {
            control.Create(mogoSingleBtnList.transform, dragIconCamera, id, -iconGridSpace * id);
            control.Clicked += OnSingleBtnDown;
            SingleButtonControlList.Add(control);
        }
        else
            TempSingleButtonControlList.Add(id, control);
    }

    public void Clear()
    {
        foreach (var item in SingleButtonControlList)
        {
            item.Clicked -= OnSingleBtnDown;
            item.Release();
        }
    }

    public void OnSingleBtnDown(MogoSingleButtonControl btn, int id)
    {
        ItemClicked(btn, id);
        SetCurrentDownButton(btn);
    }

    public void SetCurrentDownButton(int gridId)
    {
        if (mogoSingleBtnList)
        {
            if (gridId < SingleButtonControlList.Count)
            {
                if (m_currentBtnControl != null)
                    m_currentBtnControl.IsDown = false;
                m_currentBtnControl = SingleButtonControlList[gridId];
                m_currentBtnControl.IsDown = true;
            }
        }
        else
        {
            lastSelectedId = gridId;
        }
    }

    public void SetCurrentDownButton(MogoSingleButtonControl btn)
    {
        if (m_currentBtnControl == null)
        {
            m_currentBtnControl = btn;
            m_currentBtnControl.IsDown = true;
        }
        else if (m_currentBtnControl.IsDown || btn.Id != m_currentBtnControl.Id)
        {
            m_currentBtnControl.IsDown = false;
            m_currentBtnControl = btn;
            m_currentBtnControl.IsDown = true;
        }
    }
}