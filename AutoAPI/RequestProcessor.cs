using AutoAPI.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace AutoAPI
{
    public class RequestProcessor : IRequestProcessor
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IObjectModelValidator objectModelValidator;
        public RequestProcessor(IServiceProvider serviceProvider,IObjectModelValidator objectModelValidator)
        {
            this.serviceProvider = serviceProvider;
            this.objectModelValidator = objectModelValidator;
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

        public IRestAPIController GetController(ActionContext actionContext,Type dbContextType)
        {
            var dbContext = (DbContext)serviceProvider.GetService(dbContextType);
            return new RESTAPIController(dbContext, actionContext, this.objectModelValidator);
        }

        public IActionResultExecutor<ObjectResult> GetActionExecutor()
        {
            return serviceProvider.GetRequiredService<IActionResultExecutor<ObjectResult>>();
        }
    }
}
