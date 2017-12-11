#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityDefProperties
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：实体属性声明。
//----------------------------------------------------------------*/
#endregion

namespace Mogo.RPC
{
    /// <summary>
    /// 实体属性声明。
    /// </summary>
    public class EntityDefProperties
    {
        private string m_name;
        private VObject m_vType;//int8,string,list of ...
        private bool m_bSaveDb;//是否存盘
        private string m_defaultValue;//缺省值

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public VObject VType
        {
            get { return m_vType; }
            set { m_vType = value; }
        }

        public bool BSaveDb
        {
            get { return m_bSaveDb; }
            set { m_bSaveDb = value; }
        }

        public string DefaultValue
        {
            get { return m_defaultValue; }
            set { m_defaultValue = value; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}