using System;
using Mogo.Util;
using Mogo.Game;

namespace Mogo.RPC
{
    public class CheckDefMD5Pluto : Pluto
    {
        public Byte[] Encode(Byte[] bytes)
        {
            LoggerHelper.Debug("CheckDefMD5 Encode");
            Push(VUInt16.Instance.Encode(MSGIDType.LOGINAPP_CHECK));
#if UNITY_IPHONE
			string str = IOSUtils.FormatMD5(bytes);
#else
            var str = Mogo.Util.Utils.FormatMD5(bytes);
#endif
            LoggerHelper.Debug("String MD5: " + str);
            Push(VString.Instance.Encode(str));

            Byte[] result = new Byte[m_unLen];
            Buffer.BlockCopy(m_szBuff, 0, result, 0, m_unLen);
            EndPluto(result);
            return result;
        }

        protected override void DoDecode(byte[] data, ref int unLen)
        {
            LoggerHelper.Debug("CheckDefMD5 DoDecode");
            FuncName = MSGIDType.CLIENT_CHECK_RESP.ToString();

            Arguments = new Object[1];
            Arguments[0] = VUInt8.Instance.Decode(data, ref unLen);
        }

        public override void HandleData()
        {
            LoggerHelper.Debug("CheckDefMD5 HandleData");
            var result = (DefCheckResult)(byte)Arguments[0];
            EventDispatcher.TriggerEvent<DefCheckResult>(Events.FrameWorkEvent.CheckDef, result);
        }

        internal static Pluto Create()
        {
            return new CheckDefMD5Pluto();
        }
    }
}
