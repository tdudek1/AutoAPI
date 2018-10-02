using Microsoft.AspNetCore.Mvc;

namespace AutoAPI
{
    public interface IRestAPIController
    {
        ObjectResult Delete(RouteInfo routeInfo);
        ObjectResult Get(RouteInfo routeInfo);
        ObjectResult Post(RouteInfo routeInfo, object entity);
        ObjectResult Put(RouteInfo routeInfo, object entity);
    }
}