using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AutoAPI
{
    public interface IAutoAPIController
    {
        ObjectResult Delete(RouteInfo routeInfo);
        ObjectResult Get(RouteInfo routeInfo);
        ObjectResult Post(RouteInfo routeInfo, object entity);
        ObjectResult Put(RouteInfo routeInfo, object entity);
    }
}