using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;
using System.Text;

public class EventMessageData
{
	public bool is_doing { get; set; }

	public int is_reward { get; set; }
	public int is_finish { get; set; }
	public int accept_time { get; set; }
	public int arg { get; set; }
}

public class AchievementMessageData
{
	public int aid { get; set; }
	public int cur_num { get; set; }
	public int level { get; set; }
	public int reward_level { get; set; }
}

public class OperationSystem : IEventManager
{
	protected DateTime theCurTime;
	protected DateTime theLastTime;

	protected EntityMyself theOwner;

	protected List<int> eventEnable = new List<int>();
	protected Dictionary<int, EventMessageData> eventDoing = new Dictionary<int, EventMessageData>();
	protected List<int> eventListening = new List<int>();

	protected List<int> chargeGotID = new List<int>();
	protected List<int> eventGotID = new List<int>();
	protected List<int> eventSharedID = new List<int>();

	protected List<int> loginGotID = new List<int>();

	protected List<AchievementMessageData> achievementDoing = new List<AchievementMessageData>();
	protected List<int> achievementSharedID = new List<int>();

	public bool loginFirstShow = true;
	public bool loginRewardHasGot = false;

	protected List<KeyValuePair<int, EventMessageData>> sortedActivity = new List<KeyValuePair<int, EventMessageData>>();

	protected List<KeyValuePair<int, EventMessageData>> eventCanGetGroup = new List<KeyValuePair<int, EventMessageData>>();
	protected List<KeyValuePair<int, EventMessageData>> eventDoingGroup = new List<KeyValuePair<int, EventMessageData>>();
	protected List<KeyValuePair<int, EventMessageData>> eventGotGroup = new List<KeyValuePair<int, EventMessageData>>();
	protected List<KeyValuePair<int, EventMessageData>> eventTimeOutGroup = new List<KeyValuePair<int, EventMessageData>>();
	protected List<KeyValuePair<int, EventMessageData>> eventNotOpenGroup = new List<KeyValuePair<int, EventMessageData>>();

	// 静态表示同步下来的属性，先这样写着

	public static bool isOld;
	public static bool IsOld
	{
		get { return isOld; }
		set
		{
			isOld = value;
			// to do binding
			LoginRewardUILogicManager.Instance.isOldServer = isOld;
		}
	}

	public OperationSystem(EntityMyself owner)
	{
		theOwner = owner;

		loginFirstShow = theOwner.IsLoginFirstShow;

		loginRewardHasGot = theOwner.IsLoginRewardHasGot;

		AddListeners();

		#region 活动RPC
		theOwner.RpcCall("EventOpenList");
		// theOwner.RpcCall("get_event_ing");
		#endregion

		#region 成就RPC
		theOwner.RpcCall("get_achievement", 0);
		#endregion
	}


	#region 事件监听

