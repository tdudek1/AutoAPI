using AutoAPI.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AutoAPI
{
    public class RequestProcessor : IRequestProcessor
    {
        
        public RouteInfo GetRoutInfo(RouteData routeData, HttpRequest request)
        {
            
            var result = new RouteInfo();

            var route = routeData.Values["query"]?.ToString().Split("/", StringSplitOptions.RemoveEmptyEntries);

            if (route == null || route.Length == 0)
                return result;

            var apiEntity = APIConfiguration.AutoAPIEntityCache.Where(x => x.Route?.ToLower() == route[0]?.ToLower()).FirstOrDefault();

            if (apiEntity == null)
                return result;
            else
                result.Entity = apiEntity;

            if (route.Length > 1)
            {
                result.Id = route[1];
            }

            if (request.Query?.Keys.Count > 0)
            {
                var expressionBuilder = new ExpressionBuilder(request.Query, apiEntity);

                var filterResult = expressionBuilder.BuildFilterResult();
                result.FilterExpression = filterResult.Filter;
                result.FilterValues = filterResult.Values;

                result.SortExpression = expressionBuilder.BuildSortResult();

                var pageResult = expressionBuilder.BuildPagingResult();
                result.Take = pageResult.Take;
                result.Skip = pageResult.Skip;

            }


            return result;
        }

        public object GetData(HttpRequest request, Type type)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(request.Body))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize(jsonTextReader, type);
            }
        }

        public bool Validate(ControllerBase controllerBase, object entity)
        {
            return controllerBase.TryValidateModel(entity);
        }

        public bool Authorize(ClaimsPrincipal claimsPrincipal, string policy, IAuthorizationService authorizationService)
        {
            if (string.IsNullOrWhiteSpace(policy) || authorizationService == null)
                return true;
            else
            {
                var result = authorizationService.AuthorizeAsync(claimsPrincipal, policy).Result;
                return result.Succeeded;
            }
        }
    }
}
