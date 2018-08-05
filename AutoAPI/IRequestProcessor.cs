using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI
{
    public interface IRequestProcessor
    {
        RouteInfo GetRoutInfo(RouteData routeData);

        object GetData(HttpRequest request, Type type);

        (string Expression, object[] Values) GetFilter(APIEntity entity, IQueryCollection queryString);

        (uint PageSize, uint Page) GetPaging(IQueryCollection queryString);

    }
}
