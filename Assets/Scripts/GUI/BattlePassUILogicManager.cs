using UnityEngine;
using System.Collections;
using Mogo.Util;

public class BattlePassUILogicManager : MFUILogicUnit
{
    static BattlePassUILogicManager m_instance;

    public static BattlePassUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new BattlePassUILogicManager();
            }

            return BattlePassUILogicManager.m_instance;
        }
    }

    string m_strTime;
    string m_strComboNum;
    string m_strScore;
    string m_strRewardList;
    int m_iGrade;
    bool m_playMark = false;

    public void SetPassTime(string time)
    {

        m_strTime = (string)MFUIUtils.SafeSetValue(BattlePassUIViewManager.Instance, () => { BattlePassUIViewManager.Instance.SetPassTime(time); }, time);
    }

    public void SetComboNum(string num)
    {

        m_strComboNum = (string)MFUIUtils.SafeSetValue(BattlePassUIViewManager.Instance, () => { BattlePassUIViewManager.Instance.SetComboNum(num); }, num);
    }

    public void SetSocre(string score)
    {

        m_strScore = (string)MFUIUtils.SafeSetValue(BattlePassUIViewManager.Instance, () => { BattlePassUIViewManager.Instance.SetSocre(score); }, score);
    }

    public void SetRewardList(string rewardList)
    {

        m_strRewardList = (string)MFUIUtils.SafeSetValue(BattlePassUIViewManager.Instance, () => { BattlePassUIViewManager.Instance.SetRewardList(rewardList); }, rewardList);
    }

    public void SetGrade(int mark)
    {
        m_iGrade = (int)MFUIUtils.SafeSetValue(BattlePassUIViewManager.Instance, () => { BattlePassUIViewManager.Instance.SetGrade(mark); }, mark);
    }

    public override void FillBufferedData()
    {
        SetComboNum(m_strComboNum);
        SetPassTime(m_strTime);
        SetSocre(m_strScore);
        SetRewardList(m_strRewardList);
        SetGrade(m_iGrade);

        if (m_playMark)
            PlayScroeAnim();
    }

    public void PlayScroeAnim()
    {
        if (BattlePassUIViewManager.Instance)
        {
            m_playMark = false;
            BattlePassUIViewManager.Instance.SetUIDirty();

            BattlePassUIViewManager.Instance.PlayPassTimeAnim();
            TimerHeap.AddTimer(500, 0, BattlePassUIViewManager.Instance.PlayMaxComboAnim);
            TimerHeap.AddTimer(1000, 0, BattlePassUIViewManager.Instance.PlayScoreAnim);
            TimerHeap.AddTimer(1500, 0, BattlePassUIViewManager.Instance.PlayRewardAnim);
            TimerHeap.AddTimer(2000, 0, BattlePassUIViewManager.Instance.PlayMarkAnim);
            TimerHeap.AddTimer(2500, 0, () => 
            {
                MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.BattlePassCardListUI);
            });
        }
        else
        {
            m_playMark = true;
        }
    }
}
