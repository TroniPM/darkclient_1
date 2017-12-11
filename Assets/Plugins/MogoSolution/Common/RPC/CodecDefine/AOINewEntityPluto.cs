using System;
using System.Collections.Generic;
using Mogo.Util;

namespace Mogo.RPC
{
	class AOINewEntityPluto : Pluto
	{
        protected override void DoDecode(byte[] data, ref int unLen)
        {
            var info = new CellAttachedInfo();
            info.typeId = (ushort)VUInt16.Instance.Decode(data, ref unLen);
            info.id = (uint)VUInt32.Instance.Decode(data, ref unLen); // eid
            //info.face = (byte)VUInt8.Instance.Decode(data, ref unLen);// rotation
            UInt16 x = (UInt16)VUInt16.Instance.Decode(data, ref unLen);
            UInt16 y = (UInt16)VUInt16.Instance.Decode(data, ref unLen);
            info.x = (short)x;
            info.y = (short)y;
            var entity = DefParser.Instance.GetEntityByID(info.typeId);
            if (entity != null)
            {
                info.entity = entity;
                info.props = new List<KeyValuePair<EntityDefProperties, object>>();
                while (unLen < data.Length)
                {//还有数据就解析
                    var index = VUInt16.Instance.Decode(data, ref unLen);
                    EntityDefProperties prop;
                    var flag = entity.Properties.TryGetValue((ushort)index, out prop);
                    if (flag)
                        info.props.Add(new KeyValuePair<EntityDefProperties, object>(prop, prop.VType.Decode(data, ref unLen)));
                }
            }
            Arguments = new object[1] { info };
        }

        public override void HandleData()
        {
            var info = Arguments[0] as CellAttachedInfo;
            Mogo.Util.LoggerHelper.Debug("aoi new " + info.id);
            EventDispatcher.TriggerEvent<CellAttachedInfo>(Events.FrameWorkEvent.AOINewEntity, info);
        }

        internal static Pluto Create()
        {
            return new AOINewEntityPluto();
        }
	}
}
