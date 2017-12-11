using System;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class MarketData
    {
        public int id { get; protected set; }
        public int marketVersion { get; protected set; }
        public int hot { get; protected set; }
        public int hotSort { get; protected set; }
        public int jewel { get; protected set; }
        public int jewelSort { get; protected set; }
        public int item { get; protected set; }
        public int itemSort { get; protected set; }
        public int wing { get; protected set; }
        public int wingSort { get; protected set; }
        public int mode { get; protected set; }
        public int label { get; protected set; }
        public int itemId { get; protected set; }
        public int itemNumber { get; protected set; }
        public int priceOrg { get; protected set; }
        public int priceNow { get; protected set; }
        public int vipLevel { get; protected set; }
        public int totalCount { get; protected set; }
        public int[] startTime { get; protected set; }
        public int duration { get; protected set; }

        //public static readonly string fileName = "xml/MarketData"; //从网上下载在MarketManager中下载插值
        public static Dictionary<int, MarketData> dataMap = new Dictionary<int, MarketData>();

        public static void InsertMarketData(int id, int version, LuaTable data)
        {
            if (dataMap.ContainsKey(id))
            {
                dataMap.Remove(id);
            }
            MarketData d = new MarketData();
            d.id = id;
            d.marketVersion = version;
            d.hot = Int32.Parse((string)data["1"]);
            d.hotSort = Int32.Parse((string)data["2"]);
            d.jewel = Int32.Parse((string)data["3"]);
            d.jewelSort = Int32.Parse((string)data["4"]);
            d.item = Int32.Parse((string)data["5"]);
            d.itemSort = Int32.Parse((string)data["6"]);
            d.wing = Int32.Parse((string)data["7"]);
            d.wingSort = Int32.Parse((string)data["8"]);
            d.mode = Int32.Parse((string)data["9"]);
            d.label = Int32.Parse((string)data["10"]);
            d.itemId = Int32.Parse((string)data["11"]);
            d.itemNumber = Int32.Parse((string)data["12"]);
            d.priceOrg = Int32.Parse((string)data["13"]);
            d.priceNow = Int32.Parse((string)data["14"]);
            d.vipLevel = Int32.Parse((string)data["15"]);
            d.totalCount = Int32.Parse((string)data["16"]);
            d.startTime = null;
            d.duration = Int32.Parse((string)data["18"]);
            dataMap.Add(id, d);
        }

    }
}
