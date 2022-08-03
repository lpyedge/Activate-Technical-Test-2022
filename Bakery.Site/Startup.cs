using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Bakery.Site
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                // .AddNewtonsoftJson(options =>
                // {
                //     Global.Json.Settings.JsonSerializerSettingsAction(options.SerializerSettings);
                // })
                .AddJsonOptions(options =>
                {
                    Utils.JsonUtility.Settings.JsonSerializerSettingsAction(options.JsonSerializerOptions);
                    //首字母小写驼峰式命名
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                })
                .AddRazorRuntimeCompilation();

            #region 添加 压缩规则和支持类型

            //一定记得在Configure方法中调用 app.UseResponseCompression(); 开启并使用压缩规则
            services.AddResponseCompression(options =>
                {
                    //https请求下是否开启压缩
                    options.EnableForHttps = true;
                    //添加Brotli压缩支持
                    options.Providers.Add<BrotliCompressionProvider>();
                    //配置Gzip压缩支持
                    options.Providers.Add<GzipCompressionProvider>();
                    //针对哪些下发数据类型开启压缩 
                    //ResponseCompressionDefaults.MimeTypes  常见需要开启压缩的数据类型
                    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                    {
                        "application/xhtml+xml",
                        "application/atom+xml",
                        "image/*",
                        "video/*",
                    });
                })
                //配置Brotli压缩等级
                .Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; })
                //配置Gzip压缩等级
                .Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });

            #endregion

            #region 添加缓存规则

            services.AddResponseCaching();

            #endregion

            #region 添加 路由规则

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true; //网址小写 
                options.AppendTrailingSlash = false; //添加反斜杠
            });

            #endregion

            //注册全局HttpContext并缓存全局Configuration
            Global.AppService.Inited = SiteContext.Inited;
            Global.AppService.ConfigureServices(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            #region 开启并使用压缩规则

            app.UseResponseCompression();

            #endregion

            #region 开启并使用http缓存

            //在 app.UseRouting();app.UseAuthorization();之前
            app.UseResponseCaching();

            #endregion

            #region 开启并使用默认静态文件访问

            app.UseStaticFiles();
            
            #endregion

            #region 开启并使用自定义静态文件访问

            if(!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "Home")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "Home"));
            }

            app.UseStaticFiles(new StaticFileOptions()
            {
                //通过配置FileProvider和RequestPath实现某个目录映射某个路径的访问
                //linux下大小写敏感，注意目录名称
                FileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "Home")),
                RequestPath = "/themes",


                //通过配置ContentTypeProvider设置Mime和文件后缀的对应关系,方便一些特殊类型的文件可以被访问
                //下面设置允许访问图片类型的文件
                ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string>
                {
                    {".otf", "application/octet-stream"},
                    {".eot", "application/octet-stream"},
                    {".woff", "application/octet-stream"},
                    {".ttf", "application/octet-stream"},
                    {".svg", "image/svg+xml"},
                    {".png", "image/png"},
                    {".jpg", "image/jpeg"},
                    {".gif", "image/gif"},
                    {".bmp", "image/bmp"},
                    {".css", "text/css"},
                }),

                HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress,
            });

            #endregion


            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });
        }
    }
}