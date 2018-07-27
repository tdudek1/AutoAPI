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
    public static class Extensions
    {
        public static List<APIEntity> AutoAPIEntityCache = new List<APIEntity>();

        public static void AddAutoAPI<T>(this IServiceCollection serviceCollection)
        {
            AutoAPIEntityCache = (from p in typeof(T).GetProperties()
                                  let g = p.PropertyType.GetGenericArguments()
                                  where p.IsDefined(typeof(AutoAPIEntity)) && g.Count() == 1
                                  select new APIEntity() { Route = p.GetCustomAttribute<AutoAPIEntity>().Route, DbSet = p, EntityType = g.First(), Properties = g.First().GetProperties().ToList(), Id = g.First().GetProperties().Where(x=> x.IsDefined(typeof(KeyAttribute))).FirstOrDefault() }).ToList();

        }

        public static RouteInfo GetRoutInfo(this ControllerBase controller)
        {
            var result = new RouteInfo();

            var route = controller.RouteData.Values["query"].ToString().Split("/", StringSplitOptions.RemoveEmptyEntries);

            if (route.Length == 0)
                return result;

            var apiEntity = AutoAPIEntityCache.Where(x => x.Route == route[0]).FirstOrDefault();

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

        private static object GetData(Stream stream, Type type)
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
