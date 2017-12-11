using System;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.Game;

namespace Mogo.RPC
{
    class OtherAttriSyncPluto : Pluto
    {
        protected override void DoDecode(byte[] data, ref int unLen)
        {
            var info = new AttachedInfo();
            info.id = (uint)VUInt32.Instance.Decode(data, ref unLen); //entity unique id
            info.props = new List<KeyValuePair<EntityDefProperties, object>>();
            var entityParent = MogoWorld.Entities.GetValueOrDefault(info.id, null);
            if (entityParent != null)
            {
                var entity = entityParent.entity;
                while (unLen < data.Length)
                {//还有数据就解析
                    var index = VUInt16.Instance.Decode(data, ref unLen);
                    EntityDefProperties prop;
                    var flag = entity.Properties.TryGetValue((ushort)index, out prop);
                    if (flag)
                        info.props.Add(new KeyValuePair<EntityDefProperties, object>(prop, prop.VType.Decode(data, ref unLen)));
                }
            }
            //LoggerHelper.Debug("other attr sync : " + info.props.Count);
            Arguments = new object[2] { entityParent, info };
        }

        public override void HandleData()
        {
            var entity = Arguments[0] as EntityParent;
            var info = Arguments[1] as AttachedInfo;
            if (entity != null)
                entity.SynEntityAttrs(info);
        }

        /// <summary>
        /// 创建新EntityAttachedPluto实例。
        /// </summary>
        /// <returns>EntityAttachedPluto实例</returns>
        internal static Pluto Create()
        {
            return new OtherAttriSyncPluto();
        }
    }
}
