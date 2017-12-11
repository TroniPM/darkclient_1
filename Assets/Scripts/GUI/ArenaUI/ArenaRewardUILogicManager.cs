using System;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;

public class ArenaRewardUILogicManager : UILogicManager
{
    #region 公共变量
    private static ArenaRewardUILogicManager m_instance;
    public static ArenaRewardUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ArenaRewardUILogicManager();
            }
            return m_instance;
        }
    }
    #endregion
    private ArenaRewardUIViewManager m_view;
    public void Initialize(ArenaRewardUIViewManager view)
    {
        m_view = view;        
    }
    public void AddRewardUnit(List<int> hasGetArenaRewardList, Dictionary<int, ArenaRewardData> allRewardDataList, Action callback)
    {
        if (m_view!=null)
        {
            m_view.AddRewardUnit(hasGetArenaRewardList, allRewardDataList, callback);
        }
    }
    public void OnGetReward(int id)
    {
        LoggerHelper.Debug("OnClick" + id);
        EventDispatcher.TriggerEvent<int>(Events.ArenaEvent.GetArenaReward,id);
    }
}
