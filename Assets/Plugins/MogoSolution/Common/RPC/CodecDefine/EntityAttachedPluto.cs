#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityAttachedPluto
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.4.17
// 模块描述：实体获取编解码处理类。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using Mogo.Util;

namespace Mogo.RPC
{
    public class EntityAttachedPluto : Pluto
    {
        /// <summary>
        /// 将远程调用的方法解码为EntityAttachedPluto调用。
        /// </summary>
        /// <param name="data">远程调用方法的二进制数组</param>
        /// <param name="unLen">数据偏移量</param>
        protected override void DoDecode(byte[] data, ref int unLen)
        {

            //ushort klen = (ushort)VUInt16.Instance.Decode(data, ref unLen);
            string key = (string)VString.Instance.Decode(data, ref unLen);
            EventDispatcher.TriggerEvent<string>(Events.FrameWorkEvent.ReConnectKey, key);
            var info = new BaseAttachedInfo();
            info.typeId = (ushort)VUInt16.Instance.Decode(data, ref unLen); //entity type id
            info.id = (uint)VUInt32.Instance.Decode(data, ref unLen);//entity unique id
            info.dbid = (ulong)VUInt64.Instance.Decode(data, ref unLen); //dbid
            var entity = DefParser.Instance.GetEntityByID(info.typeId);
            if (entity != null)
            {
                info.entity = entity;
                info.props = new List<KeyValuePair<EntityDefProperties, object>>();
                while (unLen < data.Length)
                {//还有数据就解析
                    var index = (ushort)VUInt16.Instance.Decode(data, ref unLen);
                    EntityDefProperties prop;
                    var flag = entity.Properties.TryGetValue(index, out prop);
                    if (flag)
                        info.props.Add(new KeyValuePair<EntityDefProperties, object>(prop, prop.VType.Decode(data, ref unLen)));
                }
            }
            Arguments = new object[1] { info };
        }

        public override void HandleData()
        {
            var info = Arguments[0] as BaseAttachedInfo;
            CurrentEntity = info.entity;
            EventDispatcher.TriggerEvent<BaseAttachedInfo>(Events.FrameWorkEvent.EntityAttached, info);
        }

        /// <summary>
        /// 创建新EntityAttachedPluto实例。
        /// </summary>
        /// <returns>EntityAttachedPluto实例</returns>
        internal static Pluto Create()
        {
            return new EntityAttachedPluto();
        }
    }
}