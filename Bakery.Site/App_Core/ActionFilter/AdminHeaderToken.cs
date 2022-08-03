using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Bakery.Site;


public sealed class AdminHeaderToken : HeaderToken
{
    public const string TokenKey = "AdminToken";

    public const string ItemKey = "Admin";

    /// <summary>
    ///     可以传参传入要忽略的Action名称,传入的Action不会执行判断
    /// </summary>
    /// <param name="ignoreactions"></param>
    public AdminHeaderToken(params string[] ignoreactions) : base(TokenKey, ItemKey, ignoreactions)
    {
        TokenToModel = (tokendata) => {
                try
                {
                    var model = BLL.AdminBLL.QueryModelById(tokendata.Id);
                    if (model != null)
                    {
                        if (model.ClientKey == tokendata.Key
#if DEBUG
                            || true
#endif
                           )
                        {
                            return model;
                        }
                    }
                }
                catch
                {
                }

                return null;
            };
    }


}
