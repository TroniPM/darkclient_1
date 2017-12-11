using System;
using System.Collections.Generic;
using Mogo.Util;

namespace Mogo.RPC
{
    class EntityPosSyncPluto : Pluto
    {
        protected override void DoDecode(byte[] data, ref int unLen)
        {
            var info = new CellAttachedInfo();
            //info.id = (uint)VUInt32.Instance.Decode(data, ref unLen); // eid
            //info.face = (byte)VUInt8.Instance.Decode(data, ref unLen);// rotation
            //info.x = (short)VInt16.Instance.Decode(data, ref unLen); //x
            //info.y = (short)VInt16.Instance.Decode(data, ref unLen); //y
            UInt16 x = (UInt16)VUInt16.Instance.Decode(data, ref unLen);
            UInt16 y = (UInt16)VUInt16.Instance.Decode(data, ref unLen);
            info.x = (short)x;
            info.y = (short)y;

            Arguments = new Object[1];
            Arguments[0] = info;
        }

        public override void HandleData()
        {
            LoggerHelper.Debug("server sync self pos");
            var info = Arguments[0] as CellAttachedInfo;
            MogoWorld.thePlayer.MoveTo(info.position.x, info.position.z, 0, 0/*info.face * 2.0f*/, 0);
        }

        internal static Pluto Create()
        {
            return new EntityPosSyncPluto();
        }
    }
}
