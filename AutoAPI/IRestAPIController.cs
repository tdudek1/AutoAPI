using Microsoft.AspNetCore.Mvc;

namespace AutoAPI
{
    public interface IRestAPIController
    {
        JsonResult Delete(RouteInfo routeInfo);
        JsonResult Get(RouteInfo routeInfo);
        JsonResult Post(RouteInfo routeInfo, object entity);
        JsonResult Put(RouteInfo routeInfo, object entity);
    }
}