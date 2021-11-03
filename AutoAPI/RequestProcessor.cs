using AutoAPI.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public async Task<object> GetData(HttpRequest request, Type type)
        {
            object result;
            if (APIConfiguration.AutoAPIOptions.UseNewtonoftSerializer)
            {
                var jsonString = string.Empty;
                using (var sr = new StreamReader(request.Body))
                {
                    jsonString = sr.ReadToEnd();
                }
                if (APIConfiguration.AutoAPIOptions.JsonSerializerSettings != null)
                {
                    result = JsonConvert.DeserializeObject(jsonString, APIConfiguration.AutoAPIOptions.JsonSerializerSettings);
                }
                else
                {
                    result = JsonConvert.DeserializeObject(jsonString);
                }

                return Task.FromResult<object>(result);
            }
            else
            {
                if(APIConfiguration.AutoAPIOptions.JsonSerializerOptions != null)
                    return await System.Text.Json.JsonSerializer.DeserializeAsync(request.Body, type, APIConfiguration.AutoAPIOptions.JsonSerializerOptions);
                else
                    return await System.Text.Json.JsonSerializer.DeserializeAsync(request.Body, type);
            }
        }


        public RouteInfo GetRoutInfo(HttpRequest request)
        {
            PathString path = null;
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

        public IActionResultExecutor<JsonResult> GetActionExecutor() 
        {
            return serviceProvider.GetRequiredService<IActionResultExecutor<JsonResult>>();
        }
    }
}
