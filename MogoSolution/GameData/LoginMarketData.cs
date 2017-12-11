using System;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class LoginMarketData : GameData<LoginMarketData>
    {
        public int version { get; protected set; }
        public int itemId { get; protected set; }
        public int priceType { get; protected set; }
        public int price { get; protected set; }

        public static readonly string fileName = "xml/LoginMarketData"; //从网上下载在MarketManager中下载插值
        //public static Dictionary<int, LoginMarketData> dataMap { get; set; }
        //public static Dictionary<int, LoginMarketData> dataMap = new Dictionary<int, LoginMarketData>();

    }
}