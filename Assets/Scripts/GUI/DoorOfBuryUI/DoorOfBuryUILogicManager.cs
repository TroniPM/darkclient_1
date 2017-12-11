using UnityEngine;
using System.Collections;
using Mogo.Util;

public class DoorOfBuryUILogicManager 
{
    private static DoorOfBuryUILogicManager m_instance;

    public static DoorOfBuryUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new DoorOfBuryUILogicManager();
            }

            return DoorOfBuryUILogicManager.m_instance;

        }
    }

    private int m_iChooseId = 0;

    void OnFriendGridUp(int id)
    {
        m_iChooseId = id;
        EventDispatcher.TriggerEvent<int>(DoorOfBurySystem.ON_SELECT,id);
    }

    void OnEnterDoorOfBury()
    {
       
    }
        
    public void Initialize()
    {
        DoorOfBuryUIViewManager.Instance.FRIENDGRIDUP += OnFriendGridUp;
        EventDispatcher.AddEventListener(DoorOfBuryUIEvent.ENTERDOOROFBURY, OnEnterDoorOfBury);
    }

    public void Release()
    {
        DoorOfBuryUIViewManager.Instance.FRIENDGRIDUP -= OnFriendGridUp;
        EventDispatcher.RemoveEventListener(DoorOfBuryUIEvent.ENTERDOOROFBURY, OnEnterDoorOfBury);
    }

    public static class DoorOfBuryUIEvent
    {
        public const string FRIENDGRIDUP = "DoorOfBuryUIEvent_FriendGridUp";
        public const string ENTERDOOROFBURY = "DoorOfBuryUIEvent_EnterDoorOfBury";
        public const string FRIENDGRIDCOUNTDOWNEND = "DoorOfBuryUIEvent_FriendGridCountDownEnd";

        public static string ENTERDOOROFBURYBYTIP = "DoorOfBuryUIEvent.ENTERDOOROFBURYBYTIP";
    }
}
