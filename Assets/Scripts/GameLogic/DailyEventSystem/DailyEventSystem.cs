using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public class DailyEventMessageData
{
    public int cur_num { get; set; }
    public int is_reward { get; set; }
    public int is_finish { get; set; }
    public int exp { get; set;}
    public int gold { get; set;}
}

public class DailyEventSystem : IEventManager
{
    #region 变量

    protected EntityMyself theOwner;

    protected Dictionary<int, DailyEventMessageData> dailyEventSourceData = new Dictionary<int, DailyEventMessageData>();
    protected List<KeyValuePair<int, DailyEventMessageData>> dailyEventData = new List<KeyValuePair<int,DailyEventMessageData>>();

    #endregion


    #region 初始化

    public DailyEventSystem(EntityMyself owner)
    {
        theOwner = owner;
        dailyEventSourceData = new Dictionary<int, DailyEventMessageData>();
        dailyEventData = new List<KeyValuePair<int, DailyEventMessageData>>();

        AddListeners();

        GetDailyEventData();
    }

    public void AddListeners()
    {
        #region UI事件

        EventDispatcher.AddEventListener(Events.DailyTaskEvent.ShowDailyEvent, ShowDailyEventData);

        #endregion

        #region 逻辑事件

        EventDispatcher.AddEventListener(Events.DailyTaskEvent.GetDailyEventData, GetDailyEventData);
        EventDispatcher.AddEventListener<int>(Events.DailyTaskEvent.GetDailyEventReward, GetDailyEventReward);

        #endregion
    }

    public void RemoveListeners()
    {
        #region UI事件

        EventDispatcher.RemoveEventListener(Events.DailyTaskEvent.ShowDailyEvent, ShowDailyEventData);

        #endregion

        #region 逻辑事件

        EventDispatcher.RemoveEventListener(Events.DailyTaskEvent.GetDailyEventData, GetDailyEventData);
        EventDispatcher.RemoveEventListener<int>(Events.DailyTaskEvent.GetDailyEventReward, GetDailyEventReward);

        #endregion
    }

    #endregion


    #region 事件触发

    public void GetDailyEventData()
    {
        RpcGetDailyEventData();
    }

    public void GetDailyEventReward(int id)
    {
        RpcGetDailyEventReward(id);
    }

    #endregion


    #region RPC请求

    protected void RpcGetDailyEventData()
    {
        // Debug.LogError("RpcGetDailyEventData");
        theOwner.RpcCall("get_day_task");
    }

    protected void RpcGetDailyEventReward(int id)
    {
        theOwner.RpcCall("get_reward_day_task", (ushort)id);
    }

    #endregion


    #region RPC回调

    public void OnFlushAllData(LuaTable luaTable)
    {
        // Debug.LogError("OnFlushAllData: " + luaTable.ToString());
        HandleAllDailyEventLuaTable(luaTable);
    }

    public void OnFlushTaskData(int id, LuaTable luaTable)
    {
        // Debug.LogError("OnFlushTaskData: " + id + " " + luaTable.ToString());
        DailyEventUpdateData(id, luaTable);
    }

    public void OnFinishDayTask(int id)
    {
        // Debug.LogError("OnFinishDayTask: " + id);
        DailyEventFinished(id);
    }

    public void OnGetDailyEventReward(int id)
    {
        // Debug.LogError("OnGetDailyEventReward: " + id);
        DailyEventGotReward(id);
    }


    public void OnHandleErrorCode(int errorCode)
    {
        // Debug.LogError("HandleErrorCode: " + errorCode);
        HandleErrorCode(errorCode);
    }

    #endregion


    #region 数据

