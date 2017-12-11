using System;
using System.Collections.Generic;
using Mogo.Util;

namespace Mogo.RPC
{
    /// <summary>
    /// 一组entity数据
    /// </summary>
    class AOIEntitiesPluto : Pluto
    {
        protected override void DoDecode(byte[] data, ref int unLen)
        {
            /*[UINT16][UINT16][UINT32][UINT8][INT16][INT16] [UINT16]  [UINT16][UINT32][UINT8][INT16][INT16] [....]  [UINT16]  [UINT16][UINT32][UINT8][INT16][INT16] [....] ...
               总长度   etype   eid     face    x      y    单个aoi长度 etype    eid     face   x       y     属性    单个aoi长度 etype    eid     face   x       y     属性
            */
            var list = new List<CellAttachedInfo>();
            var info = new CellAttachedInfo();
            //VUInt16.Instance.Decode(data, ref unLen); //总长度
            info.typeId = (ushort)VUInt16.Instance.Decode(data, ref unLen); //etype
            info.id = (uint)VUInt32.Instance.Decode(data, ref unLen); // eid
            //info.face = (byte)VUInt8.Instance.Decode(data, ref unLen);// rotation
            UInt16 x = (UInt16)VUInt16.Instance.Decode(data, ref unLen);
            UInt16 y = (UInt16)VUInt16.Instance.Decode(data, ref unLen);
            info.x = (short)x;
            info.y = (short)y;
            //list.Add(info);//暂时忽略自身数据
            while (unLen < data.Length)
            {
                LoggerHelper.Debug("aois");
                var entityInfo = new CellAttachedInfo();
                var entityLength = (ushort)VUInt16.Instance.Decode(data, ref unLen); //单个entity数据总长度
                var endIdx = unLen + entityLength; //结束位置
                entityInfo.typeId = (ushort)VUInt16.Instance.Decode(data, ref unLen);
                entityInfo.id = (uint)VUInt32.Instance.Decode(data, ref unLen); // eid
                //entityInfo.face = (byte)VUInt8.Instance.Decode(data, ref unLen);// rotation
                UInt16 entityX = (UInt16)VUInt16.Instance.Decode(data, ref unLen);
                UInt16 entityY = (UInt16)VUInt16.Instance.Decode(data, ref unLen);
                entityInfo.x = (short)entityX;
                entityInfo.y = (short)entityY;
                var entity = DefParser.Instance.GetEntityByID(entityInfo.typeId);
                if (entity != null)
                {
                    entityInfo.entity = entity;
                    entityInfo.props = new List<KeyValuePair<EntityDefProperties, object>>();
                    while (unLen < endIdx)
                    {//还有数据就解析
                        var index = VUInt16.Instance.Decode(data, ref unLen);
                        EntityDefProperties prop;
                        var flag = entity.Properties.TryGetValue((ushort)index, out prop);
                        if (flag)
                            entityInfo.props.Add(new KeyValuePair<EntityDefProperties, object>(prop, prop.VType.Decode(data, ref unLen)));
                    }
                }
                list.Add(entityInfo);
            }
            Arguments = new Object[1];
            Arguments[0] = list;
        }

        public override void HandleData()
        {
            if (Arguments == null || Arguments.Length == 0)
            {
                LoggerHelper.Debug("aoi entities is empty");
                return;
            }
            LoggerHelper.Debug("aoi entities");
            var list = (List < CellAttachedInfo >)Arguments[0];
            foreach (var info in list)
            {
                Mogo.Util.LoggerHelper.Debug("aoi new " + info.id);
                EventDispatcher.TriggerEvent<CellAttachedInfo>(Events.FrameWorkEvent.AOINewEntity, info);
            }
        }

        /// <summary>
        /// 创建新EntityAttachedPluto实例。
        /// </summary>
        /// <returns>EntityAttachedPluto实例</returns>
        internal static Pluto Create()
        {
            return new AOIEntitiesPluto();
        }
    }
}
