/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：FumoUIViewManager
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;
using System;
using Mogo.GameData;

public class FumoUIViewManager : MogoUIParent
{
    public static FumoUIViewManager Instance;

    GameObject m_goReplace;
    UILabel m_lblReplaceNewAttr;
    UILabel m_lblReplaceOlfAttr;
    MogoButton m_btnReplaceOk;
    MogoButton m_btnReplaceCancel;

    GameObject m_goFull;
    UILabel m_lblFullNewAttr;
    UILabel m_lblFullTip;
    List<GameObject> m_gridList;
    List<UILabel> m_lblFullOlfAttrList;
    List<MogoButton> m_btnReplaceOkList;
    MogoButton m_btnFullCancel;
    private UILabel m_lblReplaceTip;
    private UILabel m_lblFullTip2;
    private UILabel m_lblFullCancelBtnName;

    GameObject m_goProcess;
    UILabel m_lblProcessTitle;
    List<GameObject> m_tipList;
    List<UILabel> m_tipLblList;
    List<TweenAlpha> m_tipTweenList;
    MogoButton m_btnProcessCancel;
    UILabel m_btnProcessName;

    void Awake()
    {
        Instance = this;
        base.Init();

        InitReplaceUI();
        InitFullUI();
        InitProcessUI();
    }

    private void InitProcessUI()
    {
        m_goProcess = GetUIChild("EnhantProcess").gameObject;

        m_tipList = new List<GameObject>();
        m_tipLblList = new List<UILabel>();
        m_tipTweenList = new List<TweenAlpha>();
        for (int i = 1; i <= 5; i++)
        {
            m_tipList.Add(GetUIChild("EnhantProcessTip" + i).gameObject);
            m_tipLblList.Add(GetUIChild("EnhantProcessTip" + i).GetComponent<UILabel>());
            m_tipTweenList.Add(GetUIChild("EnhantProcessTip" + i).GetComponent<TweenAlpha>());
        }
        m_btnProcessCancel = GetUIChild("EnhantProcessCancleBtn").GetComponent<MogoButton>();
        m_btnProcessName = GetUIChild("EnhantProcessText").GetComponent<UILabel>();
        m_lblProcessTitle = GetUIChild("EnhantProcessTip").GetComponent<UILabel>();

        m_goProcess.SetActive(false);
    }

    private void InitFullUI()
    {
        m_lblFullCancelBtnName = GetUIChild("EnhantForFullCancleText").GetComponent<UILabel>();
        m_lblFullTip2 = GetUIChild("EnhantForFullTip2").GetComponent<UILabel>();
        m_lblFullTip = GetUIChild("EnhantForFullTip").GetComponent<UILabel>();
        m_goFull = GetUIChild("EnhantForFull").gameObject;
        m_lblFullNewAttr = GetUIChild("EnhantForFullNewAttrText").GetComponent<UILabel>();
        m_lblFullOlfAttrList = new List<UILabel>();
        m_btnReplaceOkList = new List<MogoButton>();
        m_gridList = new List<GameObject>();
        for (int i = 1; i <= 4; i++)
        {
            m_gridList.Add(GetUIChild("EnhantForFullGrid" + i).gameObject);
            m_lblFullOlfAttrList.Add(GetUIChild("EnhantForFullGrid" + i + "Txt").GetComponent<UILabel>());
            m_btnReplaceOkList.Add(GetUIChild("EnhantForFullGrid" + i + "OkBtn").GetComponent<MogoButton>());
        }
        m_btnFullCancel = GetUIChild("EnhantForFullCancleBtn").GetComponent<MogoButton>();

        m_goFull.SetActive(false);
    }

