using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI.API
{
    public class RequestProcessor : IRequestProcessor
    {
        public RouteInfo GetRoutInfo(RouteData routeData)
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
    }
}
