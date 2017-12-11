using System;

namespace Mogo.RPC
{
	class ReConnectRefusePluto : Pluto
	{
        public Byte[] Encode()
        {
            return null;
        }

        protected override void DoDecode(byte[] data, ref int unLen)
        {
        }

        public override void HandleData()
        {
            Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.FrameWorkEvent.ReConnectRefuse);
        }

        internal static Pluto Create()
        {
            return new ReConnectRefusePluto();
        }
	}
}
