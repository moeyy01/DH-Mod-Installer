using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DreadHunger
{


    public class AesEncryption
    {
        /// <summary>
        /// 加密算法
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <returns></returns>
        public static string EncryptString(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // 设置密钥和初始化向量  
                aesAlg.Key = Encoding.UTF8.GetBytes("dhcmallkey1234551298765134567890");
                aesAlg.IV = Encoding.UTF8.GetBytes("dhcmallkey123456");

                // 创建加密器  
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // 将明文转换为字节数组  
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                // 使用加密器将明文加密为密文  
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);
                        csEncrypt.Close();
                    }

                    // 返回Base64编码的密文字符串  
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
                     
        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="cipherText">密文</param>
        /// <returns></returns>
        public static string DecryptString(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // 设置密钥和初始化向量  
                aesAlg.Key = Encoding.UTF8.GetBytes("dhcmallkey1234551298765134567890");
                aesAlg.IV = Encoding.UTF8.GetBytes("dhcmallkey123456");

                // 创建解密器  
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // 将密文从Base64解码为字节数组  
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

                // 使用解密器将密文解密为明文  
                using (MemoryStream msDecrypt = new MemoryStream(cipherTextBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        } 

    }
}


