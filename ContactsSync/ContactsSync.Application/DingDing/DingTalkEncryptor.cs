using System.Security.Cryptography;
using System.Text;

namespace ContactsSync.Application.DingDing;

public class DingTalkEncryptor
{
    private byte[] aesKey;
    private string token;
    private string corpId;

    /// <summary>
    /// ask getPaddingBytes key固定长度
    /// </summary>
    private const int AES_ENCODE_KEY_LENGTH = 43;

    /// <summary>
    /// 加密随机字符串字节长度
    /// </summary>
    private const int RANDOM_LENGTH = 16;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token">钉钉开放平台上，开发者设置的token</param>
    /// <param name="encodingAesKey">钉钉开放台上，开发者设置的EncodingAESKey</param>
    /// <param name="corpId">
    /// 企业自建应用 - 事件订阅, 使用appKey<br/>
    /// 企业自建应用 - 注册回调地址, 使用corpId<br/>
    /// 第三方企业应用, 使用suiteKey
    /// </param>
    /// <exception cref="DingTalkEncryptException"></exception>
    public DingTalkEncryptor(string token, string encodingAesKey, string corpId)
    {
        if (null == encodingAesKey || encodingAesKey.Length != AES_ENCODE_KEY_LENGTH)
        {
            throw new DingTalkEncryptException(DingTalkEncryptException.AES_KEY_ILLEGAL);
        }

        this.token = token;
        this.corpId = corpId;
        aesKey = Convert.FromBase64String(encodingAesKey + "=");
    }

    /// <summary>
    /// 将和钉钉开放平台同步的消息体加密,返回加密字典
    /// </summary>
    /// <param name="plaintext">传递的消息体明文</param>
    /// <returns></returns>
    public Dictionary<string, string> GetEncryptedMap(string plaintext)
    {
        var time = DateTime.UtcNow.Millisecond;
        return GetEncryptedMap(plaintext, time);
    }

    /// <summary>
    /// 将和钉钉开放平台同步的消息体加密,返回加密字典
    /// </summary>
    /// <param name="plaintext">传递的消息体明文</param>
    /// <param name="timeStamp">时间戳</param>
    /// <returns></returns>
    /// <exception cref="DingTalkEncryptException"></exception>
    public Dictionary<string, string> GetEncryptedMap(string plaintext, long timeStamp)
    {
        if (null == plaintext)
        {
            throw new DingTalkEncryptException(DingTalkEncryptException.ENCRYPTION_PLAINTEXT_ILLEGAL);
        }

        var nonce = Utils.GetRandomStr(RANDOM_LENGTH);
        if (null == nonce)
        {
            throw new DingTalkEncryptException(DingTalkEncryptException.ENCRYPTION_NONCE_ILLEGAL);
        }

        string encrypt = this.Encrypt(nonce, plaintext);
        string signature = GetSignature(token, timeStamp.ToString(), nonce, encrypt);
        Dictionary<string, string> resultMap = new Dictionary<string, string>();
        resultMap["msg_signature"] = signature;
        resultMap["encrypt"] = encrypt;
        resultMap["timeStamp"] = timeStamp.ToString();
        resultMap["nonce"] = nonce;
        return resultMap;
    }

    /// <summary>
    /// 密文解密
    /// </summary>
    /// <param name="msgSignature">签名串</param>
    /// <param name="timeStamp">时间戳</param>
    /// <param name="nonce">随机串</param>
    /// <param name="encryptMsg">密文</param>
    /// <returns>解密后的原文</returns>
    /// <exception cref="DingTalkEncryptException"></exception>
    public string GetDecryptMsg(string msgSignature, string timeStamp, string nonce, string encryptMsg)
    {
        //校验签名
        string signature = GetSignature(token, timeStamp, nonce, encryptMsg);
        if (!signature.Equals(msgSignature))
        {
            throw new DingTalkEncryptException(DingTalkEncryptException.COMPUTE_SIGNATURE_ERROR);
        }

        // 解密
        string result = Decrypt(encryptMsg);
        return result;
    }

