/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：IconData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-4-23
// 模块描述：
//----------------------------------------------------------------*/

using Mogo.Util;
using Mogo.Game;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Mogo.GameData
{
    public class IconData : GameData<IconData>
    {
        private const int PORTRAIT = 309;
        public string path { get; protected set; }
        public int color { get; protected set; }
        static public readonly string fileName = "xml/Icon";

        static public string none { get { return "emptyItem"; } }
        static public string JewelSlotSelectedIcon { get { return "jn_down"; } }
        static public string blankBox { get { return dataMap.Get(0).path; } }
        static public string locked { get { return dataMap.Get(3).path; } }
        //static public Dictionary<int, IconData> dataMap { get; set; }

        public IconData()
        {
            path = string.Empty;
        }

        static public string GetIconByQuality(int quality)
        {
            int index = 4;
            return dataMap[quality + index].path;
            //switch (quality)
            //{
            //    case 1: return "bb_bai";
            //    case 2: return "bb_lv";
            //    case 3: return "bb_lan";
            //    case 4: return "bb_zi";
            //    case 5: return "bb_cheng";
            //    case 6: return "bb_anjin";
            //    default: return none;
            //}
        }

        static public string GetJewelSlotIconUpByType(int type)
        {
            int index = 30001;
            return dataMap.Get((type - 1) * 2 + index).path;
        }

        static public string GetJewelSlotIconDownByType(int type)
        {
            int index = 30002;
            return dataMap.Get((type - 1) * 2 + index).path;
        }

        static public string GetPortraitByVocation(Vocation voc)
        {
            return (PORTRAIT + (byte)voc).ToString();
        }
        static public string GetHeadImgByVocation(int vocation)
        {
            return dataMap.Get((14000 + vocation)).path;
        }

        static public string GetIconPath(int id)
        {
            if (dataMap.ContainsKey(id))
                return dataMap[id].path;
            return null;
        }
    }
}
