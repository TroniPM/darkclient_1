using System;
using System.Collections.Generic;
using Mogo.GameData;
using Mogo.Util;
using UnityEngine;

public class MissionPathPointData
{
    public int id { get; protected set; }
    public int preID { get; protected set; }
    public int isEnable { get; protected set; }
    public float range { get; protected set; }

    public List<float> pathPoint { get; protected set; }
    public List<float> pathPointRotation { get; protected set; }
    public List<float> pathPointSize { get; protected set; }

    public List<float> movePosition { get; protected set; }

    public List<int> deleteList { get; protected set; }

    public int isPointer { get; protected set; }
    public int isNormalType { get; protected set; }

    public static Dictionary<int, MissionPathPointData> dataMap = new Dictionary<int, MissionPathPointData>();

    public static string IMPORT_FILE_PATH = "MissionPathPoint/";
    public static string IMPORT_FILE_EXTENSION = "_PathPoint";

    public static MissionPathPointData[] missionPathPoint = null;

    public static Dictionary<int, GameObject> dataGameObjects = new Dictionary<int, GameObject>();

    public static bool LoadMissionPathPointMessage(int missionID)
    {
        ClearData();
        return ReloadData(missionID);
    }

    public static void ClearData()
    {
        dataMap.Clear();
        dataGameObjects.Clear();
        missionPathPoint = null;
    }

    public static void SetType(int key, int value)
    {
        if (missionPathPoint == null || key >= missionPathPoint.Length || key < 0)
        {
            return;
        }
        //LoggerHelper.Error("SetType: " + (key+1) +  " value:" + value);
        missionPathPoint[key].isEnable = value;
    }

    public static bool ReloadData(int missionID)
    {
        var xml = XMLParser.Load(string.Concat(SystemConfig.CONFIG_SUB_FOLDER, IMPORT_FILE_PATH, missionID, IMPORT_FILE_EXTENSION, SystemConfig.CONFIG_FILE_EXTENSION));

        if (xml != null)
        {
            var map = XMLParser.LoadIntMap(xml, missionID + IMPORT_FILE_EXTENSION);

            missionPathPoint = new MissionPathPointData[map.Count];

            List<MissionPathPointData> missionPathPointUnsorted = new List<MissionPathPointData>();

            foreach (var node in map)
            {
                MissionPathPointData temp = new MissionPathPointData();

                temp.id = Convert.ToInt32(node.Key);

                if (node.Value.ContainsKey("preID"))
                    temp.preID = Convert.ToInt32(node.Value["preID"]);
                else
                    temp.preID = 0;

                if (node.Value.ContainsKey("isEnable"))
                    temp.isEnable = Convert.ToInt32(node.Value["isEnable"]);
                else
                    temp.isEnable = 0;

                if (node.Value.ContainsKey("type"))
                    temp.isNormalType = Convert.ToInt32(node.Value["type"]);
                else
                    temp.isNormalType = 0;

                if (node.Value.ContainsKey("isPointer"))
                    temp.isPointer = Convert.ToInt32(node.Value["isPointer"]);
                else
                    temp.isPointer = 0;

                if (node.Value.ContainsKey("range"))
                    temp.range = Convert.ToSingle(node.Value["range"]);
                else
                    temp.range = 2;

                temp.pathPoint = Utils.ParseListAny<float>(node.Value["position"]);

                temp.pathPointRotation = Utils.ParseListAny<float>(node.Value["rotation"]);
                temp.pathPointSize = Utils.ParseListAny<float>(node.Value["size"]);

                if (node.Value.ContainsKey("movePosition"))
                {
                    temp.movePosition = Utils.ParseListAny<float>(node.Value["movePosition"]);
                }
                else
                {
                    temp.movePosition = new List<float>();
                    temp.movePosition.Add(0);
                    temp.movePosition.Add(0);
                    temp.movePosition.Add(0);
                }

                if (node.Value.ContainsKey("deleteList"))
                    temp.deleteList = Utils.ParseListAny<int>(node.Value["deleteList"]);
                else
                    temp.deleteList = new List<int>();

                //LoggerHelper.Error("   " + temp.pathPoint[0] + "   " + temp.pathPoint[1] + "  " + temp.pathPoint[2]);
                dataMap.Add(node.Key, temp);

                missionPathPointUnsorted.Add(temp);
                missionPathPoint[node.Key - 1] = temp;
            }

            missionPathPointUnsorted.Sort(delegate(MissionPathPointData p1, MissionPathPointData p2)
            {
                if (p1.id < p2.id)
                    return 1;
                else
                    return -1;
            });

            return true;
        }
        return false;
    }

    public static void CreateAllMissionPathPointObject()
    {
        if (dataMap == null)
            return;

        foreach (var data in dataMap)
            CreateMissionPathPointObject(data);
    }

    protected static void CreateMissionPathPointObject(KeyValuePair<int, MissionPathPointData> data)
    {
        CreateMissionPathPointObject(data.Key, data.Value);
    }

    public static void CreateMissionPathPointObject(int id, MissionPathPointData data)
    {
        GameObject go = new GameObject();
        // go.name = "MissionPathPoint_" + id;

        go.transform.position = new Vector3(data.pathPoint[0], data.pathPoint[1], data.pathPoint[2]);
        go.transform.Rotate(data.pathPointRotation[0], data.pathPointRotation[1], data.pathPointRotation[2]);

        var collider = go.AddComponent<BoxCollider>();
        collider.size = new Vector3(data.pathPointSize[0], data.pathPointSize[1], data.pathPointSize[2]);
        collider.isTrigger = true;

        var script = go.AddComponent<MissionPathPointTriggerGear>();
        script.id = id;

        dataGameObjects.Add(id, go);
    }
}