    /// <summary>
    /// 对明文加密
    /// </summary>
    /// <param name="random">随机字符串</param>
    /// <param name="plaintext">需要加密的明文</param>
    /// <returns>加密后base64编码的字符串</returns>
    /// <exception cref="DingTalkEncryptException"></exception>
    private string Encrypt(string random, string plaintext)
    {
        try
        {
            byte[] randomBytes = Encoding.UTF8.GetBytes(random); // random.getBytes(CHARSET);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plaintext); // plaintext.getBytes(CHARSET);
            byte[] lengthByte = Utils.Int2Bytes(plainTextBytes.Length);
            byte[] corpidBytes = Encoding.UTF8.GetBytes(corpId); // corpId.getBytes(CHARSET);
            //MemoryStream byteStream = new MemoryStream();
            var bytestmp = new List<byte>();
            bytestmp.AddRange(randomBytes);
            bytestmp.AddRange(lengthByte);
            bytestmp.AddRange(plainTextBytes);
            bytestmp.AddRange(corpidBytes);
            byte[] padBytes = PKCS7Padding.GetPaddingBytes(bytestmp.Count);
            bytestmp.AddRange(padBytes);
            byte[] unencrypted = bytestmp.ToArray();

            var rDel = new RijndaelManaged();
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;
            rDel.Key = aesKey;
            rDel.IV = aesKey.ToList().Take(16).ToArray();
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(unencrypted, 0, unencrypted.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        catch (Exception e)
        {
            throw new DingTalkEncryptException(DingTalkEncryptException.COMPUTE_ENCRYPT_TEXT_ERROR);
        }
    }


    /// <summary>
    /// 对密文进行解密
    /// </summary>
    /// <param name="text">需要解密的密文</param>
    /// <returns>解密得到的明文</returns>
    /// <exception cref="DingTalkEncryptException"></exception>
    private string Decrypt(string text)
    {
        byte[] originalArr;
        try
        {
            byte[] toEncryptArray = Convert.FromBase64String(text);
            var rDel = new RijndaelManaged();
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;
            rDel.Key = aesKey;
            rDel.IV = aesKey.ToList().Take(16).ToArray();
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            originalArr = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            //return System.Text.UTF8Encoding.UTF8.GetString(resultArray);

            //// 设置解密模式为AES的CBC模式
            //Cipher cipher = Cipher.getInstance("AES/CBC/NoPadding");
            //SecretKeySpec keySpec = new SecretKeySpec(aesKey, "AES");
            //IvParameterSpec iv = new IvParameterSpec(Arrays.copyOfRange(aesKey, 0, 16));
            //cipher.init(Cipher.DECRYPT_MODE, keySpec, iv);
            //// 使用BASE64对密文进行解码
            //byte[] encrypted = Base64.decodeBase64(text);
            //// 解密
            //originalArr = cipher.doFinal(encrypted);
        }
        catch (Exception e)
        {
            throw new DingTalkEncryptException(DingTalkEncryptException.COMPUTE_DECRYPT_TEXT_ERROR);
        }

        string plainText;
        string fromCorpid;
        try
        {
            // 去除补位字符
            byte[] bytes = PKCS7Padding.RemovePaddingBytes(originalArr);
            //Console.Out.WriteLine("bytes size:" + bytes.Length);

            // 分离16位随机字符串,网络字节序和corpId
            byte[] networkOrder = bytes.Skip(16).Take(4).ToArray(); // Arrays.copyOfRange(bytes, 16, 20);
            for (int i = 0; i < 4; i++)
            {
                //Console.Out.WriteLine("networkOrder size:" + (int)networkOrder[i]);
            }

            //Console.Out.WriteLine("bytes plainText:" + networkOrder.Length + " " + JsonSerializer.Serialize(networkOrder));
            int plainTextLegth = Utils.Bytes2Int(networkOrder);
            //Console.Out.WriteLine("bytes size:" + plainTextLegth);

            plainText = Encoding.UTF8.GetString(bytes.Skip(20).Take(plainTextLegth)
                .ToArray()); // new String(Arrays.copyOfRange(bytes, 20, 20 + plainTextLegth), CHARSET);
            fromCorpid =
                Encoding.UTF8.GetString(bytes.Skip(20 + plainTextLegth)
                    .ToArray()); //new String(Arrays.copyOfRange(bytes, 20 + plainTextLegth, bytes.length), CHARSET);
            //Console.Out.WriteLine("bytes plainText:" + plainText);
        }
        catch (Exception e)
        {
            throw new DingTalkEncryptException(DingTalkEncryptException.COMPUTE_DECRYPT_TEXT_LENGTH_ERROR);
        }
        //Console.Out.WriteLine(fromCorpid + "=====" + corpId);


        // corpid不相同的情况
        if (!fromCorpid.Equals(corpId))
        {
            throw new DingTalkEncryptException(DingTalkEncryptException.COMPUTE_DECRYPT_TEXT_CORPID_ERROR);
        }

        return plainText;
    }

    /// <summary>
    /// 数字签名
    /// </summary>
    /// <param name="token">isv token</param>
    /// <param name="timestamp">时间戳</param>
    /// <param name="nonce">随机串</param>
    /// <param name="encrypt">加密文本</param>
    /// <returns></returns>
    /// <exception cref="DingTalkEncryptException"></exception>
    public static string GetSignature(string token, string timestamp, string nonce, string encrypt)
    {
        try
        {
            string[] array = new string[] { token, timestamp, nonce, encrypt };
            Array.Sort(array, StringComparer.Ordinal);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                sb.Append(array[i]);
            }

            string str = sb.ToString();
            SHA1 hash = SHA1.Create();
            Encoding encoder = Encoding.ASCII;
            byte[] combined = encoder.GetBytes(str);


            byte[] digest = hash.ComputeHash(combined);
            StringBuilder hexstr = new StringBuilder();
            string shaHex = "";
            for (int i = 0; i < digest.Length; i++)
            {
                shaHex = ((int)digest[i]).ToString("x"); // Integer.toHexString(digest[i] & 0xFF);
                if (shaHex.Length < 2)
                {
                    hexstr.Append(0);
                }

                hexstr.Append(shaHex);
            }

            return hexstr.ToString();
        }
        catch (Exception e)
        {
            throw new DingTalkEncryptException(DingTalkEncryptException.COMPUTE_SIGNATURE_ERROR);
        }
    }
}

/**
* 钉钉开放平台加解密异常类
*/
public class DingTalkEncryptException : Exception
{
    /**成功**/
    public const int SUCCESS = 0;

