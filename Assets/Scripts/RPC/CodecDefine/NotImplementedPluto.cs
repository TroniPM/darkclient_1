using System;
using Mogo.Util;

namespace Mogo.RPC
{
    internal class NotImplementedPluto : Pluto
    {
        private static NotImplementedPluto m_instance = new NotImplementedPluto();

        //public override byte[] Encode(params object[] args)
        //{
        //    LoggerHelper.Warning(String.Format("Calling a NotImplementedPluto encode."));
        //    return new byte[0];
        //}

        protected override void DoDecode(byte[] data, ref int unLen)
        {
            LoggerHelper.Warning(String.Format("Calling a NotImplementedPluto decode."));
        }

        public override void HandleData()
        {
            LoggerHelper.Warning(String.Format("Calling a NotImplementedPluto decode."));
        }

        internal static Pluto Create()
        {
            return m_instance;
        }
    }
}