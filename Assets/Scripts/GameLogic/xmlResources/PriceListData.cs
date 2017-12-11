using System;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class PriceListData : GameData<PriceListData>
	{
        public int type { get; protected set; }
        public int currencyType { get; protected set; }
        public Dictionary<int, int> priceList { get; protected set; }

        static public readonly string fileName = "xml/PriceList";
        //static public Dictionary<int, PriceListData> dataMap { get; set; }

        static public int GetPrice(int id,int times)
        {
            var data = dataMap.Get(id);
            if (data.type == 1)
            {
                return data.priceList[times+1];
            }
            else if(data.type == 2)
            {
                return data.priceList[1];
            }
            return 0;
        }

        public string GetPriceUnitName()
        {
            if (currencyType == 1)
            {
                return ItemParentData.GetItem(2).Name;
            }
            else
            {
                return ItemParentData.GetItem(3).Name;
            }
        }
	}
}
