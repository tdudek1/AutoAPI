using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace AutoAPI
{
    public static class APIConfiguration
    {
        private static readonly List<string> stringOperators = new List<string> { "eq", "neq", "like", "nlike" };
        private static readonly List<string> valueTypeOperators = new List<string> { "eq", "neq", "lt", "gt", "gteq", "lteq" };
        private static readonly List<string> guidOperators = new List<string> { "eq", "neq" };

        public static List<APIEntity> AutoAPIEntityCache = new List<APIEntity>();

        public static void AddAutoAPI<T>(this IServiceCollection serviceCollection, string path) where T : DbContext
        {

            AutoAPIEntityCache.AddRange(Init<T>(path));
            serviceCollection.AddTransient<IRequestProcessor, RequestProcessor>();
        }

        public static List<APIEntity> Init<T>(string path) where T : DbContext
        {
            return (from p in typeof(T).GetProperties()
                    let g = p.PropertyType.GetGenericArguments()
                    let a = p.GetCustomAttribute<AutoAPIEntity>()
                    where p.IsDefined(typeof(AutoAPIEntity)) && g.Count() == 1
                    select new APIEntity()
                    {
                        Route = (new PathString(path)).Add(a.Route.StartsWith("/") ? a.Route : $"/{a.Route}"),
                        GETPolicy = a.GETPolicy,
                        POSTPolicy = a.POSTPolicy,
                        PUTPolicy = a.PUTPolicy,
                        DELETEPolicy = a.DELETEPolicy,
                        DbSet = p,
                        EntityType = g.First(),
                        Properties = g.First().GetProperties().Where(x => x.PropertyType.IsTypeSupported()).ToList(),
                        Id = g.First().GetProperties().Where(x => x.IsDefined(typeof(KeyAttribute))).FirstOrDefault(),
                        DbContextType = typeof(T)
                    }).ToList();
        }

        public static bool IsOperatorSuported(this Type type, string comparisonOperator)
        {

            if (type == typeof(string) && stringOperators.Contains(comparisonOperator))
            {
                return true;
            }

            if (type.IsValueType && valueTypeOperators.Contains(comparisonOperator))
            {
                return true;
            }

            if (type == typeof(Guid) && valueTypeOperators.Contains(comparisonOperator))
            {
                return true;
            }

            return false;
        }

        public static bool IsTypeSupported(this Type type)
        {
            if (type.IsValueType)
            {
                return true;
            }

            if (type == typeof(string) || type == typeof(Guid))
            {
                return true;
            }

            return false;
        }

        public static IApplicationBuilder UseAutoAPI(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AutoAPIMiddleware>();
        }

		public static string ToOperationID(this string path)
		{
			return string.Join("", path.Split("/",StringSplitOptions.RemoveEmptyEntries).Select(x => x.First().ToString().ToUpper() + x.Substring(1)));
		}
    }
}