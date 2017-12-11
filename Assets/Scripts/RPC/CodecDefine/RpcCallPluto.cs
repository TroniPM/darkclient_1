using Mogo.Util;
using System;

namespace Mogo.RPC
{
    public class RpcCallPluto : Pluto
    {
        public EntityDefMethod svrMethod = null;
        public bool toCell = false;
        /// <summary>
        /// 将远程方法调用编码为二进制数组。
        /// </summary>
        /// <param name="args">参数列表</param>
        /// <returns>编码后的二进制数组</returns>
        public Byte[] Encode(params Object[] args)
        {
            EntityDef ety = CurrentEntity;//DefParser.Instance.GetEntityByName(EntityName);
            if (ety == null)
            {
                throw new DefineParseException(String.Format("Encode error: CurrentEntity is null."));
            }

            EntityDefMethod method = svrMethod;
            FuncID = method.FuncID;
            
            if (method.ArgsType.Count != args.Length)
            {
                throw new DefineParseException(String.Format("Encode error: The number of parameters is not match. func: {0}, require num: {1}, current num: {2}.", FuncID, method.ArgsType.Count, args.Length));
            }
            //屏蔽类型检查，报错就报吧
            //for (int i = 0; i < method.ArgsType.Count; i++)
            //{
            //    if (!args[i].GetType().IsAssignableFrom(method.ArgsType[i].VValueType))
            //        throw new DefineParseException(String.Format("Encode error: Parameters type is not match '{0}' in entity '{1}' of index '{2}'.", FuncID, ety.Name, i));
            //}

            if (toCell)
            {
                Push(VUInt16.Instance.Encode(MSGIDType.BASEAPP_CLIENT_RPC2CELL_VIA_BASE));
                Push(VUInt16.Instance.Encode(FuncID));
                for (int j = 0; j < args.Length; j++)
                {
                    Push(method.ArgsType[j].Encode(args[j]));
                }

                Byte[] cellRst = new Byte[m_unLen];
                Buffer.BlockCopy(m_szBuff, 0, cellRst, 0, m_unLen);
                EndPluto(cellRst);
                return cellRst;
            }

            Push(VUInt16.Instance.Encode(MSGIDType.BASEAPP_CLIENT_RPCALL));  // 指定 pluto 类型为 rpc
            Push(VUInt16.Instance.Encode(FuncID));                         // 指定 调用的 func 标识
            for (int i = 0; i < args.Length; i++)
            {
                Push(method.ArgsType[i].Encode(args[i]));                    // 增加参数
            }

            Byte[] result = new Byte[m_unLen];
            Buffer.BlockCopy(m_szBuff, 0, result, 0, m_unLen);
            EndPluto(result);
            return result;
        }

        /// <summary>
        /// 将远程调用的方法解码为RpcCall调用。
        /// </summary>
        /// <param name="data">远程调用方法的二进制数组</param>
        /// <param name="unLen">数据偏移量</param>
        protected override void DoDecode(byte[] data, ref int unLen)
        {
            FuncID = (ushort)VUInt16.Instance.Decode(data, ref unLen);
            if (CurrentEntity == null)
                throw new DefineParseException(String.Format("Decode error: Current Entity is not set."));

            EntityDefMethod method = CurrentEntity.TryGetClientMethod(FuncID);

            if (method == null)
                throw new DefineParseException(String.Format("Decode error: Can not find function '{0}' in entity '{1}'.", FuncID, CurrentEntity.Name));

            FuncName = method.FuncName;
            Arguments = new Object[method.ArgsType.Count];
            for (int i = 0; i < method.ArgsType.Count; i++)
            {
                Arguments[i] = method.ArgsType[i].Decode(data, ref unLen);
            }
            //if (FuncName != "GetServerTimeResp")
            //    LoggerHelper.Debug("Client call RPC_" + FuncName);
        }

        public override void HandleData()
        {
            if (!String.IsNullOrEmpty(FuncName) && Arguments != null)
            {
                //EventDispatcher.TriggerEvent(MSGIDType.CLIENT_RPC_RESP.ToString(), FuncName, Arguments);
                //LoggerHelper.Debug("HandleData RPC_" + FuncName + " " + Arguments.Length);
                EventDispatcher.TriggerEvent<object[]>(Util.Utils.RPC_HEAD + FuncName, Arguments);
            }
        }

        /// <summary>
        /// 创建新RpcCallPluto实例。
        /// </summary>
        /// <returns>RpcCallPluto实例</returns>
        internal static Pluto Create()
        {
            return new RpcCallPluto();
        }
    }
}