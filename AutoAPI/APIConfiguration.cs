﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoAPI
{
	public static class APIConfiguration
	{
        private static readonly List<string> stringOperators = new List<string> { "eq", "neq", "like", "nlike" };
        private static readonly List<string> valueTypeOperators = new List<string> { "eq", "neq", "lt", "gt", "gteq", "lteq" };
        private static readonly List<string> guidOperators = new List<string> { "eq", "neq" };

        public static List<APIEntity> AutoAPIEntityCache = new List<APIEntity>();

		public static void AddAutoAPI<T>(this IServiceCollection serviceCollection)
		{
			AutoAPIEntityCache = Init<T>();
		}

		public static List<APIEntity> Init<T>()
		{
			return (from p in typeof(T).GetProperties()
					let g = p.PropertyType.GetGenericArguments()
					let a = p.GetCustomAttribute<AutoAPIEntity>()
					where p.IsDefined(typeof(AutoAPIEntity)) && g.Count() == 1
					select new APIEntity()
					{
						Route = a.Route,
						GETPolicy = a.GETPolicy,
						POSTtPolicy = a.POSTPolicy,
						PUTPolicy = a.PUTPolicy,
						DELETEPolicy = a.DELETEPolicy,
						DbSet = p,
						EntityType = g.First(),
						Properties = g.First().GetProperties().Where(x=>x.PropertyType.IsTypeSupported()).ToList(),
						Id = g.First().GetProperties().Where(x => x.IsDefined(typeof(KeyAttribute))).FirstOrDefault()
					}).ToList();
		}

        public static bool IsOperatorSuported(this Type type,string comparisonOperator)
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
            if(type.IsValueType)
            {
                return true;
            }

            if(type == typeof(string)|| type == typeof(Guid))
            {
                return true;
            }

            return false;
        }

        
	}
}