using UnityEngine;
using System.Collections;

public class BattlePassUINoCardUILogicManager : MFUILogicUnit
{
    static BattlePassUINoCardUILogicManager m_instance;

    public static BattlePassUINoCardUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new BattlePassUINoCardUILogicManager();
            }

            return BattlePassUINoCardUILogicManager.m_instance;
        }
    }

    System.Action act;

    public void SetUIDirty()
    {
        act = MFUIUtils.SafeDoAction(BattlePassUINoCardUIViewManager.Instance, () =>
        {
            BattlePassUINoCardUIViewManager.Instance.SetUIDirty();
        });
    }

    public override void FillBufferedData()
    {
        if (act != null)
            act();

        SetIsMVP(m_isMVP);
        SetTitile(m_strTiltle);
        SetDefenceNum(m_strDefenceNum);
        SetOutPut(m_strOutputNum);
        SetMVPName(m_strMVPName);
        SetMVPOutput(m_strMVPOutputNum);
        //SetRewardItemList(m_listID);
        SetRewardItemList(m_dictItemList);

        if (m_actMVPAnim != null)
        {
            m_actMVPAnim();
            m_actMVPAnim = null;
        }
    }

    bool m_isMVP;
    string m_strDefenceNum;
    string m_strOutputNum;
    string m_strMVPName;
    string m_strMVPOutputNum;
    string m_strTiltle;

    System.Collections.Generic.List<int> m_listID;
    System.Collections.Generic.Dictionary<int, int> m_dictItemList = new System.Collections.Generic.Dictionary<int,int>();

    System.Action m_actMVPAnim;

    public void SetIsMVP(bool isMVP)
    {
        m_isMVP = (bool)MFUIUtils.SafeSetValue(BattlePassUINoCardUIViewManager.Instance,
            () => { BattlePassUINoCardUIViewManager.Instance.SetIsMVP(isMVP); }, isMVP);
    }

    public void SetTitile(string title)
    {
        m_strTiltle = (string)MFUIUtils.SafeSetValue(BattlePassUINoCardUIViewManager.Instance,
            () => { BattlePassUINoCardUIViewManager.Instance.SetUITitle(title); }, title);
    }

    public void SetDefenceNum(string num)
    {
        m_strDefenceNum = (string)MFUIUtils.SafeSetValue(BattlePassUINoCardUIViewManager.Instance,
            () => { BattlePassUINoCardUIViewManager.Instance.SetDefenceNum(num); }, num);
    }

    public void SetOutPut(string output)
    {
        m_strOutputNum = (string)MFUIUtils.SafeSetValue(BattlePassUINoCardUIViewManager.Instance,
            () => { BattlePassUINoCardUIViewManager.Instance.SetOutPutNum(output); }, output);
    }

    public void SetMVPName(string name)
    {
        m_strMVPName = (string)MFUIUtils.SafeSetValue(BattlePassUINoCardUIViewManager.Instance,
            () => { BattlePassUINoCardUIViewManager.Instance.SetMVP(name); }, name);
    }

    public void SetMVPOutput(string mvpOutput)
    {
        m_strMVPOutputNum = (string)MFUIUtils.SafeSetValue(BattlePassUINoCardUIViewManager.Instance,
            () => { BattlePassUINoCardUIViewManager.Instance.SetMVPOutPut(mvpOutput); }, mvpOutput);
    }

    public void SetRewardItemList(System.Collections.Generic.List<int> itemId)
    {
        m_listID = (System.Collections.Generic.List<int>)MFUIUtils.SafeSetValue(BattlePassUINoCardUIViewManager.Instance,
            () => { BattlePassUINoCardUIViewManager.Instance.SetRewardItemList(itemId); }, itemId);
    }

    public void SetRewardItemList(System.Collections.Generic.Dictionary<int, int> itemList)
    {
        m_dictItemList = (System.Collections.Generic.Dictionary<int, int>)MFUIUtils.SafeSetValue(
            BattlePassUINoCardUIViewManager.Instance, () => { BattlePassUINoCardUIViewManager.Instance.SetRewardItemList(itemList); },
            itemList);
    }

    public void PlayAnim(bool isMVP)
    {
        m_actMVPAnim = MFUIUtils.SafeDoAction(BattlePassUINoCardUIViewManager.Instance,
            () => { BattlePassUINoCardUIViewManager.Instance.PlayAnim(isMVP); });            
    }
}
