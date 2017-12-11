// 模块名   :  BitCryto
// 创建者   :  Steven Yang
// 创建日期 :  2013-1-10
// 描    述 :  简单的bit 加密

using System.Collections;

namespace Mogo.Util
{
//测试用， 从 c++ 移植过来。 待修改
    public class BitCryto
    {

        private short[] crytoKey;
        private int offsetOfKey;

        public BitCryto(short[] sKey)
        {
            crytoKey = sKey;
        }

        public byte Encode(byte inputByte)
        {
            if (offsetOfKey >= crytoKey.Length)
            {
                offsetOfKey = 0;
            }
            ushort offset = (ushort)crytoKey[offsetOfKey];
            ++offsetOfKey;

            byte outputByte = (byte)((offset + (ushort)inputByte) & 0xff);
            return outputByte;
        }

        public byte Decode(byte inputByte)
        {
            if (offsetOfKey >= crytoKey.Length)
            {
                offsetOfKey = 0;
            }
            short offset = (short)crytoKey[offsetOfKey];
            ++offsetOfKey;

            short outputByte = (short)((short)inputByte - offset);
            if (outputByte < 0)
            {
                outputByte += 256;
            }
            return (byte)outputByte;
        }

        public void Reset()
        {
            offsetOfKey = 0;
        }

    }
}