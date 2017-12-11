/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：RSACrypto
// 创建者：Charles Zuo
// 创建日期：2013.8.9
//----------------------------------------------------------------*/
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Mogo.Util
{
    public static class RSACrypto
    {
        public static Byte[] Encrypt(Byte[] ToEncrypt, String Key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(Key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(Key);
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
        public static Byte[] Decrypt(Byte[] ToDecrypt, String Key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(Key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(Key);
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
        public static void GenerateKey()
        {
            RSACryptoServiceProvider rcp = new RSACryptoServiceProvider();
            string keyString = rcp.ToXmlString(true);
            XMLParser.SaveText("E:\\key.xml", keyString);
            //FileStream fs = File.Create("E:\\key.xml");
            //fs.Write(Encoding.UTF8.GetBytes(keyString), 0, Encoding.UTF8.GetBytes(keyString).Length);
            //fs.Flush();
            //fs.Close();
        }
    }
}