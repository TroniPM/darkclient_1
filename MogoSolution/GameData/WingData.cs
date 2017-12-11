/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SpellData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-2-6
// 模块描述：翅膀数据结构
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class WingData : GameData<WingData>
	{
        public int name { get; protected set; }
        public int descrip { get; protected set; }
        public string modes { get; protected set; }
        public int icon { get; protected set; }
        public int type { get; protected set; }
        public int subtype { get; protected set; }
        public Dictionary<int, int> limit { get; protected set; }
        public Dictionary<int, int> unlock { get; protected set; }
        public Dictionary<int, int> unlockCost { get; protected set; }
        public int unlockDescrip { get; protected set; }
        public Dictionary<int, int> activeCost { get; protected set; }
        public int activeDescrip { get; protected set; }
        public int price { get; protected set; }
        
        static public readonly string fileName = "xml/Wing";

        public WingLevelData GetLevelData(int level)
        {
            return WingLevelData.GetLevelData(id, level);
        }
	}
}
