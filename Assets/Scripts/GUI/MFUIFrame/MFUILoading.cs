using UnityEngine;
using System.Collections;

public class MFUILoading
{
    static MFUILoading m_singleton;

    public static MFUILoading GetSingleton()
    {
        if (m_singleton == null)
        {
            m_singleton = new MFUILoading();
        }

        return m_singleton;
    }

    public void Show()
    {
        //MFUIUtils.MFUIDebug("ShowLoadingUI");
    }

    public void Hide()
    {
        //MFUIUtils.MFUIDebug("HideLoadingUI");
    }

    public void SetBG()
    { }

    public void SetTipText()
    { }

    public void SetLoadingInfo()
    { }

    public void SetLoadingProgress()
    { }
}
