using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Mogo.Util;
using Mogo.GameData;

namespace Mogo.GameLogic.LocalServer
{
    public enum SpaceType
    {
        SpawnPoint
    }

    public class LocalServerResManager
    {
        public static readonly string spaceFilePath = "spaces/";

        public static Dictionary<int, MissionData> MissionDataMap { get; set; }
        public static Dictionary<int, MissionEventData> MissionEventDataMap { get; set; }
        public static Dictionary<int, SpaceData> SpaceDataMap { get; set; }
        public static Dictionary<int, SpawnPointLevelData> SpawnPointLevelDataMap { get; set; }
        public static Dictionary<int, DropData> DropDataMap { get; set; }
        public static Dictionary<int, MapData> MapDataMap { get; set; }


        public static void Initialize()
        {
            MissionDataMap = MissionData.dataMap;
            MissionEventDataMap = MissionEventData.dataMap;
            SpaceDataMap = SpaceData.dataMap;
            SpawnPointLevelDataMap = SpawnPointLevelData.dataMap;
            DropDataMap = DropData.dataMap;
            MapDataMap = MapData.dataMap;
        }

        public static int GetSceneId(int mission, int difficulty)
        {
            foreach (var item in MissionDataMap)
            {
                if (item.Value.mission == mission && item.Value.difficulty == difficulty)
                {
                    return item.Value.scene;
                }
            }
            return -1;
        }

        public static List<int> GetEnterXY(int sceneId)
        {
            MapData m = null;
            List<int> rst = new List<int>();
            if (MapDataMap.TryGetValue(sceneId, out m))
            {
                rst.Add(m.enterX);
                rst.Add(m.enterY);
            }
            return rst;
        }

        public static void Release()
        {
            MissionDataMap = null;
            MissionEventDataMap = null;
            SpaceDataMap = null;
            SpawnPointLevelDataMap = null;
            DropDataMap = null;
        }

        public static void ResetSpaceData(MissionData missionData)
        {
            SpaceDataMap.Clear();
            int spaceID = missionData.scene;
            string spaceFileName = string.Concat("s", spaceID);
            ReadSpaceFile(spaceFileName);
        }

        protected static void ReadSpaceFile(string name)
        {
            var path = string.Concat(SystemConfig.CONFIG_SUB_FOLDER, spaceFilePath, name, SystemConfig.CONFIG_FILE_EXTENSION);
            var xml = XMLParser.Load(path);
            if (xml != null && xml.Children != null && xml.Children.Count != 0)
                SetSpaceData(xml.Children[0] as System.Security.SecurityElement);
            else
                LoggerHelper.Error("Load space error: " + path);
            //using (FileStream fs = new FileStream(Path.Combine(spaceFilePath, name), FileMode.Open))
            //{
            //    if (fs != null)
            //    {
            //        XmlDocument xmldoc = new XmlDocument();
            //        xmldoc.Load(fs);
            //        XmlNode entities = xmldoc.SelectSingleNode("root/entities");
            //        string text = entities.OuterXml;
            //        if (text != null)
            //            SetSpaceData(text);
            //    }
            //}
        }

        protected static void SetSpaceData(System.Security.SecurityElement xml)
        {
            //var xml = XMLParser.LoadXML(text);
            var map = XMLParser.LoadIntMap(xml, String.Empty);
            var type = typeof(SpaceData);
            var props = type.GetProperties();
            foreach (var item in map)
            {
                var t = new SpaceData();
                foreach (var prop in props)
                {
                    if (prop.Name == "id")
                    {
                        prop.SetValue(t, item.Key, null);
                    }
                    else
                    {
                        if (item.Value.ContainsKey(prop.Name))
                        {
                            var value = Utils.GetValue(item.Value[prop.Name], prop.PropertyType);
                            prop.SetValue(t, value, null);
                        }
                    }
                }
                if (!SpaceDataMap.ContainsKey(item.Key))
                {
                    SpaceDataMap.Add(item.Key, t);
                }
            }
        }

        public static MonsterData GetMonsterData(int monsterId)
        {
            return MonsterData.GetData(monsterId);
        }
    }
}
