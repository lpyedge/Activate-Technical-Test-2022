using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bakery.Model;

namespace Bakery.Site
{
    public class SiteContext
    {
        public static ConfigModel Config { get; set; }
        
        public static void ConfigSave()
        {
            var configJson = Utils.JsonUtility.SerializePretty(new {Config});
            
            Global.FileSave(Config.AppData + "Config.json", configJson);
        }

        /// <summary>
        ///     Init
        /// </summary>
        public static void Inited(Microsoft.Extensions.DependencyInjection.ServiceProvider services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {

            Config = Microsoft.Extensions.Configuration.ConfigurationBinder.Get<ConfigModel>(configuration.GetSection("Config"));

            if (Config == null)
            {
                Config = new ConfigModel();
                ConfigSave();
            }

            BLL.BaseBLL.Init(SqlSugar.DbType.Sqlite, Config.AppData + "Data.db");


            if (!File.Exists(Config.AppData + "Data.db"))
            {
                BLL.BaseBLL.InitDataBase();
                BLL.BaseBLL.InitDataTables();
                
                InitDevData();
            }

            if (BLL.AdminBLL.QueryCount(p => p.IsRoot) == 0)
                BLL.AdminBLL.Insert(new AdminModel
                {
                    Account = "Bob",
                    ClientKey = Guid.NewGuid().ToString(),
                    CreateDate = DateTime.Now,
                    IsRoot = true,
                    Password = Global.Hash("Bob's_Bakery", "012345"),
                    Salt = "012345"
                });
        }

        private static void InitDevData()
        {
            var productList = new List<ProductModel>();
            for (int i = 0; i < 15; i++)
            {
                productList.Add(new ProductModel
                {
                    Id = Global.Generator.Guid()+i.ToString("00"),
                    Category = "",
                    Name = "Bread"+i.ToString("00"),
                    Memo = "Introduction of Bread"+i.ToString("00") + "<br/><b>test</b>",
                    IsShow = true,
                    Price = 5.9+i*2,
                    Cost = 3.9+i,
                    Icon = "/product_logo_default.png",
                    Sort = i,
                });
            }
            
            BLL.ProductBLL.InsertRangeAsync(productList);
        }


        public class ConfigModel { 
            [System.Text.Json.Serialization.JsonIgnore]
            public string FormatDate => "yyyy-MM-dd";

            [System.Text.Json.Serialization.JsonIgnore]
            public string AppData => AppDomain.CurrentDomain.BaseDirectory + "App_Data/";

            /// <summary>
            ///     Site Name
            /// </summary>
            public string SiteName { get; set; } = "Bob's Bakery";
            /// <summary>
            ///     Site Introduction
            /// </summary>
            public string SiteMemo { get; set; } = "Introduction of Bob's Bakery";
            /// <summary>
            ///     Site Logo
            /// </summary>
            public string SiteLogo { get; set; } = "/logo_default.png";
            
            /// <summary>
            ///     Site Template
            /// </summary>
            public ESiteTemplate SiteTemplate { get; set; } = ESiteTemplate.Template1;

            /// <summary>
            ///     Service Email
            /// </summary>
            public string ServiceEmail { get; set; } = "Bob@Bakery.store";
        }

        public static class Resource
        {
            public const string ResourcePrefix = "Resource";
            public const string Temp = "Temp";
            public const string FileSuffix = ".base64";
            public const string StoreDirectory = "Upload";

            public static ApiResult UploadFiles(string Model,  string Name, string Id, Dictionary<string, byte[]> Files)
            {
                if (!string.IsNullOrWhiteSpace(Model) && !string.IsNullOrWhiteSpace(Id) &&
                    !string.IsNullOrWhiteSpace(Name)
                    && Files != null && Files.Count > 0
                    && Files.All(p => !string.IsNullOrWhiteSpace(p.Key))
                    && Files.All(p => p.Value.Length > 0))
                {
                    if (Files.Count == 1)
                    {
                        var uripath = $"/{ResourcePrefix}_{Temp}/{Model}/{Id}/{Name}";


                        SaveTempFile(uripath, Files.First().Key, Files.First().Value);

                        return new ApiResult<string>(uripath);
                    }

                    var namelist = new List<string>();
                    for (var i = 0; i < Files.Count; i++)
                    {
                        var uripath = $"/{ResourcePrefix}_{Temp}/{Model}/{Id}/{Name}_{i.ToString("00")}";

                        SaveTempFile(uripath, Files.ElementAt(i).Key, Files.ElementAt(i).Value);

                        namelist.Add(uripath);
                    }

                    return new ApiResult<List<string>>(namelist);
                }

                return new ApiResult(ApiResult.ECode.Fail);
            }

            //文件存储至本地临时文件夹
            public static void SaveTempFile(string tempuripath, string contenttype, byte[] buffer)
            {
                if (!string.IsNullOrWhiteSpace(tempuripath) && !string.IsNullOrWhiteSpace(contenttype) &&
                    buffer.Length > 0)
                {
                    var result = new Microsoft.AspNetCore.Mvc.FileContentResult(buffer, contenttype)
                        {EnableRangeProcessing = true};
                    Utils.MemoryCacher.Set(tempuripath, result, Utils.MemoryCacher.CacheItemPriority.Normal,
                        DateTime.Now.AddMinutes(5));


                    var tempfilepath = tempuripath.Replace($"/{ResourcePrefix}_{Temp}", $"/{StoreDirectory}/{Temp}") +
                                       FileSuffix;

                    var base64str = $"data:{contenttype};base64," + Convert.ToBase64String(buffer);
                    Global.FileSave(AppDomain.CurrentDomain.BaseDirectory + tempfilepath, base64str, false); //保存数据到服务器
                }
            }

            public static string MoveTempFile(string tempuripath, bool overwrite = true) //将文件转移到正式目录下
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(tempuripath) && tempuripath.StartsWith($"/{ResourcePrefix}_{Temp}"))
                    {
                        var uripath = tempuripath.Replace($"/{ResourcePrefix}_{Temp}", $"/{ResourcePrefix}");
                        var filepath = tempuripath.Replace($"/{ResourcePrefix}_{Temp}", $"/{StoreDirectory}") +
                                       FileSuffix;
                        var tempfilepath =
                            tempuripath.Replace($"/{ResourcePrefix}_{Temp}", $"/{StoreDirectory}/{Temp}") + FileSuffix;

                        var tempfile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + tempfilepath);
                        if (tempfile.Exists)
                        {
                            var destfile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + filepath);
                            if (destfile.Exists && !overwrite) return uripath;

                            if (!destfile.Directory.Exists) //目标文件夹不存在则创建
                                destfile.Directory.Create();

                            tempfile.MoveTo(destfile.FullName, overwrite); //转移文件导目标路径
                            Utils.MemoryCacher.Remove(tempuripath);
                            Utils.MemoryCacher.Remove(uripath);
                        }

