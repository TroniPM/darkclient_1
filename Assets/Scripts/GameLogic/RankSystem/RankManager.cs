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
using Mogo.Game;
using Mogo.Util;
using System.Collections.Generic;
using TDBID = System.UInt64;
using Mogo.GameData;

static public class RankEvent
{
    public readonly static string GetRankList = "RankEvent.GetRankList";
    public readonly static string GetRankAvatarInfo = "RankEvent.GetRankAvatarInfo";
    public readonly static string GetHasOnRank = "RankEvent.GetHasOnRank";
    public readonly static string OnIsIdolChangedTodayReq = "RankEvent.OnIsIdolChangedTodayReq";
    public readonly static string OnChangeIdolReq = "RankEvent.OnChangeIdolReq";
    public readonly static string OnSelfIdolReq = "RankEvent.OnSelfIdolReq";
}

//AVATAR_RANK_UNIQUE_RANK    =  1, -- 角色排名
//AVATAR_RANK_RECORD_NAME   =  2, -- 角色名称
//AVATAR_RANK_HIGHESTLEVEL   =  3, -- 角色等级
//AVATAR_RANK_ATTRIBUTION       =  4, -- 角色排行榜属性
//AVATAR_RANK_FANS_COUNT        =  5, -- 粉丝数量
//AVATAR_RANK_CLIENT_DBID = 6
public class RankingMainData
{
    public int uniqieRank{get; set;}
    public string recordName{get; set;}
    public string level{get; set;}
    public uint attrib{get; set;}
    public int fansCount { get; set; }
    public TDBID tdbID { get; set; }
}

//AVATAR_INFO_NAME           =  1, --角色名称
//AVATAR_INFO_LEVEL          =  2, --角色等级
//AVATAR_INFO_VOCATION       =  3, --角色职业
//AVATAR_INFO_EQUIPMENT      =  4, --角色装备
//{
//    AVATAR_RANK_TYPEID         =  1, --角色装备ID
//    AVATAR_RANK_INDEX          =  2, --角色装备部位
//    AVATAR_RANK_SLOTS          =  3, --角色装备宝石插槽
//}
//AVATAR_INFO_RANK_LIST      =  5
public class AvatarInfoData
{
    public string name{get; set;}
    public byte level { get; set; }
    public byte vocation { get; set; }
    public List<RankEquipData> equipList { get; set; }
    public List<int> rankNumList { get; set; }
}

public class RankEquipData
{
    public int equipID{get; set;}
    public int equipSlot{get; set;}
    public List<int> jewelSlots{get; set;}
}

public class RankManager : IEventManager
{
    protected EntityMyself theOwner;

    static public RankManager Instance;
    public RankManager(EntityMyself owner)
    {
        Instance = this;
        theOwner = owner;        
		AddListeners();
    }

    /// <summary>
    /// 保留服务器的时间戳，发给服务用于判断是否更新数据
    /// </summary>
    private uint m_timeUseSendToServer = 0;
    public uint TimeUseSendToServer
    {
        get
        {
            return m_timeUseSendToServer;
        }
        set
        {
            m_timeUseSendToServer = value;
        }
    }

    private bool m_bRefreshMainData = false;
    public bool RefreshMainData
    {
        get
        {
            return m_bRefreshMainData;
        }
        set
        {
            m_bRefreshMainData = value;
        }
    }

    // 排行榜类型(排行榜ID)
    readonly static int CANREQRANKTYPE = -1;
    private int m_rankType = CANREQRANKTYPE;
    public int RankType
    {
        get
        {
            return m_rankType;
        }
        set
        {
            m_rankType = value;
        }
    }
    
    // 排行榜当前请求页
    // 为去除警告暂时屏蔽以下代码
    //readonly static int MAXRANKPAGE = 10; // 每个排行榜限定页数
    readonly static int CANREQRANKPAGE = -1;
    private int m_rankPage = CANREQRANKPAGE;
    public int RankPage
    {
        get
        {
            return m_rankPage;
        }
        set
        {
            m_rankPage = value;
        }
    }

    /// <summary>
    /// 某个榜单某一页的数据
    /// </summary>
    public class RankingMainDataPage
    {
        public int page = 0;
        public List<RankingMainData> RankingMainDataList = new List<RankingMainData>();
    }