	public void AddListeners()
	{
		// 直接UI体现
		EventDispatcher.AddEventListener(Events.OperationEvent.Charge, Charge);
		EventDispatcher.AddEventListener<int>(Events.OperationEvent.ChargeGetReward, ChargeGetReward);
		EventDispatcher.AddEventListener<int>(Events.OperationEvent.EventGetReward, EventGetReward);
		EventDispatcher.AddEventListener(Events.OperationEvent.EventShareToGetDiamond, EventShareToGetDiamond);
		EventDispatcher.AddEventListener<int>(Events.OperationEvent.LogInGetReward, LogInGetReward);
		EventDispatcher.AddEventListener(Events.OperationEvent.LogInBuy, LogInBuy);
		EventDispatcher.AddEventListener<int>(Events.OperationEvent.AchievementGetReward, AchievementGetReward);
		EventDispatcher.AddEventListener<int>(Events.OperationEvent.AchievementShareToGetDiamond, AchievementShareToGetDiamond);

		EventDispatcher.AddEventListener(Events.OperationEvent.GetChargeRewardMessage, GetChargeRewardMessage);
		EventDispatcher.AddEventListener(Events.OperationEvent.GetActivityMessage, GetActivityMessage);
		EventDispatcher.AddEventListener(Events.OperationEvent.GetLoginMessage, GetLoginMessage);
		EventDispatcher.AddEventListener(Events.OperationEvent.GetAchievementMessage, GetAchievementMessage);

		EventDispatcher.AddEventListener<EntityMyself>(Events.OperationEvent.CheckEventOpen, CheckEventOpen);

		EventDispatcher.AddEventListener(Events.OperationEvent.FlushCharge, FlushCharge);

		// EventDispatcher.AddEventListener(Events.OperationEvent.CheckFirstShow, CheckLoginFirstShow);

		EventDispatcher.AddEventListener<int>(Events.OperationEvent.EventTimesUp, EventTimesUp);

		EventDispatcher.AddEventListener(Events.OperationEvent.GetLoginMarket, GetLoginMarket);

		// EventDispatcher.AddEventListener(Events.OperationEvent.GetAllActivity, GetAllActivity);

		EventDispatcher.AddEventListener("ShowMogoNormalMainUI", CheckLoginFirstShow);

		EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, CheckAssistantShow);
	}

	public void RemoveListeners()
	{
		EventDispatcher.RemoveEventListener(Events.OperationEvent.Charge, Charge);
		EventDispatcher.RemoveEventListener<int>(Events.OperationEvent.ChargeGetReward, ChargeGetReward);
		EventDispatcher.RemoveEventListener<int>(Events.OperationEvent.EventGetReward, EventGetReward);
		EventDispatcher.RemoveEventListener(Events.OperationEvent.EventShareToGetDiamond, EventShareToGetDiamond);
		EventDispatcher.RemoveEventListener<int>(Events.OperationEvent.LogInGetReward, LogInGetReward);
		EventDispatcher.RemoveEventListener(Events.OperationEvent.LogInBuy, LogInBuy);
		EventDispatcher.RemoveEventListener<int>(Events.OperationEvent.AchievementGetReward, AchievementGetReward);
		EventDispatcher.RemoveEventListener<int>(Events.OperationEvent.AchievementShareToGetDiamond, AchievementShareToGetDiamond);

		EventDispatcher.RemoveEventListener(Events.OperationEvent.GetChargeRewardMessage, GetChargeRewardMessage);
		EventDispatcher.RemoveEventListener(Events.OperationEvent.GetActivityMessage, GetActivityMessage);
		EventDispatcher.RemoveEventListener(Events.OperationEvent.GetLoginMessage, GetLoginMessage);
		EventDispatcher.RemoveEventListener(Events.OperationEvent.GetAchievementMessage, GetAchievementMessage);

		EventDispatcher.RemoveEventListener<EntityMyself>(Events.OperationEvent.CheckEventOpen, CheckEventOpen);

		EventDispatcher.RemoveEventListener(Events.OperationEvent.FlushCharge, FlushCharge);

		// EventDispatcher.RemoveEventListener(Events.OperationEvent.CheckFirstShow, CheckLoginFirstShow);

		EventDispatcher.RemoveEventListener<int>(Events.OperationEvent.EventTimesUp, EventTimesUp);

		EventDispatcher.RemoveEventListener(Events.OperationEvent.GetLoginMarket, GetLoginMarket);

		// EventDispatcher.RemoveEventListener(Events.OperationEvent.GetAllActivity, GetAllActivity);

		EventDispatcher.RemoveEventListener("ShowMogoNormalMainUI", CheckLoginFirstShow);

		EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, CheckAssistantShow);
	}

	#endregion



	#region 事件触发函数

	protected void Charge()
	{
		LoggerHelper.Debug("OperationSystem Charge");
		RpcCharge();
	}

	protected void ChargeGetRewardGot()
	{
		LoggerHelper.Debug("OperationSystem ChargeGetReward");
		RpcChargeGetRewardGot();
	}

	protected void ChargeGetReward(int id)
	{
		LoggerHelper.Debug("OperationSystem ChargeGetReward");
		RpcChargeGetReward(id);
	}

	protected void EventGetReward(int id)
	{
		LoggerHelper.Debug("OperationSystem EventGetReward");
		RpcEventGetReward(id);
	}

	protected void EventShareToGetDiamond()
	{
		LoggerHelper.Debug("OperationSystem EventShareToGetDiamond");
		RpcEventShareToGetDiamond();
	}

	protected void LogInGetReward(int id)
	{
		LoggerHelper.Debug("OperationSystem LogInGetReward");

		//if (RewardLoginData.dataMap[id].days != theOwner.login_days)
		//{
		//    if (RewardLoginData.dataMap[id].days == theOwner.login_days - 1)
		//    {
		//        LoggerHelper.Debug("Maybe You Just Pass 0:00... Check Your Mail");
		//        return;
		//    }
		//    else
		//    {
		//        LoggerHelper.Debug("OperationSystem LogInGetReward Days: " + RewardLoginData.dataMap[id].days + " The Owner Days" + theOwner.login_days);
		//        LoggerHelper.Debug("If you see the debug here, means that the login_days is 0");
		//        LoggerHelper.Debug("Check you def file and property synchronization");
		//        return;
		//    }
		//}

        #region 进度记录

        GameProcManager.GetLoginReward();

        #endregion

		theOwner.RpcCall("get_reward_login");

		// LoginRewardUILogicManager.Instance.PlayGetRewardSignAnim();
	}

	protected void LogInBuy()
	{
		LoggerHelper.Debug("OperationSystem LogInBuy");
		RpcLoginMarketBuy();
	}

	protected void GetAllActivity()
	{
		LoggerHelper.Debug("OperationSystem GetAllActivity");
		RpcGetAllActivityEnable();
	}

	protected void GetAllAchievement()
	{
		LoggerHelper.Debug("OperationSystem GetAllAchievement");
		RpcGetAllAchievement();
	}

	protected void AchievementGetReward(int id)
	{
		LoggerHelper.Debug("OperationSystem AchievementGetReward");
		RpcGetAchievementID(id);
	}

	protected void AchievementShareToGetDiamond(int id)
	{
		LoggerHelper.Debug("OperationSystem AchievementShareToGetDiamond");
		RpcAchievemenShareToGetDiamond(id);
	}



	protected void GetChargeRewardMessage()
	{
		LoggerHelper.Debug("OperationSystem GetChargeRewardMessage");
		SetChargeRewardMessage();
	}

	protected void GetActivityMessage()
	{
		LoggerHelper.Debug("OperationSystem GetActivityMessage");
		// ShowEventDoing();
		SetActivityMessage();
		// RpcGetActivityID();
	}

	protected void GetLoginMessage()
	{
		LoggerHelper.Debug("OperationSystem GetLoginMessage");

		
		SetLoginMessage();
	}

	protected void GetAchievementMessage()
	{
		LoggerHelper.Debug("OperationSystem GetAchievementMessage");
		SetAchievementMessage();
		// RpcGetAllAchievement();
	}

	protected void EventTimesUp(int eventID)
	{
		LoggerHelper.Debug("OperationSystem EventTimesUp");
		RpcLeaveEvent(eventID);
	}

	protected void GetLoginMarket()
	{
		LoggerHelper.Debug("OperationSystem GetLoginMarket");

		if (LoginMarketData.dataMap == null)
			RpcUpdateLoginMarketCheck(0);
		else if (LoginMarketData.dataMap.Count == 0)
			RpcUpdateLoginMarketCheck(0);
		else
			RpcUpdateLoginMarketCheck(LoginMarketData.dataMap[1].version);
	}

	#endregion



	#region RPC请求

	public void RpcCharge()
	{
		LoggerHelper.Debug("RpcCharge");
		// Charge
		// theOwner.RpcCall("");
		// OnCharge();
	}

	public void RpcChargeGetRewardGot()
	{
		LoggerHelper.Debug("RpcCharge");
		theOwner.RpcCall("get_done_recharge");
	}

	public void RpcChargeGetReward(int id)
	{
		LoggerHelper.Debug("RpcChargeGetReward");
		theOwner.RpcCall("get_reward_recharge", (UInt16)id);
		// OnChargeGetReward();
	}

	public void RpcGetDoneCharge()
	{
		LoggerHelper.Debug("RpcGetDoneCharge");
		theOwner.RpcCall("get_done_recharge");
		// OnGetDoneRecharge(null);
	}

	public void RpcEventGetReward(int id)
	{
		LoggerHelper.Debug("RpcEventGetReward: " + id);
		theOwner.RpcCall("get_reward", (UInt16)id);
		// OnEventGetReward();
	}

	public void RpcEventShareToGetDiamond()
	{
		LoggerHelper.Debug("RpcEventShareToGetDiamond");
		// theOwner.RpcCall("");
		// OnEventShareToGetDiamond();
	}

	public void RpcGetAllActivityEnable()
	{
		LoggerHelper.Debug("RpcGetAllActivityEnable");
		theOwner.RpcCall("EventOpenList");
	}

	public void RpcGetAllActivityDoing()
	{
		LoggerHelper.Debug("RpcGetAllActivityDoing");
		theOwner.RpcCall("get_event_ing");
	}

	public void RpcAddActivity(int id)
	{
		LoggerHelper.Debug("RpcAddActivity: " + id);
		theOwner.RpcCall("join_event", (UInt16)id);
		// OnGetActivityID(null);
	}

	public void RpcGetLogin()
	{
		LoggerHelper.Debug("RpcGetLogin");
		theOwner.RpcCall("get_reward_login");
	}


	public void RpcGetAllAchievement()
	{
		LoggerHelper.Debug("RpcGetLogin");
		theOwner.RpcCall("get_achievement");
		// SetAchievementMessage();
	}

	public void RpcGetAchievementID(int id)
	{
		LoggerHelper.Debug("RpcGetAchievementID: " + id);
		theOwner.RpcCall("get_reward_achievement", (UInt16)id);
		// OnGetAchievementID();
	}

	public void RpcAchievemenShareToGetDiamond(int id)
	{
		LoggerHelper.Debug("RpcAchievemenShareToGetDiamond: " + id);
		// Temp
		theOwner.RpcCall("get_reward_achievement", (UInt16)id);
		// OnAchievemenShareToGetDiamond();
	}

	public void RpcUpdateLoginMarketCheck(int version)
	{
		LoggerHelper.Debug("RpcUpdateMarketCheck: " + version);

		theOwner.RpcCall("HotSalesVersionCheck", (UInt16)version);
	}

	public void RpcLoginMarketBuy()
	{
		LoggerHelper.Debug("RpcUpdateMarket");
        if ((MogoTime.Instance.GetCurrentDateTime().Year == MogoTime.Instance.GetDateTimeBySecond((int)theOwner.buyHotSalesLastTime).Year && MogoTime.Instance.GetCurrentDateTime().DayOfYear != MogoTime.Instance.GetDateTimeBySecond((int)theOwner.buyHotSalesLastTime).DayOfYear)
            || (MogoTime.Instance.GetCurrentDateTime().Year != MogoTime.Instance.GetDateTimeBySecond((int)theOwner.buyHotSalesLastTime).Year))
		{
			LoggerHelper.Debug("RpcUpdateMarket Time > 86400");
			var dataID = MogoTime.Instance.GetCurrentDateTime().Day;
			if (LoginMarketData.dataMap.ContainsKey(dataID))
			{
				var data = LoginMarketData.dataMap[dataID];

				LoggerHelper.Debug("RpcUpdateMarket Version: " + data.version + " ItemID: " + data.itemId + " PriceType: " + data.priceType + " Price: " + data.price + " Date: " + MogoTime.Instance.GetCurrentDateTime().ToString());

				theOwner.RpcCall("HotSalesBuy", (UInt16)data.version, (UInt32)data.itemId, (ushort)data.priceType, (UInt32)data.price);
			}
		}
	}

	public void RpcJoinEvent(int eventID)
	{
		//return;
        //为消除警告而注释以下代码
        //LoggerHelper.Debug("RpcJoinEvent");
        //theOwner.RpcCall("join_event", (UInt16)eventID);
	}

	public void RpcLeaveEvent(int eventID)
	{
        // 为去除警告暂时屏蔽以下代码
        //return;
        //LoggerHelper.Debug("RpcLeaveEvent");
        //theOwner.RpcCall("event_timeout", (UInt16)eventID);
	}

	#endregion



	#region RPC回调

	public void OnCharge()
	{
		SetCharge();
	}

	public void OnGetDoneRecharge(LuaTable luaTable)
	{
		object obj;
		List<int> temp = new List<int>();
		if (Utils.ParseLuaTable(luaTable, typeof(List<int>), out obj))
		{
			temp = obj as List<int>;
		}
		SetChargeGetReward(temp);
	}

	public void OnEventGetReward(UInt32 theID)
	{
		LoggerHelper.Debug("OnEventGetReward" + theID);
		int id = (int)theID;
		SetEventGetReward(id);
		RpcGetAllActivityEnable();
	}

	public void OnEventShareToGetDiamond(UInt32 theID)
	{
		int id = (int)theID;
		SetEventShareToGetDiamond(id);
	}

	public void OnGetAllActivityEnable(LuaTable luaTable)
	{
		HandleActivityEnableLuaTable(luaTable);

		RpcGetAllActivityDoing();

		//if (MogoUIManager.Instance.m_OperatingUI != null && MogoUIManager.Instance.m_CurrentUI == MogoUIManager.Instance.m_OperatingUI)
		//    SetActivityMessage();
	}

	public void OnGetAllActivityDoing(LuaTable luaTable, LuaTable luaTableToDo)
	{
		HandleActivityDoingLuaTable(luaTable, luaTableToDo);

		//if (MogoUIManager.Instance.m_OperatingUI != null && MogoUIManager.Instance.m_CurrentUI == MogoUIManager.Instance.m_OperatingUI)
		//    SetActivityMessage();
	}


	public void OnGetActivityID(LuaTable luaTable)
	{
		HandleActivityEnableLuaTable(luaTable);
		SetActivityMessage();
	}

	public void OnAddActivityID(UInt32 theID)
	{
		int id = (int)theID;
		if (eventEnable.Contains(id))
		{
			// MogoMsgBox.Instance.ShowFloatingText("Event Has Added");
			return;
		}
		eventEnable.Add(id);
		eventListening.Add(id);
		CheckEventOpen(theOwner);
	}

	public void OnJoinEvent(int eventID)
	{
		RpcGetAllActivityEnable();
	}

	public void OnLeaveEvent(int eventID)
	{
		RpcGetAllActivityEnable();
	}

    /// <summary>
    /// 领取登录奖励
    /// </summary>
	public void OnLoginGetReward()
	{
		LoginRewardUILogicManager.Instance.SetRewardGot();
	}

    /// <summary>
    /// 购买登录商品
    /// </summary>
	public void OnLoginMarketBuy()
	{
		LoginRewardUILogicManager.Instance.SetItemBought();
	}

	public void OnGetAllAchievement(LuaTable luaTable)
	{
		LoggerHelper.Debug("OnGetAllAchievement");
		HandleAchievementLuaTable(luaTable);

	}

	public void OnGetAchievementID(LuaTable luaTable, int index)
	{
		LoggerHelper.Debug("OnGetAchievementID");
		UpdateAchievementLuaTable(index, luaTable);

		if (MogoUIManager.Instance.m_OperatingUI != null && MogoUIManager.Instance.CurrentUI == MogoUIManager.Instance.m_OperatingUI && AttributeRewardLogicManager.hasInit)
			SetAchievementMessage();
	}

	public void OnAchievemenShareToGetDiamond(int theID)
	{
		LoggerHelper.Debug("OnAchievemenShareToGetDiamond");
		SetAchievemenShareToGetDiamond(theID);

		if (MogoUIManager.Instance.m_OperatingUI != null && MogoUIManager.Instance.CurrentUI == MogoUIManager.Instance.m_OperatingUI && AttributeRewardLogicManager.hasInit)
			SetAchievementMessage();
	}


	#endregion



	#region 刷新页

	protected void FlushCharge()
	{
		if (ChargeRewardUIViewManager.Instance != null)
			SetChargeRewardMessage();
		if (TimeLimitActivityUIViewManager.Instance != null)
			SetActivityMessage();
		if (LoginRewardUIViewManager.Instance != null)
			SetLoginMessage();
		if (MogoUIManager.Instance.m_OperatingUI != null && MogoUIManager.Instance.CurrentUI == MogoUIManager.Instance.m_OperatingUI && AttributeRewardLogicManager.hasInit)
			SetAchievementMessage();
	}


	#endregion



	#region 数据处理


	#region 充值

	protected void SetCharge()
	{
		SetChargeRewardMessage();
	}

	protected void SetChargeGetReward(List<int> ids)
	{
		chargeGotID.Clear();
		for (int i = 0; i < ids.Count; ++i)
		{
			chargeGotID.Add(ids[i]);
		}
		SetChargeRewardMessage();
	}

	protected void SetChargeRewardMessage()
	{
		LoggerHelper.Debug("chargeSum" + theOwner.chargeSum);

		if (theOwner.chargeSum > 0)
		{
			// to do all in this block
			int i = -1;
			foreach (var data in RewardRechargeData.dataMap)
			{
				i++;
				if (theOwner.chargeSum < data.Value.money || i == RewardRechargeData.dataMap.Count - 1)
				{
					LoggerHelper.Debug("SetChargeRewardMessage" + i);
					ChargeRewardUILogicManager.Instance.SetChargeProgress(i);
					break;
				}
			}
			ChargeRewardUIViewManager.Instance.ShowChargeRewardMessage();

			// doing hear and ignore the code before this
			ChargeRewardUILogicManager.Instance.UpdateRewardGot(chargeGotID);
		}
		else
		{
			var items = GetDefaultChargeReward();
			List<KeyValuePair<int, int>> itemMessages = new List<KeyValuePair<int, int>>();
			List<int> ids = new List<int>();
			if (items != null)
			{
				ChargeRewardUIViewManager.Instance.SetChargeRewardListEnable(items.Count);
				foreach (var item in items)
				{
					if (ItemParentData.GetItem(item.Key) != null)
					{
						ids.Add(item.Key);
						itemMessages.Add(item);
					}
				}
				LoggerHelper.Debug(ids.Count);

				ChargeRewardUIViewManager.Instance.SetChargeRewardListGrid(ids);
				ChargeRewardUIViewManager.Instance.SetChargeRewardListIcon(itemMessages);
				ChargeRewardUIViewManager.Instance.ShowFirstChargeRewardMessage();
			}
		}
		//  //MogoUIManager.Instance.SwitchChargeRewardUI();
	}

	protected Dictionary<int, int> GetDefaultChargeReward()
	{
		switch (theOwner.vocation)
		{
			case Vocation.Warrior:
				return RewardRechargeData.dataMap[1].items1;
			case Vocation.Assassin:
				return RewardRechargeData.dataMap[1].items2;
			case Vocation.Archer:
				return RewardRechargeData.dataMap[1].items3;
			case Vocation.Mage:
				return RewardRechargeData.dataMap[1].items4;
			default:
				return null;
		}
	}

	#endregion


	#region 活动

    /// <summary>
    /// 限时活动领取奖励
    /// </summary>
    /// <param name="id"></param>
	protected void SetEventGetReward(int id)
	{
		// to do 
		Mogo.Util.LoggerHelper.Debug("SetEventGetReward: =" + id);

		if (id == 5)
			PlatformSdkManager.Instance.AddNotificationRecord(1);
		else if (id == 6)
			PlatformSdkManager.Instance.AddNotificationRecord(2);

		if (MogoUIManager.Instance.m_NewTimeLimitActivityUI != null)
			TimeLimitActivityUIViewManager.Instance.SetLimitActivityInfoHasReward(id);
		MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(47000));
	}

    /// <summary>
    /// 限时活动分享
    /// </summary>
    /// <param name="id"></param>
	protected void SetEventShareToGetDiamond(int id)
	{
		// to do
        if (MogoUIManager.Instance.m_NewTimeLimitActivityUI != null)
			TimeLimitActivityUIViewManager.Instance.SetLimitActivityInfoHasReward(id);
		MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(47001));
	}

	protected void HandleActivityEnableLuaTable(LuaTable luaTable)
	{
		LoggerHelper.Debug("HandleActivityEnableLuaTable");

		object obj;
		if (Utils.ParseLuaTable(luaTable, typeof(List<int>), out obj))
		{
			Mogo.Util.LoggerHelper.Debug("HandleActivityEnableLuaTable" + Mogo.RPC.Utils.PackLuaTable(luaTable));
			eventEnable.Clear();
			eventEnable = obj as List<int>;
			LoggerHelper.Debug("HandleActivityEnableLuaTable Format Size: " + eventEnable.Count);

			SetActivityMessage();
		}
		else
		{
			LoggerHelper.Debug("HandleActivityEnableLuaTable Failed");
		}
	}

	protected void HandleActivityDoingLuaTable(LuaTable luaTable, LuaTable luaTableToDo)
	{
		LoggerHelper.Debug("HandleActivityDoingLuaTable");

		object obj;
		object objToDo;
		//if (Utils.ParseLuaTable(luaTable, typeof(Dictionary<int, EventMessageData>), out obj))
		//{
		//    Mogo.Util.LoggerHelper.Debug("HandleActivityDoingLuaTable" + Mogo.RPC.Utils.PackLuaTable(luaTable));
		//    eventDoing.Clear();
		//    eventDoing = obj as Dictionary<int, EventMessageData>;

		//    if (eventDoing == null)
		//        LoggerHelper.Debug("HandleActivityDoingLuaTable Format Size: " + "eventDoing.Count == null");
		//    else
		//        LoggerHelper.Debug("HandleActivityDoingLuaTable Format Size: " + eventDoing.Count);

		//    SetActivityMessage();
		//}
		//else
		//{
		//    MogoMsgBox.Instance.ShowFloatingText("HandleActivityDoingLuaTable Failed");
		//}

		if (Utils.ParseLuaTable(luaTable, typeof(Dictionary<int, Dictionary<int, int>>), out obj) && Utils.ParseLuaTable(luaTableToDo, typeof(Dictionary<int, Dictionary<int, int>>), out objToDo))
		{
			//Mogo.Util.LoggerHelper.Error("HandleActivityDoingLuaTable" + Mogo.RPC.Utils.PackLuaTable(luaTable));
			//Mogo.Util.LoggerHelper.Error("HandleActivityToDoLuaTable" + Mogo.RPC.Utils.PackLuaTable(luaTableToDo));

			eventDoing.Clear();

			Dictionary<int, Dictionary<int, int>> adapter = obj as Dictionary<int, Dictionary<int, int>>;
			Dictionary<int, Dictionary<int, int>> toDoAdapter = objToDo as Dictionary<int, Dictionary<int, int>>;

			if (toDoAdapter != null && adapter != null)
			{
				foreach (var temp in adapter)
				{
					if (toDoAdapter.ContainsKey(temp.Key))
						toDoAdapter.Remove(temp.Key);
				}
			}

			if (adapter != null)
			{
				foreach (var temp in adapter)
				{
					EventMessageData data = new EventMessageData();
					data.is_finish = temp.Value[2];
					data.is_reward = temp.Value[3];
					data.accept_time = temp.Value[4];

					data.is_doing = true;

					eventDoing.Add(temp.Key, data);
				}
			}

			if (toDoAdapter != null)
			{
				foreach (var tempToDo in toDoAdapter)
				{
					if (tempToDo.Value.Count == 1 && tempToDo.Value.ContainsKey(4))
					{
						EventMessageData data = new EventMessageData();
						data.is_finish = 0;
						data.is_reward = 0;
						data.accept_time = 0;

						data.is_doing = false;

						eventDoing.Add(tempToDo.Key, data);
					}
				}
			}

			if (eventDoing == null)
				LoggerHelper.Debug("HandleActivityDoingLuaTable Format Size: " + "eventDoing.Count == null");
			else
				LoggerHelper.Debug("HandleActivityDoingLuaTable Format Size: " + eventDoing.Count);

			SetActivityMessage();
		}
		else
		{
			LoggerHelper.Debug("HandleActivityDoingLuaTable Failed");
		}
	}

	protected void SetActivityMessage()
	{
		eventListening.Clear();

		foreach (int enableID in eventEnable)
		{
			if (!EventData.dataMap.ContainsKey(enableID))
			{
				LoggerHelper.Debug("Activity " + enableID + " Not Exist!");
				continue;
			}
			else if (!eventDoing.ContainsKey(enableID))
			{
				var data = EventData.dataMap[enableID];
				if (data.conditions == null)
				{
					RpcJoinEvent(enableID);
					continue;
				}
				int conditionCount = 0;
				foreach (var condition in data.conditions)
				{
					switch (condition.Key)
					{
						case 1:
							// Rpc Pass Judge
							if (theOwner.level >= condition.Value)
								conditionCount++;
							break;

						case 2:
							// Rpc Pass Judge
							if (FriendManager.Instance.GetFriendList().Count >= condition.Value)
								conditionCount++;
							break;

						case 3:
							// Rpc Pass Judge
							// Kill Monster Number
							break;

						case 4:
							// Rpc Pass Judge
							// Kill Boss Number
							break;

						case 5:
							// Rpc Pass Judge
							// Pass Challenge Tower Number
							break;

						default:
							break;
					}

					if (conditionCount == data.conditions.Count)
					{
						RpcAddActivity(enableID);
					}
					else
					{
						eventListening.Add(enableID);
					}
				}
			}
		}

		sortedActivity.Clear();
		sortedActivity = SortAllEvent(eventDoing);

		#region Debug
		StringBuilder sb = new StringBuilder();
		sb.Append("eventEnable:");
		foreach (var id in eventEnable)
		{
			sb.Append(" " + id);
		}
		//Mogo.Util.LoggerHelper.Debug(sb.ToString());

		sb = new StringBuilder();
		sb.Append("eventDoing:");
		foreach (var id in eventDoing)
		{
			sb.Append(" " + id.Key);
		}
		//Mogo.Util.LoggerHelper.Debug(sb.ToString());

		sb = new StringBuilder();
		sb.Append("eventListening:");
		foreach (var id in eventListening)
		{
			sb.Append(" " + id);
		}
		//Mogo.Util.LoggerHelper.Debug(sb.ToString());
		#endregion

        //if (MogoUIManager.Instance.m_OperatingUI != null && 
        //    MogoUIManager.Instance.m_TimeLimitActivityUI != null && 
        //    MogoUIManager.Instance.CurrentUI == MogoUIManager.Instance.m_OperatingUI)

        if (MogoUIManager.Instance.m_NewTimeLimitActivityUI != null)
			ShowEventDoing();
	}

	protected void CheckEventOpen(EntityParent entity)
	{
		LoggerHelper.Debug("CheckEventOpen");

		if (entity.ID != theOwner.ID)
		{
			LoggerHelper.Debug("CheckEventOpen Not EntityMyself!");
			return;
		}

		foreach (int listeningID in eventListening)
		{
			if (EventData.dataMap.ContainsKey(listeningID))
			{
				var data = EventData.dataMap[listeningID];
				if (data.conditions == null)
				{
					RpcAddActivity(listeningID);
					continue;
				}
				int conditionCount = 0;
				foreach (var condition in data.conditions)
				{
					switch (condition.Key)
					{
						case 1:
							// Rpc Pass Judge
							LoggerHelper.Debug("CheckEventOpen Rpc Pass Judge");
							if (theOwner.level >= condition.Value)
								conditionCount++;
							break;

						case 2:
							// Rpc Pass Judge
							if (FriendManager.Instance.GetFriendList().Count >= condition.Value)
								conditionCount++;
							break;

						case 3:
							// Rpc Pass Judge
							// Kill Monster Number
							break;

						case 4:
							// Rpc Pass Judge
							// Kill Boss Number
							break;

						case 5:
							// Rpc Pass Judge
							// Pass Challenge Tower Number
							break;

						default:
							break;
					}
				}
				LoggerHelper.Debug("CheckEventOpen Rpc Pass Judge: conditionCount: " + conditionCount);
				LoggerHelper.Debug("CheckEventOpen Rpc Pass Judge: data.conditions.Count: " + data.conditions.Count);
				if (conditionCount == data.conditions.Count)
				{
					LoggerHelper.Debug("CheckEventOpen Rpc Pass Judge: conditionCount == data.conditions.Count");
					RpcAddActivity(listeningID);
				}
			}
		}
	}

	protected List<KeyValuePair<int, EventMessageData>> SortAllEvent(Dictionary<int, EventMessageData> eventIDMessages)
	{
		List<KeyValuePair<int, EventMessageData>> result = new List<KeyValuePair<int, EventMessageData>>();

		eventCanGetGroup.Clear();
		eventDoingGroup.Clear();
		eventGotGroup.Clear();
		eventTimeOutGroup.Clear();
		eventNotOpenGroup.Clear();

		foreach (var eventIDMessage in eventIDMessages)
		{
            if (!EventData.dataMap.ContainsKey(eventIDMessage.Key))
                continue;

			if (eventIDMessage.Value.is_doing)
			{
				if (eventIDMessage.Value.is_reward > 0)
					eventGotGroup.Add(eventIDMessage);
				else if (eventIDMessage.Value.is_finish > 0)
					eventCanGetGroup.Add(eventIDMessage);
				else
					eventDoingGroup.Add(eventIDMessage);
			}
			else
			{
				eventNotOpenGroup.Add(eventIDMessage);
			}
		}

        // 显示叹号
        if (eventCanGetGroup.Count > 0)
            NormalMainUILogicManager.Instance.TimeLimitActivityTip = true;
        else
            NormalMainUILogicManager.Instance.TimeLimitActivityTip = false;

        foreach (var item in eventCanGetGroup)
            result.Add(item);

		foreach (var item in eventDoingGroup)
			result.Add(item);

		foreach (var item in eventNotOpenGroup)
			result.Add(item);

		foreach (var item in eventGotGroup)
			result.Add(item);

		return result;
	}

	//public void OnJoinEvent(int eventID)
	//{
	//    if (EventData.dataMap.ContainsKey(eventID))
	//    {
	//        // List<EventMessageData> temp = new List<EventMessageData>();
	//        //foreach (var condition in EventData.dataMap[eventID].task_conditions)
	//        //{
	//        //    temp.Add(new EventMessageData());
	//        //}
	//        EventMessageData temp = new EventMessageData();
	//        if (!eventDoing.ContainsKey(eventID))
	//            eventDoing.Add(eventID, temp);
	//        eventListening.Remove(eventID);
	//        SetActivityMessage();

	//        if (MogoUIManager.Instance.m_OperatingUI != null && MogoUIManager.Instance.m_CurrentUI == MogoUIManager.Instance.m_OperatingUI)
	//            ShowEventDoing();
	//    }
	//}

	public void ShowEventDoing()
	{
		LoggerHelper.Debug("ShowEventDoing");
        TimeLimitActivityUILogicManager.Instance.SetAllActivityAndAcitivityInfoList(eventDoing);
	}

	#endregion


	#region 登陆

	// 
	protected void CheckLoginFirstShow()
	{
		LoggerHelper.Debug("CheckLoginFirstShow: " + loginFirstShow + " " + loginRewardHasGot);

        if (theOwner.IsNewPlayer)
            return;

		if (loginFirstShow && !loginRewardHasGot)
		{
			LoggerHelper.Debug("CheckLoginFirstShow In");

            #region 进度记录

            GameProcManager.ShowLoginReward();

            #endregion

			MogoUIManager.Instance.ShowMogoOperatingUI(2, false);
		}
	}

	protected void CheckTimeResetFirstShow(int missionID, bool isInstance)
	{
		if (!isInstance)
		{
			if (loginFirstShow)
			{
				CheckLoginFirstShow();
			}
		}
	}

	protected void SetLoginMessage()
	{
		LoginRewardUIViewManager.Instance.EmptyLoginRewardGridList();

		int index = 0;

		foreach (var data in RewardLoginData.dataMap)
		{
			if (data.Value.level.Count != 2)
			{
				LoggerHelper.Debug("RewardLoginData level wrong, id: " + data.Key);
				continue;
			}

			if (theOwner.level >= data.Value.level[0] && theOwner.level <= data.Value.level[1])
			{
				LoginRewardUILogicManager.Instance.SetGridInfo(data.Key, data.Value);
			}
		}


        LoginRewardUIViewManager.Instance.IsFirstLoadRewardGrid = false;
	} 

	#endregion


	#region 成就

	protected void HandleAchievementLuaTable(LuaTable luaTable)
	{
		LoggerHelper.Debug("HandleAchievementLuaTable");

		object obj;
		//if (Utils.ParseLuaTable(luaTable, typeof(List<AchievementMessageData>), out obj))
		//{
		//    Mogo.Util.LoggerHelper.Debug("achievementDoing" + Mogo.RPC.Utils.PackLuaTable(luaTable));
		//    achievementDoing.Clear();
		//    achievementDoing = obj as List<AchievementMessageData>;
		//    LoggerHelper.Debug("HandleAchievementLuaTable Format Size: " + achievementDoing.Count);
		//}
		//else
		//{
		//    MogoMsgBox.Instance.ShowFloatingText("HandleAchievementLuaTable Failed");
		//}


		// to do 
		if (Utils.ParseLuaTable(luaTable, typeof(List<Dictionary<int, int>>), out obj))
		{
			Mogo.Util.LoggerHelper.Debug("achievementDoing" + Mogo.RPC.Utils.PackLuaTable(luaTable));

			List<Dictionary<int, int>> tempAchievementDoing = obj as List<Dictionary<int, int>>;
			achievementDoing.Clear();

			foreach (var tempAchievementDoingData in tempAchievementDoing)
			{
				AchievementMessageData tempAchievementMessageData = new AchievementMessageData();
				tempAchievementMessageData.aid = tempAchievementDoingData[2];
				tempAchievementMessageData.cur_num = tempAchievementDoingData[1];
				tempAchievementMessageData.level = tempAchievementDoingData[3];
				tempAchievementMessageData.reward_level = tempAchievementDoingData[4];

				achievementDoing.Add(tempAchievementMessageData);
			}

			LoggerHelper.Debug("HandleAchievementLuaTable Format Size: " + achievementDoing.Count);
		}
		else
		{
			LoggerHelper.Debug("HandleAchievementLuaTable Failed");
		}

		SetAchievementMessage(false);
	}

	protected void UpdateAchievementLuaTable(int index, LuaTable luaTable)
	{
		LoggerHelper.Debug("UpdateAchievementLuaTable");

		object obj;
		//if (Utils.ParseLuaTable(luaTable, typeof(List<AchievementMessageData>), out obj))
		//{
		//    Mogo.Util.LoggerHelper.Debug("achievementDoing" + Mogo.RPC.Utils.PackLuaTable(luaTable));
		//    List<AchievementMessageData> motifyList = obj as List<AchievementMessageData>;
		//    LoggerHelper.Debug("UpdateAchievementLuaTable Format Size: " + motifyList[0].aid + " " + motifyList[0].level + " " + motifyList[0].reward_level + " " + motifyList[0].cur_num);

		//    foreach (var data in achievementDoing)
		//    {
		//        if (data.aid == motifyList[0].aid)
		//        {
		//            data.cur_num = motifyList[0].cur_num;
		//            data.level = motifyList[0].level;
		//            data.reward_level = motifyList[0].reward_level;
		//            break;
		//        }
		//    }
		//}
		//else
		//{
		//    MogoMsgBox.Instance.ShowFloatingText("HandleAchievementLuaTable Failed");
		//}


		// to do 
		if (Utils.ParseLuaTable(luaTable, typeof(List<Dictionary<int, int>>), out obj))
		{
			Mogo.Util.LoggerHelper.Debug("achievementDoing" + Mogo.RPC.Utils.PackLuaTable(luaTable));
			List<Dictionary<int, int>> motifyList = obj as List<Dictionary<int, int>>;

			foreach (var data in achievementDoing)
			{
				if (data.aid == motifyList[0][2])
				{
					data.cur_num = motifyList[0][1];
					data.level = motifyList[0][3];
					data.reward_level = motifyList[0][4];
					break;
				}
			}
		}
		else
		{
			LoggerHelper.Debug("HandleAchievementLuaTable Failed");
		}

	}

	protected void SetAchievementMessage(bool isRefreshUI = true)
	{
		LoggerHelper.Debug("SetAchievementMessage");
		
		if (isRefreshUI && AttributeRewardUIViewManager.Instance != null)
		{
			AttributeRewardUIViewManager.Instance.EmptyAttributeGridList();
		}

		//int id = 0;
		//bool isShared = false;
		//bool isFinished = false;
		//bool isRunning = false;

		bool bAttributeRewardTip = false;
		foreach (var item in achievementDoing)
		{
			int id = 0;
			bool isShared = false;
			bool isFinished = false;
			bool isRunning = false;

			Mogo.Util.LoggerHelper.Debug("Achievement: " + item.aid + " " + item.level + " " + item.reward_level + " " + item.cur_num);

			foreach (var data in RewardAchievementData.dataMap)
			{
				if (data.Value.aid == item.aid && data.Value.level == item.reward_level + 1)
				{
					id = data.Key;
					if (data.Value.args[0] > item.cur_num)
					{
						isShared = false;
						isFinished = false;
						isRunning = true;
					}
					else
					{
						bool hasBigger = false;
						foreach (var curData in RewardAchievementData.dataMap)
						{
							if (curData.Value.aid == item.aid && curData.Value.level > item.reward_level)
							{
								hasBigger = true;
								break;
							}
						}
						if (hasBigger)
						{
							isShared = true;
							isFinished = false;
							isRunning = false;

							bAttributeRewardTip = true;
						}
						else
						{
							isShared = false;
							isFinished = true;
							isRunning = false;
						}
					}
					break;
				}
				else if (data.Value.aid == item.aid && data.Value.level == item.reward_level)
				{
					bool hasBigger = false;
					foreach (var curData in RewardAchievementData.dataMap)
					{
						if (curData.Value.aid == item.aid && curData.Value.level > item.reward_level)
						{
							hasBigger = true;
							break;
						}
					}
					if (!hasBigger)
					{
						id = data.Key;
						isShared = false;
						isFinished = true;
						isRunning = false;
					}
				}
			}

			if (RewardAchievementData.dataMap.ContainsKey(id))
			{
				if (isRefreshUI)
				{
					Mogo.Util.LoggerHelper.Debug("id: " + id + " item.cur_num: " + item.cur_num + " isShared: " + isShared + " isFinished: " + isFinished);
					AttributeRewardLogicManager.Instance.SetGridInfo(
						id,
						RewardAchievementData.dataMap[id],
						item.cur_num,
						isShared,
						isFinished,
						isRunning);
				}              
			}
			else
			{
				LoggerHelper.Debug("Achievement Id = " + id + " Not Exist");
			}
		}

		// AttributeRewardTip       
		NormalMainUILogicManager.Instance.AttributeRewardTip = bAttributeRewardTip;
	}
	

	protected void SetAchievemenShareToGetDiamond(int theID)
	{
		LoggerHelper.Debug("SetAchievemenShareToGetDiamond: " + theID);

		// 显示放在下一次返回数据的时候做了
		// 这里就不处理了
	}

	#endregion


	#endregion



	#region RPC Error Code

	public void HandleErrorMsg(int errorCode, string from, int arg = 0)
	{
		// StringBuilder sb = new StringBuilder();
		switch (errorCode)
		{
			case 1:
				LoggerHelper.Debug("error_code_event_ing " + arg + " ");
				// sb.Append("error_code_event_ing " + arg + " ");
				break;
			case 2:
				LoggerHelper.Debug("error_code_event_not_in_time " + arg + " ");
				// sb.Append("error_code_event_not_in_time");
				break;
			case 3:
				LoggerHelper.Debug("error_code_event_beyond_count " + arg + " ");
				// sb.Append("error_code_event_beyond_count");
				break;
			case 4:
				LoggerHelper.Debug("error_code_event_closed " + arg + " ");
				// sb.Append("error_code_event_closed");
				break;
			case 5:
				LoggerHelper.Debug("error_code_event_done " + arg + " ");
				// sb.Append("error_code_event_done");
				break;
			case 6:
				LoggerHelper.Debug("error_code_event_no_begin " + arg + " ");
				// sb.Append("error_code_event_no_begin");
				break;
			case 7:
				LoggerHelper.Debug("error_code_event_not_finish " + arg + " ");
				// sb.Append("error_code_event_not_finish");
				break;
			case 8:
				LoggerHelper.Debug("error_code_event_rewarded ");
				// sb.Append("error_code_event_rewarded ");
				break;
			case 9:
				LoggerHelper.Debug("error_code_event_rewarded ");
				// sb.Append("error_code_event_less_recharge");
				break;
			case 10:
				LoggerHelper.Debug("error_code_event_rewarded ");
				// sb.Append("error_code_event_not_login");
				break;
			case 11:
				LoggerHelper.Debug("error_code_event_rewarded ");
				// sb.Append("error_code_event_config_not_found");
				break;
			case 12:
				LoggerHelper.Debug("error_code_event_rewarded ");
				// sb.Append("error_code_event_unknow from 12");
				break;
			default:
				LoggerHelper.Debug("error_code_event_rewarded ");
				// sb.Append("error_code_event_unknow from default");
				break;
		}
		//if (sb.Length > 0)
		//{
		//    sb.Append(" From ");
		//    sb.Append(from);
		//    MogoMsgBox.Instance.ShowFloatingText(sb.ToString());
		//}
	}

	public void HandleLoginMarketErrorMsg(int errorCode)
	{
		switch (errorCode)
		{
			case 1:
				// MogoMsgBox.Instance.ShowFloatingText("Error Version");
				LoggerHelper.Debug("Error Version");
				break;
			case 2:
				// MogoMsgBox.Instance.ShowFloatingText("Data Check Error");
				LoggerHelper.Debug("Data Check Error");
				break;
			case 3:
				// MogoMsgBox.Instance.ShowFloatingText("Time Check Error");
				LoggerHelper.Debug("Time Check Error");
				break;
			case 4:
				// MogoMsgBox.Instance.ShowFloatingText("Gold Check Error");
				LoggerHelper.Debug("Gold Check Error");
				break;
			case 5:
				// MogoMsgBox.Instance.ShowFloatingText("Diamond Check Error");
				LoggerHelper.Debug("Diamond Check Error");
				break;
			case 6:
				// MogoMsgBox.Instance.ShowFloatingText("Inventory Check Error");
				LoggerHelper.Debug("Inventory Check Error");
				break;
			default:
				break;
		}
	}

	#endregion



	#region 获取数据

	public EventMessageData GetEventDoingMessage(int id)
	{
		return eventDoing.ContainsKey(id) ? eventDoing[id] : new EventMessageData();
	}

	#region 小助手

	public void CheckAssistantShow(int missionID, bool isInstance)
	{
		if (missionID != MogoWorld.globalSetting.homeScene)
			return;

		if (eventCanGetGroup.Count == 0)
			return;

        //获取宝石的邮件DBID
        //LoggerHelper.Error("......................................................");
        theOwner.RpcCall("JewlMailReq");

        foreach (var item in eventCanGetGroup)
        {
            if (!EventData.dataMap.ContainsKey(item.Key))
                return;

            TipViewData temp = new TipViewData();
            temp.priority = TipManager.TIP_TYPE_ENERGY_EVENT;
            temp.icon = IconData.dataMap.Get(EventData.dataMap[item.Key].icon).path;
            temp.btnAction = () =>
            {
                MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RewardUI);
                RewardUIViewManager.Instance.SetUIDirty();
                MogoUIManager.Instance.SwitchNewTimeLimitActivityUI(true);
            };
            temp.atlasName = "MogoOperatingUI";

            if (EventData.dataMap[item.Key].energy > 0)
            {
                temp.tipText = LanguageData.GetContent(3035);
                temp.btnName = LanguageData.GetContent(3036);
            }
            else
            {
                temp.tipText = LanguageData.GetContent(3033);
                temp.btnName = LanguageData.GetContent(3034);
            }

            TipUIManager.Instance.AddTipViewData(temp);
        }


	}

	public string IsContainEventCanGainEnergy()
	{
        foreach (var item in eventCanGetGroup)
        {
            if (EventData.dataMap.ContainsKey(item.Key)
                && EventData.dataMap[item.Key].energy > 0)
                return IconData.dataMap.Get(EventData.dataMap[item.Key].icon).path;
        }
        return string.Empty;
	}

	#endregion

	#region 体力不足系统

	public List<EnergyLimitActivityData> GetEnergyLimitActivityDataList()
	{
		List<EnergyLimitActivityData> result = new List<EnergyLimitActivityData>();

        if (eventCanGetGroup != null)
        {
            foreach (var item in eventCanGetGroup)
            {
                if (EventData.dataMap.ContainsKey(item.Key) && EventData.dataMap[item.Key].energy > 0)
                {
                    EnergyLimitActivityData temp = new EnergyLimitActivityData();

                    temp.finished = true;
                    temp.title = LanguageData.GetContent(EventData.dataMap[item.Key].name);
                    temp.icon = IconData.dataMap[EventData.dataMap[item.Key].icon].path;
                    temp.limitActivityDesc = LanguageData.dataMap.Get(EventData.dataMap[item.Key].reminder).Format(EventData.dataMap[item.Key].energy);
                    temp.buttonName = LanguageData.GetContent(47902);

                    result.Add(temp);
                }
            }
        }

        if (eventDoingGroup != null)
        {
            foreach (var item in eventDoingGroup)
            {
                if (EventData.dataMap.ContainsKey(item.Key) && EventData.dataMap[item.Key].energy > 0)
                {
                    EnergyLimitActivityData temp = new EnergyLimitActivityData();

                    temp.finished = false;
                    temp.title = LanguageData.GetContent(EventData.dataMap[item.Key].name);
                    temp.icon = IconData.dataMap[EventData.dataMap[item.Key].icon].path;
                    temp.limitActivityDesc = LanguageData.dataMap.Get(EventData.dataMap[item.Key].reminder).Format(EventData.dataMap[item.Key].energy);
                    temp.buttonName = LanguageData.GetContent(47902);

                    result.Add(temp);
                }
            }
        }

        if (eventNotOpenGroup != null)
        {
            foreach (var item in eventNotOpenGroup)
            {
                if (EventData.dataMap.ContainsKey(item.Key) && EventData.dataMap[item.Key].energy > 0)
                {
                    EnergyLimitActivityData temp = new EnergyLimitActivityData();

                    temp.finished = false;
                    temp.title = LanguageData.GetContent(EventData.dataMap[item.Key].name);
                    temp.icon = IconData.dataMap[EventData.dataMap[item.Key].icon].path;
                    temp.limitActivityDesc = LanguageData.dataMap.Get(EventData.dataMap[item.Key].reminder).Format(EventData.dataMap[item.Key].energy);
                    temp.buttonName = LanguageData.GetContent(47902);

                    result.Add(temp);
                }
            }
        }

        return result;
	}

	#endregion

	#endregion

}

