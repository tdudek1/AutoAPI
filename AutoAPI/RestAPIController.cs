using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;

namespace AutoAPI
{

	public class RESTAPIController : IRestAPIController
	{
		private readonly DbContext dbContext;
		private readonly ActionContext actionContext;
		private readonly IObjectModelValidator objectModelValidator;

		public RESTAPIController(DbContext dbContext, ActionContext actionContext, IObjectModelValidator objectModelValidator)
		{
			this.dbContext = dbContext;
			this.actionContext = actionContext;
			this.objectModelValidator = objectModelValidator;
		}

		public JsonResult Get(RouteInfo routeInfo)
		{
			if (routeInfo.Id != null)
			{
				var result = ((dynamic)routeInfo.Entity.DbSet.GetValue(dbContext)).Find(Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));
				if (result != null)
				{
					return GetJsonResult(result);
				}
				else
				{
					return GetJsonResult(null, HttpStatusCode.NotFound);
				}
			}
			else if (routeInfo.HasModifiers)
			{
				IQueryable dbSet = ((IQueryable)routeInfo.Entity.DbSet.GetValue(dbContext));

				foreach (var include in routeInfo.IncludeExpression)
				{
					dbSet = (dynamic)EntityFrameworkQueryableExtensions.Include((dynamic)dbSet, include);
				}

				if (!string.IsNullOrWhiteSpace(routeInfo.FilterExpression))
				{
					dbSet = dbSet.Where(routeInfo.FilterExpression, routeInfo.FilterValues);
				}

				if (routeInfo.Take != 0)
				{
					dbSet = dbSet.Skip(routeInfo.Skip).Take(routeInfo.Take);
				}

				if (!string.IsNullOrWhiteSpace(routeInfo.SortExpression))
				{
					dbSet = dbSet.OrderBy(routeInfo.SortExpression);
				}

				if (routeInfo.IsCount)
				{
					return GetJsonResult(dbSet.Count());
				}

				if (routeInfo.IsPageResult)
				{
					return GetJsonResult(GetPagedResult(dbSet, (IQueryable)routeInfo.Entity.DbSet.GetValue(dbContext), routeInfo.Page, routeInfo.Take));
				}

				return GetJsonResult(dbSet.ToDynamicList());
			}
			else
			{
				if (routeInfo.IsCount)
				{
					return GetJsonResult(((IQueryable)routeInfo.Entity.DbSet.GetValue(dbContext)).Count());
				}
				else if (routeInfo.IsPageResult)
				{
					return GetJsonResult(GetPagedResult((IQueryable)routeInfo.Entity.DbSet.GetValue(dbContext), (IQueryable)routeInfo.Entity.DbSet.GetValue(dbContext), 1, 0));
				}
				else
				{
					return GetJsonResult(routeInfo.Entity.DbSet.GetValue(dbContext));
				}
			}
		}

		public JsonResult Post(RouteInfo routeInfo, object entity)
		{
			if (!IsValid(entity))
			{
				return GetJsonResult(this.actionContext.ModelState, HttpStatusCode.BadRequest);
			}

			dbContext.Add(entity);
			dbContext.SaveChanges();

			return GetJsonResult(entity, HttpStatusCode.Created);
		}

		public JsonResult Put(RouteInfo routeInfo, object entity)
		{

			var objectId = Convert.ChangeType(routeInfo.Entity.Id.GetValue(entity), routeInfo.Entity.Id.PropertyType);
			var routeId = Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType);

			if (!objectId.Equals(routeId))
			{
				return GetJsonResult(null, HttpStatusCode.BadRequest);
			}

			if (!IsValid(entity))
			{
				return GetJsonResult(null, HttpStatusCode.BadRequest);
			}

			dbContext.Entry(entity).State = EntityState.Modified;
			dbContext.SaveChanges();

			return GetJsonResult(entity);
		}

		private JsonResult GetJsonResult(object result, HttpStatusCode? code = null)
		{
			JsonResult jsonResult = null;
			if (APIConfiguration.AutoAPIOptions.UseNewtonoftSerializer)
			{
				if (APIConfiguration.AutoAPIOptions.JsonSerializerSettings != null)
				{
					jsonResult = new JsonResult(result, APIConfiguration.AutoAPIOptions.JsonSerializerSettings);
				}
				else
				{
					jsonResult = new JsonResult(result, new JsonSerializerSettings());
				}
			}
			else
			{
				if (APIConfiguration.AutoAPIOptions.JsonSerializerSettings != null)
				{
					jsonResult = new JsonResult(result, APIConfiguration.AutoAPIOptions.JsonSerializerOptions);
				}
				else
				{
					jsonResult = new JsonResult(result);
				}
			}

			if (code.HasValue)
			{
				jsonResult.StatusCode = (int?)code.Value;
			}

			return jsonResult;
		}

		public JsonResult Delete(RouteInfo routeInfo)
		{
			object entity = ((dynamic)routeInfo.Entity.DbSet.GetValue(dbContext)).Find(Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));

			if (entity == null)
			{
				return GetJsonResult(null, HttpStatusCode.NotFound);
			}

			dbContext.Remove(entity);
			dbContext.SaveChanges();

			return GetJsonResult(null);
		}

		protected virtual bool IsValid(object entity)
		{
			objectModelValidator.Validate(this.actionContext, null, "", entity);
			return this.actionContext.ModelState.IsValid;
		}

		private PagedResult GetPagedResult(IQueryable dbSet, IQueryable totalDbSet, int page, int pageSize)
		{
			var total = totalDbSet.Count();

			return new PagedResult()
			{
				Items = dbSet.ToDynamicList(),
				Page = page,
				PageSize = pageSize == 0 ? total : pageSize,
				PageCount = pageSize == 0 ? 1 : (int)Math.Ceiling((decimal)total / (decimal)pageSize),
				Total = total
			};
		}
	}
}