    // 排行榜排行数据缓存列表
    private Dictionary<int, Dictionary<int, RankingMainDataPage>> m_rankingMainDataMap = new Dictionary<int, Dictionary<int, RankingMainDataPage>>();
    // 排行榜排行数据缓存列表是否还有数据可以获取
    private Dictionary<int, bool> m_rankingMainDataHasDataMap = new Dictionary<int, bool>();
    // 当前查看的角色信息 
    private AvatarInfoData m_currentAvatarInfoData = new AvatarInfoData();
    private byte m_currentAvatarSex = 0;
    private byte m_currentAvatarIsMyIdol = 0;
    // 请求偶像的类型：false = 是否可以变更偶像; true = 请求变更偶像
    private bool m_bOnChangeIdolReq = false;
    // 玩家偶像
    private TDBID m_selfIdolTDBID = 0;
    public TDBID SelfIdolTDBID
    {
        get { return m_selfIdolTDBID; }
        set
        {
            m_selfIdolTDBID = value;
            if (RankingUIViewManager.Instance != null)
                RankingUIViewManager.Instance.RefreshRankingMainDataList();
        }
    }   

    #region 事件监听

    public void AddListeners()
    {
        EventDispatcher.AddEventListener<int, int>(RankEvent.GetRankList, OnRankListReq);
        EventDispatcher.AddEventListener<TDBID>(RankEvent.GetRankAvatarInfo, OnRankAvatarInfoReq);
        EventDispatcher.AddEventListener<int>(RankEvent.GetHasOnRank, OnHasOnRankReq);
        EventDispatcher.AddEventListener<TDBID>(RankEvent.OnIsIdolChangedTodayReq, OnIsIdolChangedTodayReq);
        EventDispatcher.AddEventListener<TDBID>(RankEvent.OnChangeIdolReq, OnChangeIdolReq);
        EventDispatcher.AddEventListener(RankEvent.OnSelfIdolReq, OnSelfIdolReq);
    }

