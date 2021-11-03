using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
        RouteInfo GetRoutInfo(HttpRequest request);

        Task<object> GetData(HttpRequest request, Type type);

        IRestAPIController GetController(ActionContext actionContext, Type dbContextType);

        IActionResultExecutor<JsonResult> GetActionExecutor();
    }
}
