#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SystemConfig
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.3.19
// 模块描述：系统参数配置。
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;

namespace Mogo.Util
{
    /// <summary>
    /// 系统参数配置。
    /// </summary>
    public partial class SystemConfig
    {
        //true
#if !UNITY_IPHONE
        public const string PLATFORM_SSJJ = "4399";
        public const string PLATFORM_DJ = "91";
        public const string PLATFORM_PPTV = "pptv";
        public const string PLATFORM_360 = "360";
        public const string PLATFORM_DOWNJOY = "dangle";
        public const string PLATFORM_UC = "uc";
        public const string PLATFORM_MI = "mi";
        public const string PLATFORM_PPS = "pps";
        public const string PLATFORM_ANZHI = "anzhi";
        public const string PLATFORM_DK = "DK";
        /// <summary>
        /// key是平台缩写，value是平台包名及平台文件夹名
        /// </summary>
        public static Dictionary<string, string> PlatformDic = new Dictionary<string, string>()
        {
            {"com.ahzs.sy4399",PLATFORM_SSJJ},
            {"com.ahzs.dj",PLATFORM_DJ},
            {"com.ahzs.pptv",PLATFORM_PPTV},
            {"com.ahzs.downjoy",PLATFORM_DOWNJOY},
            {"com.ahzs.uc",PLATFORM_UC},
            {"com.ahzs.qihoo360",PLATFORM_360},
            {"com.ahzs.mi",PLATFORM_MI},
            {"com.ahzs.pps",PLATFORM_PPS},
            {"com.ahzs.anzhi",PLATFORM_ANZHI},
            {"com.ahzs.jiuyi",PLATFORM_DJ},
            {"com.ahzs.DK",PLATFORM_DK}
        };
#endif
        //false
    }
}