﻿using AutoAPI.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace AutoAPI
{
    public class RequestProcessor : IRequestProcessor
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IObjectModelValidator objectModelValidator;
        public RequestProcessor(IServiceProvider serviceProvider, IObjectModelValidator objectModelValidator)
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
            PathString path;
            var result = new RouteInfo();

            var apiEntity = APIConfiguration.AutoAPIEntityCache.Where(x => request.Path.StartsWithSegments(x.Route, out path)).FirstOrDefault();

            result.Entity = apiEntity;

            if (path.HasValue)
            {
                var value = path.Value.TrimStart('/');
                switch (value)
                {
                    case "count":
                        result.IsCount = true;
                        break;
                    case "pagedresult":
                        result.IsPageResult = apiEntity.ExposePagedResult;
                        break;
                    default:
                        result.Id = value;
                        break;
                }
            }

            if (String.IsNullOrWhiteSpace(result.Id) && request.Query?.Keys.Count > 0)
            {
                var expressionBuilder = new ExpressionBuilder(request.Query, apiEntity);

                var filterResult = expressionBuilder.BuildFilterResult();
                result.FilterExpression = filterResult.Filter;
                result.FilterValues = filterResult.Values;

                result.SortExpression = expressionBuilder.BuildSortResult();

                var pageResult = expressionBuilder.BuildPagingResult();
                result.Take = pageResult.Take;
                result.Skip = pageResult.Skip;
                result.Page = pageResult.Page;

                result.IncludeExpression = expressionBuilder.BuildIncludeResult();
            }

            return result;

        }

        public IRestAPIController GetController(ActionContext actionContext, Type dbContextType)
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