    /**加密明文文本非法**/
    public const int ENCRYPTION_PLAINTEXT_ILLEGAL = 900001;

    /**加密时间戳参数非法**/
    public const int ENCRYPTION_TIMESTAMP_ILLEGAL = 900002;

    /**加密随机字符串参数非法**/
    public const int ENCRYPTION_NONCE_ILLEGAL = 900003;

    /**不合法的aeskey**/
    public const int AES_KEY_ILLEGAL = 900004;

    /**签名不匹配**/
    public const int SIGNATURE_NOT_MATCH = 900005;

    /**计算签名错误**/
    public const int COMPUTE_SIGNATURE_ERROR = 900006;

    /**计算加密文字错误**/
    public const int COMPUTE_ENCRYPT_TEXT_ERROR = 900007;

    /**计算解密文字错误**/
    public const int COMPUTE_DECRYPT_TEXT_ERROR = 900008;

    /**计算解密文字长度不匹配**/
    public const int COMPUTE_DECRYPT_TEXT_LENGTH_ERROR = 900009;

    /**计算解密文字corpid不匹配**/
    public const int COMPUTE_DECRYPT_TEXT_CORPID_ERROR = 900010;

    private static Dictionary<int, string> msgMap = new Dictionary<int, string>();

    static DingTalkEncryptException()
    {
        msgMap[SUCCESS] = "成功";
        msgMap[ENCRYPTION_PLAINTEXT_ILLEGAL] = "加密明文文本非法";
        msgMap[ENCRYPTION_TIMESTAMP_ILLEGAL] = "加密时间戳参数非法";
        msgMap[ENCRYPTION_NONCE_ILLEGAL] = "加密随机字符串参数非法";
        msgMap[SIGNATURE_NOT_MATCH] = "签名不匹配";
        msgMap[COMPUTE_SIGNATURE_ERROR] = "签名计算失败";
        msgMap[AES_KEY_ILLEGAL] = "不合法的aes key";
        msgMap[COMPUTE_ENCRYPT_TEXT_ERROR] = "计算加密文字错误";
        msgMap[COMPUTE_DECRYPT_TEXT_ERROR] = "计算解密文字错误";
        msgMap[COMPUTE_DECRYPT_TEXT_LENGTH_ERROR] = "计算解密文字长度不匹配";
        msgMap[COMPUTE_DECRYPT_TEXT_CORPID_ERROR] = "计算解密文字corpid不匹配";
    }

