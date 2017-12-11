/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DESCrypto
// 创建者：Charles Zuo
// 创建日期：2013.8.9
//----------------------------------------------------------------*/
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Mogo.Util
{
    public static class DESCrypto
    {
        public static Byte[] Encrypt(Byte[] ToEncrypt, byte[] Key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider() { Key = Key, IV = Key };
            Byte[] encrypted;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(ToEncrypt, 0, ToEncrypt.Length);
                    cs.FlushFinalBlock();
                    encrypted = ms.ToArray();
                }
            }
            return encrypted;
        }
        public static Byte[] Decrypt(Byte[] ToDecrypt, byte[] Key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider() { Key = Key, IV = Key };
            Byte[] decrypted;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(ToDecrypt, 0, ToDecrypt.Length);
                    cs.FlushFinalBlock();
                    decrypted = ms.ToArray();
                }
            }
            return decrypted;
        }
        public static String GenerateKey()
        {
            var desCrypto = DESCryptoServiceProvider.Create();
            return ASCIIEncoding.ASCII.GetString(desCrypto.Key);
        }
    }
}