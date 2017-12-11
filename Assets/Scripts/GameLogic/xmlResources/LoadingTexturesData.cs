/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：LoadingTexturesData
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013-9-22
// 模块描述：加载背景图
//----------------------------------------------------------------*/

using Mogo.Util;
using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class LoadingTexturesData : GameData<LoadingTexturesData>
    {
        public string path { get; protected set; }

        static public readonly string fileName = "xml/loadingTextures";
        //static public Dictionary<int, LoadingTexturesData> dataMap { get; set; }

        public static string GetTexture(int textureId)
        {
            if (textureId == 0)
            {
                textureId = RandomHelper.GetRandomInt(0, dataMap.Count) + 1;
            }
            if (dataMap.ContainsKey(textureId))
            {
                LoggerHelper.Debug("LoadingTexturesImg:" + dataMap.Get(textureId).path);
                return dataMap.Get(textureId).path;
            }
            else
            {
                LoggerHelper.Error("can not find LoadingTexturesData:" + textureId);
            }
            return string.Empty;
        }
    }
}