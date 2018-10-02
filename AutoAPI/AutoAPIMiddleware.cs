using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using System;
using System.Security.Claims;
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

        public async Task InvokeAsync(HttpContext context, IRequestProcessor requestProcessor, IAuthorizationService authorizationService)
        {
            var routeInfo = requestProcessor.GetRoutInfo(context.Request);
            var result = new ObjectResult(null);

            if (routeInfo.Entity != null)
            {
                if (!IsAuthorized(authorizationService, context.User, routeInfo.Entity, context.Request.Method))
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                var executor = requestProcessor.GetActionExecutor();

                var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
                var controller = requestProcessor.GetController(actionContext, routeInfo.Entity.DbContextType);

                switch (context.Request.Method)
                {
                    case "GET":
                        result = controller.Get(routeInfo);
                        break;
                    case "POST":
                        result = controller.Post(routeInfo, requestProcessor.GetData(context.Request, routeInfo.Entity.EntityType));
                        break;
                    case "PUT":
                        result = controller.Put(routeInfo, requestProcessor.GetData(context.Request, routeInfo.Entity.EntityType));
                        break;
                    case "DELETE":
                        result = controller.Delete(routeInfo);
                        break;
                    default:
                        throw new NotSupportedException("Http Method not supported");
                }

                await executor.ExecuteAsync(actionContext, result);
            }
            else
            {
                await next(context);
            }
        }

        public bool IsAuthorized(IAuthorizationService authorizationService, ClaimsPrincipal claimsPrincipal, APIEntity aPIEntity, String method)
        {
            if (!claimsPrincipal.Identity.IsAuthenticated && aPIEntity.Authorize)
            {
                return false;
            }

            if (!String.IsNullOrWhiteSpace(aPIEntity.EntityPolicy) && !Authorize(authorizationService, claimsPrincipal, aPIEntity.EntityPolicy))
            {
                return false;
            }

            var methodPolicy = "";

            switch (method)
            {
                case "GET":
                    methodPolicy = aPIEntity.GETPolicy;
                    break;
                case "POST":
                    methodPolicy = aPIEntity.POSTPolicy;
                    break;
                case "PUT":
                    methodPolicy = aPIEntity.PUTPolicy;
                    break;
                case "DELETE":
                    methodPolicy = aPIEntity.DELETEPolicy;
                    break;
            }

            if (!String.IsNullOrWhiteSpace(methodPolicy) && !Authorize(authorizationService, claimsPrincipal, methodPolicy))
            {
                return false;
            }

            return true;
        }

        protected virtual bool Authorize(IAuthorizationService authorizationService, ClaimsPrincipal claimsPrincipal, string policy)
        {
            return authorizationService.AuthorizeAsync(claimsPrincipal, policy).Result.Succeeded;
        }

    }
}
