using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI
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

		public Tuple<string, object[]> GetFilter(APIEntity entity, IQueryCollection queryString)
		{

			var filters = new Dictionary<string, object>();
			var filterCount = 0;

			foreach (var key in queryString.Keys.Where(x => x.StartsWith("filter")))
			{
				var filterName = getFilterProperty(key);
				var property = entity.Properties.Where(x => x.Name.ToLower() == filterName.ToLower()).FirstOrDefault();

				if (property != null)
				{
					filters.Add($"{property.Name} == @{filterCount}", Convert.ChangeType(queryString[key].ToString(), property.PropertyType));
					filterCount++;
				}
			}

			if (filters.Count > 0)
			{
				return new Tuple<string, object[]>(String.Join(" && ", filters.Keys.ToArray()), filters.Values.ToArray());
			}
			else
			{
				return null;
			}
		}

		private string getFilterProperty(string key)
		{
			return key.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)[1];
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
