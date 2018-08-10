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

        private const string FILTERPREFIX = "filter";
        private const string SORTPREFIX = "sort";
        private const string PAGINGPREFIX = "page";

        public RouteInfo GetRoutInfo(RouteData routeData, HttpRequest request)
        {
            var result = new RouteInfo();

            var route = routeData.Values["query"].ToString().Split("/", StringSplitOptions.RemoveEmptyEntries);

            if (route.Length == 0)
                return result;

            var apiEntity = APIConfiguration.AutoAPIEntityCache.Where(x => x.Route == route[0]).FirstOrDefault();

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
                var filterResult = GetFilter(apiEntity, request.Query);
                result.FilterExpression = filterResult.Expression;
                result.FilterValues = filterResult.Values;

                result.SortExpression = GetSort(apiEntity, request.Query);

                var pageResult = GetPaging(request.Query);
                result.Take = pageResult.Take;
                result.Skip = pageResult.Skip;

                if (result.FilterExpression != null || result.SortExpression != null || result.Take != 0)
                    result.HasModifiers = true;

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

        public (string Expression, object[] Values) GetFilter(APIEntity entity, IQueryCollection queryString)
        {
            var filters = GetOperationData(entity, queryString, FILTERPREFIX, (input, type) => Convert.ChangeType(input, type));

            if (filters.Count > 0)
            {
                return (String.Join(" && ", filters.Keys.Select((x, i) => $"{x} == @{i}").ToArray()), filters.Values.ToArray());
            }
            else
            {
                return (null, null);
            }
        }

        public (int Take, int Skip) GetPaging(IQueryCollection queryString)
        {
            var pageSize = 0U;
            var page = 1U;

            foreach (var key in queryString.Keys.Where(x => x.ToLower().StartsWith("page")))
            {
                if (key.ToLower() == "pagesize")
                {
                    uint.TryParse(queryString[key].ToString(), out pageSize);
                }

                if (key.ToLower() == "page")
                {
                    uint.TryParse(queryString[key].ToString(), out page);
                }
            }

            return ((int)pageSize, (int)((page - 1U) * pageSize));

        }

        public string GetSort(APIEntity entity, IQueryCollection queryString)
        {
            var sorts = GetOperationData(entity, queryString, SORTPREFIX, (input, type) =>
            {
                switch (input.ToLower())
                {
                    case "desc":
                    case "1":
                    case "descending":
                        return "desc";
                    default:
                        return "asc";
                }
            });

            if (sorts.Count > 0)
            {
                return string.Join(", ", sorts.Select(x => $"{x.Key} {(string)x.Value}").ToArray());
            }
            else
            {
                return null;
            }
        }

        public bool Validate(ControllerBase controllerBase, object entity)
        {
            return controllerBase.TryValidateModel(entity);
        }

        private string GetFilterProperty(string key)
        {
            return key.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        private Dictionary<string, object> GetOperationData(APIEntity entity, IQueryCollection queryString, string prefix, Func<string, Type, Object> valueConverter)
        {
            var result = new Dictionary<string, object>();

            foreach (var key in queryString.Keys.Where(x => x.ToLower().StartsWith(prefix)))
            {
                var filterName = GetFilterProperty(key);
                var property = entity.Properties.Where(x => x.Name.ToLower() == filterName.ToLower()).FirstOrDefault();

                if (property != null)
                {
                    result.Add(property.Name, valueConverter(queryString[key].ToString(), property.PropertyType));
                }
            }

            return result;
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
