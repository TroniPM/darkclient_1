using System;

namespace Mogo.RPC
{
	class ReConnectPluto : Pluto
	{
        public Byte[] Encode(string key)
        {
            Push(VUInt16.Instance.Encode(MSGIDType.BASEAPP_CLIENT_RELOGIN));
            Push(VString.Instance.Encode(key));

            Byte[] result = new Byte[m_unLen];
            Buffer.BlockCopy(m_szBuff, 0, result, 0, m_unLen);
            EndPluto(result);
            return result;
        }

        protected override void DoDecode(byte[] data, ref int unLen)
        {
        }

        public override void HandleData()
        {
        }

        internal static Pluto Create()
        {
            return new ReConnectPluto();
        }
	}
}
