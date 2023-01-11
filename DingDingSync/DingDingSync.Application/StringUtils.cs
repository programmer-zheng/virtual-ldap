using System.Security.Cryptography;
using System.Text;

namespace DingDingSync.Application
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
            byte[] byKey = null;
            byte[] iv = {26, 192, 238, 135, 155, 17, 93, 118};
            try
            {
                byKey = Encoding.UTF8.GetBytes(strEncrKey.Substring(0, strEncrKey.Length));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
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
            byte[] byKey = null;
            byte[] IV = {0x1A, 0xC0, 0xEE, 0x87, 0x9B, 0x11, 0x5D, 0x76};
            byte[] inputByteArray = new byte[strText.Length];
            try
            {
                byKey = Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
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