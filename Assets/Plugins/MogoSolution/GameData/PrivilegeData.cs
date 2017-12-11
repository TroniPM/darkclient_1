/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：PrivilegeData
// 创建者：Charles Zuo
// 修改者列表：
// 创建日期： 2013/5/29
// 模块描述： VIP 特权描述
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class PrivilegeData : GameData<PrivilegeData>
    {
        public List<uint> accumulatedAmount { get; protected set; }
        public int jewelSynthesisMaxLevel { get; protected set; }
        public int dailyGoldMetallurgyLimit { get; protected set; }
        public int dailyRuneWishLimit { get; protected set; }
        public int dailyEnergyBuyLimit { get; protected set; }
        public int dailyExtraChallengeLimit { get; protected set; }
        public int dailyHardModResetLimit { get; protected set; }
        public int dailyRaidSweepLimit { get; protected set; }
        public int dailyTowerSweepLimit { get; protected set; }
        public Dictionary<int, int> hpBottles { get; protected set; }
        public int hpMaxCount { get; protected set; }
        public List<int> iconList { get; set; }
        public List<int> stringList { get; set; }
        static public readonly string fileName = "xml/privilege";
        //static public Dictionary<int, PrivilegeData> dataMap { get; set; }
    }
}
