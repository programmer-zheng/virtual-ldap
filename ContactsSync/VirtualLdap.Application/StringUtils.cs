﻿using System.Security.Cryptography;
using System.Text;
using Abp.Extensions;

namespace VirtualLdap.Application
{
    public static class StringUtils
    {
        /// <summary>
        /// 预定义的公钥
        /// </summary>
        private const string Pubkey = "SCy9d*9@";


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="strText">需要加密的数据</param>
        /// <returns>加密后的字符串</returns>
        public static string DesEncrypt(this string strText)
        {
            return strText.DesEncrypt(Pubkey);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="strText">需要加密的数据</param>
        /// <param name="strEncrKey">8位公钥</param>
        /// <returns>加密后的字符串</returns>
        public static string DesEncrypt(this string strText, string strEncrKey)
        {
            if (strEncrKey.IsNullOrWhiteSpace() || strEncrKey.Length<8)
            {
                throw new  ArgumentException($"{nameof(strEncrKey)}不能为空，且长度不能少于8位");
            }
            byte[] byKey ;
            byte[] iv = {26, 192, 238, 135, 155, 17, 93, 118};
            try
            {
                byKey = Encoding.UTF8.GetBytes(strEncrKey.Substring(0, 8));
                var des = DES.Create();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, iv), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="strText">需要解密的数据</param>
        /// <returns>解密后的字符串</returns>
        public static string DesDecrypt(this string strText)
        {
            return strText.DesDecrypt(Pubkey);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="strText">需要解密的数据</param>
        /// <param name="sDecrKey">8位公钥</param>
        /// <returns>解密后的字符串</returns>
        public static string DesDecrypt(this string strText, string sDecrKey)
        {
            
            if (sDecrKey.IsNullOrWhiteSpace() || sDecrKey.Length<8)
            {
                throw new  ArgumentException($"{nameof(sDecrKey)}不能为空，且长度不能少于8位");
            }
            byte[] byKey ;
            byte[] iv = {26, 192, 238, 135, 155, 17, 93, 118};
            byte[] inputByteArray;
            try
            {
                byKey = Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
                var des = DES.Create();
                inputByteArray = Convert.FromBase64String(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, iv), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = new UTF8Encoding();
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}