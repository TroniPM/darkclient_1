/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoProgressBar
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MogoProgressBar : MonoBehaviour
{

    public UILabel m_progressLbl;
    public UISprite m_progressBg;
    public UISprite m_progress;
    public void SetProgress(float progress, float max)
    {
        m_progressLbl.text = progress + "/" + max;

        m_progress.transform.localScale = new Vector3(m_progressBg.transform.localScale.x * progress / max, m_progress.transform.localScale.y, m_progress.transform.localScale.z);
    }

    public void SetProgressText(string text)
    {
        m_progressLbl.text = text;
    }
}