using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mogo.GameData
{
    public class MissionData : GameData<MissionData>
    {
        public int mission { get; protected set; }
        public int difficulty { get; protected set; }

        public int scene { get; protected set; }
        public int level { get; protected set; }
        public int passTime { get; protected set; }

        public Dictionary<int, int> preMissions { get; protected set; }
        public List<int> postMissions { get; protected set; }
        public List<int> target { get; protected set; }
        public int cg { get; protected set; }
        public List<int> task { get; protected set; }
        public int dayTimes { get; protected set; }
        public int resetMoney { get; protected set; }
        public int recommend_level { get; protected set; }

        public List<int> events { get; protected set; }

        public int energy { get; protected set; }
        public int reviveTimes { get; protected set; }
        public List<int> preloadCG { get; protected set; }

        public Dictionary<int, int> drop { get; protected set; }
        public int minimumFight { get; protected set; }
        public int passEffect { get; protected set; }
        static public readonly string fileName = "xml/Mission";
        //static public Dictionary<int, MissionData> dataMap { get; set; }
        static public int GetCGByMission(int mapID)
        {
            var result = dataMap.FirstOrDefault(t => t.Value.mission == mapID);
            if (result.Value != null && result.Value.cg != 0)
            {
                return result.Value.cg;
            }
            else
            {
                return -1;
            }
        }

        static public int GetReviveTimesByMission(int mission, int level)
        {
            var result = dataMap.FirstOrDefault(t => t.Value.mission == mission && t.Value.difficulty == level);
            if (result.Value != null)
            {
                return result.Value.reviveTimes;
            }
            else
            {
                return 0;
            }
        }

        static public int GetSceneId(int mission, int difficulty)
        {
            foreach (var item in dataMap)
            {
                if (item.Value.mission == mission && item.Value.difficulty == difficulty)
                {
                    return item.Value.scene;
                }
            }
            return -1;
        }

        static public MissionData GetMissionData(int mission, int difficulty)
        {
            foreach (var item in dataMap)
            {
                if (item.Value.mission == mission && item.Value.difficulty == difficulty)
                {
                    return item.Value;
                }
            }
            return null;
        }
    }
}