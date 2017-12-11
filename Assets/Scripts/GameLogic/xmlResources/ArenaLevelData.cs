using System;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class ArenaLevelData : GameData
#else
    public class ArenaLevelData : GameData<ArenaLevelData>
#endif
	{
        public int arenicGrade { get; protected set; }
        public int credit { get; protected set; }
        public int title { get; protected set; }
        public int icon { get; protected set; }
        public int propEffect { get; protected set; }
        static public readonly string fileName = "xml/ArenaLevel";
#if UNITY_IPHONE
        static public Dictionary<int, ArenaLevelData> dataMap { get; set; }
#endif
        static public int GetNextLevelCreditNeed(int level,int credit)
        {
            var rst = dataMap.First(x => x.Value.arenicGrade == (level+1));
            if (rst.Value != null)
            {
                return rst.Value.credit;
            }
            else
            {
                return int.MaxValue;
            }
        }
        static public string GetCurTitle(int level)
        {
            var rst = dataMap.First(x=>x.Value.arenicGrade==level);
            if (rst.Value!=null)
            {
                return LanguageData.dataMap.Get(rst.Value.title).content;
            } 
            else
            {
                return String.Empty;
            }
            
        }
        static public string GetNextTitle(int level)
        {
            var rst = dataMap.First(x => x.Value.arenicGrade == (level+1));
            if (rst.Value != null)
            {
                return LanguageData.dataMap.Get(rst.Value.title).content;
            }
            else
            {
                return String.Empty;
            }

        }
        static public string GetCurTitleIcon(int level)
        {
            var rst = dataMap.First(x => x.Value.arenicGrade == level);
            if (rst.Value != null)
            {
                return IconData.dataMap.Get(rst.Value.icon).path;
            }
            else
            {
                return String.Empty;
            }

        }
        static public string GetNextTitleIcon(int level)
        {
            var rst = dataMap.First(x => x.Value.arenicGrade == (level + 1));
            if (rst.Value != null)
            {
                return IconData.dataMap.Get(rst.Value.icon).path;
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// PVPÇ¿¶È
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        static public int GetCurPVPAddition(int level)
        {
            int num = 0;
            var rst = dataMap.First(x => x.Value.arenicGrade == level);
            if (rst.Value != null)
            {
                int propEffect = rst.Value.propEffect;
                num = PropertyEffectData.GetPVPAddition(propEffect);
            }

            return num;
        }

        static public int GetNextPVPAddition(int level)
        {
            int num = 0;
            var rst = dataMap.First(x => x.Value.arenicGrade == (level + 1));
            if (rst.Value != null)
            {
                int propEffect = rst.Value.propEffect;
                num = PropertyEffectData.GetPVPAddition(propEffect);
            }

            return num;
        }

        /// <summary>
        /// PVP¿¹ÐÔ
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        static public int GetCurPVPAnti(int level)
        {
            int num = 0;
            var rst = dataMap.First(x => x.Value.arenicGrade == level);
            if (rst.Value != null)
            {
                int propEffect = rst.Value.propEffect;
                num = PropertyEffectData.GetPVPAnti(propEffect);
            }

            return num;
        }

        static public int GetNextPVPAnti(int level)
        {
            int num = 0;
            var rst = dataMap.First(x => x.Value.arenicGrade == (level + 1));
            if (rst.Value != null)
            {
                int propEffect = rst.Value.propEffect;
                num = PropertyEffectData.GetPVPAnti(propEffect);
            }

            return num;
        }
    }
}

