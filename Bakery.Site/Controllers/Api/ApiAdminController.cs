using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bakery.Model;

namespace Bakery.Site.Controllers.Api
{
    [ApiController]
    [MultipleSubmit]
    [AdminHeaderToken("Login")]
    [Produces("application/json")]
    [Route("ApiAdmin/[action]")]
    public class ApiAdminController : ControllerBase
    {
        private Model.AdminModel AdminCurrent()
        {
            return HttpContext.Items[AdminHeaderToken.ItemKey] as Model.AdminModel;
        }

        private void AdminLog(int adminId, EAdminLogType type, HttpRequest request, string memo = "")
        {
            BLL.AdminLogBLL.Insert(new Model.AdminLogModel
            {
                AdminLogId = Global.Generator.DateId(2),
                AdminLogType = type,
                AdminId = adminId,
                CreateDate = DateTime.Now,
                ClientIP = Global._ClientIP(request).ToString(),
                UserAgent = request.Headers["User-Agent"].ToString(),
                AcceptLanguage = request.Headers["Accept-Language"].ToString(),
                Memo = memo,
            });
        }

        [HttpPost]
        public IActionResult Login([FromForm] string account, [FromForm] string password)
        {
            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password))
                return ApiResult.RCode(ApiResult.ECode.DataFormatError);

            var admin = BLL.AdminBLL.QueryModelByAccount(account);
            if (admin != null && string.Equals(admin.Password, Global.Hash(password, admin.Salt),StringComparison.OrdinalIgnoreCase))
            {
                admin.ClientKey = Global.Generator.Guid();
                BLL.AdminBLL.Update(admin);
                
                Response.Headers[AdminHeaderToken.TokenKey] = HeaderToken.ToToken( new HeaderToken.TokenData()
                {
                    Id = admin.AdminId.ToString(),
                    Key = admin.ClientKey
                });
                
                admin.ClientKey = "";
                admin.Salt = "";
                admin.Password = "";

                AdminLog(admin.AdminId, EAdminLogType.Login, Request, "admin login");

                return ApiResult.RData(admin);
            }
            else
            {
                return ApiResult.RCode(ApiResult.ECode.AuthorizationFailed);
            }
        }

        
        [HttpPost]
        public IActionResult ConfigLoad()
        {
            AdminModel admin = AdminCurrent();

            return ApiResult.RData(SiteContext.Config);
        }
        
        [HttpPost]
        public IActionResult ConfigSave(SiteContext.ConfigModel config)
        {
            AdminModel admin = AdminCurrent();

            if (config != null)
            {
                config.SiteLogo =  SiteContext.Resource.MoveTempFile(config.SiteLogo);
                
                SiteContext.Config = config;

                AdminLog(admin.AdminId, EAdminLogType.SiteConfig, Request, "siteconfig edit");

                SiteContext.ConfigSave();
            }

            return ApiResult.RData(SiteContext.Config);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFormFile([FromForm] string model, [FromForm] string name,[FromForm] string id)
        {
            if (string.IsNullOrEmpty(model) || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
                return ApiResult.RCode(ApiResult.ECode.DataFormatError);
            if (Request.Form.Files.Count == 0)
                return ApiResult.RCode(ApiResult.ECode.TargetNotExist);

            try
            {
                var files = new Dictionary<string, byte[]>();
                for (var i = 0; i < Request.Form.Files.Count; i++)
                    using (Stream stream = Request.Form.Files[i].OpenReadStream())
                    {
                        var buffer = new byte[Request.Form.Files[i].Length];
                        await stream.ReadAsync(buffer, 0, buffer.Length);
                        files.Add(Request.Form.Files[i].ContentType,
                            buffer); //  {name: {key:contenttype,value:byte[]},}
                    }

                if (string.IsNullOrWhiteSpace(id)) id = Global.Generator.DateId(2);

                ApiResult res = SiteContext.Resource.UploadFiles(model,  name, id, files);
                return new JsonResult(res);
            }
            catch
            {
                return ApiResult.RCode(ApiResult.ECode.UnKonwError);
            }
        }
        
        [HttpPost]
        public IActionResult ProductList()
        {
            return ApiResult.RData(BLL.ProductBLL.QueryList());
        }
        
        [HttpPost]
        public IActionResult ProductListSave(List<ProductModel> productList)
        {
            var admin = AdminCurrent();

            var productOriginList = BLL.ProductBLL.QueryList();
            
            foreach (ProductModel productModel in productList)
            {
                ProductModel data = productOriginList.FirstOrDefault(p => p.Id == productModel.Id);
                if (data == null)
                {
                    productModel.Id = Global.Generator.DateId(1);
                    
                    productModel.Memo = string.IsNullOrWhiteSpace(productModel.Memo) || productModel.Memo.Length > 4000
                        ? ""
                        : productModel.Memo;
                    
                    productModel.Icon = SiteContext.Resource.MoveTempFile(productModel.Icon);

                    AdminLog(admin.AdminId, EAdminLogType.Product, Request, "product insert");

                    BLL.ProductBLL.Insert(productModel);

                    productOriginList.Add(productModel);
                }
                else
                {
                    
                    data.Name = productModel.Name;
                    
                    data.Icon = SiteContext.Resource.MoveTempFile(productModel.Icon);
                    data.Price = productModel.Price;
                    data.Cost = productModel.Cost;
                    
                    data.Memo = string.IsNullOrWhiteSpace(productModel.Memo) || productModel.Memo.Length > 4000
                        ? ""
                        : productModel.Memo;
                    
                    data.Sort = productModel.Sort;
                    data.IsShow = productModel.IsShow;

                    AdminLog(admin.AdminId, EAdminLogType.Product, Request, "product edit");

                    BLL.ProductBLL.Update(data);
                }
            }

            var productIds2Remove = productOriginList.Where(p => productList.All(x => x.Id != p.Id))
                .Select(p => p.Id).ToList();
            if (productIds2Remove.Count > 0)
            {
                AdminLog(admin.AdminId, EAdminLogType.Product, Request, "product remove");

                BLL.ProductBLL.DeleteByIdsAndUserId(productIds2Remove);
                foreach (var productId in productIds2Remove)
                    productOriginList.Remove(productOriginList.FirstOrDefault(p => p.Id == productId));
            }
            
            AdminLog(admin.AdminId, EAdminLogType.Product, Request);
            
            return ApiResult.RData(productOriginList);
        }
        
    }
}