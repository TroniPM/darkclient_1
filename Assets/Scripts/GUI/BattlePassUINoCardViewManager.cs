using UnityEngine;
using System.Collections;
using Mogo.Util;

public class BattlePassUINoCardUIViewManager : MFUIUnit
{
    private static BattlePassUINoCardUIViewManager m_instance;

    public static BattlePassUINoCardUIViewManager Instance
    {
        get
        {
            return BattlePassUINoCardUIViewManager.m_instance;
        }
    }

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.BattlePassUINoCard;
        m_myGameObject.name = "BattlePassUINoCard";
        AttachLogicUnit(BattlePassUINoCardUILogicManager.Instance);
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        for (int i = 0; i < 5; ++i)
        {
            m_listItemIconFG.Add(GetSprite(string.Concat("BattlePassUINoCardItem", i, "FG")));
            m_listItemIconBG.Add(GetSprite(string.Concat("BattlePassUINoCardItem", i, "BG")));
        }

        RegisterButtonHandler("BattlePassUINoCardOKBtn");
        SetButtonClickHandler("BattlePassUINoCardOKBtn", OnOKUp);
    }

    void OnOKUp(int id)
    {

        if (MogoWorld.thePlayer.sceneId == MogoWorld.globalSetting.homeScene)
        {
            MogoUIManager.Instance.ShowMogoNormalMainUI();
            return; 
        }
        EventDispatcher.TriggerEvent(Events.CampaignEvent.ExitCampaign);
    }

    public override void CallWhenShow()
    {
        MogoUIManager.Instance.ShowCurrentUI(false);
        MFUIUtils.ShowGameObject(true, m_myGameObject);
    }
    public override void CallWhenHide()
    {
        MFUIUtils.ShowGameObject(false, m_myGameObject);
        MFUIGameObjectPool.GetSingleton().DestroyGameObject(m_myGameObject);
    }

    public override void CallWhenDestroy()
    {
        m_instance = null;
        for (int i = 0; i < m_listItemIconFG.Count; ++i)
        {
            m_listItemIconFG[i] = null;
            m_listItemIconBG[i] = null;
        }

        m_listItemIconBG.Clear();
        m_listItemIconFG.Clear();
    }

    public void SetUITitle(string title)
    {
        SetLabelText("BattlePassUINoCardTitle",title);
    }

    #region 结算记录

    /// <summary>
    /// 是否是MVP
    /// </summary>
    /// <param name="isMVP"></param>
    public void SetIsMVP(bool isMVP)
    {
        if (isMVP)
        {
            MFUIUtils.ShowGameObject(true, GetSprite("BattlePassUINoCardMVPBGBG").gameObject);
            MFUIUtils.ShowGameObject(true, GetSprite("BattlePassUINoCardMVPOutPutBGBG").gameObject);
        }
        else
        {
            MFUIUtils.ShowGameObject(false, GetSprite("BattlePassUINoCardMVPBGBG").gameObject);
            MFUIUtils.ShowGameObject(false, GetSprite("BattlePassUINoCardMVPOutPutBGBG").gameObject);
        }
    }

    /// <summary>
    /// 1.防御次数
    /// </summary>
    /// <param name="num"></param>
    public void SetDefenceNum(string num)
    {
        SetLabelText("BattlePassUINoCardDefenceNumNum", num);
    }

    /// <summary>
    /// 2.个人输出
    /// </summary>
    /// <param name="num"></param>
    public void SetOutPutNum(string num)
    {
        SetLabelText("BattlePassUINoCardOutPutNum", num);
    }

    /// <summary>
    /// 3.MVP
    /// </summary>
    /// <param name="mvpName"></param>
    public void SetMVP(string mvpName)
    {
        SetLabelText("BattlePassUINoCardMVPNum", mvpName);
    }

    /// <summary>
    /// 4.MVP输出
    /// </summary>
    /// <param name="mvpOutput"></param>
    public void SetMVPOutPut(string mvpOutput)
    {
        SetLabelText("BattlePassUINoCardMVPOutPutNum", mvpOutput);
    }

    /// <summary>
    /// 播放1.防御次数动画
    /// </summary>
    public void PlayDefenceNumAnim()
    {
        MFUIUtils.ShowGameObject(true, GetTransform("BattlePassUINoCardDefenceNum").gameObject);
    }

    /// <summary>
    /// 播放2.个人输出动画
    /// </summary>
    public void PlayOutputAnim()
    {
        MFUIUtils.ShowGameObject(true, GetTransform("BattlePassUINoCardOutPut").gameObject);
    }

    /// <summary>
    /// 播放3.MVP动画
    /// </summary>
    public void PlayMVPAnim()
    {
        MFUIUtils.ShowGameObject(true, GetTransform("BattlePassUINoCardMVP").gameObject);
    }

    /// <summary>
    /// 播放4.MVP输出动画
    /// </summary>
    public void PlayMVPOutputAnim()
    {
        MFUIUtils.ShowGameObject(true, GetTransform("BattlePassUINoCardMVPOutPut").gameObject);
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="isMVP"></param>
    public void PlayAnim(bool isMVP)
    {
        PlayDefenceNumAnim();
        Mogo.Util.TimerHeap.AddTimer(500, 0, PlayOutputAnim);

        if (!isMVP)
            return;

        Mogo.Util.TimerHeap.AddTimer(1000, 0, PlayMVPAnim);
        Mogo.Util.TimerHeap.AddTimer(1500, 0, PlayMVPOutputAnim);
    }

    #endregion

    #region 物品奖励

    System.Collections.Generic.List<UISprite> m_listItemIconFG = new System.Collections.Generic.List<UISprite>();
    System.Collections.Generic.List<UISprite> m_listItemIconBG = new System.Collections.Generic.List<UISprite>();

    public void SetRewardItemList(System.Collections.Generic.List<int> itemId)
    {
        Transform listTrans = GetTransform("BattlePassUINoCardItemList");
        listTrans.localPosition = new Vector3(-(itemId.Count - 1) * 80f, listTrans.localPosition.y, listTrans.localPosition.z);

        for (int i = 0; i < m_listItemIconFG.Count; ++i)
        {
            if (i < itemId.Count)
            {
                InventoryManager.SetIcon(itemId[i], m_listItemIconFG[i], 0, null, m_listItemIconBG[i]);
                MFUIUtils.ShowGameObject(true, m_listItemIconFG[i].transform.parent.gameObject);
            }
            else
            {
                MFUIUtils.ShowGameObject(false, m_listItemIconFG[i].transform.parent.gameObject);
            }

        }
    }

    public void SetRewardItemList(System.Collections.Generic.Dictionary<int,int> itemList)
    {
        Transform listTrans = GetTransform("BattlePassUINoCardItemList");
        listTrans.localPosition = new Vector3(-(itemList.Count - 1) * 80f, listTrans.localPosition.y, listTrans.localPosition.z);
        
        for (int i = 0; i < m_listItemIconFG.Count; ++i)
        {
            MFUIUtils.ShowGameObject(false, m_listItemIconFG[i].transform.parent.gameObject);
        }

        int index = 0;

        foreach (var item in itemList)
        {
            InventoryManager.SetIcon(item.Key, m_listItemIconFG[index], 0, null, m_listItemIconBG[index]);
            SetLabelText(string.Concat("BattlePassUINoCardItem", index, "Text"), string.Concat(
                Mogo.GameData.ItemParentData.GetItem(item.Key).Name, " * ", item.Value));
            MFUIUtils.ShowGameObject(true, m_listItemIconFG[index].transform.parent.gameObject);
            ++index;
        }
    }

    #endregion   
}
