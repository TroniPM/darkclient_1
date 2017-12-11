using UnityEngine;
using System.Collections;

public class BattlePassCardListUILogicManager : MFUILogicUnit
{
    static BattlePassCardListUILogicManager m_instance;

    public static BattlePassCardListUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new BattlePassCardListUILogicManager();
            }

            return BattlePassCardListUILogicManager.m_instance;
        }
    }

    int m_iMark;
    int m_flipCount;
    string m_strText;
    bool m_hasSetEnd = false;

    System.Action m_actStartCountDown;
    System.Action m_actStopCountDown;

    public void SetGrade(int grade)
    {
        SetTipImg(grade);
        SetTipText(string.Concat("评分获得[ffd200]", (grade - 1).ToString(), "[-]次抽奖机会"));
        SetEnd();
    }

    public void SetFlipCount(int count)
    {
        m_flipCount = (int)MFUIUtils.SafeSetValue(BattlePassCardListUIViewManager.Instance, () =>
        {
            BattlePassCardListUIViewManager.Instance.SetFlipCount(count);
        }, count);
    }

    public void SetTipImg(int mark)
    {
        m_iMark = (int)MFUIUtils.SafeSetValue(BattlePassCardListUIViewManager.Instance, () =>
        {
            BattlePassCardListUIViewManager.Instance.SetTipImg(mark);
        }, mark);
    }

    public void SetTipText(string text)
    {
        m_strText = (string)MFUIUtils.SafeSetValue(BattlePassCardListUIViewManager.Instance, () =>
        {
            BattlePassCardListUIViewManager.Instance.SetTipText(text);
        }, text);
    }

    public void StartCountDown()
    {
         m_actStartCountDown = MFUIUtils.SafeDoAction(BattlePassCardListUIViewManager.Instance, () =>
            {
                BattlePassCardListUIViewManager.Instance.StartCountDown();
            });
    }

    public void StopCountDown()
    {
        m_actStopCountDown = MFUIUtils.SafeDoAction(BattlePassCardListUIViewManager.Instance, () =>
        {
            BattlePassCardListUIViewManager.Instance.StopCountDown();
        });
    }

    public override void FillBufferedData()
    {
        SetTipImg(m_iMark);
        SetTipText(m_strText);
        SetFlipCount(m_flipCount);

        if (m_hasSetEnd)
            SetEnd();

        if (m_actStartCountDown != null)
            m_actStartCountDown();

        if (m_actStopCountDown != null)
            m_actStopCountDown();

    }

    public void SetEnd()
    {
        if (BattlePassCardListUIViewManager.Instance == null)
        {
            m_hasSetEnd = true;
        }
        else 
        {
            m_hasSetEnd  =false;
            BattlePassCardListUIViewManager.Instance.SetUIDirty();
        }
    }
}
