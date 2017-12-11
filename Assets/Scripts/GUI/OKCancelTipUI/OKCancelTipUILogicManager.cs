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
using System;
using Mogo.GameData;

public class OKCancelTipUILogicManager
{
    private static OKCancelTipUILogicManager m_instance;
    public static OKCancelTipUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new OKCancelTipUILogicManager();
            }

            return OKCancelTipUILogicManager.m_instance;
        }
    }


    public void TruelyConfirm(string infoText, Action<bool, bool> Callback, bool bShowCheck, 
        string OKText = "OK", string CancelText = "CANCEL",
       ButtonBgType OKBgType = ButtonBgType.Blue, ButtonBgType CancelBgType = ButtonBgType.Brown)
    {
        if (OKText.Equals("OK"))
            OKText = LanguageData.GetContent(25561);
        if (CancelText.Equals("CANCEL"))
            CancelText = LanguageData.GetContent(25562);

        OKCancelTipUIViewManager.Instance.SetOKCancelInfo(infoText);
        OKCancelTipUIViewManager.Instance.SetBtnOKText(OKText);
        OKCancelTipUIViewManager.Instance.SetBtnCancelText(CancelText);
        OKCancelTipUIViewManager.Instance.SetBtnOKBg(OKBgType);
        OKCancelTipUIViewManager.Instance.SetBtnCancelBg(CancelBgType);
        OKCancelTipUIViewManager.Instance.SetCallback(Callback);
    }
}