    protected void HandleAllDailyEventLuaTable(LuaTable luaTable)
    {
        // Debug.LogError("HandleDailyEventLuaTable: " + luaTable.ToString());

        object obj;
        //if (Utils.ParseLuaTable(luaTable, typeof(Dictionary<int, DailyEventMessageData>), out obj))
        //{
        //    dailyEventSourceData.Clear();
        //    dailyEventData.Clear();
        //    dailyEventSourceData = obj as Dictionary<int, DailyEventMessageData>;
        //    FormatDailyEventList();
        //}
        //else
        //{
        //    MogoMsgBox.Instance.ShowFloatingText("HandleDailyEventLuaTable Failed");
        //}

        if (Utils.ParseLuaTable(luaTable, typeof(Dictionary<int, Dictionary<int, int>>), out obj))
        {
            dailyEventSourceData.Clear();
            dailyEventData.Clear();

            Dictionary<int, Dictionary<int, int>> tempDailyEventSourceData = obj as Dictionary<int, Dictionary<int, int>>;

            foreach (var tempDailyEventSource in tempDailyEventSourceData)
            {
                DailyEventMessageData tempDailyEventMessageData = new DailyEventMessageData();
                tempDailyEventMessageData.cur_num = tempDailyEventSource.Value[1];
                tempDailyEventMessageData.is_finish = tempDailyEventSource.Value[2];
                tempDailyEventMessageData.is_reward = tempDailyEventSource.Value[3];
                tempDailyEventMessageData.gold = tempDailyEventSource.Value[4];
                tempDailyEventMessageData.exp = tempDailyEventSource.Value[5];
                dailyEventSourceData.Add(tempDailyEventSource.Key, tempDailyEventMessageData);
            }
                 
            FormatDailyEventList();
        }
        else
        {
            LoggerHelper.Debug("HandleDailyEventLuaTable Failed");
        }
    }

    protected void DailyEventUpdateData(int id, LuaTable luaTable)
    {
        if (DailyEventData.dataMap.ContainsKey(id))
        {
            object obj;
            //if (Utils.ParseLuaTable(luaTable, typeof(DailyEventMessageData), out obj))
            //{
            //    DailyEventMessageData newMessageData = obj as DailyEventMessageData;
            //    if (dailyEventSourceData.ContainsKey(id))
            //    {
            //        if (dailyEventSourceData[id].is_finish == 0)
            //            dailyEventSourceData[id] = newMessageData;
            //    }
            //    else
            //    {
            //        dailyEventSourceData.Add(id, newMessageData);
            //    }
            //    FormatDailyEventList();
            //}

            if (Utils.ParseLuaTable(luaTable, typeof(Dictionary<int, int>), out obj))
            {
                Dictionary<int, int> adapter = obj as  Dictionary<int, int>;
                DailyEventMessageData newMessageData = new DailyEventMessageData();

                newMessageData.cur_num = adapter[1];
                newMessageData.is_finish = adapter[2];
                newMessageData.is_reward = adapter[3];
                newMessageData.gold = adapter[4];
                newMessageData.exp = adapter[5];

                if (dailyEventSourceData.ContainsKey(id))
                {
                    if (dailyEventSourceData[id].is_finish == 0)
                        dailyEventSourceData[id] = newMessageData;
                }
                else
                {
                    dailyEventSourceData.Add(id, newMessageData);
                }
                FormatDailyEventList();
            }
        }
        else
        {
            LoggerHelper.Debug("HandleDailyEventLuaTable id: " + id + " Not Exist");
        }
    }

    protected void DailyEventFinished(int id)
    {
        if (DailyEventData.dataMap.ContainsKey(id))
        {
            DailyEventData data = DailyEventData.dataMap[id];
            if (dailyEventSourceData.ContainsKey(id))
            {
                dailyEventSourceData[id].is_finish = 1;
                FormatDailyEventList();
            }
            //else
            //{
            //    DailyEventMessageData tempMessageData = new DailyEventMessageData();
            //    tempMessageData.cur_num = data.finish[1];
            //    tempMessageData.is_finish = 1;
            //    tempMessageData.is_reward = 0;
            //    dailyEventSourceData.Add(id, tempMessageData);
            //    FormatDailyEventList();
            //}
        }
        else
        {
            LoggerHelper.Debug("HandleDailyEventFinish id: " + id + " Not Exist");
        }
    }

    protected void DailyEventGotReward(int id)
    {
        if (DailyEventData.dataMap.ContainsKey(id))
        {
            DailyEventData data = DailyEventData.dataMap[id];
            if (dailyEventSourceData.ContainsKey(id))
            {
                dailyEventSourceData[id].is_reward = 1;
            }
            else
            {
                DailyEventMessageData tempMessageData = new DailyEventMessageData();
                tempMessageData.cur_num = data.finish[1];
                tempMessageData.is_finish = 1;
                dailyEventSourceData[id].is_reward = 1;
                dailyEventSourceData.Add(id, tempMessageData);
            }

            ShowTaskEventMessage(data);
            FormatDailyEventList();
        }
        else
        {
            LoggerHelper.Debug("HandleDailyEventFinish id: " + id + " Not Exist");
        }
    }

    protected void ShowTaskEventMessage(DailyEventData data)
    {
        // MogoMsgBox.Instance.ShowFloatingTextQueue("你获得了" + data.gold + LanguageData.dataMap[263].content);
        // MogoMsgBox.Instance.ShowFloatingTextQueue("你获得了" + data.exp + LanguageData.dataMap[264].content);
    }

