/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VictorCameraCfgData
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013-9-11
// 模块描述：胜利镜头特效配置表
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class VictorCameraCfgData : GameData<VictorCameraCfgData>
    {
        public const string SLOT = "cam_victory";
        public string path { get; protected set; }

        public string Path
        {
            get
            {
                return string.Concat(path, "/", SLOT);
            }
        }

        static public readonly string fileName = "xml/VictorCameraCfg";
        //static public Dictionary<int, VictorCameraCfgData> dataMap { get; set; }
    }
}