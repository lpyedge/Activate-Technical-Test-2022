using Microsoft.AspNetCore.Mvc;

namespace Bakery.Site.Controllers.Api
{
    [ApiController]
    [MultipleSubmit("Resouce", "ResouceTemp")]
    [Produces("application/json")]
    [Route("Api/[action]")]
    public class ApiHomeController : ControllerBase
    {
        [ResponseCache(Location =ResponseCacheLocation.Any,Duration = 600)]
        [HttpGet("/" + SiteContext.Resource.ResourcePrefix + "/{Model}/{Id}/{Name}")]
        public dynamic Resouce(string model, string id, string name)
        {
            if (!string.IsNullOrWhiteSpace(model) && !string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(name))
                return SiteContext.Resource.Result(model, id, name, false);
            return new NotFoundResult();
        }

        [ResponseCache(NoStore = true,Location = ResponseCacheLocation.None)]
        [HttpGet("/" + SiteContext.Resource.ResourcePrefix + "_" + SiteContext.Resource.Temp + "/{Model}/{Id}/{Name}")]
        public dynamic ResouceTemp(string model, string id, string name)
        {
            if (!string.IsNullOrWhiteSpace(model) && !string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(name))
                return SiteContext.Resource.Result(model, id, name, true);
            return new NotFoundResult();
        }
    }
}