    protected void FormatDailyEventList()
    {
        dailyEventData.Clear();

        SortedDictionary<int, DailyEventMessageData> doneAndNotGotReward = new SortedDictionary<int, DailyEventMessageData>();
        SortedDictionary<int, DailyEventMessageData> doing = new SortedDictionary<int, DailyEventMessageData>();
        SortedDictionary<int, DailyEventMessageData> doneAddGotReward = new SortedDictionary<int, DailyEventMessageData>();

        foreach (var messageData in dailyEventSourceData)
        {
            if (DailyEventData.dataMap.ContainsKey(messageData.Key))
            {
                // int groupID = DailyEventData.dataMap[messageData.Key].group;
                if (messageData.Value.is_finish != 0 && messageData.Value.is_reward == 0)
                    doneAndNotGotReward.Add(messageData.Key, messageData.Value);
                else if (messageData.Value.is_finish == 0)
                    doing.Add(messageData.Key, messageData.Value);
                else if (messageData.Value.is_finish != 0 && messageData.Value.is_reward != 0)
                    doneAddGotReward.Add(messageData.Key, messageData.Value);
                else
                    LoggerHelper.Debug("Can a event has got reward without finished it?");
            }
            else
            {
                LoggerHelper.Debug("FormatDailyEventList id: " + messageData.Key + " Not Exist");
            }
        }

        if (doneAndNotGotReward.Count > 0)
        {
			DailyTaskSystemController.Singleton.ShowNotification();
        }
        else
        {
			DailyTaskSystemController.Singleton.HideNotification();
        }

        foreach (var messageData in doneAndNotGotReward)
            dailyEventData.Add(messageData);
        foreach (var messageData in doing)
            dailyEventData.Add(messageData);
        foreach (var messageData in doneAddGotReward)
            dailyEventData.Add(messageData);

        //foreach (var messageData in dailyEventData)
        //{
            //Mogo.Util.LoggerHelper.Debug("messageData: " + messageData.Key + " " + messageData.Value.cur_num + " " + messageData.Value.is_finish + " " + messageData.Value.is_reward);
        //}

        //if (DailyTaskManager.Instance != null && DailyTaskManager.Instance.isOnFocus())
        //    ShowDailyEventData();
		ShowDailyEventData();
    }

    public void ShowDailyEventData()
    {
        //foreach (var messageData in dailyEventData)
        //{
            // Debug.LogError("messageData: " + messageData.Key + " " + messageData.Value.cur_num + " " + messageData.Value.is_finish + " " + messageData.Value.is_reward);
        //}
		List<DailyTaskInfo> 	tasks=new List<DailyTaskInfo>();
        foreach (var messageData in dailyEventData)
        {
            if (DailyEventData.dataMap.ContainsKey(messageData.Key))
            {
                int code = 0;
                if (messageData.Value.is_finish == 0)
                    code = 1;
                else if (messageData.Value.is_finish > 0 && messageData.Value.is_reward > 0)
                    code = 2;
				tasks.Add(new DailyTaskInfo(LanguageData.dataMap.Get(DailyEventData.dataMap[messageData.Key].title).Format(DailyEventData.dataMap[messageData.Key].finish[1]), messageData.Key,
        			messageData.Value.exp, messageData.Value.gold, 
        			messageData.Value.cur_num, DailyEventData.dataMap[messageData.Key].finish[1],
        			IconData.dataMap.Get(DailyEventData.dataMap[messageData.Key].icon).path, code));
            }
        }
		DailyTaskSystemController.Singleton.UpdateView(tasks);
    }

    protected void HandleErrorCode(int errorCode)
    {
        LoggerHelper.Debug("HandleErrorCode: " + errorCode);
    }

    #endregion


    #region 数据获取

    public int GetDailyAllEventExp()
    {
        int sum = 0;
        if (dailyEventData.Count > 0)
        {
            foreach (var item in dailyEventData)
            {
                //if (item.Value.is_finish > 0 && item.Value.is_reward > 0)
                //    continue;

                sum += item.Value.exp;
            }
        }
        else
        {
            foreach (var item in DailyEventData.dataMap)
            {
                if (item.Value.level.Count != 2)
                {
                    LoggerHelper.Error("Are you kidding me?");
                    continue;
                }

                if (item.Value.level[0] <= 15 && item.Value.level[1] >= 15)
                    sum += item.Value.exp;
            }
        }
        return sum;
    }

    #endregion
}
