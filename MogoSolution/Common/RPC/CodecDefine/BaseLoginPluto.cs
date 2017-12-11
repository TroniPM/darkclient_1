using System;
using Mogo.Util;
using Mogo.Game;

namespace Mogo.RPC
{
    public class BaseLoginPluto : Pluto
    {
        public static Action<String, Int32, String> BaseLoginFinished;

        /// <summary>
        /// 将远程方法调用编码为二进制数组。
        /// </summary>
        /// <param name="token">连接标记</param>
        /// <returns>编码后的二进制数组</returns>
        public Byte[] Encode(String token)
        {
            Push(VUInt16.Instance.Encode(MSGIDType.BASEAPP_CLIENT_LOGIN));
            Push(VString.Instance.Encode(token));

            Byte[] result = new Byte[m_unLen];
            Buffer.BlockCopy(m_szBuff, 0, result, 0, m_unLen);
            EndPluto(result);
            return result;
        }

        /// <summary>
        /// 将远程调用的方法解码为BaseLogin调用。
        /// </summary>
        /// <param name="data">远程调用方法的二进制数组</param>
        /// <param name="unLen">数据偏移量</param>
        protected override void DoDecode(byte[] data, ref int unLen)
        {
            FuncName = MSGIDType.CLIENT_NOTIFY_ATTACH_BASEAPP.ToString();

            Arguments = new Object[3];
            Arguments[0] = VString.Instance.Decode(data, ref unLen);
            Arguments[1] = VUInt16.Instance.Decode(data, ref unLen);
            Arguments[2] = VString.Instance.Decode(data, ref unLen);
LoggerHelper.Debug("------------ DoDecode 2 ---- CLIENT_NOTIFY_ATTACH_BASEAPP -----------");
        }

        public override void HandleData()
        {
            string ip = Arguments[0] as String;
            int port = (Int32)(UInt16)Arguments[1];
            string token = Arguments[2] as String;
            try
            {
                EventDispatcher.TriggerEvent<string, int, string>(Events.FrameWorkEvent.BaseLogin, ip, port, token);
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
            }
        }

        /// <summary>
        /// 创建新BaseLoginPluto实例。
        /// </summary>
        /// <returns>BaseLoginPluto实例</returns>
        internal static Pluto Create()
        {
            return new BaseLoginPluto();
        }
    }
}