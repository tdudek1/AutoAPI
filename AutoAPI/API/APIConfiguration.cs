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
    public static class APIConfiguration
    {
        public static List<APIEntity> AutoAPIEntityCache = new List<APIEntity>();

        public static void AddAutoAPI<T>(this IServiceCollection serviceCollection)
        {
            AutoAPIEntityCache = Init<T>();
        }

        public static List<APIEntity> Init<T>()
        {
            return (from p in typeof(T).GetProperties()
                    let g = p.PropertyType.GetGenericArguments()
                    where p.IsDefined(typeof(AutoAPIEntity)) && g.Count() == 1
                    select new APIEntity() { Route = p.GetCustomAttribute<AutoAPIEntity>().Route, DbSet = p, EntityType = g.First(), Properties = g.First().GetProperties().ToList(), Id = g.First().GetProperties().Where(x => x.IsDefined(typeof(KeyAttribute))).FirstOrDefault() }).ToList();
        }


    }
}