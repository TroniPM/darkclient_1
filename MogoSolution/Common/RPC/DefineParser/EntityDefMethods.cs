#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityDefMethod
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：实体方法声明。
//----------------------------------------------------------------*/
#endregion
using System.Collections.Generic;

namespace Mogo.RPC
{
    /// <summary>
    /// 实体方法声明。
    /// </summary>
    public class EntityDefMethod
    {
        private ushort m_funcID;
        private string m_funcName;
        private List<VObject> m_argsType;

        public ushort FuncID
        {
            get { return m_funcID; }
            set { m_funcID = value; }
        }

        public string FuncName
        {
            get { return m_funcName; }
            set { m_funcName = value; }
        }

        public List<VObject> ArgsType
        {
            get { return m_argsType; }
            set { m_argsType = value; }
        }

        public override string ToString()
        {
            return FuncName;
        }
    }
}