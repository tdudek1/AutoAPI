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
        public static readonly List<Operator> Operators = new List<Operator>()
        {
            new Operator() { Name = "eq", SupportsString = true, SupportsValueType = true , SupportsGuid = true, Expression = "{propertyName} == @{index}"},
            new Operator() { Name = "neq", SupportsString = true, SupportsValueType = true , SupportsGuid = true, Expression = "{propertyName} != @{index}"},
            new Operator() { Name = "like", SupportsString = true, SupportsValueType = false , SupportsGuid = false, Expression = "{propertyName}.Contains(@{index})"},
            new Operator() { Name = "nlike", SupportsString = true, SupportsValueType = false , SupportsGuid = false, Expression = "!{propertyName}.Contains(@{index})"},
            new Operator() { Name = "lt", SupportsString = false, SupportsValueType = true , SupportsGuid = false, Expression = "{propertyName} < @{index}"},
            new Operator() { Name = "lteq", SupportsString = false, SupportsValueType = true , SupportsGuid = false, Expression = "{propertyName} <= @{index}"},
            new Operator() { Name = "gt", SupportsString = false, SupportsValueType = true , SupportsGuid = false, Expression = "{propertyName} > @{index}"},
            new Operator() { Name = "gteq", SupportsString = false, SupportsValueType = true , SupportsGuid = false, Expression = "{propertyName} >= @{index}"},
            new Operator() { Name = "in", SupportsString = true, SupportsValueType = true , SupportsGuid = true, Expression = "@{index}.Contains({propertyName})"},
            new Operator() { Name = "nin", SupportsString = true, SupportsValueType = true , SupportsGuid = true, Expression = "!@{index}.Contains({propertyName})"}
        };

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
                        EntityPolicy = a.EntityPolicy,
                        Authorize = a.Authorize,
                        DbSet = p,
                        EntityType = g.First(),
                        Properties = g.First().GetProperties().Where(x => x.PropertyType.IsTypeSupported()).ToList(),
                        NavigationProperties = g.First().GetProperties().Where(x => !x.PropertyType.IsTypeSupported()).ToList(),
                        Id = g.First().GetProperties().Where(x => x.IsDefined(typeof(KeyAttribute))).FirstOrDefault(),
                        DbContextType = typeof(T),
                        ExposePagedResult = a.ExposePagedResult
                    }).ToList();
        }

        public static bool IsOperatorSuported(this Type type, string comparisonOperator)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(string) && Operators.Any(x=>x.SupportsString && x.Name.Equals(comparisonOperator,StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }

            if (type.IsValueType && Operators.Any(x => x.SupportsValueType && x.Name.Equals(comparisonOperator, StringComparison.InvariantCultureIgnoreCase)) && type != typeof(Guid))
            {
                return true;
            }

            if (type == typeof(Guid) && Operators.Any(x => x.SupportsGuid && x.Name.Equals(comparisonOperator, StringComparison.InvariantCultureIgnoreCase)))
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



        static public void Main(String[] args)
        {
            throw new Exception("This is library");
        }

    }
}