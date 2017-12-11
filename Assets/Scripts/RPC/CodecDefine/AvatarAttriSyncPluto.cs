using System;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.Game;

namespace Mogo.RPC
{
    public class AvatarAttriSyncPluto : Pluto
    {
        protected override void DoDecode(byte[] data, ref int unLen)
        {
            if (MogoWorld.thePlayer == null)
                return;
            var info = new AttachedInfo();
            info.props = new List<KeyValuePair<EntityDefProperties, object>>();
            var entity = MogoWorld.thePlayer.entity;
            if (entity != null)
            {
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
            if (MogoWorld.thePlayer == null)
                return;
            MogoWorld.thePlayer.SynEntityAttrs(Arguments[0] as AttachedInfo);
        }

        /// <summary>
        /// 创建新AvatarAttriSyncPluto实例。
        /// </summary>
        /// <returns>AvatarAttriSyncPluto实例</returns>
        internal static Pluto Create()
        {
            return new AvatarAttriSyncPluto();
        }
    }
}
