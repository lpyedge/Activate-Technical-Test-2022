using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bakery
{
    public static class Global
    {
        static Global()
        {
        }
        
        public class AppService
        {
            //nuget Microsoft.Extensions.DependencyInjection
            public static ServiceProvider ServiceProvider { get; private set; }
            public static IConfiguration Configuration { get; private set; }

            /// <summary>
            ///     HttpContext_Current
            /// </summary>
            public static HttpContext Current
            {
                get
                {
                    var factory = (HttpContextAccessor) ServiceProvider?.GetService(typeof(IHttpContextAccessor));
                    return factory?.HttpContext;
                }
            }

            /// <summary>
            ///     初始化
            /// </summary>
            /// <param name="services"></param>
            /// <param name="configuration"></param>
            public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
            {
                ServiceProvider = services.BuildServiceProvider();

                Configuration = configuration;

                if (Inited != null)
                    Inited(ServiceProvider,Configuration);
            }

            public static Action<ServiceProvider,IConfiguration> Inited;
        }
        
        private static HashAlgorithm HashSHA1Provider => Utils.HASHCrypto.Generate(Utils.HASHCrypto.CryptoEnum.SHA1);

        public static string Hash(params string[] strs)
        {
            if (strs.Length > 0)
            {
                var res = "";
                foreach (var item in strs)
                {
                    res = Utils.HASHCrypto.Encrypt(HashSHA1Provider, res + item);
                }
                return res;
            }

            return null;
        }


        public static class Generator
        {
            public static Random Random
            {
                get { return new Random(System.Guid.NewGuid().GetHashCode()); }
            }

            public static string Guid()
            {
                return System.Guid.NewGuid().ToString("N");
            }

            public static uint NextUint(uint min, uint max)
            {
                return (uint) Random.Next((int) min, (int) max + 1);
            }

            public static string Number(int length)
            {
                if (length > 0)
                {
                    var formatstr = "";
                    for (int i = 0; i < length; i++)
                    {
                        formatstr += "0";
                    }
                }

                return Random.Next(0, 999999).ToString("000000");
            }

            public static string DateId(int state = 0)
            {
                switch (state)
                {
                    case 0:
                    {
                        var ts = DateTime.Now - DateTime.Parse("2019-01-01");
                        var str = ((int) ts.TotalSeconds).ToString() + Random.Next(0, 10000).ToString("0000");
                        return Utils.Duotricemary.FromInt64(ulong.Parse(str)).StringValue;
                    }
                    case 1:
                    {
                        var str = DateTime.Now.ToString("MMddHHmmssfff") + Random.Next(0, 100).ToString("00");
                        return DateTime.Now.ToString("yy") + Utils.Duotricemary.FromInt64(ulong.Parse(str)).StringValue;
                    }
                    case 2:
                    {
                        var str = DateTime.Now.ToString("MMddHHmmssfff") + Random.Next(0, 100000).ToString("00000");
                        return DateTime.Now.ToString("yy") + Utils.Duotricemary.FromInt64(ulong.Parse(str)).StringValue;
                    }
                    default:
                    {
                        var str = DateTime.Now.ToString("HHmmssfff") + Random.Next(0, 100000000).ToString("00000000");
                        return DateTime.Now.ToString("yyMMdd") +
                               Utils.Duotricemary.FromInt64(ulong.Parse(str)).StringValue;
                    }
                }
            }

            //public static string DateId()
            //{
            //    return DateTime.UtcNow.ToString("yyMMddHHmmssfff")+GetRandom().Next(1000,10000);
            //}
        }

        public static class Regex
        {
            /// <summary>
            /// file base64 => contenttype
            /// </summary>
            public static readonly System.Text.RegularExpressions.Regex FileContentType =
                new System.Text.RegularExpressions.Regex(@"data:([\w-]+/[\w\.\-]+);base64,",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

            /// <summary>
            /// 邮箱验证
            /// </summary>
            public static readonly System.Text.RegularExpressions.Regex Email =
                new System.Text.RegularExpressions.Regex("^[A-Za-z0-9._%+-]+@(?:[A-Za-z0-9-]+.)+[A-Za-z]{2,6}$",
                    RegexOptions.Compiled);
        }

        /// <summary>
        /// Client IPAddress
        /// </summary>
        /// <returns></returns>
        public static System.Net.IPAddress _ClientIP(HttpRequest httpRequest)
        {
            System.Net.IPAddress ip = System.Net.IPAddress.None;
            if (httpRequest != null)
            {
                string ipAddress = null;

                ipAddress = httpRequest.Headers["X-Forwarded-For"];
                if (string.IsNullOrEmpty(ipAddress) ||
                    string.Equals("unknown", ipAddress, StringComparison.OrdinalIgnoreCase))
                {
                    ipAddress = httpRequest.Headers["X-Real-IP"];
                }

                if (string.IsNullOrEmpty(ipAddress) ||
                    string.Equals("unknown", ipAddress, StringComparison.OrdinalIgnoreCase))
                {
                    ipAddress = httpRequest.Headers["PROXY_FORWARDED_FOR"];
                }

                if (string.IsNullOrEmpty(ipAddress) ||
                    string.Equals("unknown", ipAddress, StringComparison.OrdinalIgnoreCase))
                {
                    ipAddress = httpRequest.Headers["Proxy-Client-IP"];
                }

                if (string.IsNullOrEmpty(ipAddress) ||
                    string.Equals("unknown", ipAddress, StringComparison.OrdinalIgnoreCase))
                {
                    ipAddress = httpRequest.Headers["WL-Proxy-Client-IP"];
                }

                if (string.IsNullOrEmpty(ipAddress) ||
                    string.Equals("unknown", ipAddress, StringComparison.OrdinalIgnoreCase))
                {
                    ipAddress = httpRequest.Headers["HTTP_CLIENT_IP"];
                }

                if (string.IsNullOrEmpty(ipAddress) ||
                    string.Equals("unknown", ipAddress, StringComparison.OrdinalIgnoreCase))
                {
                    ipAddress = httpRequest.Headers["REMOTE_ADDR"];
                }

                if (string.IsNullOrEmpty(ipAddress) ||
                    string.Equals("unknown", ipAddress, StringComparison.OrdinalIgnoreCase))
                {
                    ipAddress = httpRequest.HttpContext.Connection.RemoteIpAddress.ToString();
                }

                //对于通过多个代理的情况，第一个IP为客户端真实IP,多个IP按照','分割
                if (ipAddress != null && ipAddress.Length > 15)
                {
                    //"***.***.***.***".length() = 15
                    if (ipAddress.IndexOf(",") > 0)
                    {
                        ipAddress = ipAddress.Substring(0, ipAddress.IndexOf(","));
                    }
                }

                if (!System.Net.IPAddress.TryParse(ipAddress, out ip))
                {
                    ip = System.Net.IPAddress.None;
                }
            }

            return ip;
        }

        
        public static void FileSave(string p_Path, string p_Content, bool p_Append = true)
        {
            FileSave(p_Path, Encoding.UTF8.GetBytes(p_Content), p_Append);
        }

        public static void FileSave(string p_Path, byte[] p_Bytes, bool p_Append = true)
        {
            Task.Factory.StartNew((state) =>
            {
                var obj = (dynamic) state;
                try
                {
                    if (!obj.fileinfo.Directory.Exists)
                    {
                        obj.fileinfo.Directory.Create();
                    }

                    if (obj.append)
                    {
                        using (var fs = new FileStream(obj.fileinfo.FullName, FileMode.Append))
                        {
                            fs.Seek(0, SeekOrigin.End);
                            fs.Write(obj.bytes, 0, obj.bytes.Length);
                            fs.Flush();
                        }
                    }
                    else
                    {
                        using (var fs = new FileStream(obj.fileinfo.FullName, FileMode.Create))
                        {
                            fs.Seek(0, SeekOrigin.End);
                            fs.Write(obj.bytes, 0, obj.bytes.Length);
                            fs.Flush();
                        }
                    }
                }
                catch
                {
                }
            }, new {fileinfo = new FileInfo(p_Path), bytes = p_Bytes, append = p_Append});
        }


        public static Dictionary<int, string> EnumsDic<T>()
        {
            var res = new Dictionary<int, string>();
            var type = typeof(T);
            if (type.IsEnum)
            {
                foreach (dynamic item in Enum.GetValues(type))
                {
                    res.Add((int) item, item.ToString());
                }
            }

            return res;
        }

        public class enumOption
        {
            public string label { get; set; }
            public int value { get; set; }
        }
        
        public static List<enumOption> EnumsOptions<T>()
        {
            var res = new List<enumOption>();
            var type = typeof(T);
            if (type.IsEnum)
            {
                foreach (dynamic item in Enum.GetValues(type))
                {
                    res.Add(new enumOption {label = item.ToString(), value = (int) item});
                }
            }

            return res;
        }

        public static Dictionary<int, string> EnumsDescDic<T>()
        {
            var desctype = typeof(System.ComponentModel.DescriptionAttribute);
            var res = new Dictionary<int, string>();
            var type = typeof(T);
            if (type.IsEnum)
            {
                foreach (dynamic item in Enum.GetValues(type))
                {
                    System.Reflection.FieldInfo filed = type.GetField(item.ToString());
                    var desc = (System.ComponentModel.DescriptionAttribute) (filed.GetCustomAttributes(desctype, false)
                        .FirstOrDefault());
                    if (desc != null)
                    {
                        res.Add((int) item, desc.Description);
                    }
                }
            }

            return res;
        }
    }
}