using Mogo.Util;
using System;
using System.Collections.Generic;

namespace Mogo.RPC
{
    class OtherEntityPosPullPluto : Pluto
    {
        protected override void DoDecode(byte[] data, ref int unLen)
        {
            var info = new CellAttachedInfo();
            info.id = (uint)VUInt32.Instance.Decode(data, ref unLen); // eid
            //info.face = (byte)VUInt8.Instance.Decode(data, ref unLen);// rotation
            UInt16 x = (UInt16)VUInt16.Instance.Decode(data, ref unLen);
            UInt16 y = (UInt16)VUInt16.Instance.Decode(data, ref unLen);
            info.x = (short)x;
            info.y = (short)y;

            Arguments = new object[1] { info };
        }

        public override void HandleData()
        {
            LoggerHelper.Debug("server sync other pull");
            //var info = Arguments[0] as CellAttachedInfo;
            //if (!MogoWorld.Entities.ContainsKey(info.id))
            //    return;
            //var entity = MogoWorld.Entities[info.id];
            ////LoggerHelper.Debug("OtherEntityPosSyncPluto HandleData " + entity);
            ////entity.TurnTo(0, info.face * 2, 0);
            //entity.MoveTo(info.position.x, info.position.z, 0, info.face * 2.0f, 0);
        }

        internal static Pluto Create()
        {
            return new OtherEntityPosPullPluto();
        }
    }
}