    public void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener<int, int>(RankEvent.GetRankList, OnRankListReq);
        EventDispatcher.RemoveEventListener<TDBID>(RankEvent.GetRankAvatarInfo, OnRankAvatarInfoReq);
        EventDispatcher.RemoveEventListener<int>(RankEvent.GetHasOnRank, OnHasOnRankReq);
        EventDispatcher.RemoveEventListener<TDBID>(RankEvent.OnIsIdolChangedTodayReq, OnIsIdolChangedTodayReq);
        EventDispatcher.RemoveEventListener<TDBID>(RankEvent.OnChangeIdolReq, OnChangeIdolReq);
        EventDispatcher.RemoveEventListener(RankEvent.OnSelfIdolReq, OnSelfIdolReq);
    }

    #endregion

    #region RPC请求 

   /// <summary>
   /// 请求排行榜数据列表
   /// </summary>
   /// <param name="rankType"></param>
   /// <param name="rankPage"></param>
    void OnRankListReq(int rankType, int rankPage)
    {
        if (RankType == CANREQRANKTYPE && RankPage == CANREQRANKTYPE)
        {
            if (m_rankingMainDataHasDataMap.ContainsKey(rankType) && m_rankingMainDataHasDataMap[rankType] == false)
            {
                // 该排行榜没有后续数据可以获取，使用缓存数据刷新
                if (rankPage == 0)
                {
                    RankType = rankType;
                    RankPage = rankPage;
                    //MogoGlobleUIManager.Instance.ShowWaitingTip(true);
                    RankingUIViewManager.Instance.ShowRankingUIWaitingTip(true);
                    SetRankingMainDataList();
                }       
         
                return; 
            }

            RankType = rankType;
            RankPage = rankPage;
            //MogoGlobleUIManager.Instance.ShowWaitingTip(true);
            RankingUIViewManager.Instance.ShowRankingUIWaitingTip(true);

            // 排行榜类型，请求数据页，时间戳
            // 排行榜页数从1开始,与服务器约定
            theOwner.RpcCall("RankListReq", (byte)RankType, (byte)RankPage + 1, TimeUseSendToServer);
        }
    }

    /// <summary>
    /// 请求玩家数据
    /// </summary>
    /// <param name="tdbID"></param>
    void OnRankAvatarInfoReq(TDBID tdbID)
    {
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        theOwner.RpcCall("RankAvatarInfoReq", tdbID);
    }   

    /// <summary>
    /// 获取玩家在某榜单的排名
    /// </summary>
    /// <param name="rankType"></param>
    void OnHasOnRankReq(int rankType)
    {
        theOwner.RpcCall("HasOnRankReq", (byte)rankType);
    }

    /// <summary>
    /// 获取玩家偶像
    /// </summary>
    /// <param name="rankType"></param>
    void OnSelfIdolReq()
    {
        theOwner.RpcCall("SelfIdolReq");
    }  

    /// <summary>
    /// 是否可以变更偶像
    /// </summary>
    /// <param name="tdbID"></param>    
    void OnIsIdolChangedTodayReq(TDBID tdbID)
    {
        m_bOnChangeIdolReq = false;
        theOwner.RpcCall("IsIdolChangedTodayReq", tdbID);
    }

     /// <summary>
    /// 请求变更偶像
    /// </summary>
    /// <param name="tdbID"></param>
    void OnChangeIdolReq(TDBID tdbID)
    {
        m_bOnChangeIdolReq = true;
        theOwner.RpcCall("ChangeIdolReq", tdbID);
    }

    #endregion

    #region RPC回调
      
    /// <summary>
    /// 返回排行榜数据列表
    /// </summary>
    /// <param name="flag">返回类型码   1：需要更新数据，0：不需要更新数据</param>
    /// <param name="luaTable">排行榜数据</param>
    /// <param name="time">时间戳</param>
    /// <param name="hasData">后续数据标记 1：还有后续数据，0：没有后续数据</param>
    public void OnRankListResp(byte flag, LuaTable luaTable, uint time, byte hasData)
    {
        TimeUseSendToServer = time; // 时间戳

        // 后续数据标记 1：还有后续数据，0：没有后续数据
        if (hasData == 1)
            m_rankingMainDataHasDataMap[RankType] = true;
        else
            m_rankingMainDataHasDataMap[RankType] = false;

        // 返回类型码 1：需要更新数据
        if (flag == 1)
        {
            m_rankingMainDataMap.Clear(); // 数据已经更新,清空缓存数据
            m_rankingMainDataHasDataMap.Clear(); // 清空是否有后续数据标记列表
        }

        List<RankingMainData> rankingMainDataList = new List<RankingMainData>();
        if (Utils.ParseLuaTable(luaTable, out rankingMainDataList))
        {
            if (rankingMainDataList.Count > 0)
            {
                RankingMainDataPage rankingMainDataPage = new RankingMainDataPage();
                rankingMainDataPage.page = RankPage;
                rankingMainDataPage.RankingMainDataList = rankingMainDataList;

                Dictionary<int, RankingMainDataPage> rankingMainDataPageList = null;
                if (m_rankingMainDataMap.ContainsKey(RankType))
                    rankingMainDataPageList = m_rankingMainDataMap[RankType];
                else
                    rankingMainDataPageList = new Dictionary<int, RankingMainDataPage>();
                rankingMainDataPageList[RankPage] = rankingMainDataPage;
                m_rankingMainDataMap[RankType] = rankingMainDataPageList;
            }
            else
            {
                if (RankingUIViewManager.Instance.CurrentPage != RankingUIViewManager.DefaultPage)
                {
                    RankType = CANREQRANKTYPE;
                    RankPage = CANREQRANKPAGE;
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
					RankingUIViewManager.Instance.ShowRankingUIWaitingTip(false);
                    return;
                }
            }            
        }

        SetRankingMainDataList(); // 设置排行榜数据列表
    }

    /// <summary>
    /// 角色基本信息
    /// </summary>
    /// <param name="luaTable"></param>     
    public void RankAvatarInfoResp(LuaTable luaTable, byte sex, byte isMyIdol)
    {       
        if (Utils.ParseLuaTable(luaTable, out m_currentAvatarInfoData))
        {
            List<int> equipIDList = new List<int>();
            for (int i = 0; i < m_currentAvatarInfoData.equipList.Count; i++)
            {
                equipIDList.Add(m_currentAvatarInfoData.equipList[i].equipID);
            }
            
            RankingUIViewManager.Instance.SetModelShow(m_currentAvatarInfoData.vocation, equipIDList, true);
            RankingUIViewManager.Instance.ShowPanelPlayerEquip(m_currentAvatarInfoData.equipList, m_currentAvatarInfoData.name, m_currentAvatarInfoData.level);
            RankingUIViewManager.Instance.GeneratePlayerRankDataList(m_currentAvatarInfoData.rankNumList);

            m_currentAvatarSex = sex;
            m_currentAvatarIsMyIdol = isMyIdol;
            SetBecameFSButtonText(); // 设置粉丝按钮显示文本           
        }
    }

    public void HasOnRankResp(byte rankNum)
    {
        RankingUIViewManager.Instance.SetMyRankNum(rankNum);
    }

    /// <summary>
    /// 变更偶像
    /// </summary>
    /// <param name="flag">
    /// FANS_IDOL_HAS_CHANGE = 3,   --今日已用完偶像变更次数
    /// FANS_IDOL_HAS_REWARD = 2,   --已领取奖励
    /// FANS_IDOL_PARAS_ERROR = 1,   --参数错误
    /// FANS_IDOL_SUCCESS        = 0,   --变更偶像成功
    /// </param>
    public void FansIdolResp(byte flag, string oldIdolName)// 原来偶像
    {
        switch (flag)
        {
            case 0:
                if (RankingUILogicManager.Instance != null)
                {
                    if (m_bOnChangeIdolReq)
                    {
                        OnChangeIdolSuccess(oldIdolName);                       
                    }
                    else
                    {
                        RankingUILogicManager.Instance.OnChangeIdolEnable(oldIdolName, m_currentAvatarInfoData.name);
                    }
                }
                break;
            case 1:
                MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(47408));
                break;
            case 2:
                break;
            case 3:
                OnChangeIdolFail();
                break;          
        }
    }

    /// <summary>
    /// 获取偶像
    /// </summary>
    /// <param name="selfIdol"></param>
    public void SelfIdolResp(TDBID selfIdol)
    {
        SelfIdolTDBID = selfIdol;
    }

    #endregion

    #region 辅助函数

    /// <summary>
    /// 设置排行榜数据列表
    /// </summary>
    private void SetRankingMainDataList()
    {
        List<RankingMainData> dataList = new List<RankingMainData>();
        if (m_rankingMainDataMap.ContainsKey(RankType))
        {
            Dictionary<int, RankingMainDataPage> rankingMainDataPageList = m_rankingMainDataMap[RankType];
            foreach (RankingMainDataPage rankingMainDataPage in rankingMainDataPageList.Values)
            {
                for (int i = 0; i < rankingMainDataPage.RankingMainDataList.Count; i++)
                {                    
                     dataList.Add(rankingMainDataPage.RankingMainDataList[i]);                   
                }
            }

            if (m_rankingMainDataHasDataMap.ContainsKey(RankType))
                RankingUIViewManager.Instance.GenerateRankingMainDataList(dataList, m_rankingMainDataHasDataMap[RankType]);
            else
                RankingUIViewManager.Instance.GenerateRankingMainDataList(dataList, true);
            
        }
        else
        {
            RankingUIViewManager.Instance.GenerateRankingMainDataList(new List<RankingMainData>(), false);
        }

        // 重置RankType和RankPage,解锁向服务器请求排行榜数据
        RankType = CANREQRANKTYPE;
        RankPage = CANREQRANKPAGE;    
    }

    #endregion

    #region 粉丝

    /// <summary>
    /// 偶像变更成功
    /// </summary>
    public void OnChangeIdolSuccess(string oldIdolName)
    {
        MogoGlobleUIManager.Instance.Info(LanguageData.GetContent(47405), "OK");         
   
        if (m_currentAvatarIsMyIdol == 0)
            m_currentAvatarIsMyIdol = 1;

        ChangeFansCount(oldIdolName, m_currentAvatarIsMyIdol == 1);    
        SetBecameFSButtonText();
    }

    /// <summary>
    /// 改变粉丝数(只针对当前变更的偶像)
    /// </summary>
    /// <param name="oldIdolName"></param>
    /// <param name="currentIsIdolNow"></param>
    public string NewIdolName = null;
    public string OldIdolName = null;
    private void ChangeFansCount(string oldIdolName, bool currentIsIdolNow)
    {
        if (currentIsIdolNow) // 成为当前角色的粉丝
        {
            NewIdolName = m_currentAvatarInfoData.name; // 当前角色为我的偶像  
            OldIdolName = oldIdolName; // 变更偶像
        }
        else // 抛弃当前偶像
        {
            OldIdolName = m_currentAvatarInfoData.name;                  
        }

        if (RankingUIViewManager.Instance != null)
            RankingUIViewManager.Instance.RefreshRankingMainDataList();
    }

    /// <summary>
    /// 操作失败，一天只能更改一次偶像哦！
    /// </summary>
    private void OnChangeIdolFail()
    {
        MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(47407));
    }

    /// <summary>
    /// 设置粉丝按钮显示文本
    /// </summary>
    /// <param name="sex"></param>
    /// <param name="isMyIdol"></param>
    private void SetBecameFSButtonText()
    {
        if (RankingUILogicManager.Instance.CurrentTDBID == MogoWorld.thePlayer.ID)
        {
            RankingUIViewManager.Instance.SetBecameFSButtonText(LanguageData.GetContent(47413)); // 炫耀一下
        }
        else
        {
            if (m_currentAvatarIsMyIdol == 1)
            {
                if (m_currentAvatarSex == 1)
                    RankingUIViewManager.Instance.SetBecameFSButtonText(LanguageData.GetContent(47411)); // 抛弃他
                else
                    RankingUIViewManager.Instance.SetBecameFSButtonText(LanguageData.GetContent(47412)); // 抛弃她
            }
            else
            {
                if (m_currentAvatarSex == 1)
                    RankingUIViewManager.Instance.SetBecameFSButtonText(LanguageData.GetContent(47409)); // 成为他的粉丝
                else
                    RankingUIViewManager.Instance.SetBecameFSButtonText(LanguageData.GetContent(47410)); // 成为她的粉丝
            }
        }
    }

    #endregion
}
