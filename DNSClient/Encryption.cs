using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyDnsClient
{
    class Encryption
    {
        public static byte[] Decrypt(byte[] input)
        {
            PasswordDeriveBytes pdb =
              new PasswordDeriveBytes("hjiweykaksd", // Change this
              new byte[] { 0x43, 0x87, 0x23, 0x72 }); // Change this
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms,
              aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }

        public static byte[] Encrypt(string str)
        {
            byte[] input = Encoding.UTF8.GetBytes(str);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes("hjiweykaksd", new byte[] { 0x43, 0x87, 0x23, 0x72 }); // Change this
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// 将一个byte数组转换成16进制字符串
        /// </summary>
        /// <param name="data">byte数组</param>
        /// <returns>格式化的16进制字符串</returns>
        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将16进制字符串转换成byte数组
        /// </summary>
        /// <param name="hexString">16进制字符串</param>
        /// <returns>byte数组</returns>
        public static byte[] HexStringToByteArray(string hexString)
        {
            //将16进制秘钥转成字节数组
            var byteArray = new byte[hexString.Length / 2];
            for (var x = 0; x < byteArray.Length; x++)
            {
                var i = Convert.ToInt32(hexString.Substring(x * 2, 2), 16);
                byteArray[x] = (byte)i;
            }
            return byteArray;
        }
        /// <summary>
        /// 以每段最长63位切割数据
        /// </summary>
        /// <param name="SourceString"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static List<string> SplitLength(string SourceString)
        {
            List<string> list = new List<string>();
            //ArrayList list = new ArrayList();
            for (int i = 0; i < SourceString.Trim().Length; i += 50)
            {
                if ((SourceString.Trim().Length - i) >= 50)
                    list.Add(SourceString.Trim().Substring(i, 50));
                else
                    list.Add(SourceString.Trim().Substring(i, SourceString.Trim().Length - i));
            }
            return list;
        }
    }
}
