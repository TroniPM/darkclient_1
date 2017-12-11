/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：GuideSystem
// 创建者：Charles Zuo
// 修改者列表：
// 创建日期：2013.7.24
// 模块描述：新手指引系统 
//----------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mogo.Game;
using Mogo.Util;
using Mogo.GameData;

public class GuideSystem : IEventManager
{
    private static GuideSystem m_instance;
    private Dictionary<int, int> _guideTimes;
    private List<int> m_guideQueue = new List<int>();
    public bool IsOpen { get; set; }
    private bool m_isGuideDialog=false;

    public bool IsGuideDialog
    {
        get { return m_isGuideDialog; }
        set { m_isGuideDialog = value; }
    }
    public Dictionary<int, int> guideTimes
    {
        get
        {
            if (SystemConfig.Instance.GuideTimes.ContainsKey(MogoWorld.thePlayer.dbid))
            {
                _guideTimes = Utils.ParseMapIntInt(SystemConfig.Instance.GuideTimes[MogoWorld.thePlayer.dbid], '!', '?');
            }
            else
            {
                _guideTimes = new Dictionary<int, int>();
            }
            return _guideTimes;
        }
    }

    public GuideSystem()
    {
        IsOpen = true;
    }
    public static GuideSystem Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new GuideSystem();
            }
            return m_instance;
        }
    }
    #region implement interface
    public void AddListeners()
    {
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, HandleLeaveInstance);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, HandleEnterInstance);
    }

    public void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, HandleLeaveInstance);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, HandleEnterInstance);
    }
    #endregion


    void HandleEnterInstance(int sceneID, bool isInstance)
    {
        //if (sceneID == MogoWorld.globalSetting.homeScene)
        //{

        //}
        GuideSystem.Instance.TriggerEvent<int>(GlobalEvents.EnterInstance, sceneID);
    }
    void HandleLeaveInstance(int sceneID, bool isInstance)
    {
        GuideSystem.Instance.TriggerEvent<int>(GlobalEvents.LeaveInstance, sceneID);
        if (isInstance)
        {
            var level = MogoWorld.thePlayer.level;
            if (level > 0)
            {
                GuideSystem.Instance.TriggerEvent<byte>(GlobalEvents.ChangeLevel, level);
            }
        }
    }
    /// <summary>
    /// 过滤器，只返回bool值，一项为false则短路
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    bool Filter(int id)
    {
        if (!IsOpen)
        {
            return false;
        }
        var guideLevel = GuideXMLData.dataMap.Get(id).guideLevel;
        var playerLevel = MogoWorld.thePlayer.level;
        if (guideLevel != null && (playerLevel < guideLevel[0] || playerLevel > guideLevel[1]))
        {
            return false;
        }
        var restriction = GuideXMLData.dataMap.Get(id).restriction;
        if (restriction != null)
        {
            foreach (var item in restriction)
            {
                var temp = CalculateDamage.GetProperty(MogoWorld.thePlayer, item.Key);
                if (temp!=double.Parse(item.Value))
                {
                    return false;
                }
            }
        }
        if (!MogoWorld.inCity)
        {
            return false;
        }
        if (guideTimes.ContainsKey(id) && guideTimes[id] >= GuideXMLData.dataMap.Get(id).guideTimes)
        {
            return false;
        }
        //MogoMsgBox.Instance.ShowFloatingText(String.Format("Guide {0} Activated", id));
        return true;
    }

    public void enqueueGuide(int id,bool isTest = false)
    {
        if (Filter(id) || isTest)
        {
            if (m_guideQueue.Count > 0)
            {
                if (!m_guideQueue.Contains(id))
                {
                    m_guideQueue.Add(id);
                }
            }
            else
            {
                m_guideQueue.Add(id);
                execute(id);
            }
        }
    }
    public void dequeueGuide()
    {
        m_guideQueue.RemoveAt(0);
#if UNITY_IPHONE
		Dictionary<int,int> order=new Dictionary<int,int>();
		foreach(var v in m_guideQueue)
		{
			order.Add(GuideXMLData.dataMap.Get(v).priority,v);
		}
		order=order.SortByKey();
		m_guideQueue=order.Values.ToList();
#else
        m_guideQueue = m_guideQueue.OrderBy(t => GuideXMLData.dataMap.Get(t).priority).Distinct().ToList();
#endif
		if (m_guideQueue.Count > 0)
        {
            execute(m_guideQueue[0]);
        }
    }
    private void execute(int id)
    {
        StoryManager.Instance.AddCommand("StartGuideUI");
        StoryManager.Instance.AddCommand("Wait 8");
        foreach (var index in GuideXMLData.Links[GuideXMLData.dataMap.Get(id).group])
        {
            ConvertToCommand(index);
        }
        StoryManager.Instance.AddCommand("UnlockQueue");
        StoryManager.Instance.AddCommand("End");
        StoryManager.Instance.PrintCommandQueue();
        StoryManager.Instance.SetCallBack(
            () =>
            {
                //MogoMsgBox.Instance.ShowFloatingText("Guide Complete" + id);
                LoggerHelper.Warning("Guide Complete" + id);
                GuideSystem.Instance.IsGuideDialog = false;
                SaveGuide(id);
                dequeueGuide();
            }
            );
        StoryManager.Instance.Execute();
    }
    public void SaveGuide(int id)
    {
        if (guideTimes.ContainsKey(id))
        {
            _guideTimes[id]++;
        }
        else
        {
            _guideTimes[id] = 0;
            _guideTimes[id]++;
        }
        SystemConfig.Instance.GuideTimes[MogoWorld.thePlayer.dbid] = Utils.PackMap(_guideTimes, '!', '?');
        SystemConfig.SaveConfig();
    }
    public void DelCharacterGuideConfig(ulong dbid)
    {
        if (SystemConfig.Instance.GuideTimes.ContainsKey(dbid))
        {
            SystemConfig.Instance.GuideTimes.Remove(dbid);
            SystemConfig.SaveConfig();
        }
    }

    public void TriggerEvent(GlobalEvents eventType)
    {
        int eventID = (int)eventType;
        foreach (var index in GuideXMLData.GetIDByEvent(eventID))
        {
            enqueueGuide(index);
        }
    }

    public void TriggerEvent<T>(GlobalEvents eventType, T arg1)
    {
        int eventID = (int)eventType;
        List<int> result = new List<int>();
        foreach (var index in GuideXMLData.GetIDByEvent(eventID))
        {
            var arg1_str_list = GuideXMLData.dataMap[index].event_arg1.Split(new char[] { ',' });
            foreach (var arg1_str in arg1_str_list)
            {
                int len = arg1_str.Length;
                var arg1_opr = arg1_str.Substring(len - 1, 1);
                T arg1_value = (T)Utils.GetValue(arg1_str.Substring(0, len - 1), typeof(T));
                switch (arg1_opr)
                {
                    case "+":
                        if (Comparer.Default.Compare(arg1, arg1_value) > 0)
                        {
                            result.Add(index);
                        }
                        break;
                    case "-":
                        if (Comparer.Default.Compare(arg1, arg1_value) < 0)
                        {
                            result.Add(index);
                        }
                        break;
                    case "=":
                        if (Comparer.Default.Compare(arg1, arg1_value) == 0)
                        {
                            result.Add(index);
                        }
                        break;
                    default:
                        break;
                }
            }
            
        }
        foreach (var index in result.Distinct())
        {
            enqueueGuide(index);
        }

    }

    public void TriggerEvent<T, U>(GlobalEvents eventType, T arg1, U arg2)
    {
        //TODO
    }

    void ConvertToCommand(int guideID)
    {
        if (GuideXMLData.dataMap.Get(guideID).openDialog != null)
        {
            LoggerHelper.Debug("openDialog");
            string command = "ShowDialog " + GuideXMLData.dataMap.Get(guideID).openDialog;
            StoryManager.Instance.AddCommand(command);
        }
        if (GuideXMLData.dataMap.Get(guideID).openGUI != 0)
        {
            LoggerHelper.Debug("openGUI");
        }

        switch (GuideXMLData.dataMap.Get(guideID).target)
        {
            case 1:
                {
                    string command = "SetNonFocus " + GuideXMLData.dataMap.Get(guideID).target_arg1 + " " + GuideXMLData.dataMap.Get(guideID).text
                        + " " + GuideXMLData.dataMap.Get(guideID).target_arg2;
                    StoryManager.Instance.AddCommand(command);
                    StoryManager.Instance.AddCommand("Wait 2");
                    break;
                }
            case 2:
                {
                    string command = "SetFocus " + GuideXMLData.dataMap.Get(guideID).target_arg1 + " " + GuideXMLData.dataMap.Get(guideID).text
                        + " " + GuideXMLData.dataMap.Get(guideID).target_arg2;
                    StoryManager.Instance.AddCommand(command);
                    StoryManager.Instance.AddCommand("Wait 2");
                    break;
                }
            case 3:
                {
                    StoryManager.Instance.AddCommand("Wait " + GuideXMLData.dataMap.Get(guideID).target);
                    break;
                }
            case 4:
                {
                    string command = "SetItemFocus " + GuideXMLData.dataMap.Get(guideID).target_arg1 + " " + GuideXMLData.dataMap.Get(guideID).text
                            + " " + GuideXMLData.dataMap.Get(guideID).target_arg2;
                    StoryManager.Instance.AddCommand(command);
                    StoryManager.Instance.AddCommand("Wait 2");
                    break;
                }
            case 5:
                {
                    string command = "AddPointer " + GuideXMLData.dataMap.Get(guideID).target_arg1 + " " + GuideXMLData.dataMap.Get(guideID).target_arg2;
                    StoryManager.Instance.AddCommand(command);
                    StoryManager.Instance.AddCommand("Wait " + GuideXMLData.dataMap.Get(guideID).target);
                    break;
                }
            case 6:
                {
                    StoryManager.Instance.AddCommand("Wait " + GuideXMLData.dataMap.Get(guideID).target);
                    break;
                }
            default:
                break;
        }

    }
}
