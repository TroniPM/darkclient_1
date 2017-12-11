using System;
using System.Collections.Generic;
using UnityEngine;

using Mogo.Util;

namespace Mogo.GameData
{
    public class ClientEventData : GameData<ClientEventData>
    {
        public Dictionary<int, int> enable { get; protected set; }
        public Dictionary<int, int> disable { get; protected set; }
        public Dictionary<int, int> stateOne { get; protected set; }
        public Dictionary<int, int> stateTwo { get; protected set; }

        public static readonly string fileName = "xml/ClientEvent";
        //public static Dictionary<int, ClientEventData> dataMap { get; set; }

        public static void TriggerGearEvent(int eventID)
        {
            if (!ClientEventData.dataMap.ContainsKey(eventID))
                return;

            if (ClientEventData.dataMap[eventID].enable != null && ClientEventData.dataMap[eventID].enable.Count > 0)
            {
                foreach (var enableStatus in ClientEventData.dataMap[eventID].enable)
                {
                    TimerHeap.AddTimer((uint)enableStatus.Value, 0, SetGearControlEnable, (uint)enableStatus.Key);
                }
            }
            if (ClientEventData.dataMap[eventID].disable != null && ClientEventData.dataMap[eventID].disable.Count > 0)
            {
                foreach (var enableStatus in ClientEventData.dataMap[eventID].disable)
                {
                    TimerHeap.AddTimer((uint)enableStatus.Value, 0, SetGearControlDisable, (uint)enableStatus.Key);
                }
            }
            if (ClientEventData.dataMap[eventID].stateOne != null && ClientEventData.dataMap[eventID].stateOne.Count > 0)
            {
                foreach (var enableStatus in ClientEventData.dataMap[eventID].stateOne)
                {
                    TimerHeap.AddTimer((uint)enableStatus.Value, 0, SetGearControlStateOne, (uint)enableStatus.Key);
                }
            }
            if (ClientEventData.dataMap[eventID].stateTwo != null && ClientEventData.dataMap[eventID].stateTwo.Count > 0)
            {
                foreach (var enableStatus in ClientEventData.dataMap[eventID].stateTwo)
                {
                    TimerHeap.AddTimer((uint)enableStatus.Value, 0, SetGearControlStateTwo, (uint)enableStatus.Key);
                }
            }
        }

        protected static void SetGearControlEnable(uint gearID)
        {
            EventDispatcher.TriggerEvent<uint>(Events.GearEvent.SetGearEventEnable, gearID);
        }

        protected static void SetGearControlDisable(uint gearID)
        {
            EventDispatcher.TriggerEvent<uint>(Events.GearEvent.SetGearEventDisable, gearID);
        }

        protected static void SetGearControlStateOne(uint gearID)
        {
            EventDispatcher.TriggerEvent<uint>(Events.GearEvent.SetGearEventStateOne, gearID);
        }

        protected static void SetGearControlStateTwo(uint gearID)
        {
            EventDispatcher.TriggerEvent<uint>(Events.GearEvent.SetGearEventStateTwo, gearID);
        }
    }
}
