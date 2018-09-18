using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AutoAPI
{

    public class AutoAPIMiddleware
    {
        private readonly RequestDelegate next;

        public AutoAPIMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider, IRequestProcessor requestProcessor)
        {

            if (context.Request.Path == "/test")
            {
                var routeData = context.GetRouteData() ?? new RouteData();
                var actionContext = new ActionContext(context, routeData, new ActionDescriptor());
                var executor = serviceProvider.GetRequiredService<IActionResultExecutor<ObjectResult>>();
                await executor.ExecuteAsync(actionContext, new OkObjectResult(new { ok = "ok" }));
            }
            else
            {
                await next(context);
            }
        }
    }
}
