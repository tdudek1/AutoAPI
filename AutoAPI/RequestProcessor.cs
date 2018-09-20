using AutoAPI.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace AutoAPI
{
    public class RequestProcessor : IRequestProcessor
    {
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
            {
                return true;
            }
            else
            {
                var result = authorizationService.AuthorizeAsync(claimsPrincipal, policy).Result;
                return result.Succeeded;
            }
        }

        public RouteInfo GetRoutInfo(HttpRequest request)
        {
            PathString id;
            var result = new RouteInfo();

            var apiEntity = APIConfiguration.AutoAPIEntityCache.Where(x => request.Path.StartsWithSegments(x.Route, out id)).FirstOrDefault();

            result.Entity = apiEntity;

            if (id.HasValue)
            {
                result.Id = id.Value.TrimStart('/');
            }
            else if (request.Query?.Keys.Count > 0)
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
    }
}
