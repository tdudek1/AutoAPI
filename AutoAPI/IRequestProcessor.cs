using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AutoAPI
{
    public interface IRequestProcessor
    {
        object GetData(HttpRequest request, Type type);

        bool Validate(ControllerBase controllerBase, object entity);

        bool Authorize(ClaimsPrincipal user, string policy, IAuthorizationService authorizationService);

        RouteInfo GetRoutInfo(HttpRequest request);
    }
}
