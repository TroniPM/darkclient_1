#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：IEventManager
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.8.2
// 模块描述：子系统事件接口
//----------------------------------------------------------------*/
#endregion

namespace Mogo.Game
{
    /// <summary>
    /// 子系统事件接口
    /// </summary>
    public interface IEventManager
    {
        /// <summary>
        /// 添加事件订阅。
        /// </summary>
        void AddListeners();
        /// <summary>
        /// 移除事件订阅。
        /// </summary>
        void RemoveListeners();
    }
}
