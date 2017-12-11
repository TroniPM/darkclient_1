using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class AvatarLevelData : GameData<AvatarLevelData>
    {
        public int maxEnergy { get; protected set; }
        public int nextLevelExp { get; protected set; }
        public int expStandard { get; protected set; }
        public int goldStandard { get; protected set; }

        static public readonly string fileName = "xml/AvatarLevel";
        //static public Dictionary<int, AvatarLevelData> dataMap { get; set; }

        static public int GetExpStandard(int level)
        {
            if (dataMap.ContainsKey(level))
            {
                return dataMap[level].expStandard;
            }

            return 0;
        }

        static public int GetGoldStandard(int level)
        {
            if (dataMap.ContainsKey(level))
            {
                return dataMap[level].goldStandard;
            }

            return 0;
        }        
    }
}