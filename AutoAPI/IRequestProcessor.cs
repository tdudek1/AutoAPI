using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI
{
    public interface IRequestProcessor
    {
        RouteInfo GetRoutInfo(RouteData routeData, HttpRequest request = null);

        object GetData(HttpRequest request, Type type);

        bool Validate(ControllerBase controllerBase, object entity);
    }
}
