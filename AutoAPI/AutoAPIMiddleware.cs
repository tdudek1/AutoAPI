using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
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
            var routeInfo = requestProcessor.GetRoutInfo(context.Request);
            var result = new ObjectResult(null);

            if (routeInfo.Entity != null)
            {
                DbContext dbContext = (DbContext)serviceProvider.GetService(routeInfo.Entity.DbContextType);
                var controller = new AutoAPIController(dbContext, requestProcessor);
                switch (context.Request.Method)
                {
                    case "GET":
                        result = controller.Get(routeInfo);
                        break;

                }

                var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
                var executor = serviceProvider.GetRequiredService<IActionResultExecutor<ObjectResult>>();
                await executor.ExecuteAsync(actionContext, result);
            }
            else
            {
                await next(context);
            }
        }
    }
}
