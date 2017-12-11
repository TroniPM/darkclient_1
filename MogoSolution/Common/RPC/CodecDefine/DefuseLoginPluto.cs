using System;

namespace Mogo.RPC
{
	class DefuseLoginPluto : Pluto
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
            Mogo.Util.LoggerHelper.Error("defuse login");
            Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.FrameWorkEvent.DefuseLogin);
        }

        internal static Pluto Create()
        {
            return new DefuseLoginPluto();
        }
	}
}
