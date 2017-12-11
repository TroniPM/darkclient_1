using System;
using System.Collections.Generic;
using UnityEngine;


namespace Mogo.GameData
{
    public class MapData : GameData<MapData>
    {
        public MapType type { get; protected set; }
        public short enterX { get; protected set; }
        public short enterY { get; protected set; }
        public String lightmap { get; protected set; }
        public String lightProbes { get; protected set; }
        public Dictionary<String, bool> modelName { get; protected set; }
        public Color cameraColor { get; protected set; }
        public float cameraFar { get; protected set; }
        public bool fog { get; protected set; }
        public Color fogColor { get; protected set; }
        public FogMode fogMode { get; protected set; }
        public float linearFogStart { get; protected set; }
        public float linearFogEnd { get; protected set; }
        public Color ambientLight { get; protected set; }
        public Dictionary<int, Vector3> monstors { get; protected set; }
        public List<int> layerList { get; protected set; }
        public List<float> distanceList { get; protected set; }
        public String sceneName { get; protected set; }
        public LightType characherLight { get; protected set; }

        public List<int> npcList { get; protected set; }
        public int trapID { get; protected set; }
        public List<int> backgroundMusic { get; protected set; }
        public static readonly string fileName = "xml/map_setting";
        //public static Dictionary<int, MapData> dataMap { get; set; }



        public MapData()
        {
            sceneName = "InstanceScene";
            enterX = -200;
            enterY = -200;
        }

        public static bool IsSceneShowDeadTip(ushort sceneId)
        {
            if (dataMap == null)
                return false;

            if (!dataMap.ContainsKey(sceneId))
                return false;

            if (dataMap[sceneId].type == MapType.Special
                || dataMap[sceneId].type == MapType.ARENA)
                return true;

            return false;
        }
    }

    public enum LightType : byte
    {
        /// <summary>
        /// 不加光
        /// </summary>
        None = 0,
        /// <summary>
        /// 普通光
        /// </summary>
        Normal = 1
    }

    public enum MapType : byte
    {
        /// <summary>
        /// 普通地图
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 副本地图
        /// </summary>
        Special = 1,
        /// <summary>
        /// 试炼之塔
        /// </summary>
        ClimbTower = 2,
        /// <summary>
        /// 多人非组队
        /// </summary>
        MULTIPLAYER = 3,
        /// <summary>
        /// 世界boss
        /// </summary>
        WORLDBOSS = 4,
        /// <summary>
        /// 湮灭之门
        /// </summary>
        BURY = 5,
        /// <summary>
        /// 竞技场地图
        /// </summary>
        ARENA = 6,
        /// <summary>
        /// 塔防地图
        /// </summary>
        TOWERDEFENCE = 8,

        /// <summary>
        /// 袭击地图
        /// </summary>
        ASSAULT = 9,

        /// <summary>
        /// PVP地图
        /// </summary>
        OCCUPY_TOWER = 11,

        /// <summary>
        /// 迷雾深渊地图
        /// </summary>
        FOGGYABYSS = 12,
    }
}