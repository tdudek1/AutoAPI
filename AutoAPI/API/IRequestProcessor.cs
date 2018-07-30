using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI.API
{
    public interface IRequestProcessor
    {
        RouteInfo GetRoutInfo(RouteData routeData);

        object GetData(HttpRequest request, Type type);
    }
}
