using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoAPI.API
{
    public class RequestProcessor
    {

        private RouteInfo GetRoutInfo(ControllerBase controller)
        {
            var result = new RouteInfo();

            var route = controller.RouteData.Values["query"].ToString().Split("/", StringSplitOptions.RemoveEmptyEntries);

            if (route.Length == 0)
                return result;

            var apiEntity = RequestBuilder.AutoAPIEntityCache.Where(x => x.Route == route[0]).FirstOrDefault();

            if (apiEntity == null)
                return result;
            else
                result.Entity = apiEntity;

            if (route.Length > 1)
            {
                result.Id = route[1];
            }

            switch (controller.Request.Method)
            {
                case "POST":
                case "PUT":
                    result.Data = GetData(controller.Request.Body, apiEntity.EntityType);
                    break;
            }

            return result;
        }

        private object GetData(Stream stream, Type type)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize(jsonTextReader, type);
            }
        }
    }
}