    private int code;

    public DingTalkEncryptException(int exceptionCode) : base(msgMap[exceptionCode])
    {
        code = exceptionCode;
    }
}

/// <summary>
/// PKCS7算法的加密填充
/// </summary>
public class PKCS7Padding
{
    //private const Charset CHARSET = Charset.forName("utf-8");
    private const int BLOCK_SIZE = 32;

    /// <summary>
    /// 填充mode字节
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static byte[] GetPaddingBytes(int count)
    {
        int amountToPad = BLOCK_SIZE - count % BLOCK_SIZE;
        if (amountToPad == 0)
        {
            amountToPad = BLOCK_SIZE;
        }

        char padChr = Chr(amountToPad);
        string tmp = string.Empty;
        ;
        for (int index = 0; index < amountToPad; index++)
        {
            tmp += padChr;
        }

        return Encoding.UTF8.GetBytes(tmp);
    }

    /// <summary>
    /// 移除mode填充字节
    /// </summary>
    /// <param name="decrypted"></param>
    /// <returns></returns>
    public static byte[] RemovePaddingBytes(byte[] decrypted)
    {
        int pad = decrypted[decrypted.Length - 1];
        if (pad < 1 || pad > BLOCK_SIZE)
        {
            pad = 0;
        }

        //Array.Copy()
        var output = new byte[decrypted.Length - pad];
        Array.Copy(decrypted, output, decrypted.Length - pad);
        return output;
    }

    private static char Chr(int a)
    {
        byte target = (byte)(a & 255);
        return (char)target;
    }
}

/// <summary>
/// 加解密工具类
/// </summary>
public class Utils
{
    /// <summary>
    /// 返回随机字符串
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string GetRandomStr(int count)
    {
        string baset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < count; i++)
        {
            int number = Random.Shared.Next(baset.Length);
            sb.Append(baset[number]);
        }

        return sb.ToString();
    }

    /// <summary>
    /// int转byte数组,高位在前
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static byte[] Int2Bytes(int count)
    {
        byte[] byteArr = new byte[4];
        byteArr[3] = (byte)(count & 255);
        byteArr[2] = (byte)(count >> 8 & 255);
        byteArr[1] = (byte)(count >> 16 & 255);
        byteArr[0] = (byte)(count >> 24 & 255);
        return byteArr;
    }

    /// <summary>
    /// 高位在前bytes数组转int
    /// </summary>
    /// <param name="byteArr"></param>
    /// <returns></returns>
    public static int Bytes2Int(byte[] byteArr)
    {
        int count = 0;
        for (int i = 0; i < 4; ++i)
        {
            count <<= 8;
            count |= byteArr[i] & 255;
        }

        return count;
    }
}