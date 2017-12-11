/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：LanguageData
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-2-7
// 模块描述：本地化语言信息数据模块
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using System;

namespace Mogo.GameData
{
    public class LanguageData : GameData<LanguageData>
    {
        // Monster 数据
        public string content { get; set; }
        static public readonly string fileName = "xml/ChineseData";
        //static public Dictionary<int, LanguageData> dataMap { get; set; }

        public LanguageData()
        {
            content = string.Empty;
        }

        

        public string Format(params object[] args)
        {
            return string.Format(content, args);
        }
        public static string MONEY { get { return dataMap.Get(20002).content; }}
        public static string EXP { get { return dataMap.Get(20003).content; } }
        public static string DIAMOND { get { return dataMap.Get(20004).content; } }
        public static string GetContent(int id)
        {
        	  if (id == 10700)	//loveomg error pass
        			id = 1;

            if (dataMap.ContainsKey(id))
            {
                return dataMap.Get(id).content;
            }
            else
            {
                LoggerHelper.Error(String.Format("Language key {0:0} is not exist ", id));
                return "***";
            }
        }

        public static string GetContent(int id, params object[] args)
        {
            if (dataMap.ContainsKey(id))
            {
                return dataMap.Get(id).Format(args);
            }
            else
            {
                LoggerHelper.Error(String.Format("Language key {0:0} is not exist ", id));
                return "***";
            }
        }

        // 通过物品ID获得物品名称
        //static public string GetNameByItemID(int itemID)
        //{
        //    if (ItemParentData.GetItem(itemID) !=null)
        //    {
        //        return ItemParentData.GetItem(itemID).Name;
        //    }
           
        //    else
        //    {
        //        LoggerHelper.Debug(String.Format("cannot find itemID {0} in all item xml", itemID));
        //        return String.Empty;
        //    }
        //}        

        static public string GetPVPLevelName(int PVPLevel)
        {
            return dataMap.Get(3000 + PVPLevel).content;
        }
    }
}
