using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

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

        public ActionResult Get(RouteInfo routeInfo)
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
                    return new NotFoundObjectResult(null);
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

        public ActionResult Post(RouteInfo routeInfo, object entity)
        {
            if (!IsValid(entity))
            {
                return new BadRequestObjectResult(this.actionContext.ModelState);
            }

            dbContext.Add(entity);
            dbContext.SaveChanges();

            var result = GetJsonResult(entity);
            return result;
        }

        public ObjectResult Put(RouteInfo routeInfo, object entity)
        {

            var objectId = Convert.ChangeType(routeInfo.Entity.Id.GetValue(entity), routeInfo.Entity.Id.PropertyType);
            var routeId = Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType);

            if (!objectId.Equals(routeId))
            {
                return new BadRequestObjectResult(null);
            }

            if (!IsValid(entity))
            {
                return new BadRequestObjectResult(this.actionContext.ModelState);
            }

            dbContext.Entry(entity).State = EntityState.Modified;
            dbContext.SaveChanges();

            return new OkObjectResult(entity);
        }

        private JsonResult GetJsonResult(object result)
        {
            if (APIConfiguration.AutoAPIOptions.UseNewtonoftSerializer)
            {
                if (APIConfiguration.AutoAPIOptions.JsonSerializerSettings != null)
                {
                    return new JsonResult(result, APIConfiguration.AutoAPIOptions.JsonSerializerSettings);
                }
                else
                {
                    return new JsonResult(result, JsonConvert.DefaultSettings);
                }
            }
            else
            {
                if (APIConfiguration.AutoAPIOptions.JsonSerializerSettings != null)
                {
                    return new JsonResult(result, APIConfiguration.AutoAPIOptions.JsonSerializerOptions);
                }
                else
                {
                    return new JsonResult(result);
                }
            }
        }

        public ObjectResult Delete(RouteInfo routeInfo)
        {
            object entity = ((dynamic)routeInfo.Entity.DbSet.GetValue(dbContext)).Find(Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));

            if (entity == null)
            {
                return new NotFoundObjectResult(null);
            }

            dbContext.Remove(entity);
            dbContext.SaveChanges();

            return new OkObjectResult("");
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

