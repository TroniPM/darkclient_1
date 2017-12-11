using UnityEngine;
using System.Collections;
using Mogo.Util;

public class BattlePassUIViewManager : MFUIUnit
{
    private static BattlePassUIViewManager m_instance;

    public static BattlePassUIViewManager Instance
    {
        get
        {
            return BattlePassUIViewManager.m_instance;
        }
    }

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.BattlePassUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        m_myGameObject.name = "BattlePassUI";
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(BattlePassUILogicManager.Instance);

        m_ts = m_myTransform.GetComponent<TweenScale>();
        m_tp = m_myTransform.GetComponent<TweenPosition>();
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
    }

    public override void CallWhenHide()
    {
        //MFUIUtils.ShowGameObject(false, m_myGameObject);
        //MFUIGameObjectPool.GetSingleton().DestroyGameObject(m_myGameObject);
        PlayTranslateAnim();
        PlayScaleAnim();
    }

    public override void CallWhenUnloadResources()
    {
        m_instance = null;
    }

    public void PlayPassTimeAnim()
    {
        MFUIUtils.ShowGameObject(true,GetTransform("BattlePassUIPassTime").gameObject);
        EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound,"BattlePassUIPassTime");
    }

    public void PlayMaxComboAnim()
    {
        MFUIUtils.ShowGameObject(true, GetTransform("BattlePassUIMaxBatter").gameObject);
        EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, "BattlePassUIMaxBatter");
    }

    public void PlayScoreAnim()
    {
        MFUIUtils.ShowGameObject(true, GetTransform("BattlePassUIScore").gameObject);
        EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, "BattlePassUIScore");
    }

    public void PlayMarkAnim()
    {
        MFUIUtils.ShowGameObject(true, GetTransform("BattlePassUIStarType").gameObject);
        EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, "BattlePassUIStarType");
    }

    public void PlayRewardAnim()
    {
        MFUIUtils.ShowGameObject(true, GetTransform("BattlePassUIReward").gameObject);
        EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, "BattlePassUIReward");
    }

    public void PlayScaleAnim()
    {
        if (m_ts != null)
        {
            m_ts.Reset();
            m_ts.enabled = true;
            m_ts.Play(true);
        }
    }

    public void PlayTranslateAnim()
    {
        if (m_tp != null)
        {
            m_tp.Reset();
            m_tp.enabled = true;
            m_tp.Play(true);
        }
    }

    public void PlayRotateCardAnim(int id)
    {
        
    }

    public void DestroySelf()
    {
        MFUIGameObjectPool.GetSingleton().DestroyGameObject(m_myGameObject);
    }

    public void SetPassTime(string time)
    {
        SetLabelText("BattlePassUIPassTimeNum", time);
    }

    public void SetPassTime(int minute, int second)
    {
        SetPassTime(string.Concat(minute.ToString("d2"), " : ", second.ToString("d2")));
    }


    public void SetComboNum(string num)
    {
        SetLabelText("BattlePassUIMaxBatterNum", num);
    }

    public void SetSocre(string score)
    {
        SetLabelText("BattlePassUIScoreNum", score);
    }

    public void SetRewardList(string rewardList)
    {
        SetLabelText("BattlePassUIRewardText", rewardList);
    }

    public void SetGrade(int mark)
    {
        string texName;

        switch (mark)
        {
            case 1:
                texName = "fb-dc";
                break;

            case 2:
                texName = "fb-db";
                break;

            case 3:
                texName = "fb-da";
                break;

            case 4:
                texName = "fb-ds";
                break;

            default:
                texName = "fb-dc";
                break;
        }
        if (GetTexture("BattlePassUIStarType").mainTexture.name != texName)
        {
            MFUIResourceManager.GetSingleton().LoadResource(ID, string.Concat(texName, ".png"), (obj) =>
            {
                SetTexture("BattlePassUIStarType", (Texture)obj);
            });
        }
    }

    //public override void CallWhenUpdate()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        SetPassTime("888");
    //        PlayPassTimeAnim();
    //    }

    //    if (Input.GetKeyDown(KeyCode.LeftAlt))
    //    {
    //        SetComboNum("888");
    //        PlayMaxComboAnim();
    //    }

    //    if (Input.GetKeyDown(KeyCode.RightAlt))
    //    {
    //        SetSocre("888");
    //        PlayScoreAnim();
    //    }

    //    if (Input.GetKeyDown(KeyCode.F1))
    //    {
    //        SetGrade(0);
    //        PlayMarkAnim();
    //    }

    //    if (Input.GetKeyDown(KeyCode.F2))
    //    {
    //        SetRewardList("88888888");
    //        PlayRewardAnim();
    //    }

    //    if (Input.GetKeyDown(KeyCode.F3))
    //    {
    //        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.BattlePassCardListUI);
    //    }
    //}

    TweenScale m_ts;
    TweenPosition m_tp;
}