    private void InitReplaceUI()
    {
        m_goReplace = GetUIChild("EnhantForReplace").gameObject;
        m_lblReplaceNewAttr = GetUIChild("EnhantForReplaceNewAttrText").GetComponent<UILabel>();
        m_lblReplaceTip = GetUIChild("EnhantForReplaceTip").GetComponent<UILabel>();
        m_lblReplaceOlfAttr = GetUIChild("EnhantForReplaceOldAttrText").GetComponent<UILabel>();
        m_btnReplaceOk = GetUIChild("EnhantForReplaceOkBtn").GetComponent<MogoButton>();
        m_btnReplaceCancel = GetUIChild("EnhantForReplaceCancleBtn").GetComponent<MogoButton>();

        m_goReplace.SetActive(false);
    }

    

    public void ShowReplace(string newAttr, string oldAttr, string tip, Action onOk, Action onCancle)
    {
        //Debug.LogError("ShowReplace,newAttr:" + newAttr + ",oldAttr:" + oldAttr);
        m_lblReplaceNewAttr.text = newAttr;
        m_lblReplaceOlfAttr.text = oldAttr;
        m_lblReplaceTip.text = tip;
        if (m_btnReplaceOk == null)
        {
            //Debug.LogError("fuck!");
        }
        m_btnReplaceOk.clickHandler = onOk;
        m_btnReplaceCancel.clickHandler = onCancle;
        m_goReplace.SetActive(true);
    }

    public void ShowFull(string newAttr, List<string> oldAttrList, string tip, string btnCancelName, Action<int> onOk, Action onCancle, string tip2 = "")
    {
        m_lblFullCancelBtnName.text = btnCancelName;
        if (tip2 != null)
        {
            m_lblFullTip2.gameObject.SetActive(true);
            m_lblFullTip2.text = tip2;
        }
        else
        {
            m_lblFullTip2.gameObject.SetActive(false);
        }
        m_lblFullTip.text = tip;
        //Debug.LogError("ShowFull,newAttr:" + newAttr);
        m_lblFullNewAttr.text = newAttr;
        int i = 0;
        for (; i < oldAttrList.Count; i++)
        {
            m_gridList[i].SetActive(true);
            int index = i;
            m_lblFullOlfAttrList[i].text = oldAttrList[i];
            if (onOk == null) m_btnReplaceOkList[i].gameObject.SetActive(false);
            else
            {
                m_btnReplaceOkList[i].gameObject.SetActive(true);
                m_btnReplaceOkList[i].clickHandler = () => { onOk(index); };
            }

        }
        for (; i < 4; i++)
        {
            m_gridList[i].SetActive(false);
        }

        m_btnFullCancel.clickHandler = onCancle;

        //Debug.LogError(" m_goFull.SetActive(true);");
        m_goFull.SetActive(true);
    }

    public void CloseUI()
    {
        m_goReplace.SetActive(false);
        m_goFull.SetActive(false);
        m_goProcess.SetActive(false);
    }

    const int TIMES = 5;
    public void ShowFumoProcess(string title, List<string> contentList, Action onDone, Action onCancel)
    {
        string cancelBtnName = LanguageData.GetContent(28121);
        m_btnProcessName.text = string.Format(cancelBtnName, TIMES);
        m_btnProcessCancel.clickHandler = onCancel;
        m_lblProcessTitle.text = title;
        for (int i = 0; i < m_tipList.Count; i++)
        {
            m_tipList[i].SetActive(false);
            m_tipLblList[i].text = contentList[i];
            m_tipTweenList[i].enabled = false;
            int index = i;
            if (i == m_tipList.Count - 1)
            {
                m_tipTweenList[i].onFinished = (t) =>
                    {
                        m_btnProcessName.text = string.Format(cancelBtnName, 0);
                        onDone();
                    };
            }
            else
            {
                m_tipTweenList[i].onFinished = (t) =>
                {
                    m_tipList[index + 1].SetActive(true);
                    m_tipTweenList[index + 1].enabled = true;
                    m_tipTweenList[index + 1].Reset();
                    m_btnProcessName.text = string.Format(cancelBtnName, TIMES - index - 1);
                };
            }

        }
        m_tipList[0].SetActive(true);
        m_tipTweenList[0].Reset();
        m_tipTweenList[0].enabled = true;

        m_goProcess.SetActive(true);


    }
}