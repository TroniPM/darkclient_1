using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.GameData;
using Mogo.Game;

public class NPCManager
{
    public static Dictionary<uint, EntityNPC> npcEntities = new Dictionary<uint, EntityNPC>();
    public static Dictionary<uint, Vector3> npcEntitiePosition = new Dictionary<uint, Vector3>();
    protected static int curSceneID;

    public static void Init()
    {
        LoggerHelper.Debug("InitNPCManager");
    }

    static NPCManager()
    {
        LoggerHelper.Debug("ConstructNPCManager");
        AddListeners();
    }

    ~NPCManager()
    {
        RemoveListeners();
    }

    public static void AddListeners()
    {
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, UpdateNPC);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.BeforeInstanceLoaded, ReleaseNPC);
        EventDispatcher.AddEventListener<int, int>(Events.TaskEvent.NPCSetSign, SetNPCSign);
    }

    public static void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, UpdateNPC);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.BeforeInstanceLoaded, ReleaseNPC);
        EventDispatcher.RemoveEventListener<int, int>(Events.TaskEvent.NPCSetSign, SetNPCSign);
    }

    public static void ReleaseNPC(int sceneID, bool isInstance)
    {
        if (npcEntities.Count > 0)
        {
            foreach (var npc in npcEntities)
                npc.Value.OnLeaveWorld();
        }
        npcEntities.Clear();
    }

    public static void UpdateNPC(int sceneID, bool isInstance)
    {
        curSceneID = sceneID;

        if (!MapData.dataMap.ContainsKey(curSceneID) || MapData.dataMap[curSceneID].npcList == null || MapData.dataMap[curSceneID].npcList.Count == 0)
            return;

        for (int i = 0; i < MapData.dataMap[curSceneID].npcList.Count; i++)
        {
            int index = i;
            int id = MapData.dataMap[curSceneID].npcList[index];

            if (!NPCData.dataMap.ContainsKey(id))
                continue;

            EntityNPC entityNPC = new EntityNPC();
            entityNPC.ID = (uint)id;
            entityNPC.name = LanguageData.dataMap[NPCData.dataMap[id].name].content;

            entityNPC.position.x = (float)NPCData.dataMap[id].mapx / 100;
            entityNPC.position.z = (float)NPCData.dataMap[id].mapy / 100;

            entityNPC.rotation = new Vector3(NPCData.dataMap[id].rotation[0] % 360, NPCData.dataMap[id].rotation[1] % 360, NPCData.dataMap[id].rotation[2] % 360);

            entityNPC.standbyAction = NPCData.dataMap[id].standbyAction;
            entityNPC.actionList = NPCData.dataMap[id].actionList;
            entityNPC.thinkInterval = NPCData.dataMap[id].thinkInterval;
            entityNPC.idleTimeRange = NPCData.dataMap[id].idleTimeRange;

            entityNPC.CreateModel();

            npcEntities.Add((uint)id, entityNPC);
        }
    }

    public static EntityNPC GetNPC(int theID)
    {
        uint id = (uint)theID;
        return GetNPC(id);
    }

    public static EntityNPC GetNPC(uint id)
    {
        if (npcEntities.ContainsKey(id))
            return npcEntities[id];
        else
            return null;
    }

    public static Vector3 GetNPCPosition(int theID)
    {
        uint id = (uint)theID;
        return GetNPCPosition(id);
    }

    public static Vector3 GetNPCPosition(uint id)
    {
        if (npcEntitiePosition.ContainsKey(id))
            return npcEntitiePosition[id];
        else
            return Vector3.zero;
    }

    public static void SetNPCSign(int curTaskNPCID, int nextTaskNPCID)
    {
        foreach (var npcData in npcEntities)
            npcData.Value.SetNPCSign(EntityNPC.NPCSignState.None);

        if (npcEntities.ContainsKey((uint)curTaskNPCID))
            GetNPC(curTaskNPCID).SetNPCSign(EntityNPC.NPCSignState.Doing);

        if (npcEntities.ContainsKey((uint)nextTaskNPCID))
            GetNPC(nextTaskNPCID).SetNPCSign(EntityNPC.NPCSignState.Done);
    }
}
