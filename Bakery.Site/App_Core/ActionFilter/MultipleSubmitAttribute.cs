using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Bakery.Site
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class MultipleSubmitAttribute : ActionFilterAttribute
    {
        private readonly string[] _Ignoreactions;

        /// <summary>
        /// 允许重复提交的时间间隔（毫秒）
        /// </summary>
        public int submitInterval { get; set; } = 500;

        /// <summary>
        ///     可以传参传入要忽略的Action名称,传入的Action不会执行判断
        /// </summary>
        /// <param name="ignoreactions"></param>
        public MultipleSubmitAttribute(params string[] ignoreactions)
        {
            _Ignoreactions = ignoreactions.Select(p => p.ToLowerInvariant()).ToArray();
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var controller = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context?.ActionDescriptor;
            if (controller != null)
            {
                var actionname = controller.ActionName.ToLowerInvariant();
                if (!_Ignoreactions.Contains(actionname))
                {
                    var clientipaddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
                    var clientuseragent = context.HttpContext.Request.Headers["User-Agent"].ToString();
                    var querystr = context.HttpContext.Request.QueryString.Value;

                    var formstr = string.Equals(context.HttpContext.Request.Method, "POST", StringComparison.OrdinalIgnoreCase) ?
                        (context.HttpContext.Request.HasFormContentType ? context.HttpContext.Request.Form.Aggregate("", (s, item) => s += item.Key + "=" + item.Value.ToString() + "&") : "")
                        : "";
                    var requesthash = Global.Hash(context.HttpContext.Request.Path.Value + "$" + clientipaddress + "$" + clientuseragent + "$" + querystr + "$" + formstr);

                    if (Utils.MemoryCacher.TryGet(requesthash, out object data))
                    {
                        if (string.Equals(context.HttpContext.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Result = ApiResult.RCode( "通信接口繁忙" );
                        }
                        else
                        {
                            context.Result = new ForbidResult();
                        }
                    }
                    Utils.MemoryCacher.Set(requesthash, this, Utils.MemoryCacher.CacheItemPriority.Low, null, TimeSpan.FromMilliseconds(submitInterval));
                }
            }
        }
    }
}