                        return uripath;
                    }
                }
                catch (Exception ex)
                {
                }

                return tempuripath;
            }

            public static Microsoft.AspNetCore.Mvc.ActionResult Result(string Model, string Id, string Name,
                bool istemp)
            {
                if (!string.IsNullOrWhiteSpace(Model) && !string.IsNullOrWhiteSpace(Id) &&
                    !string.IsNullOrWhiteSpace(Name))
                {
                    var uripath = istemp
                        ? $"/{ResourcePrefix}_{Temp}/{Model}/{Id}/{Name}"
                        : $"/{ResourcePrefix}/{Model}/{Id}/{Name}";
                    var filepath = istemp
                        ? $"/{StoreDirectory}/{Temp}/{Model}/{Id}/{Name}{FileSuffix}"
                        : $"/{StoreDirectory}/{Model}/{Id}/{Name}{FileSuffix}";

                    Microsoft.AspNetCore.Mvc.FileContentResult result = null;
                    if (!Utils.MemoryCacher.TryGet(uripath, out result))
                    {
                        var file = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + filepath);
                        if (file.Exists)
                            using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read))
                            {
                                var byteArray = new byte[fs.Length];
                                fs.Read(byteArray, 0, byteArray.Length);
                                var base64str = Encoding.UTF8.GetString(byteArray);
                                var content = base64str.Split(",")[1];
                                var contentbytes = Convert.FromBase64String(content);
                                var contenttype = Global.Regex.FileContentType.Match(base64str).Groups[1].Value;
                                result = new Microsoft.AspNetCore.Mvc.FileContentResult(contentbytes, contenttype)
                                    {LastModified = file.LastWriteTime, EnableRangeProcessing = true};
                                Utils.MemoryCacher.Set(uripath, result, Utils.MemoryCacher.CacheItemPriority.Normal,
                                    DateTime.Now.AddMinutes(5));
                            }
                    }

                    if (result != null) return result;
                }

                return new Microsoft.AspNetCore.Mvc.NotFoundResult();
            }
        }
    }
}