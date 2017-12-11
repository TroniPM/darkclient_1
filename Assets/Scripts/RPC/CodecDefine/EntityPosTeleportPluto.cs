using System;
using System.Collections.Generic;
using Mogo.Util;

namespace Mogo.RPC
{
    class EntityPosTeleportPluto : Pluto
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
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var item in data)
            {
                sb.Append(item);
            }
            LoggerHelper.Debug("EntityPosTeleportPluto: " + sb.ToString());
            Arguments = new Object[1];
            Arguments[0] = info;
        }

        public override void HandleData()
        {//用于同场景跳点传送
            LoggerHelper.Debug("server sync self teleport");
            MogoWorld.thePlayer.SetEntityCellInfo(Arguments[0] as CellAttachedInfo);
            MogoWorld.thePlayer.UpdatePosition();
        }

        internal static Pluto Create()
        {
            return new EntityPosTeleportPluto();
        }
    }
}
