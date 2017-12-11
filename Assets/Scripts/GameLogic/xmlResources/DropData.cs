using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Util;

namespace Mogo.GameData
{
    public class DropData : GameData<DropData>
    {
        public Dictionary<int, int> dropGroup0 { get; set; }
        public Dictionary<int, int> dropGroup1 { get; set; }
        public Dictionary<int, int> dropGroup2 { get; set; }
        public Dictionary<int, int> dropGroup3 { get; set; }

        public static readonly string fileName = "xml/Drop";
        //public static Dictionary<int, DropData> dataMap { get; set; }

        public static void getAwards(Dictionary<int, int> dstAwards, int groupId, int vocation)
        {
            if (!DropData.dataMap.ContainsKey(groupId))
                return;

            DropData dropCfgData = DropData.dataMap[groupId];

            Dictionary<int, int> tblDataCopy = new Dictionary<int,int>();
            if (vocation == 1)
                tblDataCopy = dropCfgData.dropGroup0;
            else if (vocation == 2)
                tblDataCopy = dropCfgData.dropGroup1;
            else if (vocation == 3)
                tblDataCopy = dropCfgData.dropGroup2;
            else if (vocation == 4)
                tblDataCopy = dropCfgData.dropGroup3;

            int weightSum = 0;
            foreach (KeyValuePair<int, int> subTblDataCopy in tblDataCopy)
            {
                weightSum = weightSum + subTblDataCopy.Value;
            }

            if (weightSum <= 0)
                return;

            int ran = RandomHelper.GetRandomInt(1, weightSum);

            foreach (KeyValuePair<int, int> subTblDataCopy in tblDataCopy)
            {
                ran = ran - subTblDataCopy.Value;
                if (ran <= 0)
                {
                    if (!dstAwards.ContainsKey(subTblDataCopy.Key))
                        dstAwards[subTblDataCopy.Key] = 0;
                    dstAwards[subTblDataCopy.Key] = dstAwards[subTblDataCopy.Key] + 1;
                }
            }
        }
    }
}
