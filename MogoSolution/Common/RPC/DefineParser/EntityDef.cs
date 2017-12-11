#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityDef
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：实体类声明。
//----------------------------------------------------------------*/
#endregion
using System.Collections.Generic;

namespace Mogo.RPC
{
    /// <summary>
    /// 实体类声明。
    /// </summary>
    public class EntityDef
    {
        private ushort m_id;
        private string m_name;
        private string m_parentName;
        private EntityDef m_parent;
        private string m_strUniqueIndex;
        private bool m_bHasCellClient;  //是否拥有client可见的cell属性
        private Dictionary<ushort, EntityDefProperties> m_properties;
        private List<EntityDefProperties> m_propertiesList;
        private Dictionary<string, EntityDefMethod> m_clientMethodsByName;
        private Dictionary<string, EntityDefMethod> m_baseMethodsByName;
        private Dictionary<string, EntityDefMethod> m_cellMethodsByName;
        private Dictionary<ushort, EntityDefMethod> m_clientMethodsByID;
        private Dictionary<ushort, EntityDefMethod> m_baseMethodsByID;
        private Dictionary<ushort, EntityDefMethod> m_cellMethodsByID;

        public ushort ID
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public string ParentName
        {
            get { return m_parentName; }
            set { m_parentName = value; }
        }

        public EntityDef Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public string StrUniqueIndex
        {
            get { return m_strUniqueIndex; }
            set { m_strUniqueIndex = value; }
        }

        public bool BHasCellClient
        {
            get { return m_bHasCellClient; }
            set { m_bHasCellClient = value; }
        }

        public Dictionary<ushort, EntityDefProperties> Properties
        {
            get { return m_properties; }
            set { m_properties = value; }
        }

        public List<EntityDefProperties> PropertiesList
        {
            get { return m_propertiesList; }
            set { m_propertiesList = value; }
        }

        /// <summary>
        /// 服务端回调的方法
        /// </summary>
        public Dictionary<string, EntityDefMethod> ClientMethodsByName
        {
            get { return m_clientMethodsByName; }
            set { m_clientMethodsByName = value; }
        }

        /// <summary>
        /// 客户端调用服务端的方法
        /// </summary>
        public Dictionary<string, EntityDefMethod> BaseMethodsByName
        {
            get { return m_baseMethodsByName; }
            set { m_baseMethodsByName = value; }
        }

        /// <summary>
        /// 客户端调用服务端的方法
        /// </summary>
        public Dictionary<string, EntityDefMethod> CellMethodsByName
        {
            get { return m_cellMethodsByName; }
            set { m_cellMethodsByName = value; }
        }

        public Dictionary<ushort, EntityDefMethod> ClientMethodsByID
        {
            get { return m_clientMethodsByID; }
            set { m_clientMethodsByID = value; }
        }

        public Dictionary<ushort, EntityDefMethod> BaseMethodsByID
        {
            get { return m_baseMethodsByID; }
            set { m_baseMethodsByID = value; }
        }

        public Dictionary<ushort, EntityDefMethod> CellMethodsByID
        {
            get { return m_cellMethodsByID; }
            set { m_cellMethodsByID = value; }
        }

        public EntityDefMethod TryGetBaseMethod(string name)
        {
            return TryGetBaseMethod(name, this);
        }

        public EntityDefMethod TryGetBaseMethod(string name, EntityDef entity)
        {
            EntityDefMethod method = null;
            if (entity != null)
            {
                entity.BaseMethodsByName.TryGetValue(name, out method);
                if (method == null)
                    return TryGetBaseMethod(name, entity.Parent);
            }
            return method;
        }

        public EntityDefMethod TryGetClientMethod(string name)
        {
            return TryGetClientMethod(name, this);
        }

        public EntityDefMethod TryGetClientMethod(string name, EntityDef entity)
        {
            EntityDefMethod method = null;
            if (entity != null)
            {
                entity.ClientMethodsByName.TryGetValue(name, out method);
                if (method == null)
                    return TryGetClientMethod(name, entity.Parent);
            }
            return method;
        }

        public EntityDefMethod TryGetBaseMethod(ushort id)
        {
            return TryGetBaseMethod(id, this);
        }

        public EntityDefMethod TryGetBaseMethod(ushort id, EntityDef entity)
        {
            EntityDefMethod method = null;
            if (entity != null)
            {
                entity.BaseMethodsByID.TryGetValue(id, out method);
                if (method == null)
                    return TryGetBaseMethod(id, entity.Parent);
            }
            return method;
        }

        public EntityDefMethod TryGetClientMethod(ushort id)
        {
            return TryGetClientMethod(id, this);
        }

        public EntityDefMethod TryGetClientMethod(ushort id, EntityDef entity)
        {
            EntityDefMethod method = null;
            if (entity != null)
            {
                entity.ClientMethodsByID.TryGetValue(id, out method);
                if (method == null)
                    return TryGetClientMethod(id, entity.Parent);
            }
            return method;
        }

        public EntityDefMethod TryGetCellMethod(ushort id)
        {
            return TryGetCellMethod(id, this);
        }

        public EntityDefMethod TryGetCellMethod(ushort id, EntityDef entity)
        {
            EntityDefMethod method = null;
            if (entity != null)
            {
                entity.CellMethodsByID.TryGetValue(id, out method);
                if (method == null)
                {
                    return TryGetCellMethod(id, entity.Parent);
                }
            }
            return method;
        }

        public EntityDefMethod TryGetCellMethod(string name)
        {
            return TryGetCellMethod(name, this);
        }

        public EntityDefMethod TryGetCellMethod(string name, EntityDef entity)
        {
            EntityDefMethod method = null;
            if (entity != null)
            {
                entity.CellMethodsByName.TryGetValue(name, out method);
                if (method == null)
                {
                    return TryGetCellMethod(name, entity.Parent);
                }
            }
            return method;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}