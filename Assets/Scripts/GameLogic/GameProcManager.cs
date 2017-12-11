using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogo.Util;
using Mogo.GameData;

public class GameProcManager
{
    public static void ChangeScene(int lastSceneId, int newSceneId, bool isBegin)
    {
        SetGameProgress(ProcType.ChangeScene, lastSceneId, newSceneId, isBegin);
    }

    public static void TriggerSpawnPoint(int mission, int difficulty, int spawnPointID)
    {
        SetGameProgress(ProcType.TriggerSpawnPoint, mission, difficulty, spawnPointID);
    }

    public static void ClientTeleport(int mission, int difficulty, int gearID)
    {
        SetGameProgress(ProcType.ClientTeleport, mission, difficulty, gearID);
    }

    public static void NotifyToClientEvent(int mission, int difficulty, int eventID)
    {
        SetGameProgress(ProcType.NotifyToClientEvent, mission, difficulty, eventID);
    }

    public static void ShowLoginReward()
    {
        SetGameProgress(ProcType.ShowLoginReward, "ShowLoginReward");
    }

    public static void GetLoginReward()
    {
        SetGameProgress(ProcType.GetLoginReward, "GetLoginReward");
    }

    public static void HandInTask(int taskID)
    {
        SetGameProgress(ProcType.HandInTask, taskID);
    }

    public static void OpenInstanceUI(int mission, int difficulty)
    {
        SetGameProgress(ProcType.OpenInstanceUI, mission, difficulty);
    }

    public static void BattleWin(int mission, int difficulty)
    {
        SetGameProgress(ProcType.BattleWin, mission, difficulty);
    }

    public static void GuideUI(string gameObjectName, int missionID)
    {
        var search = UIMapData.dataMap.FirstOrDefault(x=>x.Value.control.Equals(gameObjectName));
        if (search.Value != null)
        {
            SetGameProgress(ProcType.GuideUI, search.Key, missionID);
        }
        
    }

    public static void SetGameProgress(ProcType procType, params Object[] args)
    {
        if (!GameProcData.ProcData.ContainsKey(procType))
            return;
        var targetParas = args.PackArray().ToLower();
        //LoggerHelper.Error("targetParas: " + targetParas);
        if (GameProcData.ProcData[procType].ContainsKey(targetParas))
        {
            var progress = GameProcData.ProcData[procType][targetParas].Progress;
            MogoWorld.SetGameProgress(progress);
        }
    }


}
