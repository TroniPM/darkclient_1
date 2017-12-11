﻿#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EventException
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.17
// 模块描述：事件处理异常。
//----------------------------------------------------------------*/
#endregion
using System;

namespace Mogo.Util
{

    /// <summary>
    /// 事件处理异常。
    /// </summary>
    [Serializable]
    public class EventException : Exception
    {
        /// <summary>
        /// 使用指定的错误消息初始化 EventException 类的新实例。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public EventException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 EventException 类的新实例。
        /// </summary>
        /// <param name="message">解释异常原因的错误消息。</param>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用。</param>
        public EventException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}