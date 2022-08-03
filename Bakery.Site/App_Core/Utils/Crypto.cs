using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Bakery.Utils
{
    /// <summary>
    /// 不可逆加密辅助类
    /// </summary>
    public static class HASHCrypto
    {
        public enum CryptoEnum
        {
            MD5 = 32,
            SHA1 = 40,
            SHA256 = 64,
            SHA384 = 96,
            SHA512 = 128
        }

        /// <summary>
        /// 创建不可逆加密类
        /// </summary>
        /// <param name="p_CryptoEnum">加密类型</param>
        /// <returns>返回加密类HashAlgorithm</returns>
        public static HashAlgorithm Generate(CryptoEnum p_CryptoEnum = CryptoEnum.SHA1)
        {
            return Generate(p_CryptoEnum, new byte[0]);
        }
        
        /// <summary>
        /// 创建不可逆加密类
        /// </summary>
        /// <param name="p_CryptoEnum">加密类型</param>
        /// <param name="p_SecretKey">密钥</param>
        /// <returns>返回加密类HashAlgorithm</returns>
        public static HashAlgorithm Generate(CryptoEnum p_CryptoEnum = CryptoEnum.SHA1, string p_SecretKey = "")
        {
            byte[] secretBuff = null;
            if (!string.IsNullOrEmpty(p_SecretKey))
            {
                secretBuff = Encoding.UTF8.GetBytes(p_SecretKey);
            }

            return Generate(p_CryptoEnum, secretBuff);
        }

        /// <summary>
        /// 创建不可逆加密类
        /// </summary>
        /// <param name="p_CryptoEnum">加密类型</param>
        /// <param name="p_SecretBuff">密钥</param>
        /// <returns>返回加密类HashAlgorithm</returns>
        public static HashAlgorithm Generate(CryptoEnum p_CryptoEnum = CryptoEnum.SHA1, byte[] p_SecretBuff = null)
        {
            HashAlgorithm ha = null;
            if (p_SecretBuff != null && p_SecretBuff.Length > 0)
            {
                switch (p_CryptoEnum)
                {
                    case CryptoEnum.MD5:
                        ha = new HMACMD5()
                        {
                            Key = p_SecretBuff
                        };
                        break;

                    case CryptoEnum.SHA1:
                        ha = new HMACSHA1()
                        {
                            Key = p_SecretBuff
                        };
                        break;

                    case CryptoEnum.SHA256:
                        ha = new HMACSHA256()
                        {
                            Key = p_SecretBuff
                        };
                        break;

                    case CryptoEnum.SHA384:
                        ha = new HMACSHA384()
                        {
                            Key = p_SecretBuff
                        };
                        break;

                    case CryptoEnum.SHA512:
                        ha = new HMACSHA512()
                        {
                            Key = p_SecretBuff
                        };
                        break;
                }
            }
            else
            {
                switch (p_CryptoEnum)
                {
                    case CryptoEnum.MD5:
                        ha = MD5.Create(); // new MD5CryptoServiceProvider();
                        break;

                    case CryptoEnum.SHA1:
                        ha = SHA1.Create(); // new SHA1CryptoServiceProvider();
                        break;

                    case CryptoEnum.SHA256:
                        ha = SHA256.Create(); //new SHA256CryptoServiceProvider();
                        break;

                    case CryptoEnum.SHA384:
                        ha = SHA384.Create(); //new SHA384CryptoServiceProvider();
                        break;

                    case CryptoEnum.SHA512:
                        ha = SHA512.Create(); // new SHA512CryptoServiceProvider();
                        break;
                }
            }

            return ha;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="p_Provider">加密类HashAlgorithm</param>
        /// <param name="p_InputText">待加密字符串</param>
        /// <returns>返回大写字符串</returns>
        public static string Encrypt(this HashAlgorithm p_Provider, string p_InputText)
        {
            using (p_Provider)
            {
                return Encrypt(p_Provider, Encoding.UTF8.GetBytes(p_InputText));
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="p_Provider">加密类HashAlgorithm</param>
        /// <param name="p_InputBuff">待加密字节数组</param>
        /// <returns>返回大写字符串</returns>
        public static string Encrypt(this HashAlgorithm p_Provider, byte[] p_InputBuff)
        {
            using (p_Provider)
            {
                var buff = Encrypt2Byte(p_Provider, p_InputBuff);
                return BitConverter.ToString(buff).Replace("-", "");
            }
        }

        /// <summary>
        ///加密
        /// </summary>
        /// <param name="p_Provider">加密类HashAlgorithm</param>
        /// <param name="p_InputText">待加密字符串</param>
        /// <returns>返回字节数组</returns>
        public static byte[] Encrypt2Byte(this HashAlgorithm p_Provider, string p_InputText)
        {
            using (p_Provider)
            {
                return Encrypt2Byte(p_Provider, Encoding.UTF8.GetBytes(p_InputText));
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="p_Provider">加密类HashAlgorithm</param>
        /// <param name="p_InputBuff">待加密字节数组</param>
        /// <returns>返回字节数组</returns>
        public static byte[] Encrypt2Byte(this HashAlgorithm p_Provider, byte[] p_InputBuff)
        {
            using (p_Provider)
            {
                var res = string.Empty;
                if (p_InputBuff != null)
                {
                    return p_Provider.ComputeHash(p_InputBuff);
                }

                return new byte[0];
            }
        }

        ///// <summary>
        ///// 加密
        ///// </summary>
        ///// <param name="p_InputText">待加密字符串</param>
        ///// <param name="p_CryptoEnum">加密类类型 DEncryptEnum</param>
        ///// <param name="p_SecretKey">加密盐值密钥</param>
        ///// <returns>返回大写字符串</returns>
        //public static string Encrypt(string p_InputText, CryptoEnum p_CryptoEnum, string p_SecretKey = "")
        //{
        //    return Generate(p_CryptoEnum, p_SecretKey).Encrypt(p_InputText);
        //}

        ///// <summary>
        /////加密
        ///// </summary>
        ///// <param name="p_InputText">待加密字符串</param>
        ///// <param name="p_CryptoEnum">加密类类型 DEncryptEnum</param>
        ///// <param name="p_SecretKey">加密盐值密钥</param>
        ///// <returns>返回字节数组</returns>
        //public static byte[] Encrypt2Byte(string p_InputText, CryptoEnum p_CryptoEnum, string p_SecretKey = "")
        //{
        //    return Generate(p_CryptoEnum, p_SecretKey).Encrypt2Byte(Encoding.UTF8.GetBytes(p_InputText));
        //}

        ///// <summary>
        ///// 加密
        ///// </summary>
        ///// <param name="p_InputBuff">待加密字节数组</param>
        ///// <param name="p_CryptoEnum">加密类类型 DEncryptEnum</param>
        ///// <param name="p_SecretKey">加密盐值密钥</param>
        ///// <returns>返回大写字符串</returns>
        //public static string Encrypt(byte[] p_InputBuff, CryptoEnum p_CryptoEnum, string p_SecretKey = "")
        //{
        //    return Generate(p_CryptoEnum, p_SecretKey).Encrypt(p_InputBuff);
        //}

        ///// <summary>
        ///// 加密
        ///// </summary>
        ///// <param name="p_InputBuff">待加密字节数组</param>
        ///// <param name="p_CryptoEnum">加密类类型 DEncryptEnum</param>
        ///// <param name="p_SecretKey">加密盐值密钥</param>
        ///// <returns>返回字节数组</returns>
        //public static byte[] Encrypt2Byte(byte[] p_InputBuff, CryptoEnum p_CryptoEnum, string p_SecretKey = "")
        //{
        //    return Generate(p_CryptoEnum, p_SecretKey).Encrypt2Byte(p_InputBuff);
        //}
    }

    /// <summary>
    /// 对称加密辅助类
    /// </summary>
    public static class DESCrypto
    {
        public enum CryptoEnum
        {
            AES,
            DES,
            TripleDES,
            RC2,
            Rijndael,
        }

        //private const int m_BufferSize = 1024;

        /// <summary>
        /// 简易生成密钥和初始化向量
        /// </summary>
        /// <param name="p_Key">任意字符串</param>
        /// <param name="p_CryptoEnum">加密类型</param>
        /// <returns>返回KeyValuePair,其中Key = Key,Value = IV</returns>
        public static KeyValuePair<string, string> KeyIV(string p_KeyIV, CryptoEnum p_CryptoEnum = CryptoEnum.AES)
        {
            var m_Key = HASHCrypto.Generate(HASHCrypto.CryptoEnum.MD5).Encrypt(p_KeyIV).ToLowerInvariant();
            var res = new KeyValuePair<string, string>(m_Key.Substring(0, 8), m_Key.Substring(24, 8));
            switch (p_CryptoEnum)
            {
                case CryptoEnum.AES:
                {
                    res = new KeyValuePair<string, string>(m_Key.Substring(0, 16), m_Key.Substring(16, 16));
                }
                    break;

                case CryptoEnum.DES:
                {
                    res = new KeyValuePair<string, string>(m_Key.Substring(0, 24), m_Key.Substring(24, 8));
                }
                    break;

                case CryptoEnum.TripleDES:
                {
                    res = new KeyValuePair<string, string>(m_Key.Substring(0, 24), m_Key.Substring(24, 8));
                }
                    break;

                case CryptoEnum.RC2:
                {
                    res = new KeyValuePair<string, string>(m_Key.Substring(0, 16), m_Key.Substring(16, 8));
                }
                    break;

                case CryptoEnum.Rijndael:
                {
                    res = new KeyValuePair<string, string>(m_Key, m_Key.Substring(16, 16));
                }
                    break;
            }

            return res;
        }

        //aes加解密
        //http://outofmemory.cn/code-snippet/35524/AES-with-javascript-java-csharp-python-or-php
        //http://blog.csdn.net/flysknow/article/details/324748?locationNum=2&fps=1
        //http://www.sufeinet.com/thread-12099-1-1.html
        public static SymmetricAlgorithm Generate(string p_KeyIV,
            CryptoEnum p_CryptoEnum = CryptoEnum.AES,
            CipherMode p_CipherMode = CipherMode.CBC, PaddingMode p_PaddingMode = PaddingMode.PKCS7,
            int p_BlockSize = 0, int p_KeySize = 0)
        {
            var kv = KeyIV(p_KeyIV, p_CryptoEnum);
            return Generate(kv.Key, kv.Value, p_CryptoEnum, p_CipherMode, p_PaddingMode, p_BlockSize, p_KeySize);
        }

        /// <summary>
        /// 创建对称加密类
        /// </summary>
        /// <param name="p_Key">密钥</param>
        /// <param name="p_IV">初始化向量</param>
        /// <param name="p_CryptoEnum">加密类型</param>
        /// <param name="p_CipherMode">块密码模式</param>
        /// <returns>返回加密类SymmetricAlgorithm</returns>
        public static SymmetricAlgorithm Generate(string p_Key, string p_IV,
            CryptoEnum p_CryptoEnum = CryptoEnum.AES,
            CipherMode p_CipherMode = CipherMode.CBC, PaddingMode p_PaddingMode = PaddingMode.PKCS7,
            int p_BlockSize = 0, int p_KeySize = 0)
        {
            SymmetricAlgorithm provider = null;
            switch (p_CryptoEnum)
            {
                case CryptoEnum.AES:
                    provider = Aes.Create();
                    break;

                case CryptoEnum.DES:
                    provider = DES.Create();
                    break;

                case CryptoEnum.TripleDES:
                    provider = TripleDES.Create();
                    break;

                case CryptoEnum.RC2:
                    provider = RC2.Create();
                    break;

                case CryptoEnum.Rijndael:
                    provider = Rijndael.Create();
                    break;

                //default:
                //    provider = System.Security.Cryptography.DSA.Create();
                //    break;
            }

            provider.Mode = p_CipherMode;
            provider.Padding = p_PaddingMode;
            if (p_BlockSize != 0)
            {
                provider.BlockSize = p_BlockSize;
            }

            if (p_KeySize != 0)
            {
                provider.KeySize = p_KeySize;
            }

            switch (p_CryptoEnum)
            {
                case CryptoEnum.AES:
                {
                    if (!string.IsNullOrWhiteSpace(p_Key) && (Encoding.UTF8.GetByteCount(p_Key) == 16 ||
                                                              Encoding.UTF8.GetByteCount(p_Key) == 24 ||
                                                              Encoding.UTF8.GetByteCount(p_Key) == 32))
                    {
                        provider.Key = Encoding.UTF8.GetBytes(p_Key);
                    }

                    if (p_CipherMode != CipherMode.ECB)
                    {
                        if (!string.IsNullOrWhiteSpace(p_IV) && Encoding.UTF8.GetByteCount(p_IV) == 16)
                        {
                            provider.IV = Encoding.UTF8.GetBytes(p_IV);
                        }
                        else
                        {
                            provider.GenerateIV();
                        }
                    }
                }
                    break;

                //todo 未测试
                case CryptoEnum.DES:
                {
                    if (!string.IsNullOrWhiteSpace(p_Key) && (Encoding.UTF8.GetByteCount(p_Key) == 16 ||
                                                              Encoding.UTF8.GetByteCount(p_Key) == 24))
                    {
                        provider.Key = Encoding.UTF8.GetBytes(p_Key);
                    }

                    if (p_CipherMode != CipherMode.ECB)
                    {
                        if (!string.IsNullOrWhiteSpace(p_IV) && Encoding.UTF8.GetByteCount(p_IV) == 8)
                        {
                            provider.IV = Encoding.UTF8.GetBytes(p_IV);
                        }
                        else
                        {
                            provider.GenerateIV();
                        }
                    }
                }
                    break;

                case CryptoEnum.TripleDES:
                {
                    if (!string.IsNullOrWhiteSpace(p_Key) && (Encoding.UTF8.GetByteCount(p_Key) == 16 ||
                                                              Encoding.UTF8.GetByteCount(p_Key) == 24))
                    {
                        provider.Key = Encoding.UTF8.GetBytes(p_Key);
                    }

                    if (p_CipherMode != CipherMode.ECB)
                    {
                        if (!string.IsNullOrWhiteSpace(p_IV) && Encoding.UTF8.GetByteCount(p_IV) == 8)
                        {
                            provider.IV = Encoding.UTF8.GetBytes(p_IV);
                        }
                        else
                        {
                            provider.GenerateIV();
                        }
                    }
                }
                    break;

                case CryptoEnum.RC2:
                {
                    if (!string.IsNullOrWhiteSpace(p_Key) && (Encoding.UTF8.GetByteCount(p_Key) >= 5 &&
                                                              Encoding.UTF8.GetByteCount(p_Key) <= 16))
                    {
                        provider.Key = Encoding.UTF8.GetBytes(p_Key);
                    }

                    if (p_CipherMode != CipherMode.ECB)
                    {
                        if (!string.IsNullOrWhiteSpace(p_IV) && Encoding.UTF8.GetByteCount(p_IV) == 8)
                        {
                            provider.IV = Encoding.UTF8.GetBytes(p_IV);
                        }
                        else
                        {
                            provider.GenerateIV();
                        }
                    }
                }
                    break;

                case CryptoEnum.Rijndael:
                {
                    if (!string.IsNullOrWhiteSpace(p_Key) && (Encoding.UTF8.GetByteCount(p_Key) == 16 ||
                                                              Encoding.UTF8.GetByteCount(p_Key) == 24 ||
                                                              Encoding.UTF8.GetByteCount(p_Key) == 32))
                    {
                        provider.Key = Encoding.UTF8.GetBytes(p_Key);
                    }

                    if (p_CipherMode != CipherMode.ECB)
                    {
                        if (!string.IsNullOrWhiteSpace(p_IV) && Encoding.UTF8.GetByteCount(p_IV) == 16)
                        {
                            provider.IV = Encoding.UTF8.GetBytes(p_IV);
                        }
                        else
                        {
                            provider.GenerateIV();
                        }
                    }
                }
                    break;
            }

            return provider;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="p_Provider">加密类SymmetricAlgorithm</param>
        /// <param name="p_InputText">待加密字符串</param>
        /// <returns>返回字节数组</returns>
        public static byte[] Encrypt2Byte(this SymmetricAlgorithm p_Provider, string p_InputText)
        {
            using (p_Provider)
            {
                if (p_InputText != null)
                {
                    return Encrypt2Byte(p_Provider, Encoding.UTF8.GetBytes(p_InputText));
                }

                return null;
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="p_Provider">加密类SymmetricAlgorithm</param>
        /// <param name="p_InputBuff">待加密字节数组</param>
        /// <returns>返回字节数组</returns>
        public static byte[] Encrypt2Byte(this SymmetricAlgorithm p_Provider, byte[] p_InputBuff)
        {
            using (p_Provider)
            {
                if (p_InputBuff != null)
                {
                    ICryptoTransform transform = p_Provider.CreateEncryptor();
                    return transform.TransformFinalBlock(p_InputBuff, 0, p_InputBuff.Length);
                }

                return null;
            }
        }

        public static string Encrypt(this SymmetricAlgorithm p_Provider, string p_InputText)
        {
            using (p_Provider)
            {
                if (p_InputText != null)
                {
                    return Encrypt(p_Provider, Encoding.UTF8.GetBytes(p_InputText));
                }

                return null;
            }
        }

        public static string Encrypt(this SymmetricAlgorithm p_Provider, byte[] p_InputBuff)
        {
            using (p_Provider)
            {
                if (p_InputBuff != null)
                {
                    return Convert.ToBase64String(Encrypt2Byte(p_Provider, p_InputBuff));
                }

                return null;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="p_Provider">加密类SymmetricAlgorithm</param>
        /// <param name="p_InputText">待加密字符串</param>
        /// <returns>返回字节数组</returns>
        public static byte[] Decrypt2Byte(this SymmetricAlgorithm p_Provider, string p_InputText)
        {
            using (p_Provider)
            {
                if (p_InputText != null)
                {
                    return Decrypt2Byte(p_Provider, Convert.FromBase64String(p_InputText));
                }

                return null;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="p_Provider">加密类SymmetricAlgorithm</param>
        /// <param name="p_InputBuff">待解密字节数组</param>
        /// <returns>返回字节数组</returns>
        public static byte[] Decrypt2Byte(this SymmetricAlgorithm p_Provider, byte[] p_InputBuff)
        {
            using (p_Provider)
            {
                if (p_InputBuff != null)
                {
                    ICryptoTransform transform = p_Provider.CreateDecryptor();
                    return transform.TransformFinalBlock(p_InputBuff, 0, p_InputBuff.Length);
                }

                return null;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="p_Provider">加密类SymmetricAlgorithm</param>
        /// <param name="p_InputText">待加密字符串</param>
        /// <returns>返回字节数组</returns>
        public static string Decrypt(this SymmetricAlgorithm p_Provider, string p_InputText)
        {
            using (p_Provider)
            {
                if (p_InputText != null)
                {
                    return Decrypt(p_Provider, Convert.FromBase64String(p_InputText));
                }

                return null;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="p_Provider">加密类SymmetricAlgorithm</param>
        /// <param name="p_InputBuff">待解密字节数组</param>
        /// <returns>返回字节数组</returns>
        public static string Decrypt(this SymmetricAlgorithm p_Provider, byte[] p_InputBuff)
        {
            using (p_Provider)
            {
                if (p_InputBuff != null)
                {
                    return Encoding.UTF8.GetString(Decrypt2Byte(p_Provider, p_InputBuff));
                }

                return null;
            }
        }
    }

    /// <summary>
    /// 非对称加密辅助类（公钥私钥）
    /// </summary>
    public static class RSACrypto
    {
        private class Extensions
        {
            static public Regex _PEMCode = new Regex(@"--+.+?--+|\s+");

            static public byte[] _SeqOID = new byte[]
                {0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00};

            static public byte[] _Ver = new byte[] {0x02, 0x01, 0x00};

            /// <summary>
            /// 把字符串按每行多少个字断行
            /// </summary>
            static public string TextBreak(string text, int line)
            {
                var idx = 0;
                var len = text.Length;
                var str = new StringBuilder();
                while (idx < len)
                {
                    if (idx > 0)
                    {
                        str.Append('\n');
                    }

                    if (idx + line >= len)
                    {
                        str.Append(text.Substring(idx));
                    }
                    else
                    {
                        str.Append(text.Substring(idx, line));
                    }

                    idx += line;
                }

                return str.ToString();
            }

            /// <summary>
            /// 从数组start开始到指定长度复制一份
            /// </summary>
            static public T[] sub<T>(T[] arr, int start, int count)
            {
                T[] val = new T[count];
                for (var i = 0; i < count; i++)
                {
                    val[i] = arr[start + i];
                }

                return val;
            }

            static public void writeAll(Stream stream, byte[] byts)
            {
                stream.Write(byts, 0, byts.Length);
            }

            static public int GetIntegerSize(BinaryReader binr)
            {
                byte bt = 0;
                byte lowbyte = 0x00;
                byte highbyte = 0x00;
                int count = 0;
                bt = binr.ReadByte();
                if (bt != 0x02) //expect integer
                    return 0;
                bt = binr.ReadByte();

                if (bt == 0x81)
                    count = binr.ReadByte(); // data size in next byte
                else if (bt == 0x82)
                {
                    highbyte = binr.ReadByte(); // data size in next 2 bytes
                    lowbyte = binr.ReadByte();
                    byte[] modint = {lowbyte, highbyte, 0x00, 0x00};
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt; // we already have the data size
                }

                while (binr.ReadByte() == 0x00)
                {
                    //remove high order zeros in data
                    count -= 1;
                }

                binr.BaseStream.Seek(-1, SeekOrigin.Current); //last ReadByte wasn't a removed zero, so back up a byte
                return count;
            }
        }


        /// <summary>
        /// 获取证书ID
        /// </summary>
        /// <param name="Cert"></param>
        /// <returns></returns>
        public static string CertId(this X509Certificate2 Cert)
        {
            return string.IsNullOrWhiteSpace(Cert.SerialNumber)
                ? ""
                : BigInteger.Parse(Cert.SerialNumber, NumberStyles.HexNumber).ToString();
        }

        /// <summary>
        /// 根据路径和密码得到证书
        /// </summary>
        /// <param name="certPath"></param>
        /// <param name="certPwd"></param>
        /// <returns></returns>
        public static X509Certificate2 CertFile(string certPath, string certPwd = "")
        {
            return CertByte(File.ReadAllBytes(certPath), certPwd);
        }

        /// <summary>
        /// 根据字节数组和密码得到证书
        /// </summary>
        /// <param name="certData"></param>
        /// <param name="certPwd"></param>
        /// <returns></returns>
        public static X509Certificate2 CertByte(byte[] certData, string certPwd = "")
        {
            return string.IsNullOrWhiteSpace(certPwd)
                ? new X509Certificate2(certData)
                : new X509Certificate2(certData, certPwd, X509KeyStorageFlags.Exportable);
        }

        public static RSACryptoServiceProvider Cert2Provider(this X509Certificate2 cert,
            bool includePrivateParameters = false)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.PersistKeyInCsp = false;
            if (includePrivateParameters)
            {
                if (cert.HasPrivateKey)
                {
                    RSAParameters key =
                        ((RSACryptoServiceProvider) cert.PrivateKey).ExportParameters(true);
                    provider.ImportParameters(key);
                    //provider.FromXmlString(cert.PrivateKey.ToXmlString(true));
                }
                else
                {
                    provider = null;
                }
            }
            else
            {
                RSAParameters key =
                    ((RSACryptoServiceProvider) cert.PublicKey.Key).ExportParameters(false);
                provider.ImportParameters(key);
                //provider.FromXmlString(cert.PublicKey.Key.ToXmlString(false));
            }

            return provider;
        }

        /// <summary>
        /// RSACryptoServiceProvider
        /// 如果安装了 Microsoft 增强的加密提供程序, 则支持从384位到16384位的密钥大小 
        /// 如果安装了 Microsoft 基本加密提供程序, 则它支持从384位到512位的密钥大小
        /// </summary>
        /// <param name="keySize">384位 - 512位 (增量8位)</param>
        /// <returns></returns>
        public static RSACryptoServiceProvider Generate(int keySize)
        {
            var rsaParams = new CspParameters();
            rsaParams.Flags = CspProviderFlags.UseMachineKeyStore;
            return new RSACryptoServiceProvider(keySize, rsaParams);
        }

        public static RSACryptoServiceProvider FromXmlKey(this string xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            if (xmlDoc.DocumentElement != null && xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                RSAParameters parameters = new RSAParameters();
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus":
                            parameters.Modulus = Convert.FromBase64String(node.InnerText);
                            break;

                        case "Exponent":
                            parameters.Exponent = Convert.FromBase64String(node.InnerText);
                            break;

                        case "P":
                            parameters.P = Convert.FromBase64String(node.InnerText);
                            break;

                        case "Q":
                            parameters.Q = Convert.FromBase64String(node.InnerText);
                            break;

                        case "DP":
                            parameters.DP = Convert.FromBase64String(node.InnerText);
                            break;

                        case "DQ":
                            parameters.DQ = Convert.FromBase64String(node.InnerText);
                            break;

                        case "InverseQ":
                            parameters.InverseQ = Convert.FromBase64String(node.InnerText);
                            break;

                        case "D":
                            parameters.D = Convert.FromBase64String(node.InnerText);
                            break;
                    }
                }

                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                provider.PersistKeyInCsp = false;
                provider.ImportParameters(parameters);

                return provider;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_Provider"></param>
        /// <param name="convertToPublic">convertToPublic=true含私钥的RSA将只返回公钥，仅含公钥的RSA不受影响</param>
        /// <returns></returns>
        public static string ToXmlKey(this RSACryptoServiceProvider p_Provider, bool convertToPublic = false)
        {
            if (!convertToPublic)
            {
                if (!p_Provider.PublicOnly)
                {
                    RSAParameters parameters = p_Provider.ExportParameters(true);
                    return string.Format(
                        "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                        Convert.ToBase64String(parameters.Modulus),
                        Convert.ToBase64String(parameters.Exponent),
                        Convert.ToBase64String(parameters.P),
                        Convert.ToBase64String(parameters.Q),
                        Convert.ToBase64String(parameters.DP),
                        Convert.ToBase64String(parameters.DQ),
                        Convert.ToBase64String(parameters.InverseQ),
                        Convert.ToBase64String(parameters.D));
                }
                else
                {
                    RSAParameters parameters = p_Provider.ExportParameters(false);
                    return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                        Convert.ToBase64String(parameters.Modulus),
                        Convert.ToBase64String(parameters.Exponent));
                }
            }
            else
            {
                RSAParameters parameters = p_Provider.ExportParameters(false);
                return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                    Convert.ToBase64String(parameters.Modulus),
                    Convert.ToBase64String(parameters.Exponent));
            }
        }

        /// <summary>
        /// 用PEM格式密钥对创建RSA，支持PKCS#1、PKCS#8格式的PEM
        /// </summary>
        public static RSACryptoServiceProvider FromPEM(string pem)
        {
            var rsaParams = new CspParameters();
            rsaParams.Flags = CspProviderFlags.UseMachineKeyStore;
            var rsa = new RSACryptoServiceProvider(rsaParams);

            var param = new RSAParameters();

            var base64 = Extensions._PEMCode.Replace(pem, "");
            var data = Convert.FromBase64String(base64);
            if (data == null)
            {
                throw new Exception("PEM内容无效");
            }

            var idx = 0;

            //读取长度
            Func<byte, int> readLen = (first) =>
            {
                if (data[idx] == first)
                {
                    idx++;
                    if (data[idx] == 0x81)
                    {
                        idx++;
                        return data[idx++];
                    }
                    else if (data[idx] == 0x82)
                    {
                        idx++;
                        return (((int) data[idx++]) << 8) + data[idx++];
                    }
                    else if (data[idx] < 0x80)
                    {
                        return data[idx++];
                    }
                }

                throw new Exception("PEM未能提取到数据");
            };
            //读取块数据
            Func<byte[]> readBlock = () =>
            {
                var len = readLen(0x02);
                if (data[idx] == 0x00)
                {
                    idx++;
                    len--;
                }

                var val = Extensions.sub(data, idx, len);
                idx += len;
                return val;
            };
            //比较data从idx位置开始是否是byts内容
            Func<byte[], bool> eq = (byts) =>
            {
                for (var i = 0; i < byts.Length; i++, idx++)
                {
                    if (idx >= data.Length)
                    {
                        return false;
                    }

                    if (byts[i] != data[idx])
                    {
                        return false;
                    }
                }

                return true;
            };


            if (pem.Contains("PUBLIC KEY"))
            {
                /****使用公钥****/
                //读取数据总长度
                readLen(0x30);
                if (!eq(Extensions._SeqOID))
                {
                    throw new Exception("PEM未知格式");
                }

                //读取1长度
                readLen(0x03);
                idx++; //跳过0x00
                //读取2长度
                readLen(0x30);

                //Modulus
                param.Modulus = readBlock();

                //Exponent
                param.Exponent = readBlock();
            }
            else if (pem.Contains("PRIVATE KEY"))
            {
                /****使用私钥****/
                //读取数据总长度
                readLen(0x30);

                //读取版本号
                if (!eq(Extensions._Ver))
                {
                    throw new Exception("PEM未知版本");
                }

                //检测PKCS8
                var idx2 = idx;
                if (eq(Extensions._SeqOID))
                {
                    //读取1长度
                    readLen(0x04);
                    //读取2长度
                    readLen(0x30);

                    //读取版本号
                    if (!eq(Extensions._Ver))
                    {
                        throw new Exception("PEM版本无效");
                    }
                }
                else
                {
                    idx = idx2;
                }

                //读取数据
                param.Modulus = readBlock();
                param.Exponent = readBlock();
                param.D = readBlock();
                param.P = readBlock();
                param.Q = readBlock();
                param.DP = readBlock();
                param.DQ = readBlock();
                param.InverseQ = readBlock();
            }
            else
            {
                throw new Exception("pem需要BEGIN END标头");
            }

            rsa.ImportParameters(param);
            return rsa;
        }

        /// <summary>
        /// 将RSA中的密钥对转换成PEM格式
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="convertToPublic">convertToPublic=true含私钥的RSA将只返回公钥，仅含公钥的RSA不受影响</param>
        /// <param name="usePKCS8">usePKCS8=false时返回PKCS#1格式，否则返回PKCS#8格式</param>
        /// <returns></returns>
        public static string ToPEM(this RSACryptoServiceProvider rsa, bool convertToPublic = false,
            bool usePKCS8 = false)
        {
            //https://www.jianshu.com/p/25803dd9527d
            //https://www.cnblogs.com/ylz8401/p/8443819.html
            //https://blog.csdn.net/jiayanhui2877/article/details/47187077
            //https://blog.csdn.net/xuanshao_/article/details/51679824
            //https://blog.csdn.net/xuanshao_/article/details/51672547

            var ms = new MemoryStream();
            //写入一个长度字节码
            Action<int> writeLenByte = (len) =>
            {
                if (len < 0x80)
                {
                    ms.WriteByte((byte) len);
                }
                else if (len <= 0xff)
                {
                    ms.WriteByte(0x81);
                    ms.WriteByte((byte) len);
                }
                else
                {
                    ms.WriteByte(0x82);
                    ms.WriteByte((byte) (len >> 8 & 0xff));
                    ms.WriteByte((byte) (len & 0xff));
                }
            };
            //写入一块数据
            Action<byte[]> writeBlock = (byts) =>
            {
                var addZero = (byts[0] >> 4) >= 0x8;
                ms.WriteByte(0x02);
                var len = byts.Length + (addZero ? 1 : 0);
                writeLenByte(len);

                if (addZero)
                {
                    ms.WriteByte(0x00);
                }

                ms.Write(byts, 0, byts.Length);
            };
            //根据后续内容长度写入长度数据
            Func<int, byte[], byte[]> writeLen = (index, byts) =>
            {
                var len = byts.Length - index;

                ms.SetLength(0);
                ms.Write(byts, 0, index);
                writeLenByte(len);
                ms.Write(byts, index, len);

                return ms.ToArray();
            };


            if (rsa.PublicOnly || convertToPublic)
            {
                /****生成公钥****/
                var param = rsa.ExportParameters(false);


                //写入总字节数，不含本段长度，额外需要24字节的头，后续计算好填入
                ms.WriteByte(0x30);
                var index1 = (int) ms.Length;

                //固定内容
                // encoded OID sequence for PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
                Extensions.writeAll(ms, Extensions._SeqOID);

                //从0x00开始的后续长度
                ms.WriteByte(0x03);
                var index2 = (int) ms.Length;
                ms.WriteByte(0x00);

                //后续内容长度
                ms.WriteByte(0x30);
                var index3 = (int) ms.Length;

                //写入Modulus
                writeBlock(param.Modulus);

                //写入Exponent
                writeBlock(param.Exponent);


                //计算空缺的长度
                var byts = ms.ToArray();

                byts = writeLen(index3, byts);
                byts = writeLen(index2, byts);
                byts = writeLen(index1, byts);


                return "-----BEGIN PUBLIC KEY-----\n" + Extensions.TextBreak(Convert.ToBase64String(byts), 64) +
                       "\n-----END PUBLIC KEY-----";
            }
            else
            {
                /****生成私钥****/
                var param = rsa.ExportParameters(true);

                //写入总字节数，后续写入
                ms.WriteByte(0x30);
                int index1 = (int) ms.Length;

                //写入版本号
                Extensions.writeAll(ms, Extensions._Ver);

                //PKCS8 多一段数据
                int index2 = -1, index3 = -1;
                if (usePKCS8)
                {
                    //固定内容
                    Extensions.writeAll(ms, Extensions._SeqOID);

                    //后续内容长度
                    ms.WriteByte(0x04);
                    index2 = (int) ms.Length;

                    //后续内容长度
                    ms.WriteByte(0x30);
                    index3 = (int) ms.Length;

                    //写入版本号
                    Extensions.writeAll(ms, Extensions._Ver);
                }

                //写入数据
                writeBlock(param.Modulus);
                writeBlock(param.Exponent);
                writeBlock(param.D);
                writeBlock(param.P);
                writeBlock(param.Q);
                writeBlock(param.DP);
                writeBlock(param.DQ);
                writeBlock(param.InverseQ);


                //计算空缺的长度
                var byts = ms.ToArray();

                if (index2 != -1)
                {
                    byts = writeLen(index3, byts);
                    byts = writeLen(index2, byts);
                }

                byts = writeLen(index1, byts);


                var flag = " PRIVATE KEY";
                if (!usePKCS8)
                {
                    flag = " RSA" + flag;
                }

                return "-----BEGIN" + flag + "-----\n" + Extensions.TextBreak(Convert.ToBase64String(byts), 64) +
                       "\n-----END" + flag + "-----";
            }
        }

        #region 暂时弃用

        //public static string Private2PublicKey(this RSACryptoServiceProvider p_Provider)
        //{
        //    if (!p_Provider.PublicOnly)
        //    {
        //        return Private2PublicKey(p_Provider.ToXmlKey(false));
        //    }
        //    return null;
        //}

        //public static string Private2PublicKey(this string p_PrivateXmlKey)
        //{
        //    var index = p_PrivateXmlKey.IndexOf("</Exponent><P>");
        //    if (index != -1)
        //    {
        //        var publicxmlkey = p_PrivateXmlKey.Substring(0, index);
        //        return publicxmlkey + "</Exponent></RSAKeyValue>";
        //    }
        //    return null;
        //}

        //public static RSACryptoServiceProvider FromRSAPrivatePemKey(this string pemString)
        //{
        //    pemString = pemString.Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "").Trim();

        //    byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

        //    // --------- Set up stream to decode the asn.1 encoded RSA private key ------
        //    MemoryStream mem = new MemoryStream(Convert.FromBase64String(pemString));
        //    BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
        //    byte bt = 0;
        //    ushort twobytes = 0;
        //    int elems = 0;
        //    try
        //    {
        //        twobytes = binr.ReadUInt16();
        //        if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
        //            binr.ReadByte();    //advance 1 byte
        //        else if (twobytes == 0x8230)
        //            binr.ReadInt16();    //advance 2 bytes
        //        else
        //            return null;

        //        twobytes = binr.ReadUInt16();
        //        if (twobytes != 0x0102) //version number
        //            return null;
        //        bt = binr.ReadByte();
        //        if (bt != 0x00)
        //            return null;

        //        //------ all private key components are Integer sequences ----
        //        elems = Extensions.GetIntegerSize(binr);
        //        MODULUS = binr.ReadBytes(elems);

        //        elems = Extensions.GetIntegerSize(binr);
        //        E = binr.ReadBytes(elems);

        //        elems = Extensions.GetIntegerSize(binr);
        //        D = binr.ReadBytes(elems);

        //        elems = Extensions.GetIntegerSize(binr);
        //        P = binr.ReadBytes(elems);

        //        elems = Extensions.GetIntegerSize(binr);
        //        Q = binr.ReadBytes(elems);

        //        elems = Extensions.GetIntegerSize(binr);
        //        DP = binr.ReadBytes(elems);

        //        elems = Extensions.GetIntegerSize(binr);
        //        DQ = binr.ReadBytes(elems);

        //        elems = Extensions.GetIntegerSize(binr);
        //        IQ = binr.ReadBytes(elems);

        //        // ------- create RSACryptoServiceProvider instance and initialize with public key -----
        //        CspParameters CspParameters = new CspParameters();
        //        CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;

        //        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(CspParameters);
        //        RSAParameters RSAparams = new RSAParameters();
        //        RSAparams.Modulus = MODULUS;
        //        RSAparams.Exponent = E;
        //        RSAparams.D = D;
        //        RSAparams.P = P;
        //        RSAparams.Q = Q;
        //        RSAparams.DP = DP;
        //        RSAparams.DQ = DQ;
        //        RSAparams.InverseQ = IQ;
        //        RSA.ImportParameters(RSAparams);
        //        return RSA;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        binr.Close();
        //    }
        //}

        //public static RSACryptoServiceProvider FromPublicPemKey(this string pemString)
        //{
        //    RSACryptoServiceProvider rsa = opensslkey.DecodeX509PublicKey(Convert.FromBase64String(pemString));
        //    rsa.PersistKeyInCsp = false;
        //    return rsa;
        //}


        ///// <summary>
        ///// 判断xmlkey是公钥还是私钥
        ///// </summary>
        ///// <param name="xmlkey">xmlkey</param>
        ///// <returns>公钥则为true 私钥则为false</returns>
        //public static bool IsPublic(this string xmlkey)
        //{
        //    if (xmlkey.EndsWith("</Exponent></RSAKeyValue>", StringComparison.OrdinalIgnoreCase))
        //        return true;
        //    if (xmlkey.EndsWith("</D></RSAKeyValue>", StringComparison.OrdinalIgnoreCase))
        //        return false;
        //    return true;
        //}

        #endregion


        /// <summary>
        /// 判断xmlkey是公钥还是私钥
        /// </summary>
        /// <param name="xmlkey">xmlkey</param>
        /// <returns>公钥则为true 私钥则为false</returns>
        public static bool IsPublic(this RSACryptoServiceProvider p_Provider)
        {
            return p_Provider.PublicOnly;
        }

        /// <summary>
        /// 判断xmlkey是公钥还是私钥
        /// </summary>
        /// <param name="xmlkey">xmlkey</param>
        /// <returns>公钥则为true 私钥则为false</returns>
        public static bool IsPublic(this X509Certificate2 p_Cert)
        {
            return !p_Cert.HasPrivateKey;
        }

        /// <summary>
        /// 加密  .net平台默认公钥加密 私钥解密
        /// </summary>
        /// <param name="p_Provider">加密类RSACryptoServiceProvider</param>
        /// <param name="p_InputString"></param>
        /// <returns></returns>
        public static byte[] Encrypt2Byte(this RSACryptoServiceProvider p_Provider, string p_InputString)
        {
            using (p_Provider)
            {
                if (p_InputString != null)
                {
                    return Encrypt2Byte(p_Provider, Encoding.UTF8.GetBytes(p_InputString));
                }

                return null;
            }
        }

        /// <summary>
        /// 加密  .net平台默认公钥加密 私钥解密
        /// </summary>
        /// <param name="p_Provider">加密类RSACryptoServiceProvider</param>
        /// <param name="p_InputBuff"></param>
        /// <returns></returns>
        public static byte[] Encrypt2Byte(this RSACryptoServiceProvider p_Provider, byte[] p_InputBuff)
        {
            using (p_Provider)
            {
                if (p_InputBuff != null)
                {
                    if (p_Provider.IsPublic())
                    {
                        var maxBlockSize = p_Provider.KeySize / 8 - 11;
                        if (p_InputBuff.Length <= maxBlockSize)
                        {
                            return p_Provider.Encrypt(p_InputBuff, false);
                        }

                        using (MemoryStream plaiStream = new MemoryStream(p_InputBuff))
                        {
                            using (MemoryStream crypStream = new MemoryStream())
                            {
                                byte[] buffer = new byte[maxBlockSize];
                                int blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
                                while (blockSize > 0)
                                {
                                    byte[] toEncrypt = new byte[blockSize];
                                    Array.Copy(buffer, 0, toEncrypt, 0, blockSize);
                                    byte[] data = p_Provider.Encrypt(toEncrypt, false);
                                    crypStream.Write(data, 0, data.Length);
                                    blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
                                }

                                return crypStream.ToArray();
                            }
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 加密  .net平台默认公钥加密 私钥解密
        /// </summary>
        /// <param name="p_Provider">加密类RSACryptoServiceProvider</param>
        /// <param name="p_InputString"></param>
        /// <returns></returns>
        public static string Encrypt(this RSACryptoServiceProvider p_Provider, string p_InputString)
        {
            using (p_Provider)
            {
                if (p_InputString != null)
                {
                    return Encrypt(p_Provider, Encoding.UTF8.GetBytes(p_InputString));
                }

                return null;
            }
        }

        /// <summary>
        /// 加密  .net平台默认公钥加密 私钥解密
        /// </summary>
        /// <param name="p_Provider">加密类RSACryptoServiceProvider</param>
        /// <param name="p_InputBuff"></param>
        /// <returns></returns>
        public static string Encrypt(this RSACryptoServiceProvider p_Provider, byte[] p_InputBuff)
        {
            using (p_Provider)
            {
                if (p_InputBuff != null)
                {
                    return Convert.ToBase64String(Encrypt2Byte(p_Provider, p_InputBuff));
                }

                return null;
            }
        }

        /// <summary>
        /// 解密  .net平台默认公钥加密 私钥解密
        /// </summary>
        /// <param name="p_Provider">加密类RSACryptoServiceProvider</param>
        /// <param name="p_InputString"></param>
        /// <returns></returns>
        public static byte[] Decrypt2Byte(this RSACryptoServiceProvider p_Provider, string p_InputString)
        {
            using (p_Provider)
            {
                if (p_InputString != null)
                {
                    return Decrypt2Byte(p_Provider, Convert.FromBase64String(p_InputString));
                }

                return null;
            }
        }

        /// <summary>
        ///  解密  .net平台默认公钥加密 私钥解密
        /// </summary>
        /// <param name="p_Provider">加密类RSACryptoServiceProvider</param>
        /// <param name="p_InputBuff"></param>
        /// <returns></returns>
        public static byte[] Decrypt2Byte(this RSACryptoServiceProvider p_Provider, byte[] p_InputBuff)
        {
            using (p_Provider)
            {
                if (p_InputBuff != null)
                {
                    if (!p_Provider.IsPublic())
                    {
                        int maxBlockSize = p_Provider.KeySize / 8;
                        if (p_InputBuff.Length <= maxBlockSize)
                        {
                            return p_Provider.Decrypt(p_InputBuff, false);
                        }

                        using (MemoryStream crypStream = new MemoryStream(p_InputBuff))
                        {
                            using (MemoryStream plaiStream = new MemoryStream())
                            {
                                byte[] buffer = new byte[maxBlockSize];
                                int blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                                while (blockSize > 0)
                                {
                                    byte[] toDecrypt = new byte[blockSize];
                                    Array.Copy(buffer, 0, toDecrypt, 0, blockSize);
                                    byte[] data = p_Provider.Decrypt(toDecrypt, false);
                                    plaiStream.Write(data, 0, data.Length);
                                    blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                                }

                                return plaiStream.ToArray(); //得到解密结果
                            }
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 解密  .net平台默认公钥加密 私钥解密
        /// </summary>
        /// <param name="p_Provider">加密类RSACryptoServiceProvider</param>
        /// <param name="p_InputString"></param>
        /// <returns></returns>
        public static string Decrypt(this RSACryptoServiceProvider p_Provider, string p_InputString)
        {
            using (p_Provider)
            {
                if (p_InputString != null)
                {
                    return Decrypt(p_Provider, Convert.FromBase64String(p_InputString));
                }

                return null;
            }
        }

        /// <summary>
        ///  解密  .net平台默认公钥加密 私钥解密
        /// </summary>
        /// <param name="p_Provider">加密类RSACryptoServiceProvider</param>
        /// <param name="p_InputBuff"></param>
        /// <returns></returns>
        public static string Decrypt(this RSACryptoServiceProvider p_Provider, byte[] p_InputBuff)
        {
            using (p_Provider)
            {
                if (p_InputBuff != null)
                {
                    return Encoding.UTF8.GetString(Decrypt2Byte(p_Provider, p_InputBuff));
                }

                return null;
            }
        }

        /// <summary>
        /// 计算签名
        /// </summary>
        /// <param name="p_Provider">加密类RSACryptoServiceProvider</param>
        /// <param name="p_InputBuff">要生成签名的数据</param>
        /// <param name="p_Hashenum">签名类型</param>
        /// <returns></returns>
        public static byte[] SignData(this RSACryptoServiceProvider p_Provider, Utils.HASHCrypto.CryptoEnum p_Hashenum,
            byte[] p_InputBuff)
        {
            using (p_Provider)
            {
                if (p_InputBuff != null)
                {
                    HashAlgorithm hashalg = null;
                    switch (p_Hashenum)
                    {
                        case Utils.HASHCrypto.CryptoEnum.MD5:
                            hashalg = new MD5CryptoServiceProvider();
                            break;

                        case Utils.HASHCrypto.CryptoEnum.SHA1:
                            hashalg = new SHA1CryptoServiceProvider();
                            break;

                        case Utils.HASHCrypto.CryptoEnum.SHA256:
                            hashalg = new SHA256CryptoServiceProvider();
                            break;

                        case Utils.HASHCrypto.CryptoEnum.SHA384:
                            hashalg = new SHA384CryptoServiceProvider();
                            break;

                        case Utils.HASHCrypto.CryptoEnum.SHA512:
                            hashalg = new SHA512CryptoServiceProvider();
                            break;

                        default:
                            hashalg = new MD5CryptoServiceProvider();
                            break;
                    }

                    return p_Provider.SignData(p_InputBuff, hashalg);
                }

                return null;
            }
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="p_Provider">加密类RSACryptoServiceProvider</param>
        /// <param name="p_SignedBuff">签名数据</param>
        /// <param name="p_InputBuff">要验证的数据</param>
        /// <param name="p_Hashenum">签名类型</param>
        /// <returns></returns>
        public static bool VerifyData(this RSACryptoServiceProvider p_Provider, Utils.HASHCrypto.CryptoEnum p_Hashenum,
            byte[] p_SignedBuff, byte[] p_InputBuff)
        {
            using (p_Provider)
            {
                if (p_InputBuff != null)
                {
                    HashAlgorithm hashalg = null;
                    switch (p_Hashenum)
                    {
                        case Utils.HASHCrypto.CryptoEnum.MD5:
                            hashalg = new MD5CryptoServiceProvider();
                            break;

                        case Utils.HASHCrypto.CryptoEnum.SHA1:
                            hashalg = new SHA1CryptoServiceProvider();
                            break;

                        case Utils.HASHCrypto.CryptoEnum.SHA256:
                            hashalg = new SHA256CryptoServiceProvider();
                            break;

                        case Utils.HASHCrypto.CryptoEnum.SHA384:
                            hashalg = new SHA384CryptoServiceProvider();
                            break;

                        case Utils.HASHCrypto.CryptoEnum.SHA512:
                            hashalg = new SHA512CryptoServiceProvider();
                            break;
                    }

                    return p_Provider.VerifyData(p_InputBuff, hashalg, p_SignedBuff);
                }

                return false;
            }
        }

        ///// <summary>
        ///// 签名验证  VerifyHash 和 VerifyData 方法等价
        ///// </summary>
        ///// <param name="p_Provider">加密类RSACryptoServiceProvider</param>
        ///// <param name="p_SignedBuff">签名数据</param>
        ///// <param name="p_InputBuff">要验证的数据</param>
        ///// <param name="p_OID">签名类型</param>
        ///// <returns></returns>
        //public static bool VerifyHash(RSACryptoServiceProvider p_Provider, byte[] p_SignedBuff,
        //    byte[] p_InputBuffHash, string p_Oid)
        //{
        //    if (p_InputBuffHash != null)
        //    {
        //        return p_Provider.VerifyHash(p_InputBuffHash, p_Oid, p_SignedBuff);
        //    }
        //    return false;

        //    //https://www.mgenware.com/blog/?p=105
        //    //using (var rsa = new RSACryptoServiceProvider())
        //    //using (var sha1 = SHA1.Create())
        //    //{
        //    //    //原始数据
        //    //    var data = new byte[] { 1, 2, 3 };
        //    //    //计算哈希值
        //    //    var hash = sha1.ComputeHash(data);

        //    //    //用SignHash，注意使用CryptoConfig.MapNameToOID
        //    //    var sigHash = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
        //    //    //用SignData，直接传入数据（函数内部会计算哈希值）
        //    //    var sigData = rsa.SignData(data, typeof(SHA1));

        //    //    //输出两个签名数据
        //    //    Console.WriteLine(BitConverter.ToString(sigHash));
        //    //    Console.WriteLine(BitConverter.ToString(sigData));

        //    //    //验证
        //    //    Console.WriteLine(rsa.VerifyHash(hash, "SHA1", sigHash));
        //    //    Console.WriteLine(rsa.VerifyData(data, typeof(SHA1), sigData));
        //    //}
        //}
    }

    /// <summary>
    ///http://www.jensign.com/JavaScience/cryptoutils/index.html
    ///http://www.jensign.com/opensslkey/opensslkey.cs
    ///https://www.cnblogs.com/zwei1121/p/6055277.html
    /// </summary>
    // static class opensslkey
    // {
    //     private const String pemprivheader = "-----BEGIN RSA PRIVATE KEY-----";
    //     private const String pemprivfooter = "-----END RSA PRIVATE KEY-----";
    //     private const String pempubheader = "-----BEGIN PUBLIC KEY-----";
    //     private const String pempubfooter = "-----END PUBLIC KEY-----";
    //     private const String pemp8header = "-----BEGIN PRIVATE KEY-----";
    //     private const String pemp8footer = "-----END PRIVATE KEY-----";
    //     private const String pemp8encheader = "-----BEGIN ENCRYPTED PRIVATE KEY-----";
    //     private const String pemp8encfooter = "-----END ENCRYPTED PRIVATE KEY-----";
    //
    //     private class Win32
    //     {
    //         [DllImport("crypt32.dll", SetLastError = true)]
    //         public static extern IntPtr CertCreateSelfSignCertificate(
    //             IntPtr hProv,
    //             ref CERT_NAME_BLOB pSubjectIssuerBlob,
    //             uint dwFlagsm,
    //             ref CRYPT_KEY_PROV_INFO pKeyProvInfo,
    //             IntPtr pSignatureAlgorithm,
    //             IntPtr pStartTime,
    //             IntPtr pEndTime,
    //             IntPtr other);
    //
    //         [DllImport("crypt32.dll", SetLastError = true)]
    //         public static extern bool CertStrToName(
    //             uint dwCertEncodingType,
    //             String pszX500,
    //             uint dwStrType,
    //             IntPtr pvReserved,
    //             [In, Out] byte[] pbEncoded,
    //             ref uint pcbEncoded,
    //             IntPtr other);
    //
    //         [DllImport("crypt32.dll", SetLastError = true)]
    //         public static extern bool CertFreeCertificateContext(
    //             IntPtr hCertStore);
    //     }
    //
    //     [StructLayout(LayoutKind.Sequential)]
    //     private struct CRYPT_KEY_PROV_INFO
    //     {
    //         [MarshalAs(UnmanagedType.LPWStr)] public String pwszContainerName;
    //         [MarshalAs(UnmanagedType.LPWStr)] public String pwszProvName;
    //         public uint dwProvType;
    //         public uint dwFlags;
    //         public uint cProvParam;
    //         public IntPtr rgProvParam;
    //         public uint dwKeySpec;
    //     }
    //
    //     [StructLayout(LayoutKind.Sequential)]
    //     private struct CERT_NAME_BLOB
    //     {
    //         public int cbData;
    //         public IntPtr pbData;
    //     }
    //
    //     //private static bool verbose = false;
    //
    //     /// <summary>
    //     /// Decode PEM pubic, private or pkcs8 key
    //     /// </summary>
    //     /// <param name="pemstr"></param>
    //     public static void DecodePEMKey(String pemstr, string pswd = "")
    //     {
    //         byte[] pempublickey;
    //         byte[] pemprivatekey;
    //         byte[] pkcs8privatekey;
    //         byte[] pkcs8encprivatekey;
    //
    //         if (pemstr.StartsWith(pempubheader) && pemstr.EndsWith(pempubfooter))
    //         {
    //             //Console.WriteLine("Trying to decode and parse a PEM public key ..");
    //             pempublickey = DecodeOpenSSLPublicKey(pemstr);
    //             if (pempublickey != null)
    //             {
    //                 //if (verbose)
    //                 //    showBytes("\nRSA public key", pempublickey);
    //                 //PutFileBytes("rsapubkey.pem", pempublickey, pempublickey.Length) ;
    //                 RSACryptoServiceProvider rsa = DecodeX509PublicKey(pempublickey);
    //                 //Console.WriteLine("\nCreated an RSACryptoServiceProvider instance\n");
    //                 String xmlpublickey = rsa.ToXmlString(false);
    //                 //Console.WriteLine("\nXML RSA public key: {0} bits\n{1}\n", rsa.KeySize, xmlpublickey);
    //             }
    //         }
    //         else if (pemstr.StartsWith(pemprivheader) && pemstr.EndsWith(pemprivfooter))
    //         {
    //             //Console.WriteLine("Trying to decrypt and parse a PEM private key ..");
    //             pemprivatekey = DecodeOpenSSLPrivateKey(pemstr, pswd);
    //             if (pemprivatekey != null)
    //             {
    //                 //if (verbose)
    //                 //    showBytes("\nRSA private key", pemprivatekey);
    //                 //PutFileBytes("rsaprivkey.pem", pemprivatekey, pemprivatekey.Length) ;
    //                 RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(pemprivatekey);
    //                 //Console.WriteLine("\nCreated an RSACryptoServiceProvider instance\n");
    //                 String xmlprivatekey = rsa.ToXmlString(true);
    //                 //Console.WriteLine("\nXML RSA private key: {0} bits\n{1}\n", rsa.KeySize, xmlprivatekey);
    //                 RSAtoPKCS12(rsa, pswd);
    //             }
    //         }
    //         else if (pemstr.StartsWith(pemp8header) && pemstr.EndsWith(pemp8footer))
    //         {
    //             //Console.WriteLine("Trying to decode and parse as PEM PKCS #8 PrivateKeyInfo ..");
    //             pkcs8privatekey = DecodePkcs8PrivateKey(pemstr);
    //             if (pkcs8privatekey != null)
    //             {
    //                 //if (verbose)
    //                 //    showBytes("\nPKCS #8 PrivateKeyInfo", pkcs8privatekey);
    //                 //PutFileBytes("PrivateKeyInfo", pkcs8privatekey, pkcs8privatekey.Length) ;
    //                 RSACryptoServiceProvider rsa = DecodePrivateKeyInfo(pkcs8privatekey);
    //                 if (rsa != null)
    //                 {
    //                     //Console.WriteLine("\nCreated an RSACryptoServiceProvider instance\n");
    //                     String xmlprivatekey = rsa.ToXmlString(true);
    //                     //Console.WriteLine("\nXML RSA private key: {0} bits\n{1}\n", rsa.KeySize, xmlprivatekey);
    //                     RSAtoPKCS12(rsa, pswd);
    //                 }
    //                 //else
    //                 //Console.WriteLine("\nFailed to create an RSACryptoServiceProvider");
    //             }
    //         }
    //         else if (pemstr.StartsWith(pemp8encheader) && pemstr.EndsWith(pemp8encfooter))
    //         {
    //             //Console.WriteLine("Trying to decode and parse as PEM PKCS #8 EncryptedPrivateKeyInfo ..");
    //             pkcs8encprivatekey = DecodePkcs8EncPrivateKey(pemstr);
    //             if (pkcs8encprivatekey != null)
    //             {
    //                 //if (verbose)
    //                 //    showBytes("\nPKCS #8 EncryptedPrivateKeyInfo", pkcs8encprivatekey);
    //                 //PutFileBytes("EncryptedPrivateKeyInfo", pkcs8encprivatekey, pkcs8encprivatekey.Length) ;
    //                 RSACryptoServiceProvider rsa = DecodeEncryptedPrivateKeyInfo(pkcs8encprivatekey, pswd);
    //                 if (rsa != null)
    //                 {
    //                     //Console.WriteLine("\nCreated an RSACryptoServiceProvider instance\n");
    //                     String xmlprivatekey = rsa.ToXmlString(true);
    //                     //Console.WriteLine("\nXML RSA private key: {0} bits\n{1}\n", rsa.KeySize, xmlprivatekey);
    //                     RSAtoPKCS12(rsa, pswd);
    //                 }
    //                 //else
    //                 //Console.WriteLine("\nFailed to create an RSACryptoServiceProvider");
    //             }
    //         }
    //         else
    //         {
    //             //Console.WriteLine("Not a PEM public, private key or a PKCS #8");
    //             return;
    //         }
    //     }
    //
    //     /// <summary>
    //     /// Decode PEM pubic, private or pkcs8 key
    //     /// </summary>
    //     /// <param name="filename"></param>
    //     public static void DecodeDERKey(String filename, string pswd = "")
    //     {
    //         RSACryptoServiceProvider rsa = null;
    //         byte[] keyblob = GetFileBytes(filename);
    //         if (keyblob == null)
    //             return;
    //
    //         rsa = DecodeX509PublicKey(keyblob);
    //         if (rsa != null)
    //         {
    //             //Console.WriteLine("\nA valid SubjectPublicKeyInfo\n");
    //             //Console.WriteLine("\nCreated an RSACryptoServiceProvider instance\n");
    //             String xmlpublickey = rsa.ToXmlString(false);
    //             //Console.WriteLine("\nXML RSA public key: {0} bits\n{1}\n", rsa.KeySize, xmlpublickey);
    //             return;
    //         }
    //
    //         rsa = DecodeRSAPrivateKey(keyblob);
    //         if (rsa != null)
    //         {
    //             //Console.WriteLine("\nA valid RSAPrivateKey\n");
    //             //Console.WriteLine("\nCreated an RSACryptoServiceProvider instance\n");
    //             String xmlprivatekey = rsa.ToXmlString(true);
    //             //Console.WriteLine("\nXML RSA private key: {0} bits\n{1}\n", rsa.KeySize, xmlprivatekey);
    //             RSAtoPKCS12(rsa, pswd);
    //             return;
    //         }
    //
    //         rsa = DecodePrivateKeyInfo(keyblob);    //PKCS #8 unencrypted
    //         if (rsa != null)
    //         {
    //             //Console.WriteLine("\nA valid PKCS #8 PrivateKeyInfo\n");
    //             //Console.WriteLine("\nCreated an RSACryptoServiceProvider instance\n");
    //             String xmlprivatekey = rsa.ToXmlString(true);
    //             //Console.WriteLine("\nXML RSA private key: {0} bits\n{1}\n", rsa.KeySize, xmlprivatekey);
    //             RSAtoPKCS12(rsa, pswd);
    //             return;
    //         }
    //
    //         rsa = DecodeEncryptedPrivateKeyInfo(keyblob, pswd);   //PKCS #8 encrypted
    //         if (rsa != null)
    //         {
    //             //Console.WriteLine("\nA valid PKCS #8 EncryptedPrivateKeyInfo\n");
    //             //Console.WriteLine("\nCreated an RSACryptoServiceProvider instance\n");
    //             String xmlprivatekey = rsa.ToXmlString(true);
    //             //Console.WriteLine("\nXML RSA private key: {0} bits\n{1}\n", rsa.KeySize, xmlprivatekey);
    //             RSAtoPKCS12(rsa, pswd);
    //             return;
    //         }
    //         //Console.WriteLine("Not a binary DER public, private or PKCS #8 key");
    //         return;
    //     }
    //
    //     //public static void ProcessRSA(RSACryptoServiceProvider rsa)
    //     //{
    //     //    if (verbose)
    //     //        showRSAProps(rsa);
    //     //    Console.Write("\n\nExport RSA private key to PKCS #12 file? (Y or N) ");
    //     //    String resp = Console.ReadLine().ToUpper();
    //     //    if (resp == "Y" || resp == "YES")
    //     //        RSAtoPKCS12(rsa);
    //     //}
    //
    //     /// <summary>
    //     /// Generate pkcs #12 from an RSACryptoServiceProvider
    //     /// </summary>
    //     /// <param name="rsa"></param>
    //     /// <param name="pswd"></param>
    //     public static void RSAtoPKCS12(RSACryptoServiceProvider rsa, string pswd = "")
    //     {
    //         CspKeyContainerInfo keyInfo = rsa.CspKeyContainerInfo;
    //         String keycontainer = keyInfo.KeyContainerName;
    //         uint keyspec = (uint)keyInfo.KeyNumber;
    //         String provider = keyInfo.ProviderName;
    //         uint cspflags = 0; //CryptoAPI Current User store; LM would be CRYPT_MACHINE_KEYSET	= 0x00000020
    //         String fname = keycontainer + ".p12";
    //         //---- need to pass in rsa since underlying keycontainer is not persisted and might be deleted too quickly ---
    //         byte[] pkcs12 = GetPkcs12(rsa, keycontainer, provider, keyspec, cspflags, pswd);
    //         //if ((pkcs12 != null) && verbose)
    //         //    showBytes("\npkcs #12", pkcs12);
    //         if (pkcs12 != null)
    //         {
    //             PutFileBytes(fname, pkcs12, pkcs12.Length);
    //             //Console.WriteLine("\nWrote pkc #12 file '{0}'\n", fname);
    //         }
    //         //else
    //         //Console.WriteLine("\nProblem getting pkcs#12");
    //     }
    //
    //     /// <summary>
    //     /// Get the binary PKCS #8 PRIVATE key
    //     /// </summary>
    //     /// <param name="instr"></param>
    //     /// <returns></returns>
    //     public static byte[] DecodePkcs8PrivateKey(String instr)
    //     {
    //         const String pemp8header = "-----BEGIN PRIVATE KEY-----";
    //         const String pemp8footer = "-----END PRIVATE KEY-----";
    //         String pemstr = instr.Trim();
    //         byte[] binkey;
    //         if (!pemstr.StartsWith(pemp8header) || !pemstr.EndsWith(pemp8footer))
    //             return null;
    //         StringBuilder sb = new StringBuilder(pemstr);
    //         sb.Replace(pemp8header, ""); //remove headers/footers, if present
    //         sb.Replace(pemp8footer, "");
    //
    //         String pubstr = sb.ToString().Trim();   //get string after removing leading/trailing whitespace
    //
    //         try
    //         {
    //             binkey = Convert.FromBase64String(pubstr);
    //         }
    //         catch (System.FormatException)
    //         {   //if can't b64 decode, data is not valid
    //             return null;
    //         }
    //         return binkey;
    //     }
    //
    //     /// <summary>
    //     /// Parses binary asn.1 PKCS #8 PrivateKeyInfo; returns RSACryptoServiceProvider
    //     /// </summary>
    //     /// <param name="pkcs8"></param>
    //     /// <returns></returns>
    //     public static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
    //     {
    //         // encoded OID sequence for PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
    //         // this byte[] includes the sequence byte and terminal encoded null
    //         byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
    //         byte[] seq = new byte[15];
    //         // --------- Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob ------
    //         MemoryStream mem = new MemoryStream(pkcs8);
    //         int lenstream = (int)mem.Length;
    //         BinaryReader binr = new BinaryReader(mem); //wrap Memory Stream with BinaryReader for easy reading
    //         byte bt = 0;
    //         ushort twobytes = 0;
    //
    //         try
    //         {
    //             twobytes = binr.ReadUInt16();
    //             if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
    //                 binr.ReadByte();    //advance 1 byte
    //             else if (twobytes == 0x8230)
    //                 binr.ReadInt16();   //advance 2 bytes
    //             else
    //                 return null;
    //
    //             bt = binr.ReadByte();
    //             if (bt != 0x02)
    //                 return null;
    //
    //             twobytes = binr.ReadUInt16();
    //
    //             if (twobytes != 0x0001)
    //                 return null;
    //
    //             seq = binr.ReadBytes(15);   //read the Sequence OID
    //             if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
    //                 return null;
    //
    //             bt = binr.ReadByte();
    //             if (bt != 0x04) //expect an Octet string
    //                 return null;
    //
    //             bt = binr.ReadByte();   //read next byte, or next 2 bytes is 0x81 or 0x82; otherwise bt is the byte count
    //             if (bt == 0x81)
    //                 binr.ReadByte();
    //             else
    //             if (bt == 0x82)
    //                 binr.ReadUInt16();
    //             //------ at this stage, the remaining sequence should be the RSA private key
    //
    //             byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
    //             RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKey(rsaprivkey);
    //             return rsacsp;
    //         }
    //         catch (Exception)
    //         {
    //             return null;
    //         }
    //         finally { binr.Close(); }
    //     }
    //
    //     /// <summary>
    //     /// Get the binary PKCS #8 Encrypted PRIVATE key
    //     /// </summary>
    //     /// <param name="instr"></param>
    //     /// <returns></returns>
    //     public static byte[] DecodePkcs8EncPrivateKey(String instr)
    //     {
    //         const String pemp8encheader = "-----BEGIN ENCRYPTED PRIVATE KEY-----";
    //         const String pemp8encfooter = "-----END ENCRYPTED PRIVATE KEY-----";
    //         String pemstr = instr.Trim();
    //         byte[] binkey;
    //         if (!pemstr.StartsWith(pemp8encheader) || !pemstr.EndsWith(pemp8encfooter))
    //             return null;
    //         StringBuilder sb = new StringBuilder(pemstr);
    //         sb.Replace(pemp8encheader, ""); //remove headers/footers, if present
    //         sb.Replace(pemp8encfooter, "");
    //
    //         String pubstr = sb.ToString().Trim();   //get string after removing leading/trailing whitespace
    //
    //         try
    //         {
    //             binkey = Convert.FromBase64String(pubstr);
    //         }
    //         catch (System.FormatException)
    //         {   //if can't b64 decode, data is not valid
    //             return null;
    //         }
    //         return binkey;
    //     }
    //
    //     /// <summary>
    //     /// Parses binary asn.1 EncryptedPrivateKeyInfo; returns RSACryptoServiceProvider
    //     /// </summary>
    //     /// <param name="encpkcs8"></param>
    //     /// <returns></returns>
    //     public static RSACryptoServiceProvider DecodeEncryptedPrivateKeyInfo(byte[] encpkcs8, string pswd = "")
    //     {
    //         // encoded OID sequence for PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
    //         // this byte[] includes the sequence byte and terminal encoded null
    //         byte[] OIDpkcs5PBES2 = { 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x05, 0x0D };
    //         byte[] OIDpkcs5PBKDF2 = { 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x05, 0x0C };
    //         byte[] OIDdesEDE3CBC = { 0x06, 0x08, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x03, 0x07 };
    //         byte[] seqdes = new byte[10];
    //         byte[] seq = new byte[11];
    //         byte[] salt;
    //         byte[] IV;
    //         byte[] encryptedpkcs8;
    //         byte[] pkcs8;
    //
    //         int saltsize, ivsize, encblobsize;
    //         int iterations;
    //
    //         // --------- Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob ------
    //         MemoryStream mem = new MemoryStream(encpkcs8);
    //         int lenstream = (int)mem.Length;
    //         BinaryReader binr = new BinaryReader(mem); //wrap Memory Stream with BinaryReader for easy reading
    //         byte bt = 0;
    //         ushort twobytes = 0;
    //
    //         try
    //         {
    //             twobytes = binr.ReadUInt16();
    //             if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
    //                 binr.ReadByte();    //advance 1 byte
    //             else if (twobytes == 0x8230)
    //                 binr.ReadInt16();   //advance 2 bytes
    //             else
    //                 return null;
    //
    //             twobytes = binr.ReadUInt16();   //inner sequence
    //             if (twobytes == 0x8130)
    //                 binr.ReadByte();
    //             else if (twobytes == 0x8230)
    //                 binr.ReadInt16();
    //
    //             seq = binr.ReadBytes(11);   //read the Sequence OID
    //             if (!CompareBytearrays(seq, OIDpkcs5PBES2)) //is it a OIDpkcs5PBES2 ?
    //                 return null;
    //
    //             twobytes = binr.ReadUInt16();   //inner sequence for pswd salt
    //             if (twobytes == 0x8130)
    //                 binr.ReadByte();
    //             else if (twobytes == 0x8230)
    //                 binr.ReadInt16();
    //
    //             twobytes = binr.ReadUInt16();   //inner sequence for pswd salt
    //             if (twobytes == 0x8130)
    //                 binr.ReadByte();
    //             else if (twobytes == 0x8230)
    //                 binr.ReadInt16();
    //
    //             seq = binr.ReadBytes(11);   //read the Sequence OID
    //             if (!CompareBytearrays(seq, OIDpkcs5PBKDF2))    //is it a OIDpkcs5PBKDF2 ?
    //                 return null;
    //
    //             twobytes = binr.ReadUInt16();
    //             if (twobytes == 0x8130)
    //                 binr.ReadByte();
    //             else if (twobytes == 0x8230)
    //                 binr.ReadInt16();
    //
    //             bt = binr.ReadByte();
    //             if (bt != 0x04) //expect octet string for salt
    //                 return null;
    //             saltsize = binr.ReadByte();
    //             salt = binr.ReadBytes(saltsize);
    //
    //             //if (verbose)
    //             //    showBytes("Salt for pbkd", salt);
    //             bt = binr.ReadByte();
    //             if (bt != 0x02) //expect an integer for PBKF2 interation count
    //                 return null;
    //
    //             int itbytes = binr.ReadByte();  //PBKD2 iterations should fit in 2 bytes.
    //             if (itbytes == 1)
    //                 iterations = binr.ReadByte();
    //             else if (itbytes == 2)
    //                 iterations = 256 * binr.ReadByte() + binr.ReadByte();
    //             else
    //                 return null;
    //             //if (verbose)
    //             //Console.WriteLine("PBKD2 iterations {0}", iterations);
    //
    //             twobytes = binr.ReadUInt16();
    //             if (twobytes == 0x8130)
    //                 binr.ReadByte();
    //             else if (twobytes == 0x8230)
    //                 binr.ReadInt16();
    //
    //             seqdes = binr.ReadBytes(10);    //read the Sequence OID
    //             if (!CompareBytearrays(seqdes, OIDdesEDE3CBC))  //is it a OIDdes-EDE3-CBC ?
    //                 return null;
    //
    //             bt = binr.ReadByte();
    //             if (bt != 0x04) //expect octet string for IV
    //                 return null;
    //             ivsize = binr.ReadByte();   // IV byte size should fit in one byte (24 expected for 3DES)
    //             IV = binr.ReadBytes(ivsize);
    //             //if (verbose)
    //             //    showBytes("IV for des-EDE3-CBC", IV);
    //
    //             bt = binr.ReadByte();
    //             if (bt != 0x04) // expect octet string for encrypted PKCS8 data
    //                 return null;
    //
    //             bt = binr.ReadByte();
    //
    //             if (bt == 0x81)
    //                 encblobsize = binr.ReadByte();  // data size in next byte
    //             else if (bt == 0x82)
    //                 encblobsize = 256 * binr.ReadByte() + binr.ReadByte();
    //             else
    //                 encblobsize = bt;   // we already have the data size
    //
    //             encryptedpkcs8 = binr.ReadBytes(encblobsize);
    //             //if(verbose)
    //             //	showBytes("Encrypted PKCS8 blob", encryptedpkcs8) ;
    //
    //             SecureString secpswd = new SecureString();// GetSecPswd("Enter password for Encrypted PKCS #8 ==>");
    //             foreach (char c in pswd.ToCharArray())
    //             {
    //                 secpswd.AppendChar(c);
    //             }
    //             pkcs8 = DecryptPBDK2(encryptedpkcs8, salt, IV, secpswd, iterations);
    //             if (pkcs8 == null)  // probably a bad pswd entered.
    //                 return null;
    //
    //             //if(verbose)
    //             //	showBytes("Decrypted PKCS #8", pkcs8) ;
    //             //----- With a decrypted pkcs #8 PrivateKeyInfo blob, decode it to an RSA ---
    //             RSACryptoServiceProvider rsa = DecodePrivateKeyInfo(pkcs8);
    //             return rsa;
    //         }
    //         catch (Exception)
    //         {
    //             return null;
    //         }
    //         finally { binr.Close(); }
    //     }
    //
    //     /// <summary>
    //     /// Uses PBKD2 to derive a 3DES key and decrypts data
    //     /// </summary>
    //     /// <param name="edata"></param>
    //     /// <param name="salt"></param>
    //     /// <param name="IV"></param>
    //     /// <param name="secpswd"></param>
    //     /// <param name="iterations"></param>
    //     /// <returns></returns>
    //     public static byte[] DecryptPBDK2(byte[] edata, byte[] salt, byte[] IV, SecureString secpswd, int iterations)
    //     {
    //         CryptoStream decrypt = null;
    //
    //         IntPtr unmanagedPswd = IntPtr.Zero;
    //         byte[] psbytes = new byte[secpswd.Length];
    //         unmanagedPswd = Marshal.SecureStringToGlobalAllocAnsi(secpswd);
    //         Marshal.Copy(unmanagedPswd, psbytes, 0, psbytes.Length);
    //         Marshal.ZeroFreeGlobalAllocAnsi(unmanagedPswd);
    //
    //         try
    //         {
    //             Rfc2898DeriveBytes kd = new Rfc2898DeriveBytes(psbytes, salt, iterations);
    //             TripleDES decAlg = TripleDES.Create();
    //             decAlg.Key = kd.GetBytes(24);
    //             decAlg.IV = IV;
    //             MemoryStream memstr = new MemoryStream();
    //             decrypt = new CryptoStream(memstr, decAlg.CreateDecryptor(), CryptoStreamMode.Write);
    //             decrypt.Write(edata, 0, edata.Length);
    //             decrypt.Flush();
    //             decrypt.Close();    // this is REQUIRED.
    //             byte[] cleartext = memstr.ToArray();
    //             return cleartext;
    //         }
    //         catch
    //         {
    //             return null;
    //         }
    //     }
    //
    //     /// <summary>
    //     /// Get the binary RSA PUBLIC key
    //     /// </summary>
    //     /// <param name="instr"></param>
    //     /// <returns></returns>
    //     public static byte[] DecodeOpenSSLPublicKey(String instr)
    //     {
    //         const String pempubheader = "-----BEGIN PUBLIC KEY-----";
    //         const String pempubfooter = "-----END PUBLIC KEY-----";
    //         String pemstr = instr.Trim();
    //         byte[] binkey;
    //         if (!pemstr.StartsWith(pempubheader) || !pemstr.EndsWith(pempubfooter))
    //             return null;
    //         StringBuilder sb = new StringBuilder(pemstr);
    //         sb.Replace(pempubheader, ""); //remove headers/footers, if present
    //         sb.Replace(pempubfooter, "");
    //
    //         String pubstr = sb.ToString().Trim();   //get string after removing leading/trailing whitespace
    //
    //         try
    //         {
    //             binkey = Convert.FromBase64String(pubstr);
    //         }
    //         catch (System.FormatException)
    //         {   //if can't b64 decode, data is not valid
    //             return null;
    //         }
    //         return binkey;
    //     }
    //
    //     /// <summary>
    //     /// Parses binary asn.1 X509 SubjectPublicKeyInfo; returns RSACryptoServiceProvider
    //     /// </summary>
    //     /// <param name="x509key"></param>
    //     /// <returns></returns>
    //     public static RSACryptoServiceProvider DecodeX509PublicKey(byte[] x509key)
    //     {
    //         // encoded OID sequence for PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
    //         byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
    //         byte[] seq = new byte[15];
    //         // --------- Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob ------
    //         MemoryStream mem = new MemoryStream(x509key);
    //         BinaryReader binr = new BinaryReader(mem); //wrap Memory Stream with BinaryReader for easy reading
    //         byte bt = 0;
    //         ushort twobytes = 0;
    //
    //         try
    //         {
    //             twobytes = binr.ReadUInt16();
    //             if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
    //                 binr.ReadByte();    //advance 1 byte
    //             else if (twobytes == 0x8230)
    //                 binr.ReadInt16();   //advance 2 bytes
    //             else
    //                 return null;
    //
    //             seq = binr.ReadBytes(15);   //read the Sequence OID
    //             if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
    //                 return null;
    //
    //             twobytes = binr.ReadUInt16();
    //             if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
    //                 binr.ReadByte();    //advance 1 byte
    //             else if (twobytes == 0x8203)
    //                 binr.ReadInt16();   //advance 2 bytes
    //             else
    //                 return null;
    //
    //             bt = binr.ReadByte();
    //             if (bt != 0x00) //expect null byte next
    //                 return null;
    //
    //             twobytes = binr.ReadUInt16();
    //             if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
    //                 binr.ReadByte();    //advance 1 byte
    //             else if (twobytes == 0x8230)
    //                 binr.ReadInt16();   //advance 2 bytes
    //             else
    //                 return null;
    //
    //             twobytes = binr.ReadUInt16();
    //             byte lowbyte = 0x00;
    //             byte highbyte = 0x00;
    //
    //             if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
    //                 lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
    //             else if (twobytes == 0x8202)
    //             {
    //                 highbyte = binr.ReadByte(); //advance 2 bytes
    //                 lowbyte = binr.ReadByte();
    //             }
    //             else
    //                 return null;
    //             byte[] modint = { lowbyte, highbyte, 0x00, 0x00 }; //reverse byte order since asn.1 key uses big endian order
    //             int modsize = BitConverter.ToInt32(modint, 0);
    //
    //             byte firstbyte = binr.ReadByte();
    //             binr.BaseStream.Seek(-1, SeekOrigin.Current);
    //
    //             if (firstbyte == 0x00)
    //             {   //if first byte (highest order) of modulus is zero, don't include it
    //                 binr.ReadByte();    //skip this null byte
    //                 modsize -= 1;   //reduce modulus buffer size by 1
    //             }
    //
    //             byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes
    //
    //             if (binr.ReadByte() != 0x02)    //expect an Integer for the exponent data
    //                 return null;
    //             int expbytes = (int)binr.ReadByte();    // should only need one byte for actual exponent data (for all useful values)
    //             byte[] exponent = binr.ReadBytes(expbytes);
    //
    //             //showBytes("\nExponent", exponent);
    //             //showBytes("\nModulus", modulus);
    //
    //             // ------- create RSACryptoServiceProvider instance and initialize with public key -----
    //             RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
    //             RSAParameters RSAKeyInfo = new RSAParameters();
    //             RSAKeyInfo.Modulus = modulus;
    //             RSAKeyInfo.Exponent = exponent;
    //             RSA.ImportParameters(RSAKeyInfo);
    //             return RSA;
    //         }
    //         catch (Exception)
    //         {
    //             return null;
    //         }
    //         finally { binr.Close(); }
    //     }
    //
    //     /// <summary>
    //     /// Parses binary ans.1 RSA private key; returns RSACryptoServiceProvider
    //     /// </summary>
    //     /// <param name="privkey"></param>
    //     /// <returns></returns>
    //     public static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
    //     {
    //         byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;
    //
    //         // --------- Set up stream to decode the asn.1 encoded RSA private key ------
    //         MemoryStream mem = new MemoryStream(privkey);
    //         BinaryReader binr = new BinaryReader(mem); //wrap Memory Stream with BinaryReader for easy reading
    //         byte bt = 0;
    //         ushort twobytes = 0;
    //         int elems = 0;
    //         try
    //         {
    //             twobytes = binr.ReadUInt16();
    //             if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
    //                 binr.ReadByte();    //advance 1 byte
    //             else if (twobytes == 0x8230)
    //                 binr.ReadInt16();   //advance 2 bytes
    //             else
    //                 return null;
    //
    //             twobytes = binr.ReadUInt16();
    //             if (twobytes != 0x0102) //version number
    //                 return null;
    //             bt = binr.ReadByte();
    //             if (bt != 0x00)
    //                 return null;
    //
    //             //------ all private key components are Integer sequences ----
    //             elems = GetIntegerSize(binr);
    //             MODULUS = binr.ReadBytes(elems);
    //
    //             elems = GetIntegerSize(binr);
    //             E = binr.ReadBytes(elems);
    //
    //             elems = GetIntegerSize(binr);
    //             D = binr.ReadBytes(elems);
    //
    //             elems = GetIntegerSize(binr);
    //             P = binr.ReadBytes(elems);
    //
    //             elems = GetIntegerSize(binr);
    //             Q = binr.ReadBytes(elems);
    //
    //             elems = GetIntegerSize(binr);
    //             DP = binr.ReadBytes(elems);
    //
    //             elems = GetIntegerSize(binr);
    //             DQ = binr.ReadBytes(elems);
    //
    //             elems = GetIntegerSize(binr);
    //             IQ = binr.ReadBytes(elems);
    //
    //             //Console.WriteLine("showing components ..");
    //             //if (verbose)
    //             //{
    //             //    showBytes("\nModulus", MODULUS);
    //             //    showBytes("\nExponent", E);
    //             //    showBytes("\nD", D);
    //             //    showBytes("\nP", P);
    //             //    showBytes("\nQ", Q);
    //             //    showBytes("\nDP", DP);
    //             //    showBytes("\nDQ", DQ);
    //             //    showBytes("\nIQ", IQ);
    //             //}
    //
    //             // ------- create RSACryptoServiceProvider instance and initialize with public key -----
    //             RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
    //             RSAParameters RSAparams = new RSAParameters();
    //             RSAparams.Modulus = MODULUS;
    //             RSAparams.Exponent = E;
    //             RSAparams.D = D;
    //             RSAparams.P = P;
    //             RSAparams.Q = Q;
    //             RSAparams.DP = DP;
    //             RSAparams.DQ = DQ;
    //             RSAparams.InverseQ = IQ;
    //             RSA.ImportParameters(RSAparams);
    //             return RSA;
    //         }
    //         catch (Exception)
    //         {
    //             return null;
    //         }
    //         finally { binr.Close(); }
    //     }
    //
    //     private static int GetIntegerSize(BinaryReader binr)
    //     {
    //         byte bt = 0;
    //         byte lowbyte = 0x00;
    //         byte highbyte = 0x00;
    //         int count = 0;
    //         bt = binr.ReadByte();
    //         if (bt != 0x02) //expect integer
    //             return 0;
    //         bt = binr.ReadByte();
    //
    //         if (bt == 0x81)
    //             count = binr.ReadByte();    // data size in next byte
    //         else
    //         if (bt == 0x82)
    //         {
    //             highbyte = binr.ReadByte(); // data size in next 2 bytes
    //             lowbyte = binr.ReadByte();
    //             byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
    //             count = BitConverter.ToInt32(modint, 0);
    //         }
    //         else
    //         {
    //             count = bt; // we already have the data size
    //         }
    //
    //         while (binr.ReadByte() == 0x00)
    //         {   //remove high order zeros in data
    //             count -= 1;
    //         }
    //         binr.BaseStream.Seek(-1, SeekOrigin.Current);   //last ReadByte wasn't a removed zero, so back up a byte
    //         return count;
    //     }
    //
    //     /// <summary>
    //     /// Get the binary RSA PRIVATE key, decrypting if necessary
    //     /// </summary>
    //     /// <param name="instr"></param>
    //     /// <returns></returns>
    //     public static byte[] DecodeOpenSSLPrivateKey(String instr, string pswd = "")
    //     {
    //         const String pemprivheader = "-----BEGIN RSA PRIVATE KEY-----";
    //         const String pemprivfooter = "-----END RSA PRIVATE KEY-----";
    //         String pemstr = instr.Trim();
    //         byte[] binkey;
    //         if (!pemstr.StartsWith(pemprivheader) || !pemstr.EndsWith(pemprivfooter))
    //             return null;
    //
    //         StringBuilder sb = new StringBuilder(pemstr);
    //         sb.Replace(pemprivheader, ""); //remove headers/footers, if present
    //         sb.Replace(pemprivfooter, "");
    //
    //         String pvkstr = sb.ToString().Trim();   //get string after removing leading/trailing whitespace
    //
    //         try
    //         { // if there are no PEM encryption info lines, this is an UNencrypted PEM private key
    //             binkey = Convert.FromBase64String(pvkstr);
    //             return binkey;
    //         }
    //         catch (System.FormatException)
    //         {   //if can't b64 decode, it must be an encrypted private key
    //             ////Console.WriteLine("Not an unencrypted OpenSSL PEM private key");
    //         }
    //
    //         StringReader str = new StringReader(pvkstr);
    //
    //         //-------- read PEM encryption info. lines and extract salt -----
    //         if (!str.ReadLine().StartsWith("Proc-Type: 4,ENCRYPTED"))
    //             return null;
    //         String saltline = str.ReadLine();
    //         if (!saltline.StartsWith("DEK-Info: DES-EDE3-CBC,"))
    //             return null;
    //         String saltstr = saltline.Substring(saltline.IndexOf(",") + 1).Trim();
    //         byte[] salt = new byte[saltstr.Length / 2];
    //         for (int i = 0; i < salt.Length; i++)
    //             salt[i] = Convert.ToByte(saltstr.Substring(i * 2, 2), 16);
    //         if (!(str.ReadLine() == ""))
    //             return null;
    //
    //         //------ remaining b64 data is encrypted RSA key ----
    //         String encryptedstr = str.ReadToEnd();
    //
    //         try
    //         {   //should have b64 encrypted RSA key now
    //             binkey = Convert.FromBase64String(encryptedstr);
    //         }
    //         catch (System.FormatException)
    //         { // bad b64 data.
    //             return null;
    //         }
    //
    //         //------ Get the 3DES 24 byte key using PDK used by OpenSSL ----
    //
    //         SecureString despswd = new SecureString(); //GetSecPswd("Enter password to derive 3DES key==>");
    //         foreach (char c in pswd.ToCharArray())
    //         {
    //             despswd.AppendChar(c);
    //         }
    //         //Console.Write("\nEnter password to derive 3DES key: ");
    //         //String pswd = Console.ReadLine();
    //         byte[] deskey = GetOpenSSL3deskey(salt, despswd, 1, 2); // count=1 (for OpenSSL implementation); 2 iterations to get at least 24 bytes
    //         if (deskey == null)
    //             return null;
    //         //showBytes("3DES key", deskey) ;
    //
    //         //------ Decrypt the encrypted 3des-encrypted RSA private key ------
    //         byte[] rsakey = DecryptKey(binkey, deskey, salt);   //OpenSSL uses salt value in PEM header also as 3DES IV
    //         if (rsakey != null)
    //             return rsakey;  //we have a decrypted RSA private key
    //         else
    //         {
    //             //Console.WriteLine("Failed to decrypt RSA private key; probably wrong password.");
    //             return null;
    //         }
    //     }
    //
    //     /// <summary>
    //     /// Decrypt the 3DES encrypted RSA private key
    //     /// </summary>
    //     /// <param name="cipherData"></param>
    //     /// <param name="desKey"></param>
    //     /// <param name="IV"></param>
    //     /// <returns></returns>
    //     public static byte[] DecryptKey(byte[] cipherData, byte[] desKey, byte[] IV)
    //     {
    //         MemoryStream memst = new MemoryStream();
    //         TripleDES alg = TripleDES.Create();
    //         alg.Key = desKey;
    //         alg.IV = IV;
    //         try
    //         {
    //             CryptoStream cs = new CryptoStream(memst, alg.CreateDecryptor(), CryptoStreamMode.Write);
    //             cs.Write(cipherData, 0, cipherData.Length);
    //             cs.Close();
    //         }
    //         catch (Exception exc)
    //         {
    //             Console.WriteLine(exc.Message);
    //             return null;
    //         }
    //         byte[] decryptedData = memst.ToArray();
    //         return decryptedData;
    //     }
    //
    //     /// <summary>
    //     /// OpenSSL PBKD uses only one hash cycle (count); miter is number of iterations required to build sufficient bytes
    //     /// </summary>
    //     /// <param name="salt"></param>
    //     /// <param name="secpswd"></param>
    //     /// <param name="count"></param>
    //     /// <param name="miter"></param>
    //     /// <returns></returns>
    //     private static byte[] GetOpenSSL3deskey(byte[] salt, SecureString secpswd, int count, int miter)
    //     {
    //         IntPtr unmanagedPswd = IntPtr.Zero;
    //         int HASHLENGTH = 16;    //MD5 bytes
    //         byte[] keymaterial = new byte[HASHLENGTH * miter]; //to store contatenated Mi hashed results
    //
    //         byte[] psbytes = new byte[secpswd.Length];
    //         unmanagedPswd = Marshal.SecureStringToGlobalAllocAnsi(secpswd);
    //         Marshal.Copy(unmanagedPswd, psbytes, 0, psbytes.Length);
    //         Marshal.ZeroFreeGlobalAllocAnsi(unmanagedPswd);
    //
    //         //UTF8Encoding utf8 = new UTF8Encoding();
    //         //byte[] psbytes = utf8.GetBytes(pswd);
    //
    //         // --- contatenate salt and pswd bytes into fixed data array ---
    //         byte[] data00 = new byte[psbytes.Length + salt.Length];
    //         Array.Copy(psbytes, data00, psbytes.Length);    //copy the pswd bytes
    //         Array.Copy(salt, 0, data00, psbytes.Length, salt.Length);   //concatenate the salt bytes
    //
    //         // ---- do multi-hashing and contatenate results D1, D2 ... into keymaterial bytes ----
    //         MD5 md5 = new MD5CryptoServiceProvider();
    //         byte[] result = null;
    //         byte[] hashtarget = new byte[HASHLENGTH + data00.Length]; //fixed length initial hashtarget
    //
    //         for (int j = 0; j < miter; j++)
    //         {
    //             // ---- Now hash consecutively for count times ------
    //             if (j == 0)
    //                 result = data00; //initialize
    //             else
    //             {
    //                 Array.Copy(result, hashtarget, result.Length);
    //                 Array.Copy(data00, 0, hashtarget, result.Length, data00.Length);
    //                 result = hashtarget;
    //                 ////Console.WriteLine("Updated new initial hash target:") ;
    //                 //showBytes(result) ;
    //             }
    //
    //             for (int i = 0; i < count; i++)
    //                 result = md5.ComputeHash(result);
    //             Array.Copy(result, 0, keymaterial, j * HASHLENGTH, result.Length); //contatenate to keymaterial
    //         }
    //         //showBytes("Final key material", keymaterial);
    //         byte[] deskey = new byte[24];
    //         Array.Copy(keymaterial, deskey, deskey.Length);
    //
    //         Array.Clear(psbytes, 0, psbytes.Length);
    //         Array.Clear(data00, 0, data00.Length);
    //         Array.Clear(result, 0, result.Length);
    //         Array.Clear(hashtarget, 0, hashtarget.Length);
    //         Array.Clear(keymaterial, 0, keymaterial.Length);
    //
    //         return deskey;
    //     }
    //
    //     /// <summary>
    //     /// Since we are using an RSA with nonpersisted keycontainer, must pass it in to ensure it isn't colledted
    //     /// </summary>
    //     /// <param name="rsa"></param>
    //     /// <param name="keycontainer"></param>
    //     /// <param name="cspprovider"></param>
    //     /// <param name="KEYSPEC"></param>
    //     /// <param name="cspflags"></param>
    //     /// <param name="pswd"></param>
    //     /// <returns></returns>
    //     private static byte[] GetPkcs12(RSA rsa, String keycontainer, String cspprovider, uint KEYSPEC, uint cspflags, string pswd = "")
    //     //private static byte[] GetPkcs12(RSA rsa, String keycontainer, String cspprovider, uint KEYSPEC, uint cspflags)
    //     {
    //         byte[] pfxblob = null;
    //         IntPtr hCertCntxt = IntPtr.Zero;
    //
    //         String DN = "CN=Opensslkey Unsigned Certificate";
    //
    //         hCertCntxt = CreateUnsignedCertCntxt(keycontainer, cspprovider, KEYSPEC, cspflags, DN);
    //         if (hCertCntxt == IntPtr.Zero)
    //         {
    //             //Console.WriteLine("Couldn't create an unsigned-cert\n");
    //             return null;
    //         }
    //         try
    //         {
    //             X509Certificate cert = new X509Certificate(hCertCntxt); //create certificate object from cert context.
    //             //X509Certificate2UI.DisplayCertificate(new X509Certificate2(cert));  // display it, showing linked private key
    //             //SecureString pswd = GetSecPswd("Set PFX Password ==>");
    //             pfxblob = cert.Export(X509ContentType.Pkcs12, pswd);
    //         }
    //         catch
    //         {
    //             pfxblob = null;
    //         }
    //
    //         rsa.Clear();
    //         if (hCertCntxt != IntPtr.Zero)
    //             Win32.CertFreeCertificateContext(hCertCntxt);
    //         return pfxblob;
    //     }
    //
    //     private static IntPtr CreateUnsignedCertCntxt(String keycontainer, String provider, uint KEYSPEC, uint cspflags, String DN)
    //     {
    //         const uint AT_KEYEXCHANGE = 0x00000001;
    //         const uint AT_SIGNATURE = 0x00000002;
    //         const uint CRYPT_MACHINE_KEYSET = 0x00000020;
    //         const uint PROV_RSA_FULL = 0x00000001;
    //         const String MS_DEF_PROV = "Microsoft Base Cryptographic Provider v1.0";
    //         const String MS_STRONG_PROV = "Microsoft Strong Cryptographic Provider";
    //         const String MS_ENHANCED_PROV = "Microsoft Enhanced Cryptographic Provider v1.0";
    //         const uint CERT_CREATE_SELFSIGN_NO_SIGN = 1;
    //         const uint X509_ASN_ENCODING = 0x00000001;
    //         const uint CERT_X500_NAME_STR = 3;
    //         IntPtr hCertCntxt = IntPtr.Zero;
    //         byte[] encodedName = null;
    //         uint cbName = 0;
    //
    //         if (provider != MS_DEF_PROV && provider != MS_STRONG_PROV && provider != MS_ENHANCED_PROV)
    //             return IntPtr.Zero;
    //         if (keycontainer == "")
    //             return IntPtr.Zero;
    //         if (KEYSPEC != AT_SIGNATURE && KEYSPEC != AT_KEYEXCHANGE)
    //             return IntPtr.Zero;
    //         if (cspflags != 0 && cspflags != CRYPT_MACHINE_KEYSET) //only 0 (Current User) keyset is currently used.
    //             return IntPtr.Zero;
    //         if (DN == "")
    //             return IntPtr.Zero;
    //
    //         if (Win32.CertStrToName(X509_ASN_ENCODING, DN, CERT_X500_NAME_STR, IntPtr.Zero, null, ref cbName, IntPtr.Zero))
    //         {
    //             encodedName = new byte[cbName];
    //             Win32.CertStrToName(X509_ASN_ENCODING, DN, CERT_X500_NAME_STR, IntPtr.Zero, encodedName, ref cbName, IntPtr.Zero);
    //         }
    //
    //         CERT_NAME_BLOB subjectblob = new CERT_NAME_BLOB();
    //         subjectblob.pbData = Marshal.AllocHGlobal(encodedName.Length);
    //         Marshal.Copy(encodedName, 0, subjectblob.pbData, encodedName.Length);
    //         subjectblob.cbData = encodedName.Length;
    //
    //         CRYPT_KEY_PROV_INFO pInfo = new CRYPT_KEY_PROV_INFO();
    //         pInfo.pwszContainerName = keycontainer;
    //         pInfo.pwszProvName = provider;
    //         pInfo.dwProvType = PROV_RSA_FULL;
    //         pInfo.dwFlags = cspflags;
    //         pInfo.cProvParam = 0;
    //         pInfo.rgProvParam = IntPtr.Zero;
    //         pInfo.dwKeySpec = KEYSPEC;
    //
    //         hCertCntxt = Win32.CertCreateSelfSignCertificate(IntPtr.Zero, ref subjectblob, CERT_CREATE_SELFSIGN_NO_SIGN, ref pInfo, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
    //         if (hCertCntxt == IntPtr.Zero)
    //             throw new Win32Exception(Marshal.GetLastWin32Error());
    //         Marshal.FreeHGlobal(subjectblob.pbData);
    //         return hCertCntxt;
    //     }
    //
    //     //private static SecureString GetSecPswd(String prompt)
    //     //{
    //     //    SecureString password = new SecureString();
    //
    //     //    Console.ForegroundColor = ConsoleColor.Gray;
    //     //    Console.Write(prompt);
    //     //    Console.ForegroundColor = ConsoleColor.Magenta;
    //
    //     //    while (true)
    //     //    {
    //     //        ConsoleKeyInfo cki = Console.ReadKey(true);
    //     //        if (cki.Key == ConsoleKey.Enter)
    //     //        {
    //     //            Console.ForegroundColor = ConsoleColor.Gray;
    //     //            Console.WriteLine();
    //     //            return password;
    //     //        }
    //     //        else if (cki.Key == ConsoleKey.Backspace)
    //     //        {
    //     //            // remove the last asterisk from the screen...
    //     //            if (password.Length > 0)
    //     //            {
    //     //                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
    //     //                Console.Write(" ");
    //     //                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
    //     //                password.RemoveAt(password.Length - 1);
    //     //            }
    //     //        }
    //     //        else if (cki.Key == ConsoleKey.Escape)
    //     //        {
    //     //            Console.ForegroundColor = ConsoleColor.Gray;
    //     //            Console.WriteLine();
    //     //            return password;
    //     //        }
    //     //        else if (Char.IsLetterOrDigit(cki.KeyChar) || Char.IsSymbol(cki.KeyChar))
    //     //        {
    //     //            if (password.Length < 20)
    //     //            {
    //     //                password.AppendChar(cki.KeyChar);
    //     //                Console.Write("*");
    //     //            }
    //     //            else
    //     //            {
    //     //                Console.Beep();
    //     //            }
    //     //        }
    //     //        else
    //     //        {
    //     //            Console.Beep();
    //     //        }
    //     //    }
    //     //}
    //
    //     private static bool CompareBytearrays(byte[] a, byte[] b)
    //     {
    //         if (a.Length != b.Length)
    //             return false;
    //         int i = 0;
    //         foreach (byte c in a)
    //         {
    //             if (c != b[i])
    //                 return false;
    //             i++;
    //         }
    //         return true;
    //     }
    //
    //     //private static void showRSAProps(RSACryptoServiceProvider rsa)
    //     //{
    //     //    //Console.WriteLine("RSA CSP key information:");
    //     //    CspKeyContainerInfo keyInfo = rsa.CspKeyContainerInfo;
    //     //    //Console.WriteLine("Accessible property: " + keyInfo.Accessible);
    //     //    //Console.WriteLine("Exportable property: " + keyInfo.Exportable);
    //     //    //Console.WriteLine("HardwareDevice property: " + keyInfo.HardwareDevice);
    //     //    //Console.WriteLine("KeyContainerName property: " + keyInfo.KeyContainerName);
    //     //    //Console.WriteLine("KeyNumber property: " + keyInfo.KeyNumber.ToString());
    //     //    //Console.WriteLine("MachineKeyStore property: " + keyInfo.MachineKeyStore);
    //     //    //Console.WriteLine("Protected property: " + keyInfo.Protected);
    //     //    //Console.WriteLine("ProviderName property: " + keyInfo.ProviderName);
    //     //    //Console.WriteLine("ProviderType property: " + keyInfo.ProviderType);
    //     //    //Console.WriteLine("RandomlyGenerated property: " + keyInfo.RandomlyGenerated);
    //     //    //Console.WriteLine("Removable property: " + keyInfo.Removable);
    //     //    //Console.WriteLine("UniqueKeyContainerName property: " + keyInfo.UniqueKeyContainerName);
    //     //}
    //
    //     //private static void showBytes(String info, byte[] data)
    //     //{
    //     //    //Console.WriteLine("{0} [{1} bytes]", info, data.Length);
    //     //    for (int i = 1; i <= data.Length; i++)
    //     //    {
    //     //        Console.Write("{0:X2} ", data[i - 1]);
    //     //        if (i % 16 == 0)
    //     //            Console.WriteLine();
    //     //    }
    //     //    //Console.WriteLine("\n\n");
    //     //}
    //
    //     private static byte[] GetFileBytes(String filename)
    //     {
    //         if (!File.Exists(filename))
    //             return null;
    //         using (Stream stream = new FileStream(filename, FileMode.Open))
    //         {
    //             int datalen = (int)stream.Length;
    //             byte[] filebytes = new byte[datalen];
    //             stream.Seek(0, SeekOrigin.Begin);
    //             stream.Read(filebytes, 0, datalen);
    //             stream.Close();
    //             return filebytes;
    //         }
    //     }
    //
    //     private static void PutFileBytes(String outfile, byte[] data, int bytes)
    //     {
    //         if (bytes > data.Length)
    //         {
    //             //Console.WriteLine("Too many bytes");
    //             return;
    //         }
    //         using (FileStream fs = new FileStream(outfile, FileMode.Create))
    //         {
    //             fs.Write(data, 0, bytes);
    //             fs.Close();
    //         }
    //     }
    //
    //     //private static void showWin32Error(int errorcode)
    //     //{
    //     //    Win32Exception myEx = new Win32Exception(errorcode);
    //     //    Console.ForegroundColor = ConsoleColor.Red;
    //     //    //Console.WriteLine("Error code:\t 0x{0:X}", myEx.ErrorCode);
    //     //    //Console.WriteLine("Error message:\t {0}\n", myEx.Message);
    //     //    Console.ForegroundColor = ConsoleColor.Gray;
    //     //}
    // }

  
}