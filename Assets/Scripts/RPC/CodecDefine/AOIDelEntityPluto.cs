using System;
using System.Collections.Generic;
using Mogo.Util;

namespace Mogo.RPC
{
	class AOIDelEntityPluto:Pluto
	{
        protected override void DoDecode(byte[] data, ref int unLen)
        {
            Arguments = new Object[1];
            Arguments[0] = VUInt32.Instance.Decode(data, ref unLen);
        }

        public override void HandleData()
        {
            UInt32 entityID = (UInt32)Arguments[0];
            LoggerHelper.Debug("aoi del " + entityID);
            EventDispatcher.TriggerEvent<uint>(Events.FrameWorkEvent.AOIDelEvtity, entityID);
        }

        internal static Pluto Create()
        {
            return new AOIDelEntityPluto();
        }
	}
}
