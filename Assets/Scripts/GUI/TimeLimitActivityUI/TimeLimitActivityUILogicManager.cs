using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using Mogo.GameData;
using System;
using Mogo.Game;

public class TimeLimitActivityUILogicManager
{
    protected List<KeyValuePair<int, EventMessageData>> sortResult = new List<KeyValuePair<int, EventMessageData>>();

    private static TimeLimitActivityUILogicManager m_instance;

    public static TimeLimitActivityUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new TimeLimitActivityUILogicManager();
            }

            return TimeLimitActivityUILogicManager.m_instance;

        }
    }

    public void Initialize()
    {

    }

    public void Release()
    {

    }

    public static class TimeLimitActivityUIEvent
    {
        public const string ActivityGridUp = "TimeLimitActivityUI_ActivityGridUp";
    }

    public void GetActivityReward(int activityID)
    {
        EventDispatcher.TriggerEvent(Events.OperationEvent.EventGetReward, activityID);
    }

    public void ShareToGetDiamond(int activityID)
    {
        EventDispatcher.TriggerEvent(Events.OperationEvent.EventShareToGetDiamond, activityID);
    }

    #region 设置限时活动列表和限时活动详细列表

    /// <summary>
    /// 设置限时活动列表和限时活动详细列表
    /// </summary>
    /// <param name="activityIDMessages"></param>
    public void SetAllActivityAndAcitivityInfoList(Dictionary<int, EventMessageData> activityIDMessages)
    {
        sortResult.Clear();
        sortResult = SortAllActivity(activityIDMessages);

        SetAllActivity();
        SetAllActivityInfo();
    }  

    /// <summary>
    /// 设置限时活动列表
    /// </summary>
    private void SetAllActivity()
    {
        TimeLimitActivityUIViewManager.Instance.ResetActivityGridList();    

        foreach (var activityIDMessage in sortResult)
        {
            if (activityIDMessage.Value.is_doing) // 活动已开启
            {
                int id = activityIDMessage.Key;
                if (EventData.dataMap.ContainsKey(id))
                {
                    LimitActivityGridData ad = new LimitActivityGridData();

                    ad.TitleText = LanguageData.dataMap.ContainsKey(EventData.dataMap[id].name) ? LanguageData.dataMap[EventData.dataMap[id].name].content : "";
                    ad.FGImgName = IconData.dataMap[EventData.dataMap[id].icon].path;

                    if (activityIDMessage.Value != null)
                    {
                        if (activityIDMessage.Value.is_reward > 0) // 已领取
                        {
                            ad.CDText = LanguageData.GetContent(7134);                            
                            ad.Status = ActivityStatus.HasReward;

                            TimeLimitActivityUIViewManager.Instance.SetGridCountDownStop(activityIDMessage.Key);
                        }
                        else if (activityIDMessage.Value.is_finish > 0) // 已完成
                        {
                            ad.CDText = LanguageData.GetContent(7133);
                            ad.Status = ActivityStatus.HasFinished;

                            TimeLimitActivityUIViewManager.Instance.SetGridCountDownStop(activityIDMessage.Key);                            
                        }
                        else if (EventData.dataMap[id].limit_time == 0) // 活动中-不限时间
                        {
                            ad.CDText = LanguageData.GetContent(7137);
                            ad.Status = ActivityStatus.OtherStatus;

                            TimeLimitActivityUIViewManager.Instance.SetGridCountDownStop(activityIDMessage.Key);
                        }
                        else
                        {
                            TimeSpan theInterval = MogoTime.Instance.CalculateTimeSpanDateTime(MogoTime.Instance.GetSecond() + EventData.dataMap[id].limit_time * 60);
                            theInterval = MogoTime.Instance.CalculateTimeSpanDateTime(activityIDMessage.Value.accept_time + EventData.dataMap[id].limit_time * 60);

                            ad.CDText = (int)theInterval.TotalDays + ":" + (int)theInterval.Hours + ":" + theInterval.Minutes + ":" + theInterval.Seconds;
                            ad.Status = ActivityStatus.OtherStatus;
                        }
                    }
                    else
                    {
                        TimeSpan theInterval = MogoTime.Instance.CalculateTimeSpanDateTime(activityIDMessage.Value.accept_time + EventData.dataMap[id].limit_time * 60);

                        ad.CDText = (int)theInterval.TotalDays + ":" + (int)theInterval.Hours + ":" + theInterval.Minutes + ":" + theInterval.Seconds;
                        ad.Status = ActivityStatus.OtherStatus;
                    }                    

                    TimeLimitActivityUIViewManager.Instance.AddActivityGrid(ad, id);
                }
            }
            else // 活动未开启
            {
                int idToDo = activityIDMessage.Key;
                if (EventData.dataMap.ContainsKey(idToDo))
                {
                    LimitActivityGridData ad = new LimitActivityGridData();
                    ad.TitleText = LanguageData.dataMap.ContainsKey(EventData.dataMap[idToDo].name) ? LanguageData.dataMap[EventData.dataMap[idToDo].name].content : "";
                    ad.CDText = LanguageData.GetContent(7132);
                    ad.Status = ActivityStatus.OtherStatus;
                    ad.FGImgName = IconData.dataMap[EventData.dataMap[idToDo].icon].path;
                    TimeLimitActivityUIViewManager.Instance.AddActivityGrid(ad, idToDo);
                }
            }
        }
    } 

    /// <summary>
    /// 设置限时活动详细信息列表
    /// </summary>
    /// <param name="id"></param>
    public void SetAllActivityInfo()
    {
        LoggerHelper.Debug("SetAllActivityInfo");

        List<LimitActivityInfoGridData> listActivityInfoData = new List<LimitActivityInfoGridData>();
        foreach (var activityIDMessage in sortResult)
        {
            int id = activityIDMessage.Key;
            if (!EventData.dataMap.ContainsKey(id))
                continue;

            LimitActivityInfoGridData aid = new LimitActivityInfoGridData();
            var data = activityIDMessage.Value;     

            int acceptTime = data.accept_time;
            int timeOutTime = data.accept_time + EventData.dataMap[id].limit_time * 60;

            DateTime timeOutDate = MogoTime.Instance.GetDateTimeBySecond(timeOutTime);
            TimeSpan theInterval = MogoTime.Instance.CalculateTimeSpanDateTime(timeOutTime);

            string endTime = theInterval.TotalHours >= 1000 ? "999:59:59" : (int)theInterval.TotalHours + ":" + theInterval.Minutes + ":" + theInterval.Seconds;

            string timeOutDateStr = GetTimeOutTimeStr(EventData.dataMap[id], timeOutDate);

            if (data.is_doing) // 已领取
            {
                if (data.is_reward > 0)
                {
                    aid.CDText = LanguageData.GetContent(7134);
                    aid.Status = ActivityStatus.HasReward;

                    TimeLimitActivityUIViewManager.Instance.SetGridCountDownStop(id);
                }
                else if (data.is_finish > 0) // 已完成
                {
                    aid.CDText = LanguageData.GetContent(7133);
                    aid.Status = ActivityStatus.HasFinished;

                    TimeLimitActivityUIViewManager.Instance.SetGridCountDownStop(id);
                }
                else if (EventData.dataMap[id].limit_time == 0) // 活动中-不限时间
                {
                    aid.CDText = LanguageData.GetContent(7137);
                    aid.Status = ActivityStatus.OtherStatus;

                    TimeLimitActivityUIViewManager.Instance.SetGridCountDownStop(id);
                }
                else
                {
                    aid.CDText = (int)theInterval.TotalDays + ":" + (int)theInterval.Hours + ":" + theInterval.Minutes + ":" + theInterval.Seconds;
                    aid.Status = ActivityStatus.OtherStatus;
                }
            }
            else
            {
                aid.CDText = LanguageData.GetContent(7132);
                aid.Status = ActivityStatus.OtherStatus;
            }

            aid.ID = id;
            aid.Title = LanguageData.dataMap.ContainsKey(EventData.dataMap[id].name) ? LanguageData.dataMap[EventData.dataMap[id].name].content : "";
            aid.Desc = LanguageData.dataMap.ContainsKey(EventData.dataMap[id].title) ? LanguageData.dataMap[EventData.dataMap[id].title].content : "";

            //if (data.is_doing)
            //{
                aid.Rule = LanguageData.dataMap.ContainsKey(EventData.dataMap[id].describtion) ? LanguageData.dataMap[EventData.dataMap[id].describtion].Format(timeOutDateStr) : "";
            //}
            //else
            //{
            //    if (EventData.dataMap.ContainsKey(id))
            //        aid.Rule = LanguageData.GetContent(7130) + GetBeginTimeStr(EventData.dataMap[id]) + LanguageData.GetContent(7131);
            //}

            aid.InfoImgName = IconData.dataMap[EventData.dataMap[id].icon].path;

            //if (progress != null)
            //    aid.Rule = EventData.dataMap[id].Format(progress.ToArray());
            //else
            //    aid.Rule = LanguageData.dataMap.ContainsKey(EventData.dataMap[id].rule) ? LanguageData.dataMap[EventData.dataMap[id].rule].content : "";

            if (EventData.dataMap.ContainsKey(id))
            {
                List<int> listID = new List<int>();

                Dictionary<int, int> items;
                switch (MogoWorld.thePlayer.vocation)
                {
                    case Vocation.Warrior:
                        items = EventData.dataMap[id].items1;
                        break;
                    case Vocation.Assassin:
                        items = EventData.dataMap[id].items2;
                        break;
                    case Vocation.Archer:
                        items = EventData.dataMap[id].items3;
                        break;
                    case Vocation.Mage:
                        items = EventData.dataMap[id].items4;
                        break;
                    default:
                        items = new Dictionary<int, int>();
                        break;
                }

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        if (ItemParentData.GetItem(item.Key) != null)
                        {
                            listID.Add(item.Key);
                        }
                        else
                            LoggerHelper.Debug("Item Not Exist");
                    }
                }

                aid.ItemListID = listID;
            }   
         
            listActivityInfoData.Add(aid);               
        }

        TimeLimitActivityUIViewManager.Instance.SetLimitActivityInfoListData(listActivityInfoData);     
    }

    /// <summary>
    /// 对所有的限时活动进行排序
    /// </summary>
    /// <param name="activityIDMessages"></param>
    /// <returns></returns>
    private List<KeyValuePair<int, EventMessageData>> SortAllActivity(Dictionary<int, EventMessageData> activityIDMessages)
    {
        List<KeyValuePair<int, EventMessageData>> result = new List<KeyValuePair<int, EventMessageData>>();

        List<KeyValuePair<int, EventMessageData>> canGet = new List<KeyValuePair<int, EventMessageData>>();
        List<KeyValuePair<int, EventMessageData>> doing = new List<KeyValuePair<int, EventMessageData>>();
        List<KeyValuePair<int, EventMessageData>> got = new List<KeyValuePair<int, EventMessageData>>();
        List<KeyValuePair<int, EventMessageData>> timeOut = new List<KeyValuePair<int, EventMessageData>>();
        List<KeyValuePair<int, EventMessageData>> notOpen = new List<KeyValuePair<int, EventMessageData>>();

        foreach (var activityIDMessage in activityIDMessages)
        {
            if (activityIDMessage.Value.is_doing)
            {
                if (activityIDMessage.Value.is_reward > 0)
                    got.Add(activityIDMessage);
                else if (activityIDMessage.Value.is_finish > 0)
                    canGet.Add(activityIDMessage);
                else
                    doing.Add(activityIDMessage);
            }
            else
            {
                notOpen.Add(activityIDMessage);
            }
        }

        foreach (var item in canGet)
            result.Add(item);

        foreach (var item in doing)
            result.Add(item);

        foreach (var item in notOpen)
            result.Add(item);

        foreach (var item in got)
            result.Add(item);

        return result;
    }

    #endregion

    public string GetBeginTimeStr(EventData eventData)
    {
        switch (eventData.type)
        {
            case 1:
                return "";
            case 2:
                return eventData.arg_1;
            case 3:
                return eventData.arg_1;
            case 4:
                return eventData.arg_1;
            case 5:
                return "";
            case 6:
                return "";
            case 7:
                return eventData.arg_3;
            case 8:
                return eventData.arg_5;
            case 9:
                return "";
            case 10:
                return eventData.arg_3;
            case 11:
                return eventData.arg_5;
            default:
                return "";
        }
    }

    public string GetTimeOutTimeStr(EventData eventData, DateTime timeOutDate)
    {
        switch (eventData.type)
        {
            case 1:
                return "";
            case 2:
                return eventData.arg_2;
            case 3:
                return eventData.arg_2;
            case 4:
                return eventData.arg_2;
            case 5:
                return FormatDateLongString(timeOutDate);
            case 6:
                return "";
            case 7:
                return eventData.arg_4;
            case 8:
                return eventData.arg_6;
            case 9:
                return "";
            case 10:
                return eventData.arg_4;
            case 11:
                return eventData.arg_6;
            default:
                return "";
        }
    }

    public string FormatDateString(string str)
    {
        string[] dateStr = str.Split();
        if (dateStr.Length < 2)
            return "";

        return int.Parse(dateStr[0]) + LanguageData.GetContent(7115) + int.Parse(dateStr[1]) + LanguageData.GetContent(7117);
    }

    public string FormatWeekDayString(string str)
    {
        string weekday = "";
        switch (int.Parse(str))
        {
            case 1:
                weekday = LanguageData.GetContent(7105);
                break;
            case 2:
                weekday = LanguageData.GetContent(7106);
                break;
            case 3:
                weekday = LanguageData.GetContent(7107);
                break;
            case 4:
                weekday = LanguageData.GetContent(7108);
                break;
            case 5:
                weekday = LanguageData.GetContent(7109);
                break;
            case 6:
                weekday = LanguageData.GetContent(7110);
                break;
            default:
                weekday = LanguageData.GetContent(7100);
                break;
        }

        return LanguageData.GetContent(7116) + weekday;
    }

    public string FormatDateLongString(DateTime date)
    {
        return date.Year + LanguageData.GetContent(7114) + date.Month + LanguageData.GetContent(7115) + date.Day + LanguageData.GetContent(7117) + date.Hour + LanguageData.GetContent(7101) + date.Minute + LanguageData.GetContent(7102) + date.Second + LanguageData.GetContent(7103);
    }
}
