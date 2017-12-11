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

public class FingerTailUIViewManager : MogoUIBehaviour
{
    private MFUIFingerTail m_MFUIFingerTail;
    private GameObject m_goFingerTailUIDrawRange;
    private GameObject m_goFingerTailUIDrawPanel;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goFingerTailUIDrawRange = FindTransform("FingerTailUIDrawRange").gameObject;

        m_goFingerTailUIDrawPanel = FindTransform("FingerTailUIDrawPanel").gameObject;
        DontDestroyOnLoad(m_goFingerTailUIDrawPanel);
        ShowFingerTailUIDrawPanel(false);
        m_goFingerTailUIDrawPanel.transform.parent = null;
        m_goFingerTailUIDrawPanel.transform.localPosition = Vector3.zero;
        m_goFingerTailUIDrawPanel.transform.localScale = new Vector3(1, 1, 1);

        m_MFUIFingerTail = m_goFingerTailUIDrawRange.AddComponent<MFUIFingerTail>();
        m_MFUIFingerTail.UICamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_MFUIFingerTail.GoTail = m_goFingerTailUIDrawPanel;
        m_MFUIFingerTail.TailWidth = 20;
        m_MFUIFingerTail.LoadResourceInsteadOfAwake();
    }

    /// <summary>
    /// 是否显示画板
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowFingerTailUIDrawPanel(bool isShow)
    {
        if (m_goFingerTailUIDrawPanel != null)
            m_goFingerTailUIDrawPanel.SetActive(isShow);
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        ShowFingerTailUIDrawPanel(true);
    }

    void OnDisable()
    {
        ShowFingerTailUIDrawPanel(false);
    }
}
