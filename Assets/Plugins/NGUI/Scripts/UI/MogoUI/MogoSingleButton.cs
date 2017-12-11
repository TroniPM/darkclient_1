/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoSingleButton
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Mogo.Util;

public class MogoSingleButton : MonoBehaviour
{
    //	GameObject m_lblText;
    private GameObject m_bgDown;
    private GameObject m_bgUp;    
    //public  MogoSingleButtonList m_btnList;
    private MogoSingleButtonControl m_mogoSingleBtnCtrl;
    private bool m_bIsDraging;
    private bool m_isDown;
    public bool isImage = true;

    public GameObject BGDown;
    public GameObject BGUp;

    public UILabel m_lblName; // Up状态的Label
    public GameObject TextFG; // Down状态的Label
    public GameObject TextBG;

    public Vector3 Vec3Offset = new Vector3(0, 2, 0);

    public Transform ButtonListTransform;
    public bool DragCancel = false;
    /// <summary>
    /// 为兼容久逻辑临时保留，应该直接调用 MogoSingleButtonControl 的 IsDown，用 MogoSingleButtonControl 管理。
    /// </summary>
    public bool IsDown
    {
        get
        {
            if (m_mogoSingleBtnCtrl != null)
                return m_mogoSingleBtnCtrl.IsDown;
            else
                return m_isDown;
        }
        set
        {
            if (m_mogoSingleBtnCtrl != null)
                m_mogoSingleBtnCtrl.IsDown = value;
            else
            {
                m_isDown = value;
                SetIsDown(value);
            }
        }
    }

    protected virtual void OnClicked()
    {
        if (m_mogoSingleBtnCtrl != null && m_mogoSingleBtnCtrl.Clicked != null)
            m_mogoSingleBtnCtrl.Clicked(m_mogoSingleBtnCtrl, m_mogoSingleBtnCtrl.Id);
        else if (ButtonListTransform == null)
        {
            if (transform.parent.GetComponentsInChildren<MogoSingleButtonList>(true).Length > 0)
            {
                transform.parent.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(this);
            }
        }
        else
        {
            ButtonListTransform.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(this);
        }
    }

    void OnDrag()
    {
        m_bIsDraging = true;
    }

    void Awake()
    {

        if (isImage)
        {
            var ssList = transform.GetComponentsInChildren<UISprite>(true);

            if (BGDown != null)
            {
                m_bgDown = BGDown;
            }
            else
            {

                m_bgDown = ssList[1].gameObject;
            }

            if (BGUp != null)
            {
                m_bgUp = BGUp;
            }
            else
            {
                m_bgUp = ssList[0].gameObject;
            }
        }
        else
        {
            var lblList = transform.GetComponentsInChildren<UILabel>(true);

            if (BGDown != null)
            {
                m_bgDown = BGDown;
            }
            else
            {
                m_bgDown = lblList[1].gameObject;
            }
        }

        if (m_lblName == null)
            m_lblName = GetComponentsInChildren<UILabel>(true).GetFirstOrDefault();
    }

    void Start()
    {
        if (m_mogoSingleBtnCtrl != null)
        {
            SetName(m_mogoSingleBtnCtrl.Content);
            SetIsDown(m_mogoSingleBtnCtrl.IsDown);
        }
    }

    //void OnPress(bool isPressed)
    //{
    //    if (m_bIsDraging)
    //    {
    //        if (!isPressed)
    //            m_bIsDraging = false;
    //    }
    //    else
    //    {
    //        if (!isPressed && !m_bgDown.activeSelf)
    //            OnClicked();
    //    }
    //}

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {

            m_bgDown.transform.parent.localPosition -= Vec3Offset;

            EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, gameObject.name);

        }
        else
        {
            m_bgDown.transform.parent.localPosition += Vec3Offset;

            EventDispatcher.TriggerEvent(SettingEvent.UIUpPlaySound, gameObject.name);

            if(!DragCancel)
            {
                if (!m_bgDown.activeSelf)
            OnClicked();
            }
            else
            {
                if(!m_bIsDraging)
                {
                    if (!m_bgDown.activeSelf)
            OnClicked();
                }
            }
            m_bIsDraging = false;
        }
    }

    void OnClick()
    {
        if(DragCancel)
            return;

        if (!m_bgDown.activeSelf)
            OnClicked();
    }

    public void SetControl(MogoSingleButtonControl control)
    {
        m_mogoSingleBtnCtrl = control;
    }

    public void SetIsDown(bool isDown)
    {
        if (m_bgDown)
            m_bgDown.SetActive(isDown);

        if (TextFG)
        {
            TextFG.SetActive(isDown);

        }
        if (TextBG)
        {
            TextBG.SetActive(!isDown);

        }

        if (TextFG && m_lblName)
        {
            m_lblName.gameObject.SetActive(!isDown);
        }
    }

    public void SetName(string name)
    {
        if (m_lblName)
            m_lblName.text = name;
    }
}

public class MogoSingleButtonControl
{
    private int m_id;
    private String m_content;
    public bool isImage = true;
    private bool m_isDown;
    protected MogoSingleButton m_mogoSingleBtn;
    protected List<Action<GameObject>> subControls = new List<Action<GameObject>>();

    public int Id
    {
        get { return m_id; }
        set { m_id = value; }
    }

    /// <summary>
    /// 按钮显示文本。
    /// </summary>
    public String Content
    {
        get { return m_content; }
        set
        {
            m_content = value;
            SetName(value);
        }
    }

    /// <summary>
    /// 是否显示图标。
    /// </summary>
    public bool IsImage
    {
        get { return isImage; }
        set { isImage = value; }
    }

    /// <summary>
    /// 是否显示按下状态
    /// </summary>
    public bool IsDown
    {
        get { return m_isDown; }
        set
        {
            m_isDown = value;
            SetIsDown(value);
        }
    }

    public Action<MogoSingleButtonControl, int> Clicked;

    public virtual void Create(Transform parent, Camera dragCamera, int id, float positionY)
    {
        //Id = id;
        //AssetCacheMgr.GetUIInstance("ChooseServerGrid.prefab", (prefab, guid, go) =>
        //{
        //    GameObject obj = (GameObject)go;
        //    obj.transform.parent = parent;
        //    obj.transform.localPosition = new Vector3(0, positionY, 0);
        //    obj.transform.localScale = new Vector3(UIUtils.UI_SCALE, UIUtils.UI_SCALE, 1);
        //    obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = dragCamera;
        //    obj.name = id.ToString();

        //    m_mogoSingleBtn = obj.GetComponentsInChildren<MogoSingleButton>(true).GetFirstOrDefault();
        //    m_mogoSingleBtn.SetControl(this);
        //    foreach (var action in subControls)
        //    {
        //        action(obj);
        //    }
        //    //mogoSingleBtn.Clicked += OnMogoSingleBtnClicked;
        //    //m_mogoSingleBtnList.SingleButtonList.Add(mogoSingleBtn);
        //});
    }

    public void Release()
    {
        AssetCacheMgr.ReleaseInstance(m_mogoSingleBtn.gameObject);
        subControls.Clear();
    }

    private void SetIsDown(bool isDown)
    {
        if (m_mogoSingleBtn)
            m_mogoSingleBtn.SetIsDown(isDown);
    }

    private void SetName(string name)
    {
        if (m_mogoSingleBtn)
            m_mogoSingleBtn.SetName(name);
    }
}
