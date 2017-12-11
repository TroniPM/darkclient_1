using System;

namespace Mogo.RPC
{
    public class MovePluto : Pluto
    {
        /// <summary>
        /// 将远程方法调用编码为二进制数组。
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <returns>编码后的二进制数组</returns>
        public Byte[] Encode(byte face, ushort x, ushort y)
        {
            Push(VUInt16.Instance.Encode(MSGIDType.BASEAPP_CLIENT_MOVE_REQ));
            //Push(VUInt8.Instance.Encode(face));
            Push(VUInt16.Instance.Encode(x));
            Push(VUInt16.Instance.Encode(y));

            Byte[] result = new Byte[m_unLen];
            Buffer.BlockCopy(m_szBuff, 0, result, 0, m_unLen);
            EndPluto(result);
            return result;
        }

        /// <summary>
        /// 将远程调用的方法解码为MovePluto调用。
        /// </summary>
        /// <param name="data">远程调用方法的二进制数组</param>
        /// <param name="unLen">数据偏移量</param>
        protected override void DoDecode(byte[] data, ref int unLen)
        {

        }

        public override void HandleData()
        {
        }

        /// <summary>
        /// 创建新MovePluto实例。
        /// </summary>
        /// <returns>MovePluto实例</returns>
        internal static Pluto Create()
        {
            return new MovePluto();
        }
    }
}