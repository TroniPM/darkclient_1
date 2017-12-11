using System;
using Mogo.Util;

namespace Mogo.RPC
{
    public class LoginPluto : Pluto
    {
        /// <summary>
        /// 将远程方法调用编码为二进制数组。
        /// </summary>
        /// <param name="passport">登录帐号</param>
        /// <param name="password">登录密码</param>
        /// <param name="loginArgs">登录类型参数</param>
        /// <returns>编码后的二进制数组</returns>
        public Byte[] Encode(String passport, String password, String loginArgs)
        {
            Push(VUInt16.Instance.Encode(MSGIDType.LOGINAPP_LOGIN));
            Push(VString.Instance.Encode(passport));
            Push(VString.Instance.Encode(password));
            Push(VString.Instance.Encode(loginArgs));

            Byte[] result = new Byte[m_unLen];
            Buffer.BlockCopy(m_szBuff, 0, result, 0, m_unLen);
            //string s = "";
            //for (int i = 0; i < m_unLen; i++)
            //{
            //    s = s + result[i] + " ";
            //}
            //LoggerHelper.Error("cryto first " + s);
            EndPluto(result);
            //string ss = "";
            //encryto.Reset();
            //for (int j = 2; j < result.Length; j++)
            //{
            //    result[j] = encryto.Decode(result[j]);
            //}
            //for (int k = 0; k < result.Length; k++)
            //{
            //    ss = ss + result[k] + " ";
            //}
            //LoggerHelper.Error("cryto end " + ss); ;
            return result;
        }

        /// <summary>
        /// 将远程方法调用编码为二进制数组。
        /// </summary>
        /// <returns>编码后的二进制数组</returns>
        public Byte[] Encode(params string[] args)
        {
            Push(VUInt16.Instance.Encode(MSGIDType.LOGINAPP_LOGIN));
            foreach (var item in args)
            {
                Push(VString.Instance.Encode(item));
            }

            Byte[] result = new Byte[m_unLen];
            Buffer.BlockCopy(m_szBuff, 0, result, 0, m_unLen);
            EndPluto(result);
            return result;
        }

        /// <summary>
        /// 将远程调用的方法解码为Login调用。
        /// </summary>
        /// <param name="data">远程调用方法的二进制数组</param>
        /// <param name="unLen">数据偏移量</param>
        protected override void DoDecode(byte[] data, ref int unLen)
        {
            FuncName = MSGIDType.CLIENT_LOGIN_RESP.ToString();

            Arguments = new Object[1];
            Arguments[0] = VUInt8.Instance.Decode(data, ref unLen);
            //Arguments = new Object[3];
            //Arguments[0] = VString.Instance.Decode(data, ref unLen);
            //Arguments[1] = VString.Instance.Decode(data, ref unLen);
            //Arguments[2] = VString.Instance.Decode(data, ref unLen);
        }

        public override void HandleData()
        {
            var result = (LoginResult)(byte)Arguments[0];
            EventDispatcher.TriggerEvent<LoginResult>(Events.FrameWorkEvent.Login, result);

            //if (!String.IsNullOrEmpty(FuncName) && Arguments != null)
            //    EventDispatcher.TriggerEvent<object[]>(FuncName, Arguments);
            //else
            //    LoggerHelper.Warning(String.Format("Null function in RpcCallPluto."));
            //LoggerHelper.Debug("OnDataReceive " + FuncName);
        }

        /// <summary>
        /// 创建新LoginPluto实例。
        /// </summary>
        /// <returns>LoginPluto实例</returns>
        internal static Pluto Create()
        {
            return new LoginPluto();
        }
    }
}